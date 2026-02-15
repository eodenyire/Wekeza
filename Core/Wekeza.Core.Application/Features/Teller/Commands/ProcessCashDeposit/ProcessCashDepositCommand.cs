using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Teller.Commands.ProcessCashDeposit;

/// <summary>
/// Command to process cash deposit at teller
/// </summary>
[Authorize(UserRole.Teller, UserRole.Supervisor, UserRole.BranchManager)]
public record ProcessCashDepositCommand : ICommand<Result<ProcessCashDepositResult>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
    public string? Narration { get; init; }
    public string? DepositorName { get; init; }
    public string? DepositorPhone { get; init; }
    public string? DepositorIdNumber { get; init; }
    public Dictionary<string, decimal> CashDenominations { get; init; } = new();
    public bool RequiresApproval { get; init; } = false;
}

public record ProcessCashDepositResult
{
    public Guid TransactionId { get; init; }
    public string TransactionReference { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public decimal NewBalance { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime ProcessedAt { get; init; }
}