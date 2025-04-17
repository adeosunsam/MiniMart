namespace MiniMart.Domain.Entities
{
    public class CallBackEventLog : BaseEntity
    {
        public string TransactionReference { get; set; }
        public string EventPayload { get; set; }
    }
}
