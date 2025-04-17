using System.Text.Json.Serialization;

namespace MiniMart.Application.DTO
{
    public class BankLinkDto
    {
        public class CallBackDto
        {
            public string TransactionResponseCode { get; set; }
            public string TransactionResponseMessage { get; set; }
            public string SourceBankCode { get; set; }
            public string SourceAccountName { get; set; }
            public string SourceBankName { get; set; }
            public string SourceBankAccountNumber { get; set; }
            public decimal Amount { get; set; }
            public string DestinationBankCode { get; set; }
            public string DestinationBankName { get; set; }
            public string DestinationAccountName { get; set; }
            public string DestinationAccountNumber { get; set; }
            public string TraceId { get; set; }
            public string TransactionId { get; set; }
            public string SessionId { get; set; }
            public string Terminal { get; set; }
            public string Description { get; set; }
            public string MerchantId { get; set; }
        }

        public class GenerateAccountNumberRequestDto
        {
            public decimal Amount { get; set; }
            [JsonIgnore]
            public string BankCode { get; set; } = "100067";
            [JsonIgnore]
            public string? Description { get; set; }
            [JsonIgnore]
            public string AccountName { get; set; } = "MiniMart Supermarket";
        }

        public class AuthRequestDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string MerchantId { get; set; }
            public string TerminalId { get; set; }
        }

        public class AuthResponseDto
        {
            public ResponseHeader ResponseHeader { get; set; }
            public string? Key { get; set; }
            public string? Token { get; set; }
            public DateTime? ExpiryDate { get; set; }
        }

        public class InvokePaymentRequestDto
        {
            public RequestHeader RequestHeader { get; set; }
            public decimal Amount { get; set; }
            public string BankCode { get; set; }
            public string Description { get; set; }
            public string AccountName { get; set; }
        }

        public class InvokePaymentResponseDto
        {
            public ResponseHeader ResponseHeader { get; set; }
            public string DestinationBankCode { get; set; }
            public string DestinationBankName { get; set; }
            public string Description { get; set; }
            public string DestinationAccountName { get; set; }
            public string DestinationAccountNumber { get; set; }
            public string TransactionId { get; set; }
            public string SessionId { get; set; }
            public string DestinationBankLogo { get; set; }
            public DateTime ExpiryTime { get; set; }
        }

        public class TransactionStatusQueryRequestDto
        {
            public RequestHeader RequestHeader { get; set; }
        }
        public class TransactionQueryResponseDto
        {
            public ResponseHeader ResponseHeader { get; set; }
            public string? TransactionResponseCode { get; set; }
            public string? TransactionResponseMessage { get; set; }
            public string? SourceBankCode { get; set; }
            public string? SourceAccountName { get; set; }
            public string? SourceBankName { get; set; }
            public string? SourceBankAccountNumber { get; set; }
            public decimal? Amount { get; set; }
            public string? DestinationBankCode { get; set; }
            public string? DestinationBankName { get; set; }
            public string? DestinationAccountName { get; set; }
            public string? DestinationAccountNumber { get; set; }
            public string? TraceId { get; set; }
            public string? TransactionId { get; set; }
            public string? SessionId { get; set; }
            public string? Terminal { get; set; }
            public string? Description { get; set; }
            public float? PaidAmount { get; set; }
            public DateTime? PaymentDate { get; set; }
        }

        public class RequestHeader
        {
            public string MerchantId { get; set; }
            public string TerminalId { get; set; }
            public string TraceId { get; set; }
        }
        
        public class ResponseHeader
        {
            public string? ResponseCode { get; set; }
            public string? ResponseMessage { get; set; }
            public string? TraceId { get; set; }
        }
    }
}
