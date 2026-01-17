using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

public class SecurityDeal : AggregateRoot
{
    public string DealNumber { get; private set; }
    public string SecurityId { get; private set; } // ISIN or Ticker
    public SecurityType SecurityType { get; private set; }
    public TradeType TradeType { get; private set; }
    public decimal Quantity { get; private set; }
    public Money Price { get; private set; }
    public Money TotalAmount { get; private set; }
    public decimal? YieldRate { get; private set; }
    public Money? AccruedInterest { get; private set; }
    public DateTime TradeDate { get; private set; }
    public DateTime SettlementDate { get; private set; }
    public DealStatus Status { get; private set; }
    public Guid? CounterpartyId { get; private set; }
    public string TraderId { get; private set; }
    public string? SettlementInstructions { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private SecurityDeal() { }

    public static SecurityDeal Execute(
        string dealNumber,
        string securityId,
        SecurityType securityType,
        TradeType tradeType,
        decimal quantity,
        Money price,
        string traderId,
        DateTime? settlementDate = null,
        decimal? yieldRate = null,
        Guid? counterpartyId = null)
    {
        if (string.IsNullOrWhiteSpace(dealNumber))
            throw new ArgumentException("Deal number cannot be empty", nameof(dealNumber));

        if (string.IsNullOrWhiteSpace(securityId))
            throw new ArgumentException("Security ID cannot be empty", nameof(securityId));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (price.Amount <= 0)
            throw new ArgumentException("Price must be positive", nameof(price));

        var tradeDate = DateTime.UtcNow.Date;
        var defaultSettlementDate = settlementDate ?? CalculateSettlementDate(tradeDate, securityType);

        if (defaultSettlementDate < tradeDate)
            throw new ArgumentException("Settlement date cannot be before trade date", nameof(settlementDate));

        var totalAmount = new Money(quantity * price.Amount, price.Currency);

        var deal = new SecurityDeal
        {
            Id = Guid.NewGuid(),
            DealNumber = dealNumber,
            SecurityId = securityId,
            SecurityType = securityType,
            TradeType = tradeType,
            Quantity = quantity,
            Price = price,
            TotalAmount = totalAmount,
            YieldRate = yieldRate,
            TradeDate = tradeDate,
            SettlementDate = defaultSettlementDate,
            Status = DealStatus.Booked,
            CounterpartyId = counterpartyId,
            TraderId = traderId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Calculate accrued interest for bonds
        if (securityType == SecurityType.GovernmentBond || securityType == SecurityType.CorporateBond)
        {
            deal.CalculateAccruedInterest();
        }

        deal.AddDomainEvent(new SecurityDealExecutedDomainEvent(
            deal.Id, deal.DealNumber, deal.SecurityId, deal.TradeType, deal.Quantity, deal.Price));

        return deal;
    }

    public void Settle(string? settlementInstructions = null)
    {
        if (Status != DealStatus.Booked)
            throw new InvalidOperationException($"Cannot settle deal in {Status} status");

        if (DateTime.UtcNow.Date < SettlementDate)
            throw new InvalidOperationException("Cannot settle before settlement date");

        Status = DealStatus.Settled;
        SettlementInstructions = settlementInstructions;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SecurityDealSettledDomainEvent(Id, DealNumber, SecurityId, Quantity, TotalAmount));
    }

    public void Cancel(string cancellationReason)
    {
        if (Status == DealStatus.Settled)
            throw new InvalidOperationException($"Cannot cancel settled deal");

        Status = DealStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SecurityDealCancelledDomainEvent(Id, DealNumber, cancellationReason));
    }

    public void UpdatePrice(Money newPrice, string reason)
    {
        if (Status != DealStatus.Booked)
            throw new InvalidOperationException($"Cannot update price for deal in {Status} status");

        var oldPrice = Price;
        Price = newPrice;
        TotalAmount = new Money(Quantity * newPrice.Amount, newPrice.Currency);
        UpdatedAt = DateTime.UtcNow;

        // Recalculate accrued interest if applicable
        if (SecurityType == SecurityType.GovernmentBond || SecurityType == SecurityType.CorporateBond)
        {
            CalculateAccruedInterest();
        }

        AddDomainEvent(new SecurityPriceUpdatedDomainEvent(Id, DealNumber, oldPrice, newPrice, reason));
    }

