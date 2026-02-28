using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class Transaction
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid AccountId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty; // Credit, Debit
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; } = "KES";
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PreviousBalance { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal NewBalance { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Completed";
    
    [MaxLength(100)]
    public string Reference { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    // Additional fields for comprehensive banking
    [MaxLength(50)]
    public string? RelatedAccountNumber { get; set; }
    
    [MaxLength(50)]
    public string? ChequeNumber { get; set; }
    
    [MaxLength(100)]
    public string? DrawerBank { get; set; }
    
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(100)]
    public string ProcessedBy { get; set; } = "System";
    
    // Navigation properties
    public virtual Account Account { get; set; } = null!;
}