namespace Wekeza.Core.Application.DTOs;

/// <summary>
/// Data Transfer Object for M-Pesa callback data from Safaricom Daraja API
/// </summary>
public class MpesaCallbackDto
{
    public string MerchantRequestID { get; set; } = string.Empty;
    public string CheckoutRequestID { get; set; } = string.Empty;
    public int ResultCode { get; set; }
    public string ResultDesc { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string MpesaReceiptNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
}
