namespace MiniMart.Infrastructure.Helper
{
    public class AppKeys
    {
        public BankLinkSettings BankLink { get; set; }
    }

    public class BankLinkSettings
    {
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
    }
}
