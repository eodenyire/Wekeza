using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class PaymentOrder
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(50)]
    public string PaymentReference { get; set; } = string.Empty;
    
    [Required]
    public Guid FromAccountId { get; set; }
    
    [MaxLength(50)]
    public string ToAccountNumber { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string BeneficiaryName { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string BeneficiaryBank { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string BeneficiaryBankCode { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; } = "KES";
    
    [MaxLength(50)]
    public string PaymentType { get; set; } = string.Empty; // RTGS, EFT, SWIFT, Internal
    
    [MaxLength(500)]
    public string PaymentPurpose { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed, Cancelled
    
    [MaxLength(20)]
    public string Priority { get; set; } = "Normal"; // High, Normal, Low
    
    public DateTime ValueDate { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    
    [MaxLength(100)]
    public string InitiatedBy { get; set; } = string.Empty;
    [MaxLength(100)]
    public string ProcessedBy { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Fee { get; set; } = 0;
    
    [MaxLength(100)]
    public string ExternalReference { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Account FromAccount { get; set; } = null!;
}