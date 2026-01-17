using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Events;

// Money Market Events
public record MoneyMarketDealBookedDomainEvent(
    Guid DealId, 
    string DealNumber, 
    Guid CounterpartyId, 
    Money Principal, 
    MoneyMarketDealType DealType) : IDomainEvent;

public record MoneyMarketDealSettledDomainEvent(
    Guid DealId, 
    string DealNumber, 
    Money Principal) : IDomainEvent;

public record MoneyMarketDealMaturedDomainEvent(
    Guid DealId, 
    string DealNumber, 
    Money MaturityAmount) : IDomainEvent;

public record MoneyMarketDealCancelledDomainEvent(
    Guid DealId, 
    string DealNumber, 
    string CancellationReason) : IDomainEvent;

public record MoneyMarketDealRolloverDomainEvent(
    Guid DealId, 
    string DealNumber, 
    DateTime NewMaturityDate, 
    InterestRate NewRate) : IDomainEvent;

// FX Deal Events
public record FXDealExecutedDomainEvent(
    Guid DealId, 
    string DealNumber, 
    Guid CounterpartyId, 
    Money BaseAmount, 
    Money QuoteAmount, 
    ExchangeRate Rate) : IDomainEvent;

public record FXDealSettledDomainEvent(
    Guid DealId, 
    string DealNumber, 
    Money BaseAmount, 
    Money QuoteAmount) : IDomainEvent;

public record FXDealMaturedDomainEvent(
    Guid DealId, 
    string DealNumber, 
    Money BaseAmount, 
    Money QuoteAmount) : IDomainEvent;

public record FXDealCancelledDomainEvent(
    Guid DealId, 
    string DealNumber, 
    string CancellationReason) : IDomainEvent;

public record FXRateUpdatedDomainEvent(
    Guid DealId, 
    string DealNumber, 
    ExchangeRate OldRate, 
    ExchangeRate NewRate, 
    string Reason) : IDomainEvent;

// Security Deal Events
public record SecurityDealExecutedDomainEvent(
    Guid DealId, 
    string DealNumber, 
    string SecurityId, 
    TradeType TradeType, 
    decimal Quantity, 
    Money Price) : IDomainEvent;

public record SecurityDealSettledDomainEvent(
    Guid DealId, 
    string DealNumber, 
    string SecurityId, 
    decimal Quantity, 
    Money TotalAmount) : IDomainEvent;

public record SecurityDealCancelledDomainEvent(
    Guid DealId, 
    string DealNumber, 
    string CancellationReason) : IDomainEvent;

public record SecurityPriceUpdatedDomainEvent(
    Guid DealId, 
    string DealNumber, 
    Money OldPrice, 
    Money NewPrice, 
    string Reason) : IDomainEvent;

public record CouponReceivedDomainEvent(
    Guid DealId, 
    string DealNumber, 
    string SecurityId, 
    Money CouponAmount, 
    DateTime PaymentDate) : IDomainEvent;

public record DividendReceivedDomainEvent(
    Guid DealId, 
    string DealNumber, 
    string SecurityId, 
    Money DividendAmount, 
    DateTime PaymentDate) : IDomainEvent;

// Liquidity Events
public record LiquidityShortfallDomainEvent(
    DateTime PositionDate, 
    string Currency, 
    Money ShortfallAmount, 
    Money RequiredReserves) : IDomainEvent;

public record LiquidityExcessDomainEvent(
    DateTime PositionDate, 
    string Currency, 
    Money ExcessAmount, 
    Money AvailableForInvestment) : IDomainEvent;

public record LiquidityPositionUpdatedDomainEvent(
    DateTime PositionDate, 
    string Currency, 
    Money OpeningBalance, 
    Money ClosingBalance, 
    Money NetFlow) : IDomainEvent;

// Risk Events
public record RiskLimitBreachedDomainEvent(
    string LimitType, 
    string Currency, 
    Money CurrentExposure, 
    Money LimitAmount, 
    decimal UtilizationPercentage) : IDomainEvent;

public record VaRLimitExceededDomainEvent(
    DateTime CalculationDate, 
    string Portfolio, 
    Money VaRAmount, 
    Money VaRLimit, 
    decimal ConfidenceLevel) : IDomainEvent;

public record CounterpartyLimitBreachedDomainEvent(
    Guid CounterpartyId, 
    string CounterpartyName, 
    Money CurrentExposure, 
    Money LimitAmount, 
    string LimitType) : IDomainEvent;

// Market Data Events
public record MarketDataUpdatedDomainEvent(
    string DataType, 
    string Identifier, 
    decimal OldValue, 
    decimal NewValue, 
    DateTime Timestamp, 
    string Source) : IDomainEvent;

public record YieldCurveUpdatedDomainEvent(
    string Currency, 
    DateTime EffectiveDate, 
    Dictionary<string, decimal> YieldPoints, 
    string Source) : IDomainEvent;

public record FXRatesFeedUpdatedDomainEvent(
    DateTime Timestamp, 
    Dictionary<string, ExchangeRate> Rates, 
    string Source) : IDomainEvent;