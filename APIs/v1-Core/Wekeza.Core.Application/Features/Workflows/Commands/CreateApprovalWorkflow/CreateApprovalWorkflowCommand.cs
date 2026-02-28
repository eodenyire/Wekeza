using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Workflows.Commands.CreateApprovalWorkflow;

/// <summary>
/// Command to create a new approval workflow
/// </summary>
public record CreateApprovalWorkflowCommand : IRequest<Result<Guid>>
{
    public Guid WorkflowId { get; init; }
    public string WorkflowCode { get; init; } = string.Empty;
    public string WorkflowName { get; init; } = string.Empty;
    public WorkflowType WorkflowType { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
    public decimal? Amount { get; init; }
    public string Currency { get; init; } = "KES";
    public Priority Priority { get; init; }
    public string InitiatedBy { get; init; } = string.Empty;
    public string BranchCode { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public bool RequiresMakerChecker { get; init; } = true;
    public List<ApprovalStepRequest> ApprovalSteps { get; init; } = new();
}

public record ApprovalStepRequest
{
    public int Level { get; init; }
    public string ApproverRole { get; init; } = string.Empty;
    public string? SpecificApprover { get; init; }
    public bool IsRequired { get; init; } = true;
    public decimal? MinimumAmount { get; init; }
    public decimal? MaximumAmount { get; init; }
}