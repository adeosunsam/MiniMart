using Microsoft.Extensions.Options;
using MiniMart.Application.Interface;
using MiniMart.Infrastructure.Helper;
using MiniMart.Infrastructure.RestSharpHelper;
using Newtonsoft.Json;
using static MiniMart.Application.DTO.BankLinkDto;

namespace MiniMart.Application.Services
{
    public class BankLinkService : IBankLinkService
    {
        private readonly AppKeys _settings;
        private readonly IRestSharpClient _restClientHelper;

        private AuthResponseDto tokenResponse;

        public BankLinkService(IOptions<AppKeys> option, IRestSharpClient restClientHelper)
        {
            _settings = option.Value;
            _restClientHelper = restClientHelper;
        }

        public async Task<(bool isSuccessful, InvokePaymentResponseDto? response, string? message)> InvokePayment(InvokePaymentRequestDto request)
        {
            try
            {
                await TokenValidity();

                if (tokenResponse.ResponseHeader.ResponseCode != ResponseCode.Successful)
                {
                    return (false, null, tokenResponse.ResponseHeader.ResponseMessage);
                }

                var header = new Dictionary<string, string>
                {
                    {"authorization", $"Bearer {tokenResponse.Token}"}
                };

                var response = await _restClientHelper.PostAsync(_settings.BankLink.BaseUrl, "BankLinkService/api/InvokePayment", request, header);

                if (response != null && response.IsSuccessful)
                {
                    if (string.IsNullOrEmpty(response.Content))
                    {
                        return (false, null, response.ErrorMessage);
                    }

                    var result = JsonConvert.DeserializeObject<InvokePaymentResponseDto>(response.Content);

                    return (true, result, null);
                }
                else
                {
                    return (false, null, response?.Content ?? "No response from client");
                }
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool isSuccessful, TransactionQueryResponseDto? response, string? message)> TransactionQuery(TransactionStatusQueryRequestDto request)
        {
            try
            {
                await TokenValidity();

                if (tokenResponse.ResponseHeader.ResponseCode != ResponseCode.Successful)
                {
                    return (false, null, tokenResponse.ResponseHeader.ResponseMessage);
                }

                request.RequestHeader.MerchantId = _settings.BankLink.MerchantId;
                request.RequestHeader.TerminalId = _settings.BankLink.TerminalId;

                var header = new Dictionary<string, string>
                {
                    {"authorization", $"Bearer {tokenResponse.Token}"}
                };

                var response = await _restClientHelper.PostAsync(_settings.BankLink.BaseUrl, "BankLinkService/api/transactionQuery", request, header);

                if (response != null && response.IsSuccessful)
                {
                    if (string.IsNullOrEmpty(response.Content))
                    {
                        return (false, null, response.ErrorMessage);
                    }

                    var result = JsonConvert.DeserializeObject<TransactionQueryResponseDto>(response.Content);

                    return (true, result, null);
                }
                else
                {
                    return (false, null, response?.Content ?? "No response from client");
                }
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        private async Task<(bool isSuccessful, AuthResponseDto? response, string? message)> Authentication()
        {
            try
            {
                var body = new AuthRequestDto
                {
                    Username = _settings.BankLink.Username,
                    Password = _settings.BankLink.Password,
                    MerchantId = _settings.BankLink.MerchantId,
                    TerminalId = _settings.BankLink.TerminalId
                };

                var response = await _restClientHelper.PostAsync(_settings.BankLink.BaseUrl, "BankLinkService/api/Auth", body);

                if (response != null && response.IsSuccessful)
                {
                    if (string.IsNullOrEmpty(response.Content))
                    {
                        return (false, null, response.ErrorMessage);
                    }

                    var result = JsonConvert.DeserializeObject<AuthResponseDto>(response.Content);

                    return (true, result, null);
                }
                else
                {
                    return (false, null, response?.Content ?? "No response from client");
                }
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        private async Task TokenValidity()
        {
            //if (tokenResponse != null)
            //{
            //    if (tokenResponse.ExpiryDate.HasValue && tokenResponse.ExpiryDate > DateTime.Now.AddMinutes(15))
            //    {
            //        return;
            //    }
            //}
            var (_, response, message) = await Authentication();

            tokenResponse = response ?? new AuthResponseDto
            {
                ResponseHeader = new ResponseHeader
                {
                    ResponseMessage = message
                }
            };
        }
    }
}