    public Money GetMarketValue(Money currentMarketPrice)
    {
        return new Money(Quantity * currentMarketPrice.Amount, currentMarketPrice.Currency);
    }

    public Money GetUnrealizedPnL(Money currentMarketPrice)
    {
        if (Status != DealStatus.Settled)
            return new Money(0, Price.Currency);

        var currentValue = GetMarketValue(currentMarketPrice);
        var bookValue = TotalAmount;

        var pnlAmount = TradeType == TradeType.Buy 
            ? currentValue.Amount - bookValue.Amount
            : bookValue.Amount - currentValue.Amount;

        return new Money(pnlAmount, Price.Currency);
    }

    public void ReceiveCoupon(Money couponAmount, DateTime paymentDate)
    {
        if (SecurityType != SecurityType.GovernmentBond && SecurityType != SecurityType.CorporateBond)
            throw new InvalidOperationException("Only bonds can receive coupon payments");

        if (Status != DealStatus.Settled)
            throw new InvalidOperationException("Deal must be settled to receive coupon");

        if (TradeType != TradeType.Buy)
            throw new InvalidOperationException("Only buy positions can receive coupons");

        AddDomainEvent(new CouponReceivedDomainEvent(Id, DealNumber, SecurityId, couponAmount, paymentDate));
    }

    public void ReceiveDividend(Money dividendAmount, DateTime paymentDate)
    {
        if (SecurityType != SecurityType.Equity)
            throw new InvalidOperationException("Only equities can receive dividend payments");

        if (Status != DealStatus.Settled)
            throw new InvalidOperationException("Deal must be settled to receive dividend");

        if (TradeType != TradeType.Buy)
            throw new InvalidOperationException("Only buy positions can receive dividends");

        AddDomainEvent(new DividendReceivedDomainEvent(Id, DealNumber, SecurityId, dividendAmount, paymentDate));
    }

    private void CalculateAccruedInterest()
    {
        // Simplified accrued interest calculation
        // In reality, this would use bond-specific parameters (coupon rate, frequency, day count convention)
        if (!YieldRate.HasValue)
        {
            AccruedInterest = new Money(0, Price.Currency);
            return;
        }

        var daysSinceLastCoupon = 30; // Simplified - would calculate from last coupon date
        var annualCoupon = TotalAmount.Amount * (decimal)YieldRate.Value;
        var dailyCoupon = annualCoupon / 365;
        var accruedAmount = dailyCoupon * daysSinceLastCoupon;

        AccruedInterest = new Money(accruedAmount, Price.Currency);
    }

    private static DateTime CalculateSettlementDate(DateTime tradeDate, SecurityType securityType)
    {
        // Standard settlement cycles
        return securityType switch
        {
            SecurityType.GovernmentBond => tradeDate.AddBusinessDays(1), // T+1
            SecurityType.CorporateBond => tradeDate.AddBusinessDays(2),  // T+2
            SecurityType.Equity => tradeDate.AddBusinessDays(2),         // T+2
            SecurityType.MutualFund => tradeDate.AddBusinessDays(1),     // T+1
            _ => tradeDate.AddBusinessDays(2)                            // Default T+2
        };
    }

    public bool IsSettled => Status == DealStatus.Settled;
    public bool IsActive => Status == DealStatus.Booked || Status == DealStatus.Settled;
    public bool IsBond => SecurityType == SecurityType.GovernmentBond || SecurityType == SecurityType.CorporateBond;
    public bool IsEquity => SecurityType == SecurityType.Equity;
    public int DaysToSettlement => Math.Max(0, (SettlementDate - DateTime.UtcNow.Date).Days);
    public Money NetAmount => AccruedInterest != null 
        ? new Money(TotalAmount.Amount + AccruedInterest.Amount, TotalAmount.Currency)
        : TotalAmount;
}

public enum SecurityType
{
    GovernmentBond,
    CorporateBond,
    Equity,
    MutualFund,
    ETF,
    Commodity
}

public enum TradeType
{
    Buy,
    Sell
}

// Extension method for business day calculation
public static class DateTimeExtensions
{
    public static DateTime AddBusinessDays(this DateTime date, int businessDays)
    {
        var result = date;
        var daysAdded = 0;

        while (daysAdded < businessDays)
        {
            result = result.AddDays(1);
            if (result.DayOfWeek != DayOfWeek.Saturday && result.DayOfWeek != DayOfWeek.Sunday)
            {
                daysAdded++;
            }
        }

        return result;
    }
}