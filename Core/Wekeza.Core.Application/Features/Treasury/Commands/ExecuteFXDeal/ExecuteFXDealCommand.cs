using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Treasury.Commands.ExecuteFXDeal;

/// <summary>
/// Command to execute Foreign Exchange (FX) deals
/// Supports spot, forward, and swap transactions
/// </summary>
[Authorize(UserRole.Administrator, UserRole.BranchManager)]
public record ExecuteFXDealCommand : IRequest<Result<ExecuteFXDealResponse>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    
    // Deal Basic Information
    public string DealType { get; init; } = string.Empty; // SPOT, FORWARD, SWAP
    public string DealNumber { get; init; } = string.Empty;
    public DateTime DealDate { get; init; }
    public DateTime ValueDate { get; init; }
    
    // Currency Pair
    public string BaseCurrency { get; init; } = string.Empty;
    public string QuoteCurrency { get; init; } = string.Empty;
    public decimal ExchangeRate { get; init; }
    public decimal Amount { get; init; }
    public string AmountCurrency { get; init; } = string.Empty; // Which currency the amount is in
    
    // Counterparty Information
    public Guid? CustomerId { get; init; } // For customer deals
    public CounterpartyDto? Counterparty { get; init; } // For interbank deals
    
    // Settlement Information
    public SettlementInstructionDto BaseCurrencySettlement { get; init; } = new();
    public SettlementInstructionDto QuoteCurrencySettlement { get; init; } = new();
    
    // Deal Terms
    public string Purpose { get; init; } = string.Empty;
    public string? Reference { get; init; }
    public List<string> SpecialInstructions { get; init; } = new();
    
    // Risk Management
    public decimal? StopLossRate { get; init; }
    public decimal? TakeProfitRate { get; init; }
    public string RiskCategory { get; init; } = "NORMAL";
    
    // Approval
    public bool RequiresApproval { get; init; } = true;
    public string? DealerComments { get; init; }
    
    // Forward Deal Specific
    public DateTime? MaturityDate { get; init; }
    public decimal? ForwardPoints { get; init; }
    
    // Swap Deal Specific
    public SwapLegDto? NearLeg { get; init; }
    public SwapLegDto? FarLeg { get; init; }
}

public record CounterpartyDto
{
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty; // BANK, BROKER, CORPORATE
    public string SwiftCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string ContactPerson { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}

public record SettlementInstructionDto
{
    public string Currency { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public string BankName { get; init; } = string.Empty;
    public string SwiftCode { get; init; } = string.Empty;
    public string CorrespondentBank { get; init; } = string.Empty;
    public string SpecialInstructions { get; init; } = string.Empty;
}

public record SwapLegDto
{
    public DateTime ValueDate { get; init; }
    public decimal ExchangeRate { get; init; }
    public decimal Amount { get; init; }
    public string Direction { get; init; } = string.Empty; // BUY, SELL
}

public record FXDealResult
{
    public Guid DealId { get; init; }
    public string DealNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime ExecutedAt { get; init; }
    public decimal ExchangeRate { get; init; }
    public decimal BaseCurrencyAmount { get; init; }
    public decimal QuoteCurrencyAmount { get; init; }
    public decimal ProfitLoss { get; init; }
    public string PLCurrency { get; init; } = string.Empty;
    public Guid? WorkflowId { get; init; }
    public string Message { get; init; } = string.Empty;
}