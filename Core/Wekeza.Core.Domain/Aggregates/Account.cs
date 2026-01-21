using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.Exceptions;
using Wekeza.Core.Domain.Enums;

///<summary>
/// Account Aggregate - Enhanced with Product Factory Integration
/// This is the most critical file in the bank. It manages the Ledger Balance and enforces the "Billion Dollar" rule: Money cannot be created or destroyed, only moved.
/// Now integrated with Product Factory for configuration-driven account management and GL posting
///</summary>

namespace Wekeza.Core.Domain.Aggregates;

public class Account : AggregateRoot
{
    public AccountNumber AccountNumber { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid ProductId { get; private set; } // Product Factory integration
    public Money Balance { get; private set; }
    public Money OverdraftLimit { get; private set; }
    public AccountStatus Status { get; private set; }
    public DateTime OpenedDate { get; private set; }
    public DateTime? ClosedDate { get; private set; }
    public string OpenedBy { get; private set; }
    public string? ClosedBy { get; private set; }
    
    // Interest tracking
    public decimal InterestRate { get; private set; }
    public DateTime LastInterestCalculationDate { get; private set; }
    public Money AccruedInterest { get; private set; }
    
    // Transaction limits from product
    public Money DailyTransactionLimit { get; private set; }
    public Money MonthlyTransactionLimit { get; private set; }
    public Money MinimumBalance { get; private set; }
    
    // GL Integration
    public string CustomerGLCode { get; private set; } // GL account for this customer account
    public string GLAccountCode => CustomerGLCode; // Alias for compatibility
    
    // Currency property
    public Currency Currency => Balance.Currency;
    
    // Additional properties for compatibility
    public string AccountType { get; private set; } = "SAVINGS";
    public bool IsFrozen => Status == AccountStatus.Frozen;
    
    // Navigation properties for EF Core
    public Customer? Customer { get; private set; }
    public Product? Product { get; private set; }

    private Account() : base(Guid.NewGuid()) { }

    public static Account OpenAccount(
        Guid customerId, 
        Guid productId,
        AccountNumber accountNumber, 
        Currency currency,
        string customerGLCode,
        string openedBy,
        Product product,
        string accountType = "SAVINGS")
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ProductId = productId,
            AccountNumber = accountNumber,
            Balance = Money.Zero(currency),
            Status = AccountStatus.Active,
            OpenedDate = DateTime.UtcNow,
            OpenedBy = openedBy,
            CustomerGLCode = customerGLCode,
            LastInterestCalculationDate = DateTime.UtcNow.Date,
            AccruedInterest = Money.Zero(currency),
            AccountType = accountType
        };

        // Apply product configuration
        account.ApplyProductConfiguration(product);
        
