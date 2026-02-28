using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnhancedWekezaApi.Domain.Entities;

public class Transaction
{
    [Key]
    public Guid Id { get; private set; }
    
    [Required]
    public Guid AccountId { get; private set; }
    
    [Required]
    [MaxLength(20)]
    public string Type { get; private set; } = string.Empty; // Credit, Debit
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; private set; }
    
    [MaxLength(3)]
    public string Currency { get; private set; } = "KES";
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PreviousBalance { get; private set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal NewBalance { get; private set; }
    
    [MaxLength(20)]
    public string Status { get; private set; } = "Completed";
    
    [MaxLength(100)]
    public string Reference { get; private set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; private set; } = string.Empty;
    
    public DateTime ProcessedAt { get; private set; } = DateTime.UtcNow;
    
    [MaxLength(100)]
    public string ProcessedBy { get; private set; } = "System";
    
    // Navigation properties
    public virtual Account Account { get; private set; } = null!;

    // Private constructor for EF
    private Transaction() { }

    public Transaction(Guid accountId, string type, decimal amount, string currency, 
                      decimal previousBalance, decimal newBalance, string reference, string description)
    {
        Id = Guid.NewGuid();
        AccountId = accountId;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Amount = amount;
        Currency = currency ?? "KES";
        PreviousBalance = previousBalance;
        NewBalance = newBalance;
        Reference = reference ?? throw new ArgumentNullException(nameof(reference));
        Description = description ?? string.Empty;
        ProcessedAt = DateTime.UtcNow;
    }
}