using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class GLAccount
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(20)]
    public string AccountCode { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string AccountName { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string AccountType { get; set; } = string.Empty; // Asset, Liability, Equity, Income, Expense
    
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string SubCategory { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal DebitBalance { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CreditBalance { get; set; } = 0;
    
    [MaxLength(3)]
    public string Currency { get; set; } = "KES";
    
    [MaxLength(20)]
    public string Status { get; set; } = "Active";
    
    public bool IsControlAccount { get; set; } = false;
    public bool AllowManualPosting { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}