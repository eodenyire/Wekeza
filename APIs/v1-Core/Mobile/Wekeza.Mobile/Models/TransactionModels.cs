namespace Wekeza.Mobile.Models;

public class TransferRequest
{
    public string FromAccountNumber { get; set; } = string.Empty;
    public string ToAccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "KES";
    public string Description { get; set; } = string.Empty;
    public string TransferType { get; set; } = "Internal"; // Internal, External, Mobile
}

public class TransferResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
}

public class Transaction
{
    public string TransactionId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "KES";
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}

public class TransactionHistory
{
    public List<Transaction> Transactions { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
