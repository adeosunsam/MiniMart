using MiniMart.Application.DTO;

namespace MiniMart.Application.Interface
{
    public interface IBankLinkService
    {
        Task<(bool isSuccessful, BankLinkDto.InvokePaymentResponseDto? response, string? message)> InvokePayment(BankLinkDto.InvokePaymentRequestDto request);
        Task<(bool isSuccessful, BankLinkDto.TransactionQueryResponseDto? response, string? message)> TransactionQuery(BankLinkDto.TransactionStatusQueryRequestDto request);
    }
}