using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Term Deposit aggregate - Flexible time deposits with various term options
/// Supports custom terms, flexible interest rates, and partial withdrawals
/// </summary>
public class TermDeposit : AggregateRoot
{
    public Guid AccountId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string DepositNumber { get; private set; }
    public Money PrincipalAmount { get; private set; }
    public InterestRate InterestRate { get; private set; }
    public int TermInMonths { get; private set; }
    public DateTime BookingDate { get; private set; }
    public DateTime MaturityDate { get; private set; }
    public Money MaturityAmount { get; private set; }
    public DepositStatus Status { get; private set; }
    public InterestPaymentFrequency InterestFrequency { get; private set; }
    public bool AllowPartialWithdrawal { get; private set; }
    public Money MinimumBalance { get; private set; }
    public bool AutoRenewal { get; private set; }
    public string? RenewalInstructions { get; private set; }
    public Money AccruedInterest { get; private set; }
    public Money WithdrawnAmount { get; private set; }
    public DateTime? LastInterestPostingDate { get; private set; }
    public DateTime? PrematureClosureDate { get; private set; }
    public Money? PenaltyAmount { get; private set; }
    public string? ClosureReason { get; private set; }
    public string BranchCode { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }

    // Navigation properties
    public virtual Account Account { get; private set; } = null!;
    public virtual Customer Customer { get; private set; } = null!;

    private readonly List<TermDepositTransaction> _transactions = new();
    public IReadOnlyList<TermDepositTransaction> Transactions => _transactions.AsReadOnly();

    private TermDeposit() : base(Guid.NewGuid()) { } // EF Core

    public TermDeposit(
        Guid id,
        Guid accountId,
        Guid customerId,
        string depositNumber,
        Money principalAmount,
        InterestRate interestRate,
        int termInMonths,
        InterestPaymentFrequency interestFrequency,
        bool allowPartialWithdrawal,
        Money minimumBalance,
        bool autoRenewal,
        string branchCode,
        string createdBy) : base(id) {
        Id = id;
        AccountId = accountId;
        CustomerId = customerId;
        DepositNumber = depositNumber;
        PrincipalAmount = principalAmount;
        InterestRate = interestRate;
        TermInMonths = termInMonths;
        InterestFrequency = interestFrequency;
        AllowPartialWithdrawal = allowPartialWithdrawal;
        MinimumBalance = minimumBalance;
        AutoRenewal = autoRenewal;
        BranchCode = branchCode;
        CreatedBy = createdBy;
        
        BookingDate = DateTime.UtcNow;
        MaturityDate = BookingDate.AddMonths(termInMonths);
        Status = DepositStatus.Active;
        AccruedInterest = new Money(0, principalAmount.Currency);
        WithdrawnAmount = new Money(0, principalAmount.Currency);
        CreatedAt = DateTime.UtcNow;

        // Calculate maturity amount
        CalculateMaturityAmount();

        // Add domain event
        AddDomainEvent(new TermDepositBookedDomainEvent(Id, AccountId, CustomerId, PrincipalAmount, MaturityDate));
    }

    public void CalculateMaturityAmount()
    {
        // Compound interest calculation for term deposits
        var monthlyRate = InterestRate.Rate / 100 / 12;
        var compoundFactor = Math.Pow(1 + (double)monthlyRate, TermInMonths);
        var maturityValue = PrincipalAmount.Amount * (decimal)compoundFactor;
        
        MaturityAmount = new Money(maturityValue, PrincipalAmount.Currency);
    }

    public void AccrueInterest(Money interestAmount, DateTime accrualDate)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Cannot accrue interest on inactive deposit");

        AccruedInterest = new Money(AccruedInterest.Amount + interestAmount.Amount, AccruedInterest.Currency);
        LastInterestPostingDate = accrualDate;
        ModifiedAt = DateTime.UtcNow;

        // Record transaction
        var transaction = new TermDepositTransaction(
            Guid.NewGuid(),
            Id,
            TermDepositTransactionType.InterestAccrual,
            interestAmount,
            GetCurrentBalance(),
            "Interest accrual",
            accrualDate);

        _transactions.Add(transaction);

