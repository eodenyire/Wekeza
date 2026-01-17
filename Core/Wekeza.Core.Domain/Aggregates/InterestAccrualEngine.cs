using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Interest Accrual Engine - Handles interest calculations and accruals for all deposit products
/// Core component for automated interest processing in banking operations
/// </summary>
public class InterestAccrualEngine : AggregateRoot<Guid>
{
    public string EngineName { get; private set; }
    public DateTime ProcessingDate { get; private set; }
    public AccrualStatus Status { get; private set; }
    public int TotalAccountsProcessed { get; private set; }
    public int SuccessfulAccruals { get; private set; }
    public int FailedAccruals { get; private set; }
    public Money TotalInterestAccrued { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public string? ErrorDetails { get; private set; }
    public string ProcessedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<AccrualEntry> _accrualEntries = new();
    public IReadOnlyList<AccrualEntry> AccrualEntries => _accrualEntries.AsReadOnly();

    private InterestAccrualEngine() { } // EF Core

    public InterestAccrualEngine(
        Guid id,
        string engineName,
        DateTime processingDate,
        string processedBy)
    {
        Id = id;
        EngineName = engineName;
        ProcessingDate = processingDate;
        ProcessedBy = processedBy;
        Status = AccrualStatus.Initiated;
        TotalAccountsProcessed = 0;
        SuccessfulAccruals = 0;
        FailedAccruals = 0;
        TotalInterestAccrued = new Money(0, Currency.KES);
        StartTime = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new InterestAccrualInitiatedDomainEvent(Id, ProcessingDate));
    }

    public void StartProcessing()
    {
        if (Status != AccrualStatus.Initiated)
            throw new InvalidOperationException("Accrual engine is not in initiated state");

        Status = AccrualStatus.Processing;
        StartTime = DateTime.UtcNow;

        AddDomainEvent(new InterestAccrualStartedDomainEvent(Id, StartTime));
    }

    public void ProcessAccountInterest(
        Guid accountId,
        AccountType accountType,
        Money principalAmount,
        InterestRate interestRate,
        int daysInPeriod,
        InterestCalculationMethod calculationMethod)
    {
        if (Status != AccrualStatus.Processing)
            throw new InvalidOperationException("Accrual engine is not in processing state");

        try
        {
            var interestAmount = CalculateInterest(
                principalAmount, 
                interestRate, 
                daysInPeriod, 
                calculationMethod);

            var accrualEntry = new AccrualEntry(
                Guid.NewGuid(),
                Id,
                accountId,
                accountType,
                principalAmount,
                interestRate,
                interestAmount,
                daysInPeriod,
                calculationMethod,
                ProcessingDate,
                AccrualEntryStatus.Success);

            _accrualEntries.Add(accrualEntry);
            SuccessfulAccruals++;
            TotalInterestAccrued = new Money(
                TotalInterestAccrued.Amount + interestAmount.Amount,
                TotalInterestAccrued.Currency);

            AddDomainEvent(new InterestAccruedDomainEvent(accountId, interestAmount, ProcessingDate));
        }
        catch (Exception ex)
        {
            var failedEntry = new AccrualEntry(
                Guid.NewGuid(),
                Id,
                accountId,
                accountType,
                principalAmount,
                interestRate,
                new Money(0, principalAmount.Currency),
                daysInPeriod,
                calculationMethod,
                ProcessingDate,
                AccrualEntryStatus.Failed,
                ex.Message);

            _accrualEntries.Add(failedEntry);
            FailedAccruals++;
        }
        finally
        {
            TotalAccountsProcessed++;
        }
    }

    private Money CalculateInterest(
        Money principalAmount,
        InterestRate interestRate,
        int daysInPeriod,
        InterestCalculationMethod calculationMethod)
    {
        return calculationMethod switch
        {
            InterestCalculationMethod.SimpleInterest => 
                CalculateSimpleInterest(principalAmount, interestRate, daysInPeriod),
            
            InterestCalculationMethod.CompoundInterest => 
                CalculateCompoundInterest(principalAmount, interestRate, daysInPeriod, 1),
            
            InterestCalculationMethod.CompoundQuarterly => 
                CalculateCompoundInterest(principalAmount, interestRate, daysInPeriod, 4),
            
            InterestCalculationMethod.CompoundMonthly => 
                CalculateCompoundInterest(principalAmount, interestRate, daysInPeriod, 12),
            
            InterestCalculationMethod.CompoundDaily => 
                CalculateCompoundInterest(principalAmount, interestRate, daysInPeriod, 365),
            
            _ => throw new ArgumentException("Invalid interest calculation method")
        };
    }

    private Money CalculateSimpleInterest(Money principal, InterestRate rate, int days)
    {
        // Simple Interest = P * R * T / 100
        // Where T is in years (days/365)
        var interestAmount = principal.Amount * (rate.Rate / 100) * (days / 365m);
        return new Money(interestAmount, principal.Currency);
    }