        account.AddDomainEvent(new AccountOpenedDomainEvent(account.Id, customerId));
        return account;
    }

    private void ApplyProductConfiguration(Product product)
    {
        // Apply interest rate from product
        if (product.InterestConfig != null)
        {
            InterestRate = product.InterestConfig.Rate;
        }

        // Apply limits from product
        if (product.Limits != null)
        {
            if (product.Limits.MinBalance.HasValue)
                MinimumBalance = new Money(product.Limits.MinBalance.Value, Balance.Currency);
            
            if (product.Limits.DailyTransactionLimit.HasValue)
                DailyTransactionLimit = new Money(product.Limits.DailyTransactionLimit.Value, Balance.Currency);
            
            if (product.Limits.MonthlyTransactionLimit.HasValue)
                MonthlyTransactionLimit = new Money(product.Limits.MonthlyTransactionLimit.Value, Balance.Currency);
        }
    }

    public void Credit(Money amount, string transactionReference, string description = "")
    {
        EnsureAccountActive();
        ValidateTransactionAmount(amount);
        
        Balance += amount;
        AddDomainEvent(new FundsDepositedDomainEvent(this.Id, amount, transactionReference, description));
    }

    // Alias method for compatibility
    public void Deposit(Money amount, string transactionReference, string description = "")
    {
        Credit(amount, transactionReference, description);
    }

    public void Debit(Money amount, string transactionReference, string description = "")
    {
        EnsureAccountActive();
        ValidateTransactionAmount(amount);
        
        var availableFunds = Balance + OverdraftLimit;
        if (amount.IsGreaterThan(availableFunds))
        {
            throw new InsufficientFundsException(AccountNumber, amount);
        }

        // Check minimum balance requirement
        var balanceAfterDebit = Balance - amount;
        if (balanceAfterDebit.IsLessThan(MinimumBalance))
        {
            throw new InsufficientFundsException(AccountNumber, amount);
        }

        Balance -= amount;

        if (Balance.IsNegative())
        {
            AddDomainEvent(new OverdraftLimitReachedDomainEvent(this.Id, Balance));
        }
        
        AddDomainEvent(new FundsWithdrawnDomainEvent(this.Id, amount, transactionReference, description));
    }

    // Alias method for compatibility
    public void Withdraw(Money amount, string transactionReference, string description = "")
    {
        Debit(amount, transactionReference, description);
    }

    public void Freeze(string reason, string frozenBy)
    {
        if (Status == AccountStatus.Frozen)
            throw new AccountFrozenException(AccountNumber);
            
        Status = AccountStatus.Frozen;
        AddDomainEvent(new AccountFrozenEvent(this.Id, reason, frozenBy));
    }

    public void Unfreeze(string unfrozenBy)
    {
        if (Status != AccountStatus.Frozen)
            throw new AccountFrozenException(AccountNumber);
            
        Status = AccountStatus.Active;
        AddDomainEvent(new AccountUnfrozenDomainEvent(this.Id, unfrozenBy));
    }

    public void Close(string reason, string closedBy)
    {
        if (Status == AccountStatus.Closed)
            throw new AccountFrozenException(AccountNumber);
            
        if (!Balance.IsZero())
            throw new InsufficientFundsException(AccountNumber, Balance);
        
        Status = AccountStatus.Closed;
        ClosedDate = DateTime.UtcNow;
        ClosedBy = closedBy;
        
        AddDomainEvent(new AccountClosedDomainEvent(this.Id, reason, closedBy));
    }

    public void CalculateAndAccrueInterest(DateTime calculationDate)
    {
        if (InterestRate <= 0 || Status != AccountStatus.Active)
            return;

        var daysSinceLastCalculation = (calculationDate - LastInterestCalculationDate).Days;
        if (daysSinceLastCalculation <= 0)
            return;

        // Simple interest calculation (can be enhanced with product-specific logic)
        var interestAmount = (Balance.Amount * (decimal)(InterestRate / 100) * daysSinceLastCalculation) / 365;
        var interest = new Money(interestAmount, Balance.Currency);

        AccruedInterest += interest;
        LastInterestCalculationDate = calculationDate;

        if (interest.Amount > 0)
        {
            AddDomainEvent(new InterestAccruedDomainEvent(this.Id, this.AccountNumber.Value, interest, calculationDate, "SYSTEM"));
        }
    }

    public void PostAccruedInterest()
    {
        if (AccruedInterest.IsZero())
            return;

        Balance += AccruedInterest;
        var postedInterest = AccruedInterest;
        AccruedInterest = Money.Zero(Balance.Currency);

        AddDomainEvent(new InterestPostedDomainEvent(this.Id, this.AccountNumber.Value, postedInterest, DateTime.UtcNow, "SYSTEM"));
    }

    public void ApplyFee(Money feeAmount, string feeType, string description)
    {
        EnsureAccountActive();
        
        if (feeAmount.IsGreaterThan(Balance))
        {
            throw new InsufficientFundsException(AccountNumber, feeAmount);
        }

        Balance -= feeAmount;
        AddDomainEvent(new FeeAppliedDomainEvent(this.Id, feeAmount, feeType, description));
    }

    public void UpdateOverdraftLimit(Money newLimit, string updatedBy)
    {
        OverdraftLimit = newLimit;
        AddDomainEvent(new OverdraftLimitUpdatedDomainEvent(this.Id, newLimit, updatedBy));
    }

    public void UpdateTransactionLimit(Money newLimit, string updatedBy)
    {
        DailyTransactionLimit = newLimit;
        AddDomainEvent(new OverdraftLimitUpdatedDomainEvent(this.Id, newLimit, updatedBy));
    }

    public void Verify(string verifiedBy)
    {
        if (Status == AccountStatus.PendingVerification)
        {
            Status = AccountStatus.Active;
            AddDomainEvent(new AccountVerifiedDomainEvent(this.Id, verifiedBy));
        }
    }

    private void EnsureAccountActive()
    {
        if (Status != AccountStatus.Active)
            throw new AccountFrozenException(AccountNumber);
    }

    private void ValidateTransactionAmount(Money amount)
    {
        if (amount.IsZero() || amount.IsNegative())
            throw new InsufficientFundsException(AccountNumber, amount);

        if (DailyTransactionLimit != null && amount.IsGreaterThan(DailyTransactionLimit))
            throw new InsufficientFundsException(AccountNumber, amount);
    }
}

// Account Status Enum



