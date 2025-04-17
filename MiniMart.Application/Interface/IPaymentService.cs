using MiniMart.Application.DTO;
using MiniMart.Infrastructure;

namespace MiniMart.Application.Interface
{
    public interface IPaymentService
    {
        Task<ApiResponse> GenerateVirtualAccount(BankLinkDto.GenerateAccountNumberRequestDto request);
        Task<ApiResponse> PaymentConfirmation(string paymentReference);
        Task<bool> PaymentNotification(BankLinkDto.CallBackDto request);
        Task TransactionRequery();
    }
}