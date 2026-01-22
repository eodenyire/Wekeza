namespace Wekeza.Mobile.Models;

public class AccountBalance
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal AvailableBalance { get; set; }
    public decimal BookBalance { get; set; }
    public string Currency { get; set; } = "KES";
    public DateTime LastUpdated { get; set; }
}

public class AccountSummary
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "KES";
    public string Status { get; set; } = string.Empty;
    public DateTime OpenedDate { get; set; }
}