    private Money CalculateCompoundInterest(Money principal, InterestRate rate, int days, int compoundingFrequency)
    {
        // Compound Interest = P * (1 + r/n)^(n*t) - P
        // Where r = annual rate, n = compounding frequency, t = time in years
        var annualRate = rate.Rate / 100;
        var timeInYears = days / 365m;
        var ratePerPeriod = annualRate / compoundingFrequency;
        var numberOfPeriods = compoundingFrequency * timeInYears;

        var compoundAmount = principal.Amount * 
            (decimal)Math.Pow((double)(1 + ratePerPeriod), (double)numberOfPeriods);
        
        var interestAmount = compoundAmount - principal.Amount;
        return new Money(interestAmount, principal.Currency);
    }

    public void CompleteProcessing()
    {
        if (Status != AccrualStatus.Processing)
            throw new InvalidOperationException("Accrual engine is not in processing state");

        Status = AccrualStatus.Completed;
        EndTime = DateTime.UtcNow;

        AddDomainEvent(new InterestAccrualCompletedDomainEvent(
            Id, 
            TotalAccountsProcessed, 
            SuccessfulAccruals, 
            FailedAccruals, 
            TotalInterestAccrued));
    }

    public void FailProcessing(string errorDetails)
    {
        if (Status != AccrualStatus.Processing)
            throw new InvalidOperationException("Accrual engine is not in processing state");

        Status = AccrualStatus.Failed;
        EndTime = DateTime.UtcNow;
        ErrorDetails = errorDetails;

        AddDomainEvent(new InterestAccrualFailedDomainEvent(Id, errorDetails));
    }

    public TimeSpan GetProcessingDuration()
    {
        if (EndTime.HasValue)
            return EndTime.Value - StartTime;
        
        return DateTime.UtcNow - StartTime;
    }

    public decimal GetSuccessRate()
    {
        if (TotalAccountsProcessed == 0) return 0;
        return (SuccessfulAccruals / (decimal)TotalAccountsProcessed) * 100;
    }

    public IEnumerable<AccrualEntry> GetFailedEntries()
    {
        return _accrualEntries.Where(e => e.Status == AccrualEntryStatus.Failed);
    }

    public Money GetInterestByAccountType(AccountType accountType)
    {
        var entries = _accrualEntries
            .Where(e => e.AccountType == accountType && e.Status == AccrualEntryStatus.Success);
        
        if (!entries.Any()) 
            return new Money(0, Currency.KES);

        var totalInterest = entries.Sum(e => e.InterestAmount.Amount);
        return new Money(totalInterest, Currency.KES);
    }
}

/// <summary>
/// Individual accrual entry for tracking interest calculations per account
/// </summary>
public class AccrualEntry
{
    public Guid Id { get; private set; }
    public Guid AccrualEngineId { get; private set; }
    public Guid AccountId { get; private set; }
    public AccountType AccountType { get; private set; }
    public Money PrincipalAmount { get; private set; }
    public InterestRate InterestRate { get; private set; }
    public Money InterestAmount { get; private set; }
    public int DaysInPeriod { get; private set; }
    public InterestCalculationMethod CalculationMethod { get; private set; }
    public DateTime ProcessingDate { get; private set; }
    public AccrualEntryStatus Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AccrualEntry() { } // EF Core

    public AccrualEntry(
        Guid id,
        Guid accrualEngineId,
        Guid accountId,
        AccountType accountType,
        Money principalAmount,
        InterestRate interestRate,
        Money interestAmount,
        int daysInPeriod,
        InterestCalculationMethod calculationMethod,
        DateTime processingDate,
        AccrualEntryStatus status,
        string? errorMessage = null)
    {
        Id = id;
        AccrualEngineId = accrualEngineId;
        AccountId = accountId;
        AccountType = accountType;
        PrincipalAmount = principalAmount;
        InterestRate = interestRate;
        InterestAmount = interestAmount;
        DaysInPeriod = daysInPeriod;
        CalculationMethod = calculationMethod;
        ProcessingDate = processingDate;
        Status = status;
        ErrorMessage = errorMessage;
        CreatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Status of interest accrual processing
/// </summary>
public enum AccrualStatus
{
    Initiated = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5
}

/// <summary>
/// Status of individual accrual entries
/// </summary>
public enum AccrualEntryStatus
{
    Success = 1,
    Failed = 2,
    Skipped = 3
}

// Domain Events
public record InterestAccrualInitiatedDomainEvent(
    Guid AccrualEngineId,
    DateTime ProcessingDate) : IDomainEvent;

public record InterestAccrualStartedDomainEvent(
    Guid AccrualEngineId,
    DateTime StartTime) : IDomainEvent;

public record InterestAccrualCompletedDomainEvent(
    Guid AccrualEngineId,
    int TotalAccountsProcessed,
    int SuccessfulAccruals,
    int FailedAccruals,
    Money TotalInterestAccrued) : IDomainEvent;

public record InterestAccrualFailedDomainEvent(
    Guid AccrualEngineId,
    string ErrorDetails) : IDomainEvent;