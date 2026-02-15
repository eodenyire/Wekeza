using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Teller.Commands.StartSession;

/// <summary>
/// Command to start a teller session
/// </summary>
[Authorize(UserRole.Teller, UserRole.Supervisor, UserRole.BranchManager)]
public record StartTellerSessionCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    
    public Guid BranchId { get; init; }
    public string TellerCode { get; init; } = string.Empty;
    public string TellerName { get; init; } = string.Empty;
    public string BranchCode { get; init; } = string.Empty;
    public string WorkstationId { get; init; } = string.Empty;
    public decimal OpeningCashBalance { get; init; }
    public decimal DailyTransactionLimit { get; init; } = 1000000;
    public decimal SingleTransactionLimit { get; init; } = 100000;
    public decimal CashWithdrawalLimit { get; init; } = 500000;
    public Dictionary<string, decimal> CashDenominations { get; init; } = new();
    public string? Notes { get; init; }
}