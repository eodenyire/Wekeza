using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Treasury.Commands.ExecuteFXDeal;

public record ExecuteFXDealCommand : ICommand<ExecuteFXDealResponse>
{
    public string DealNumber { get; init; } = string.Empty;
    public Guid CounterpartyId { get; init; }
    public FXDealType DealType { get; init; }
    public string BaseCurrency { get; init; } = string.Empty;
    public string QuoteCurrency { get; init; } = string.Empty;
    public decimal BaseAmount { get; init; }
    public decimal ExchangeRate { get; init; }
    public decimal Spread { get; init; } = 0;
    public DateTime ValueDate { get; init; }
    public DateTime? MaturityDate { get; init; }
    public string TraderId { get; init; } = string.Empty;
    public string RateSource { get; init; } = "SYSTEM";
}

public record ExecuteFXDealResponse
{
    public Guid DealId { get; init; }
    public string DealNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal BaseAmount { get; init; }
    public decimal QuoteAmount { get; init; }
    public decimal ExchangeRate { get; init; }
    public DateTime ExecutionTime { get; init; }
}