using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

public class MoneyMarketDeal : AggregateRoot
{
    public string DealNumber { get; private set; }
    public Guid CounterpartyId { get; private set; }
    public MoneyMarketDealType DealType { get; private set; }
    public Money Principal { get; private set; }
    public InterestRate Rate { get; private set; }
    public DateTime TradeDate { get; private set; }
    public DateTime ValueDate { get; private set; }
    public DateTime MaturityDate { get; private set; }
    public DealStatus Status { get; private set; }
    public string? CollateralReference { get; private set; }
    public Money? AccruedInterest { get; private set; }
    public Money? MaturityAmount { get; private set; }
    public string Terms { get; private set; }
    public string TraderId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private MoneyMarketDeal() { }

    public static MoneyMarketDeal Book(
        string dealNumber,
        Guid counterpartyId,
        MoneyMarketDealType dealType,
        Money principal,
        InterestRate rate,
        DateTime valueDate,
        DateTime maturityDate,
        string traderId,
        string? collateralReference = null,
        string terms = "")
    {
        if (string.IsNullOrWhiteSpace(dealNumber))
            throw new ArgumentException("Deal number cannot be empty", nameof(dealNumber));

        if (counterpartyId == Guid.Empty)
            throw new ArgumentException("Counterparty ID cannot be empty", nameof(counterpartyId));

        if (principal.Amount <= 0)
            throw new ArgumentException("Principal amount must be positive", nameof(principal));

        if (valueDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Value date cannot be in the past", nameof(valueDate));

        if (maturityDate <= valueDate)
            throw new ArgumentException("Maturity date must be after value date", nameof(maturityDate));

        if (dealType == MoneyMarketDealType.Repo && string.IsNullOrWhiteSpace(collateralReference))
            throw new ArgumentException("Collateral reference required for repo deals", nameof(collateralReference));

        var deal = new MoneyMarketDeal
        {
            Id = Guid.NewGuid(),
            DealNumber = dealNumber,
            CounterpartyId = counterpartyId,
            DealType = dealType,
            Principal = principal,
            Rate = rate,
            TradeDate = DateTime.UtcNow.Date,
            ValueDate = valueDate,
            MaturityDate = maturityDate,
            Status = DealStatus.Booked,
            CollateralReference = collateralReference,
            Terms = terms ?? string.Empty,
            TraderId = traderId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Calculate maturity amount
        deal.CalculateMaturityAmount();

        deal.AddDomainEvent(new MoneyMarketDealBookedDomainEvent(
            deal.Id, deal.DealNumber, deal.CounterpartyId, deal.Principal, deal.DealType));

        return deal;
    }

    public void Settle()
    {
        if (Status != DealStatus.Booked)
            throw new InvalidOperationException($"Cannot settle deal in {Status} status");

        if (DateTime.UtcNow.Date < ValueDate)
            throw new InvalidOperationException("Cannot settle before value date");

        Status = DealStatus.Settled;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new MoneyMarketDealSettledDomainEvent(Id, DealNumber, Principal));
    }

    public void Mature()
    {
        if (Status != DealStatus.Settled)
            throw new InvalidOperationException($"Cannot mature deal in {Status} status");

        if (DateTime.UtcNow.Date < MaturityDate)
            throw new InvalidOperationException("Cannot mature before maturity date");

        Status = DealStatus.Matured;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new MoneyMarketDealMaturedDomainEvent(Id, DealNumber, MaturityAmount!));
    }

    public void Cancel(string cancellationReason)
    {
        if (Status == DealStatus.Settled || Status == DealStatus.Matured)
            throw new InvalidOperationException($"Cannot cancel deal in {Status} status");

        Status = DealStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new MoneyMarketDealCancelledDomainEvent(Id, DealNumber, cancellationReason));
    }

    public void Rollover(DateTime newMaturityDate, InterestRate? newRate = null)
    {
        if (Status != DealStatus.Matured)
            throw new InvalidOperationException($"Cannot rollover deal in {Status} status");

        if (newMaturityDate <= MaturityDate)
            throw new ArgumentException("New maturity date must be after current maturity", nameof(newMaturityDate));

        MaturityDate = newMaturityDate;
        if (newRate.HasValue)
            Rate = newRate.Value;

        Status = DealStatus.Settled; // Reset to settled for new period
        UpdatedAt = DateTime.UtcNow;

        // Recalculate maturity amount with new terms
        CalculateMaturityAmount();

        AddDomainEvent(new MoneyMarketDealRolloverDomainEvent(Id, DealNumber, newMaturityDate, Rate));
    }

    public void UpdateAccruedInterest()
    {
        if (Status != DealStatus.Settled)
            return;

        var daysSinceValue = (DateTime.UtcNow.Date - ValueDate).Days;
        var totalDays = (MaturityDate - ValueDate).Days;

        if (daysSinceValue <= 0 || totalDays <= 0)
        {
            AccruedInterest = new Money(0, Principal.Currency);
            return;
        }

        var dailyRate = Rate.Value / 365; // Simple interest calculation
        var accruedDays = Math.Min(daysSinceValue, totalDays);
        var accruedAmount = Principal.Amount * (decimal)dailyRate * accruedDays;

        AccruedInterest = new Money(accruedAmount, Principal.Currency);
        UpdatedAt = DateTime.UtcNow;
    }

    private void CalculateMaturityAmount()
    {
        var totalDays = (MaturityDate - ValueDate).Days;
        var interestAmount = Principal.Amount * (decimal)Rate.Value * totalDays / 365;
        MaturityAmount = new Money(Principal.Amount + interestAmount, Principal.Currency);
    }

    public bool IsMatured => DateTime.UtcNow.Date >= MaturityDate;
    public bool IsActive => Status == DealStatus.Booked || Status == DealStatus.Settled;
    public int DaysToMaturity => Math.Max(0, (MaturityDate - DateTime.UtcNow.Date).Days);
    public Money CurrentValue => AccruedInterest != null 
        ? new Money(Principal.Amount + AccruedInterest.Amount, Principal.Currency)
        : Principal;
}

public enum MoneyMarketDealType
{
    CallMoney,      // Overnight lending/borrowing
    TermDeposit,    // Fixed term deposits
    Repo,           // Repurchase agreement
    ReverseRepo,    // Reverse repurchase agreement
    CommercialPaper, // CP investments
    CertificateOfDeposit // CD issuance
}

public enum DealStatus
{
    Booked,
    Settled,
    Matured,
    Cancelled,
    Defaulted
}