        AddDomainEvent(new InterestAccruedDomainEvent(Id, DepositNumber, interestAmount, accrualDate, "MONTHLY"));
    }

    public void ProcessPartialWithdrawal(Money withdrawalAmount, string reason, string processedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Deposit is not active");

        if (!AllowPartialWithdrawal)
            throw new InvalidOperationException("Partial withdrawals are not allowed for this deposit");

        var currentBalance = GetCurrentBalance();
        var balanceAfterWithdrawal = new Money(currentBalance.Amount - withdrawalAmount.Amount, currentBalance.Currency);

        if (balanceAfterWithdrawal.Amount < MinimumBalance.Amount)
            throw new InvalidOperationException("Withdrawal would result in balance below minimum required");

        WithdrawnAmount = new Money(WithdrawnAmount.Amount + withdrawalAmount.Amount, WithdrawnAmount.Currency);
        ModifiedBy = processedBy;
        ModifiedAt = DateTime.UtcNow;

        // Record transaction
        var transaction = new TermDepositTransaction(
            Guid.NewGuid(),
            Id,
            TermDepositTransactionType.PartialWithdrawal,
            withdrawalAmount,
            balanceAfterWithdrawal,
            reason,
            DateTime.UtcNow);

        _transactions.Add(transaction);

        AddDomainEvent(new TermDepositPartialWithdrawalDomainEvent(Id, AccountId, withdrawalAmount, balanceAfterWithdrawal));
    }

    public void ProcessMaturity(string processedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Deposit is not active");

        if (DateTime.UtcNow < MaturityDate)
            throw new InvalidOperationException("Deposit has not reached maturity");

        Status = DepositStatus.Matured;
        ModifiedBy = processedBy;
        ModifiedAt = DateTime.UtcNow;

        var finalAmount = GetCurrentBalance();

        // Record transaction
        var transaction = new TermDepositTransaction(
            Guid.NewGuid(),
            Id,
            TermDepositTransactionType.Maturity,
            finalAmount,
            new Money(0, finalAmount.Currency),
            "Deposit maturity",
            DateTime.UtcNow);

        _transactions.Add(transaction);

        AddDomainEvent(new TermDepositMaturedDomainEvent(Id, AccountId, CustomerId, finalAmount));
    }

    public void ProcessPrematureClosure(Money penaltyAmount, string reason, string processedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Deposit is not active");

        Status = DepositStatus.PrematurelyClosed;
        PrematureClosureDate = DateTime.UtcNow;
        PenaltyAmount = penaltyAmount;
        ClosureReason = reason;
        ModifiedBy = processedBy;
        ModifiedAt = DateTime.UtcNow;

        // Calculate closure amount (current balance - penalty)
        var currentBalance = GetCurrentBalance();
        var closureAmount = new Money(currentBalance.Amount - penaltyAmount.Amount, currentBalance.Currency);

        // Record transaction
        var transaction = new TermDepositTransaction(
            Guid.NewGuid(),
            Id,
            TermDepositTransactionType.PrematureClosure,
            closureAmount,
            new Money(0, closureAmount.Currency),
            reason,
            DateTime.UtcNow);

        _transactions.Add(transaction);

        AddDomainEvent(new TermDepositClosedDomainEvent(Id, AccountId, CustomerId, closureAmount, true));
    }

    public void Renew(InterestRate newInterestRate, int newTermInMonths, string renewedBy)
    {
        if (Status != DepositStatus.Matured)
            throw new InvalidOperationException("Only matured deposits can be renewed");

        if (!AutoRenewal)
            throw new InvalidOperationException("Auto-renewal is not enabled for this deposit");

        // Reset for new term
        var renewalAmount = GetCurrentBalance();
        PrincipalAmount = renewalAmount;
        InterestRate = newInterestRate;
        TermInMonths = newTermInMonths;
        BookingDate = DateTime.UtcNow;
        MaturityDate = BookingDate.AddMonths(newTermInMonths);
        Status = DepositStatus.Active;
        AccruedInterest = new Money(0, PrincipalAmount.Currency);
        WithdrawnAmount = new Money(0, PrincipalAmount.Currency);
        LastInterestPostingDate = null;
        ModifiedBy = renewedBy;
        ModifiedAt = DateTime.UtcNow;

        CalculateMaturityAmount();

        // Record transaction
        var transaction = new TermDepositTransaction(
            Guid.NewGuid(),
            Id,
            TermDepositTransactionType.Renewal,
            renewalAmount,
            renewalAmount,
            "Deposit renewal",
            DateTime.UtcNow);

        _transactions.Add(transaction);

        AddDomainEvent(new TermDepositRenewedDomainEvent(Id, AccountId, PrincipalAmount, MaturityDate));
    }

    public Money GetCurrentBalance()
    {
        return new Money(
            PrincipalAmount.Amount + AccruedInterest.Amount - WithdrawnAmount.Amount,
            PrincipalAmount.Currency);
    }

    public Money CalculatePrematurePenalty()
    {
        if (Status != DepositStatus.Active)
            return new Money(0, PrincipalAmount.Currency);

        // Calculate penalty based on remaining term
        var remainingMonths = GetMonthsToMaturity();
        var penaltyRate = remainingMonths switch
        {
            > 12 => 0.02m, // 2% for > 12 months remaining
            > 6 => 0.015m,  // 1.5% for 6-12 months remaining
            > 3 => 0.01m,   // 1% for 3-6 months remaining
            _ => 0.005m     // 0.5% for < 3 months remaining
        };

        var currentBalance = GetCurrentBalance();
        return new Money(currentBalance.Amount * penaltyRate, currentBalance.Currency);
    }

    public int GetMonthsToMaturity()
    {
        if (Status != DepositStatus.Active)
            return 0;

        var monthsRemaining = ((MaturityDate.Year - DateTime.UtcNow.Year) * 12) + 
                             MaturityDate.Month - DateTime.UtcNow.Month;
        return Math.Max(0, monthsRemaining);
    }

    public bool CanWithdraw(Money amount)
    {
        if (!AllowPartialWithdrawal || Status != DepositStatus.Active)
            return false;

        var currentBalance = GetCurrentBalance();
        var balanceAfterWithdrawal = currentBalance.Amount - amount.Amount;
        
        return balanceAfterWithdrawal >= MinimumBalance.Amount;
    }

    public decimal GetEffectiveInterestRate()
    {
        if (Status != DepositStatus.Active)
            return 0;

        var monthsElapsed = ((DateTime.UtcNow.Year - BookingDate.Year) * 12) + 
                           DateTime.UtcNow.Month - BookingDate.Month;
        
        if (monthsElapsed == 0)
            return InterestRate.Rate;

        var currentBalance = GetCurrentBalance();
        var totalReturn = currentBalance.Amount - PrincipalAmount.Amount + WithdrawnAmount.Amount;
        var annualizedReturn = (totalReturn / PrincipalAmount.Amount) * (12m / monthsElapsed) * 100;
        
        return annualizedReturn;
    }
}

