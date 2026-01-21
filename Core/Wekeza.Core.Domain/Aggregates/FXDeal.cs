using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

public class FXDeal : AggregateRoot
{
    public string DealNumber { get; private set; }
    public Guid CounterpartyId { get; private set; }
    public FXDealType DealType { get; private set; }
    public string BaseCurrency { get; private set; }
    public string QuoteCurrency { get; private set; }
    public Money BaseAmount { get; private set; }
    public Money QuoteAmount { get; private set; }
    public ExchangeRate Rate { get; private set; }
    public DateTime TradeDate { get; private set; }
    public DateTime ValueDate { get; private set; }
    public DateTime? MaturityDate { get; private set; } // For forwards
    public DealStatus Status { get; private set; }
    public string TraderId { get; private set; }
    public string? SettlementInstructions { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private FXDeal() : base(Guid.NewGuid()) { }

    public static FXDeal Execute(
        string dealNumber,
        Guid counterpartyId,
        FXDealType dealType,
        string baseCurrency,
        string quoteCurrency,
        Money baseAmount,
        ExchangeRate rate,
        DateTime valueDate,
        string traderId,
        DateTime? maturityDate = null)
    {
        if (string.IsNullOrWhiteSpace(dealNumber))
            throw new ArgumentException("Deal number cannot be empty", nameof(dealNumber));

        if (counterpartyId == Guid.Empty)
            throw new ArgumentException("Counterparty ID cannot be empty", nameof(counterpartyId));

        if (baseAmount.Amount <= 0)
            throw new ArgumentException("Base amount must be positive", nameof(baseAmount));

        if (baseAmount.Currency != baseCurrency)
            throw new ArgumentException("Base amount currency must match base currency", nameof(baseAmount));

        if (baseCurrency == quoteCurrency)
            throw new ArgumentException("Base and quote currencies cannot be the same", nameof(baseCurrency));

        if (valueDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Value date cannot be in the past", nameof(valueDate));

        if (dealType == FXDealType.Forward && !maturityDate.HasValue)
            throw new ArgumentException("Maturity date required for forward deals", nameof(maturityDate));

        if (maturityDate.HasValue && maturityDate <= valueDate)
            throw new ArgumentException("Maturity date must be after value date", nameof(maturityDate));

        // Calculate quote amount
        var quoteAmount = new Money(baseAmount.Amount * rate.Rate, Currency.FromCode(quoteCurrency));

        var deal = new FXDeal
        {
            Id = Guid.NewGuid(),
            DealNumber = dealNumber,
            CounterpartyId = counterpartyId,
            DealType = dealType,
            BaseCurrency = baseCurrency,
            QuoteCurrency = quoteCurrency,
            BaseAmount = baseAmount,
            QuoteAmount = quoteAmount,
            Rate = rate,
            TradeDate = DateTime.UtcNow.Date,
            ValueDate = valueDate,
            MaturityDate = maturityDate,
            Status = DealStatus.Booked,
            TraderId = traderId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        deal.AddDomainEvent(new FXDealExecutedDomainEvent(
            deal.Id, deal.DealNumber, deal.CounterpartyId, deal.BaseAmount, deal.QuoteAmount, deal.Rate));

        return deal;
    }

    public void Settle(string? settlementInstructions = null)
    {
        if (Status != DealStatus.Booked)
            throw new InvalidOperationException($"Cannot settle deal in {Status} status");

        if (DateTime.UtcNow.Date < ValueDate)
            throw new InvalidOperationException("Cannot settle before value date");

        Status = DealStatus.Settled;
        SettlementInstructions = settlementInstructions;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new FXDealSettledDomainEvent(Id, DealNumber, BaseAmount, QuoteAmount));
    }

    public void Mature()
    {
        if (DealType != FXDealType.Forward)
            throw new InvalidOperationException("Only forward deals can mature");

        if (Status != DealStatus.Settled)
            throw new InvalidOperationException($"Cannot mature deal in {Status} status");

        if (!MaturityDate.HasValue || DateTime.UtcNow.Date < MaturityDate.Value)
            throw new InvalidOperationException("Cannot mature before maturity date");

        Status = DealStatus.Matured;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new FXDealMaturedDomainEvent(Id, DealNumber, BaseAmount, QuoteAmount));
    }

    public void Cancel(string cancellationReason)
    {
        if (Status == DealStatus.Settled || Status == DealStatus.Matured)
            throw new InvalidOperationException($"Cannot cancel deal in {Status} status");

        Status = DealStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new FXDealCancelledDomainEvent(Id, DealNumber, cancellationReason));
    }

    public void UpdateRate(ExchangeRate newRate, string reason)
    {
        if (Status != DealStatus.Booked)
            throw new InvalidOperationException($"Cannot update rate for deal in {Status} status");

        var oldRate = Rate;
        Rate = newRate;
        
        // Recalculate quote amount
        QuoteAmount = new Money(BaseAmount.Amount * newRate.Rate, Currency.FromCode(QuoteCurrency));
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new FXRateUpdatedDomainEvent(Id, DealNumber, oldRate, newRate, reason));
    }

    public Money GetUnrealizedPnL(ExchangeRate currentMarketRate)
    {
        if (Status != DealStatus.Settled)
            return new Money(0, Currency.FromCode(BaseCurrency));

        var currentValue = BaseAmount.Amount * currentMarketRate.Rate;
        var bookedValue = QuoteAmount.Amount;
        var pnlAmount = currentValue - bookedValue;

        return new Money(pnlAmount, Currency.FromCode(QuoteCurrency));
    }

    public bool IsSpot => DealType == FXDealType.Spot;
    public bool IsForward => DealType == FXDealType.Forward;
    public bool IsSwap => DealType == FXDealType.Swap;
    public bool IsMatured => MaturityDate.HasValue && DateTime.UtcNow.Date >= MaturityDate.Value;
    public bool IsActive => Status == DealStatus.Booked || Status == DealStatus.Settled;
    public int? DaysToMaturity => MaturityDate.HasValue 
        ? Math.Max(0, (MaturityDate.Value - DateTime.UtcNow.Date).Days) 
        : null;
}

public enum FXDealType
{
    Spot,       // Immediate delivery (T+2)
    Forward,    // Future delivery
    Swap,       // Combination of spot and forward
    Option      // Currency option (basic)
}


