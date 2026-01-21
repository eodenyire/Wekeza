using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class POSTransaction
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid CardId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string MerchantId { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string MerchantName { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string MerchantLocation { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string MCC { get; set; } = string.Empty; // Merchant Category Code
    
    [Required]
    [MaxLength(50)]
    public string TransactionType { get; set; } = "Purchase"; // Purchase, Refund, Reversal
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; } = "KES";
    
    [MaxLength(50)]
    public string Reference { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string AuthorizationCode { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Completed"; // Completed, Failed, Reversed
    
    [MaxLength(100)]
    public string ResponseCode { get; set; } = "00";
    
    [MaxLength(200)]
    public string ResponseMessage { get; set; } = "Approved";
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Fee { get; set; } = 0;
    
    public bool IsContactless { get; set; } = false;
    public bool IsInternational { get; set; } = false;
    
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Card Card { get; set; } = null!;
}