/// <summary>
/// Individual transaction within a term deposit
/// </summary>
public class TermDepositTransaction
{
    public Guid Id { get; private set; }
    public Guid TermDepositId { get; private set; }
    public TermDepositTransactionType TransactionType { get; private set; }
    public Money Amount { get; private set; }
    public Money BalanceAfter { get; private set; }
    public string Description { get; private set; }
    public DateTime TransactionDate { get; private set; }

    private TermDepositTransaction() { Id = Guid.NewGuid(); } // EF Core

    public TermDepositTransaction(
        Guid id,
        Guid termDepositId,
        TermDepositTransactionType transactionType,
        Money amount,
        Money balanceAfter,
        string description,
        DateTime transactionDate)
    {
        Id = id;
        TermDepositId = termDepositId;
        TransactionType = transactionType;
        Amount = amount;
        BalanceAfter = balanceAfter;
        Description = description;
        TransactionDate = transactionDate;
    }
}

// Enums
public enum TermDepositTransactionType
{
    Opening = 1,
    InterestAccrual = 2,
    PartialWithdrawal = 3,
    Maturity = 4,
    PrematureClosure = 5,
    Renewal = 6
}

// Domain Events
public record TermDepositBookedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Guid CustomerId,
    Money PrincipalAmount,
    DateTime MaturityDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record TermDepositPartialWithdrawalDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Money WithdrawalAmount,
    Money RemainingBalance) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record TermDepositMaturedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Guid CustomerId,
    Money MaturityAmount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record TermDepositClosedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Guid CustomerId,
    Money ClosureAmount,
    bool IsPremature) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record TermDepositRenewedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Money NewPrincipalAmount,
    DateTime NewMaturityDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

