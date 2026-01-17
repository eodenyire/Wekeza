using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Recurring Deposit aggregate - Represents systematic investment plans with regular deposits
/// Core banking liability product for customer savings with disciplined investment
/// </summary>
public class RecurringDeposit : AggregateRoot
{
    public Guid AccountId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string DepositNumber { get; private set; }
    public Money MonthlyInstallment { get; private set; }
    public InterestRate InterestRate { get; private set; }
    public int TenureInMonths { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime MaturityDate { get; private set; }
    public Money MaturityAmount { get; private set; }
    public DepositStatus Status { get; private set; }
    public Money TotalDeposited { get; private set; }
    public Money AccruedInterest { get; private set; }
    public int InstallmentsPaid { get; private set; }
    public int InstallmentsMissed { get; private set; }
    public DateTime? LastInstallmentDate { get; private set; }
    public DateTime NextInstallmentDue { get; private set; }
    public bool AutoDebit { get; private set; }
    public Guid? AutoDebitAccountId { get; private set; }
    public Money PenaltyAmount { get; private set; }
    public DateTime? PrematureClosureDate { get; private set; }
    public string? ClosureReason { get; private set; }
    public string BranchCode { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }

    // Navigation properties
    public virtual Account Account { get; private set; } = null!;
    public virtual Customer Customer { get; private set; } = null!;
    public virtual Account? AutoDebitAccount { get; private set; }

    private FixedDeposit() { } // EF Core

    public RecurringDeposit(
        Guid id,
        Guid accountId,
        Guid customerId,
        string depositNumber,
        Money monthlyInstallment,
        InterestRate interestRate,
        int tenureInMonths,
        bool autoDebit,
        Guid? autoDebitAccountId,
        string branchCode,
        string createdBy)
    {
        Id = id;
        AccountId = accountId;
        CustomerId = customerId;
        DepositNumber = depositNumber;
        MonthlyInstallment = monthlyInstallment;
        InterestRate = interestRate;
        TenureInMonths = tenureInMonths;
        AutoDebit = autoDebit;
        AutoDebitAccountId = autoDebitAccountId;
        BranchCode = branchCode;
        CreatedBy = createdBy;
        
        StartDate = DateTime.UtcNow;
        MaturityDate = StartDate.AddMonths(tenureInMonths);
        NextInstallmentDue = StartDate.AddMonths(1);
        Status = DepositStatus.Active;
        TotalDeposited = new Money(0, monthlyInstallment.Currency);
        AccruedInterest = new Money(0, monthlyInstallment.Currency);
        PenaltyAmount = new Money(0, monthlyInstallment.Currency);
        InstallmentsPaid = 0;
        InstallmentsMissed = 0;
        CreatedAt = DateTime.UtcNow;

        // Calculate maturity amount
        CalculateMaturityAmount();

        // Add domain event
        AddDomainEvent(new RecurringDepositOpenedDomainEvent(Id, AccountId, CustomerId, MonthlyInstallment, TenureInMonths));
    }

    public void CalculateMaturityAmount()
    {
        // RD Maturity calculation: Sum of all installments + compound interest
        var totalInstallments = MonthlyInstallment.Amount * TenureInMonths;
        var monthlyRate = InterestRate.Rate / (12 * 100);
        
        // Compound interest formula for RD: P * [((1 + r)^n - 1) / r] * (1 + r)
        var compoundFactor = (decimal)(Math.Pow((double)(1 + monthlyRate), TenureInMonths) - 1) / monthlyRate;
        var maturityValue = MonthlyInstallment.Amount * compoundFactor * (1 + monthlyRate);
        
        MaturityAmount = new Money(maturityValue, MonthlyInstallment.Currency);
    }

    public void ProcessInstallment(Money installmentAmount, DateTime paymentDate, string processedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Cannot process installment for inactive deposit");

        if (installmentAmount.Amount < MonthlyInstallment.Amount)
            throw new InvalidOperationException("Installment amount is less than required monthly installment");

        TotalDeposited = new Money(TotalDeposited.Amount + installmentAmount.Amount, TotalDeposited.Currency);
        InstallmentsPaid++;
        LastInstallmentDate = paymentDate;
        NextInstallmentDue = NextInstallmentDue.AddMonths(1);
        ModifiedBy = processedBy;
        ModifiedAt = DateTime.UtcNow;

        // Calculate and accrue interest for this installment
        AccrueInterestForInstallment(installmentAmount, paymentDate);

        AddDomainEvent(new RecurringDepositInstallmentPaidDomainEvent(
            Id, AccountId, installmentAmount, InstallmentsPaid, paymentDate));

        // Check if deposit is complete
        if (InstallmentsPaid >= TenureInMonths)
        {
            ProcessMaturity(processedBy);
        }
    }

    private void AccrueInterestForInstallment(Money installmentAmount, DateTime paymentDate)
    {
        var monthsRemaining = TenureInMonths - InstallmentsPaid;
        var monthlyRate = InterestRate.Rate / (12 * 100);
        var interestForInstallment = installmentAmount.Amount * monthlyRate * monthsRemaining;
        
        AccruedInterest = new Money(AccruedInterest.Amount + interestForInstallment, AccruedInterest.Currency);
    }

    public void ProcessMissedInstallment(DateTime missedDate, string processedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Cannot process missed installment for inactive deposit");

        InstallmentsMissed++;
        NextInstallmentDue = NextInstallmentDue.AddMonths(1);
        
        // Apply penalty for missed installment
        var penaltyRate = 0.02m; // 2% of installment amount
        var penalty = MonthlyInstallment.Amount * penaltyRate;
        PenaltyAmount = new Money(PenaltyAmount.Amount + penalty, PenaltyAmount.Currency);
        
        ModifiedBy = processedBy;
        ModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new RecurringDepositInstallmentMissedDomainEvent(
            Id, AccountId, missedDate, InstallmentsMissed));

        // Check if deposit should be discontinued due to excessive missed payments
        if (InstallmentsMissed >= 3) // Allow maximum 3 missed installments
        {
            DiscontinueDeposit("Excessive missed installments", processedBy);
        }
    }

