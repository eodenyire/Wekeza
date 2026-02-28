using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class ATMTransaction
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid CardId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string ATMId { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string ATMLocation { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string TransactionType { get; set; } = string.Empty; // Withdrawal, BalanceInquiry, Transfer
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; } = "KES";
    
    [MaxLength(50)]
    public string Reference { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Completed"; // Completed, Failed, Reversed
    
    [MaxLength(100)]
    public string ResponseCode { get; set; } = "00"; // ISO 8583 response codes
    
    [MaxLength(200)]
    public string ResponseMessage { get; set; } = "Approved";
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Fee { get; set; } = 0;
    
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Card Card { get; set; } = null!;
}