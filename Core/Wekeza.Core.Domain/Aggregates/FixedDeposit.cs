using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Fixed Deposit aggregate - Represents time deposits with fixed terms and interest rates
/// Core banking liability product for customer investments
/// </summary>
public class FixedDeposit : AggregateRoot
{
    public Guid AccountId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string DepositNumber { get; private set; }
    public Money PrincipalAmount { get; private set; }
    public InterestRate InterestRate { get; private set; }
    public int TermInDays { get; private set; }
    public DateTime BookingDate { get; private set; }
    public DateTime MaturityDate { get; private set; }
    public Money MaturityAmount { get; private set; }
    public DepositStatus Status { get; private set; }
    public InterestPaymentFrequency InterestFrequency { get; private set; }
    public bool AutoRenewal { get; private set; }
    public string? RenewalInstructions { get; private set; }
    public Money AccruedInterest { get; private set; }
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

    private FixedDeposit() { } // EF Core

    public FixedDeposit(
        Guid id,
        Guid accountId,
        Guid customerId,
        string depositNumber,
        Money principalAmount,
        InterestRate interestRate,
        int termInDays,
        InterestPaymentFrequency interestFrequency,
        bool autoRenewal,
        string branchCode,
        string createdBy)
    {
        Id = id;
        AccountId = accountId;
        CustomerId = customerId;
        DepositNumber = depositNumber;
        PrincipalAmount = principalAmount;
        InterestRate = interestRate;
        TermInDays = termInDays;
        InterestFrequency = interestFrequency;
        AutoRenewal = autoRenewal;
        BranchCode = branchCode;
        CreatedBy = createdBy;
        
        BookingDate = DateTime.UtcNow;
        MaturityDate = BookingDate.AddDays(termInDays);
        Status = DepositStatus.Active;
        AccruedInterest = new Money(0, principalAmount.Currency);
        CreatedAt = DateTime.UtcNow;

        // Calculate maturity amount
        CalculateMaturityAmount();

        // Add domain event
        AddDomainEvent(new FixedDepositBookedDomainEvent(Id, AccountId, CustomerId, PrincipalAmount, MaturityDate));
    }

    public void CalculateMaturityAmount()
    {
        // Simple interest calculation: P + (P * R * T / 365)
        var interestAmount = PrincipalAmount.Amount * (InterestRate.Rate / 100) * (TermInDays / 365m);
        MaturityAmount = new Money(PrincipalAmount.Amount + interestAmount, PrincipalAmount.Currency);
    }

    public void AccrueInterest(Money interestAmount, DateTime accrualDate)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Cannot accrue interest on inactive deposit");

        AccruedInterest = new Money(AccruedInterest.Amount + interestAmount.Amount, AccruedInterest.Currency);
        LastInterestPostingDate = accrualDate;
        ModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new InterestAccruedDomainEvent(Id, AccountId, interestAmount, accrualDate));
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

        AddDomainEvent(new FixedDepositMaturedDomainEvent(Id, AccountId, CustomerId, MaturityAmount));
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

        // Calculate closure amount (principal + accrued interest - penalty)
        var closureAmount = new Money(
            PrincipalAmount.Amount + AccruedInterest.Amount - penaltyAmount.Amount,
            PrincipalAmount.Currency);

        AddDomainEvent(new FixedDepositClosedDomainEvent(Id, AccountId, CustomerId, closureAmount, true));
    }

    public void Renew(InterestRate newInterestRate, int newTermInDays, string renewedBy)
    {
        if (Status != DepositStatus.Matured)
            throw new InvalidOperationException("Only matured deposits can be renewed");

        if (!AutoRenewal)
            throw new InvalidOperationException("Auto-renewal is not enabled for this deposit");

        // Reset for new term
        PrincipalAmount = MaturityAmount; // Use maturity amount as new principal
        InterestRate = newInterestRate;
        TermInDays = newTermInDays;
        BookingDate = DateTime.UtcNow;
        MaturityDate = BookingDate.AddDays(newTermInDays);
        Status = DepositStatus.Active;
        AccruedInterest = new Money(0, PrincipalAmount.Currency);
        LastInterestPostingDate = null;
        ModifiedBy = renewedBy;
        ModifiedAt = DateTime.UtcNow;

        CalculateMaturityAmount();

        AddDomainEvent(new FixedDepositRenewedDomainEvent(Id, AccountId, PrincipalAmount, MaturityDate));
    }

    public void UpdateRenewalInstructions(string instructions, string updatedBy)
    {
        RenewalInstructions = instructions;
        ModifiedBy = updatedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ToggleAutoRenewal(bool autoRenewal, string updatedBy)
    {
        AutoRenewal = autoRenewal;
        ModifiedBy = updatedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public bool IsEligibleForPrematureClosure()
    {
        return Status == DepositStatus.Active && 
               DateTime.UtcNow > BookingDate.AddDays(7); // Minimum 7 days lock-in period
    }

    public Money CalculatePrematurePenalty()
    {
        if (!IsEligibleForPrematureClosure())
            return new Money(0, PrincipalAmount.Currency);

        // Calculate penalty as percentage of principal (e.g., 1% penalty)
        var penaltyRate = 0.01m; // 1%
        var penaltyAmount = PrincipalAmount.Amount * penaltyRate;
        
        return new Money(penaltyAmount, PrincipalAmount.Currency);
    }

    public int GetDaysToMaturity()
    {
        if (Status != DepositStatus.Active)
            return 0;

        var daysRemaining = (MaturityDate - DateTime.UtcNow).Days;
        return Math.Max(0, daysRemaining);
    }

    public Money GetCurrentValue()
    {
        return new Money(PrincipalAmount.Amount + AccruedInterest.Amount, PrincipalAmount.Currency);
    }
}

// Domain Events
public record FixedDepositBookedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Guid CustomerId,
    Money PrincipalAmount,
    DateTime MaturityDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record FixedDepositMaturedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Guid CustomerId,
    Money MaturityAmount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record FixedDepositClosedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Guid CustomerId,
    Money ClosureAmount,
    bool IsPremature) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record FixedDepositRenewedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Money NewPrincipalAmount,
    DateTime NewMaturityDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}