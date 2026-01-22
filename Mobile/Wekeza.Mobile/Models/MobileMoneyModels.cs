namespace Wekeza.Mobile.Models;

public class MobileMoneyRequest
{
    public string FromAccountNumber { get; set; } = string.Empty;
    public string ToPhoneNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Provider { get; set; } = "MPESA"; // MPESA, AIRTEL, etc.
}

public class MobileMoneyResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string MpesaReceiptNumber { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
}

public class AirtimeRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Provider { get; set; } = "MPESA";
    public string FromAccountNumber { get; set; } = string.Empty;
}

public class BillPaymentRequest
{
    public string FromAccountNumber { get; set; } = string.Empty;
    public string BillNumber { get; set; } = string.Empty;
    public string BillType { get; set; } = string.Empty; // Paybill, Till
    public string BusinessNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class BillPaymentResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
}
