using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Treasury.Commands.BookMoneyMarketDeal;

public record BookMoneyMarketDealCommand : ICommand<BookMoneyMarketDealResponse>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string DealNumber { get; init; } = string.Empty;
    public Guid CounterpartyId { get; init; }
    public MoneyMarketDealType DealType { get; init; }
    public decimal Principal { get; init; }
    public string Currency { get; init; } = "USD";
    public double InterestRate { get; init; }
    public DateTime ValueDate { get; init; }
    public DateTime MaturityDate { get; init; }
    public string TraderId { get; init; } = string.Empty;
    public string? CollateralReference { get; init; }
    public string Terms { get; init; } = string.Empty;
}

public record BookMoneyMarketDealResponse
{
    public Guid DealId { get; init; }
    public string DealNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal MaturityAmount { get; init; }
    public DateTime BookingTime { get; init; }
}