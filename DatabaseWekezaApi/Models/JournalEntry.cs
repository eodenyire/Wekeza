using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class JournalEntry
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(50)]
    public string JournalNumber { get; set; } = string.Empty;
    
    [Required]
    public Guid GLAccountId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string EntryType { get; set; } = string.Empty; // Debit, Credit
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; } = "KES";
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Reference { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string TransactionId { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Posted"; // Draft, Posted, Reversed
    
    public DateTime ValueDate { get; set; } = DateTime.UtcNow;
    public DateTime PostedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(100)]
    public string PostedBy { get; set; } = "System";
    
    // Navigation properties
    public virtual GLAccount GLAccount { get; set; } = null!;
}