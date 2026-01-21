namespace Wekeza.Core.Application.Features.Treasury.Commands.ExecuteFXDeal;

/// <summary>
/// Response for FX Deal execution
/// </summary>
public record ExecuteFXDealResponse
{
    public Guid DealId { get; init; }
    public string DealNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal ExchangeRate { get; init; }
    public decimal FromAmount { get; init; }
    public decimal ToAmount { get; init; }
    public string FromCurrency { get; init; } = string.Empty;
    public string ToCurrency { get; init; } = string.Empty;
    public DateTime ValueDate { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool RequiresApproval { get; init; }
    public Guid? WorkflowId { get; init; }
}