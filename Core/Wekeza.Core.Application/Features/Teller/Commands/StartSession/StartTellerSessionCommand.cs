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
    public string WorkstationId { get; init; } = string.Empty;
    public decimal OpeningCashBalance { get; init; }
    public Dictionary<string, decimal> CashDenominations { get; init; } = new();
    public string? Notes { get; init; }
}