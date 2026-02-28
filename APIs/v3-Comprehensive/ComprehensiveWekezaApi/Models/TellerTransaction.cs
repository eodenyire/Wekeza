using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class TellerTransaction
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid TellerSessionId { get; set; }
    
    [Required]
    public Guid TransactionId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string TransactionType { get; set; } = string.Empty; // CashDeposit, CashWithdrawal, Transfer
    
    // Alias for Type property used in API
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; } = "KES";
    
    [MaxLength(50)]
    public string Reference { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string AccountNumber { get; set; } = string.Empty;
    
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual TellerSession TellerSession { get; set; } = null!;
    public virtual Transaction Transaction { get; set; } = null!;
}