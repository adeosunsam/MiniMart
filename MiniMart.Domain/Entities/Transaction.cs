namespace MiniMart.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public string TransactionReference { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionResponseCode { get; set; }
        public string? TransactionResponseMessage { get; set; }
        public string? SourceAccountName { get; set; }
        public string? SourceBankName { get; set; }
        public string? SourceBankAccountNumber { get; set; }
        public string DestinationBankName { get; set; }
        public string DestinationAccountName { get; set; }
        public string DestinationAccountNumber { get; set; }
        public string? TransactionId { get; set; }
        public string? SessionId { get; set; }
        public string Description { get; set; }
    }
}
