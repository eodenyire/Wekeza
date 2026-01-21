using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class Card
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(20)]
    public string CardNumber { get; set; } = string.Empty;
    
    [Required]
    public Guid CustomerId { get; set; }
    
    [Required]
    public Guid AccountId { get; set; }
    
    [MaxLength(50)]
    public string CardType { get; set; } = "Debit"; // Debit, Credit, Prepaid
    
    [MaxLength(100)]
    public string CardHolderName { get; set; } = string.Empty;
    
    public DateTime ExpiryDate { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Active"; // Active, Blocked, Expired, Lost, Stolen
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal DailyLimit { get; set; } = 100000;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal MonthlyLimit { get; set; } = 1000000;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal UsedToday { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal UsedThisMonth { get; set; } = 0;
    
    public bool IsContactless { get; set; } = true;
    public bool IsInternational { get; set; } = false;
    
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? BlockedAt { get; set; }
    public string? BlockReason { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Account Account { get; set; } = null!;
    public virtual ICollection<ATMTransaction> ATMTransactions { get; set; } = new List<ATMTransaction>();
    public virtual ICollection<POSTransaction> POSTransactions { get; set; } = new List<POSTransaction>();
}