    public void ProcessMaturity(string processedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Deposit is not active");

        Status = DepositStatus.Matured;
        ModifiedBy = processedBy;
        ModifiedAt = DateTime.UtcNow;

        // Final maturity amount = total deposited + accrued interest - penalties
        var finalMaturityAmount = new Money(
            TotalDeposited.Amount + AccruedInterest.Amount - PenaltyAmount.Amount,
            TotalDeposited.Currency);

        AddDomainEvent(new RecurringDepositMaturedDomainEvent(Id, AccountId, CustomerId, finalMaturityAmount));
    }

    public void ProcessPrematureClosure(string reason, string processedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Deposit is not active");

        Status = DepositStatus.PrematurelyClosed;
        PrematureClosureDate = DateTime.UtcNow;
        ClosureReason = reason;
        ModifiedBy = processedBy;
        ModifiedAt = DateTime.UtcNow;

        // Calculate closure amount (reduced interest rate for premature closure)
        var prematureInterestRate = InterestRate.Rate * 0.75m; // 75% of original rate
        var prematureInterest = TotalDeposited.Amount * (prematureInterestRate / 100) * 
                               (InstallmentsPaid / (decimal)TenureInMonths);
        
        var closureAmount = new Money(
            TotalDeposited.Amount + prematureInterest - PenaltyAmount.Amount,
            TotalDeposited.Currency);

        AddDomainEvent(new RecurringDepositClosedDomainEvent(Id, AccountId, CustomerId, closureAmount, true));
    }

    public void DiscontinueDeposit(string reason, string processedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Deposit is not active");

        Status = DepositStatus.Discontinued;
        ClosureReason = reason;
        ModifiedBy = processedBy;
        ModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new RecurringDepositDiscontinuedDomainEvent(Id, AccountId, reason));
    }

    public void UpdateAutoDebitSettings(bool autoDebit, Guid? autoDebitAccountId, string updatedBy)
    {
        AutoDebit = autoDebit;
        AutoDebitAccountId = autoDebitAccountId;
        ModifiedBy = updatedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public bool IsInstallmentOverdue()
    {
        return Status == DepositStatus.Active && DateTime.UtcNow > NextInstallmentDue.AddDays(7); // 7 days grace period
    }

    public int GetOverdueDays()
    {
        if (!IsInstallmentOverdue())
            return 0;

        return (DateTime.UtcNow - NextInstallmentDue).Days;
    }

    public Money GetCurrentValue()
    {
        return new Money(TotalDeposited.Amount + AccruedInterest.Amount - PenaltyAmount.Amount, 
                        TotalDeposited.Currency);
    }

    public decimal GetCompletionPercentage()
    {
        if (TenureInMonths == 0) return 0;
        return (InstallmentsPaid / (decimal)TenureInMonths) * 100;
    }

    public Money GetProjectedMaturityAmount()
    {
        if (Status != DepositStatus.Active)
            return GetCurrentValue();

        // Project maturity amount based on current progress
        var remainingInstallments = TenureInMonths - InstallmentsPaid;
        var projectedTotalDeposits = TotalDeposited.Amount + (MonthlyInstallment.Amount * remainingInstallments);
        
        // Recalculate with projected deposits
        var monthlyRate = InterestRate.Rate / (12 * 100);
        var projectedInterest = projectedTotalDeposits * monthlyRate * (TenureInMonths / 2m); // Average interest
        
        return new Money(projectedTotalDeposits + projectedInterest - PenaltyAmount.Amount, 
                        TotalDeposited.Currency);
    }
}

// Domain Events
public record RecurringDepositOpenedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Guid CustomerId,
    Money MonthlyInstallment,
    int TenureInMonths) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record RecurringDepositInstallmentPaidDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Money InstallmentAmount,
    int InstallmentNumber,
    DateTime PaymentDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record RecurringDepositInstallmentMissedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    DateTime MissedDate,
    int TotalMissedInstallments) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record RecurringDepositMaturedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Guid CustomerId,
    Money MaturityAmount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record RecurringDepositClosedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Guid CustomerId,
    Money ClosureAmount,
    bool IsPremature) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record RecurringDepositDiscontinuedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}