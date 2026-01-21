using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class Account
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(50)]
    public string AccountNumber { get; set; } = string.Empty;
    
    [Required]
    public Guid CustomerId { get; set; }
    
    // Business account support
    public Guid? BusinessId { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "KES";
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal AvailableBalance { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal OverdraftLimit { get; set; } = 0;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Active";
    
    [MaxLength(50)]
    public string AccountType { get; set; } = "Savings";
    
    // Enhanced account features
    public bool IsFrozen { get; set; } = false;
    public DateTime? FrozenAt { get; set; }
    public string? FreezeReason { get; set; }
    public string? FrozenBy { get; set; }
    
    public bool IsDeactivated { get; set; } = false;
    public DateTime? DeactivatedAt { get; set; }
    public string? DeactivationReason { get; set; }
    
    public bool IsClosed { get; set; } = false;
    public DateTime? ClosedAt { get; set; }
    public string? ClosureReason { get; set; }
    public string? ClosedBy { get; set; }
    
    // Product-based account fields
    [MaxLength(100)]
    public string? ProductCode { get; set; }
    
    [MaxLength(200)]
    public string? ProductName { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal? InterestRate { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MinimumBalance { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MonthlyFee { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Business? Business { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<Signatory> Signatories { get; set; } = new List<Signatory>();
}