using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Workflows.Queries.GetPendingApprovals;

/// <summary>
/// Query to get pending approvals for current user
/// </summary>
[Authorize(UserRole.Teller, UserRole.RiskOfficer, UserRole.Administrator)]
public record GetPendingApprovalsQuery : IQuery<List<PendingApprovalDto>>;

public record PendingApprovalDto
{
    public Guid WorkflowId { get; init; }
    public string WorkflowName { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string EntityReference { get; init; } = string.Empty;
    public int CurrentLevel { get; init; }
    public int RequiredLevels { get; init; }
    public DateTime InitiatedDate { get; init; }
    public string InitiatedBy { get; init; } = string.Empty;
    public DateTime? DueDate { get; init; }
    public bool IsOverdue { get; init; }
    public bool IsEscalated { get; init; }
    public string RequestSummary { get; init; } = string.Empty;
}
