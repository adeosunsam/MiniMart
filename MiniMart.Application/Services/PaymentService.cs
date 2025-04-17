using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniMart.Application.Interface;
using MiniMart.Domain.Entities;
using MiniMart.Infrastructure;
using MiniMart.Infrastructure.Context;
using MiniMart.Infrastructure.Helper;
using Newtonsoft.Json;
using static MiniMart.Application.DTO.BankLinkDto;

namespace MiniMart.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBankLinkService _bankLinkService;
        private readonly MiniMartContext _context;
        private readonly ILogger<PaymentService> _logger;
        private readonly AppKeys _settings;

        public PaymentService(IBankLinkService bankLinkService, IOptions<AppKeys> option,
            MiniMartContext context, ILogger<PaymentService> logger)
        {
            _bankLinkService = bankLinkService;
            _context = context;
            _logger = logger;
            _settings = option.Value;
        }

        public async Task<ApiResponse> GenerateVirtualAccount(GenerateAccountNumberRequestDto request)
        {
            var result = new ApiResponse();

            try
            {
                string transactionRef = Guid.NewGuid().ToString("N");

                request.Description = $"{transactionRef} | ";

                var (isSuccessful, response, message) = await _bankLinkService.InvokePayment(new InvokePaymentRequestDto
                {
                    RequestHeader = new RequestHeader
                    {
                        MerchantId = _settings.BankLink.MerchantId,
                        TerminalId = _settings.BankLink.TerminalId,
                        TraceId = transactionRef
                    },
                    Amount = request.Amount,
                    AccountName = request.AccountName,
                    BankCode = request.BankCode,
                    Description = request.Description
                });

                if (isSuccessful && response != null && response.ResponseHeader.ResponseCode == ResponseCode.Successful)
                {
                    await _context.Transactions.AddAsync(new Transaction
                    {
                        TransactionReference = transactionRef,
                        Amount = request.Amount,
                        DestinationBankName = response.DestinationBankName,
                        DestinationAccountNumber = response.DestinationAccountNumber,
                        DestinationAccountName = response.DestinationAccountName,
                        Description = request.Description
                    });

                    await _context.SaveChangesAsync();

                    result.Message = "Payment details generated successfully";
                    result.Status = true;
                    result.Data = response;

                    return result;
                }
                else if (isSuccessful && response != null && response.ResponseHeader.ResponseCode != ResponseCode.Successful)
                {
                    result.Message = response.ResponseHeader.ResponseMessage ?? "error generating payment details";
                    return result;
                }
                else
                {
                    result.Message = message ?? "error generating payment details";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<bool> PaymentNotification(CallBackDto request)
        {
            try
            {
                await _context.CallBackEventLogs.AddAsync(new CallBackEventLog
                {
                    TransactionReference = request.TraceId,
                    EventPayload = JsonConvert.SerializeObject(request)
                });

                var transaction = await (from t in _context.Transactions
                                         where t.TransactionReference == request.TraceId
                                         select t).FirstOrDefaultAsync();

                //this should never happen, but just in case
                if (transaction == null)
                {
                    //log exception
                    return true;// there's no point firing the same event again if the traceId is not found on our system
                }

                transaction.TransactionResponseCode = request.TransactionResponseCode;
                transaction.TransactionResponseMessage = request.TransactionResponseMessage;
                transaction.SourceAccountName = request.SourceAccountName;
                transaction.SourceBankName = request.SourceBankName;
                transaction.SourceBankAccountNumber = request.SourceBankAccountNumber;
                transaction.TransactionId = request.TransactionId;
                transaction.SessionId = request.SessionId;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ApiResponse> PaymentConfirmation(string paymentReference)
        {
            var response = new ApiResponse();

            var transaction = await (from t in _context.Transactions
                                     where t.TransactionReference == paymentReference
                                     select t).FirstOrDefaultAsync();

            if (transaction == null)
            {
                response.Message = "Invalid transaction reference";
                return response;
            }
            else if (string.IsNullOrWhiteSpace(transaction.TransactionResponseCode))
            {
                response.Message = "Payment pending confirmation";
                return response;
            }
            else if (transaction.TransactionResponseCode != ResponseCode.Successful)
            {
                response.Message = transaction.TransactionResponseMessage ?? "Unable to confirm payment";
                return response;
            }
            response.Message = transaction.TransactionResponseMessage;
            response.Status = true;
            return response;
        }

        public async Task TransactionRequery()
        {
            try
            {
                var getAllPendingTransaction = await (from t in _context.Transactions
                                                      where string.IsNullOrWhiteSpace(t.TransactionResponseCode) ||
                                                      t.TransactionResponseCode == ResponseCode.PendingTransaction
                                                      select t).ToListAsync();

                if (getAllPendingTransaction == null || !getAllPendingTransaction.Any())
                {
                    _logger.LogInformation("----NO PENDING TRANSACTION TO QUERY");
                    return;
                }

                foreach (var transaction in getAllPendingTransaction)
                {
                    var (isSuccessful, response, message) = await _bankLinkService.TransactionQuery(new TransactionStatusQueryRequestDto
                    {
                        RequestHeader = new RequestHeader
                        {
                            MerchantId = _settings.BankLink.MerchantId,
                            TerminalId = _settings.BankLink.TerminalId,
                            TraceId = transaction.TransactionReference
                        }
                    });

                    if (!isSuccessful || response == null)
                    {
                        _logger.LogError($"TRANSACTION QUERY FOR {transaction.TransactionReference}: {message}");
                        continue;
                    }

                    transaction.TransactionResponseCode = response.TransactionResponseCode;
                    transaction.TransactionResponseMessage = response.TransactionResponseMessage;
                    transaction.SourceAccountName = response.SourceAccountName;
                    transaction.SourceBankName = response.SourceBankName;
                    transaction.SourceBankAccountNumber = response.SourceBankAccountNumber;
                    transaction.TransactionId = response.TransactionId;
                    transaction.SessionId = response.SessionId;

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TRANSACTION QUERY ERRORRR: {ex.Message}");
            }
        }
    }
}
