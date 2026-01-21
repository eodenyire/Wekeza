using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnhancedWekezaApi.Domain.ValueObjects;

namespace EnhancedWekezaApi.Domain.Entities;

public class Account
{
    [Key]
    public Guid Id { get; private set; }
    
    [Required]
    public Guid CustomerId { get; private set; }
    
    [Required]
    [MaxLength(50)]
    public string AccountNumber { get; private set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceAmount { get; private set; }
    
    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; private set; } = "KES";
    
    [MaxLength(20)]
    public string Status { get; private set; } = "Active";
    
    public bool IsFrozen { get; private set; } = false;
    
    [MaxLength(50)]
    public string AccountType { get; private set; } = "Savings";
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Customer Customer { get; private set; } = null!;
    public virtual ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    // Domain properties (not mapped to database)
    [NotMapped]
    public Money Balance => new Money(BalanceAmount, Currency.FromCode(CurrencyCode));
    
    [NotMapped]
    public AccountNumber AccountNumberValue => new AccountNumber(AccountNumber);

    // Private constructor for EF
    private Account() { }

    public Account(Guid id, Guid customerId, AccountNumber accountNumber, Currency currency)
    {
        Id = id;
        CustomerId = customerId;
        AccountNumber = accountNumber?.Value ?? throw new ArgumentNullException(nameof(accountNumber));
        BalanceAmount = 0;
        CurrencyCode = currency?.Code ?? throw new ArgumentNullException(nameof(currency));
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Credit(Money amount)
    {
        if (amount.Currency.Code != CurrencyCode)
            throw new InvalidOperationException("Currency mismatch");
        
        if (IsFrozen)
            throw new InvalidOperationException("Account is frozen");
        
        BalanceAmount += amount.Amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Debit(Money amount)
    {
        if (amount.Currency.Code != CurrencyCode)
            throw new InvalidOperationException("Currency mismatch");
        
        if (IsFrozen)
            throw new InvalidOperationException("Account is frozen");
        
        if (BalanceAmount < amount.Amount)
            throw new InvalidOperationException("Insufficient funds");
        
        BalanceAmount -= amount.Amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Freeze(string reason)
    {
        IsFrozen = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unfreeze()
    {
        IsFrozen = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        if (BalanceAmount != 0)
            throw new InvalidOperationException("Cannot close account with non-zero balance");
        
        Status = "Closed";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = "Inactive";
        UpdatedAt = DateTime.UtcNow;
    }
}