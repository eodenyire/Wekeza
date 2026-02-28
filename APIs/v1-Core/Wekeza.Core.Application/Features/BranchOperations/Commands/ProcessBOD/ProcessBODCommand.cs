using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.BranchOperations.Commands.ProcessBOD;

/// <summary>
/// Command to process Beginning of Day (BOD) operations for a branch
/// Handles all daily startup procedures and validations
/// </summary>
[Authorize(UserRole.BranchManager, UserRole.Supervisor, UserRole.CashOfficer)]
public record ProcessBODCommand : IRequest<Result<BODResult>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    
    public Guid BranchId { get; init; }
    public DateTime BusinessDate { get; init; }
    public decimal CashOnHand { get; init; }
    public Dictionary<string, decimal> CashDenominations { get; init; } = new();
    public List<ATMStatusDto> ATMStatuses { get; init; } = new();
    public List<SystemCheckDto> SystemChecks { get; init; } = new();
    public string? Notes { get; init; }
    public bool ForceProcess { get; init; } = false;
    public string ProcessedBy { get; init; } = string.Empty;
}

public record ATMStatusDto
{
    public string ATMId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty; // ONLINE, OFFLINE, MAINTENANCE
    public decimal CashBalance { get; init; }
    public Dictionary<string, int> CashCassettes { get; init; } = new();
    public List<string> Issues { get; init; } = new();
}

public record SystemCheckDto
{
    public string SystemName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty; // ONLINE, OFFLINE, DEGRADED
    public DateTime LastChecked { get; init; }
    public string? ErrorMessage { get; init; }
}

public record BODResult
{
    public Guid ProcessId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime ProcessedAt { get; init; }
    public List<string> CompletedTasks { get; init; } = new();
    public List<string> FailedTasks { get; init; } = new();
    public List<string> Warnings { get; init; } = new();
    public decimal TotalCashPosition { get; init; }
    public int ActiveTellers { get; init; }
    public int OnlineATMs { get; init; }
    public string Message { get; init; } = string.Empty;
}