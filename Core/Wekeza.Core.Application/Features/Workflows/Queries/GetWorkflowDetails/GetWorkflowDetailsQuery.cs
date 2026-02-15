using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Workflows.Queries.GetWorkflowDetails;

[Authorize(UserRole.Teller, UserRole.RiskOfficer, UserRole.Administrator)]
public record GetWorkflowDetailsQuery : IQuery<WorkflowDetailsDto>
{
    public Guid WorkflowId { get; init; }
}

public record WorkflowDetailsDto
{
    public Guid WorkflowId { get; init; }
    public string WorkflowCode { get; init; } = string.Empty;
    public string WorkflowName { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string EntityReference { get; init; } = string.Empty;
    public int CurrentLevel { get; init; }
    public int RequiredLevels { get; init; }
    public DateTime InitiatedDate { get; init; }
    public string InitiatedBy { get; init; } = string.Empty;
    public DateTime? CompletedDate { get; init; }
    public string? CompletedBy { get; init; }
    public DateTime? DueDate { get; init; }
    public bool IsOverdue { get; init; }
    public bool IsEscalated { get; init; }
    public List<ApprovalStepDto> ApprovalSteps { get; init; } = new();
    public List<CommentDto> Comments { get; init; } = new();
    public string RequestData { get; init; } = string.Empty;
}

public record ApprovalStepDto
{
    public int Level { get; init; }
    public string? AssignedTo { get; init; }
    public DateTime AssignedDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? ApprovedBy { get; init; }
    public DateTime? ApprovedDate { get; init; }
    public string? Comments { get; init; }
    public string? ApproverRole { get; init; }
}

public record CommentDto
{
    public string CommentBy { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
    public DateTime CommentDate { get; init; }
}
