using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Workflows.Commands.InitiateWorkflow;

/// <summary>
/// Command to initiate a new workflow for maker-checker approval
/// This is the core command that all operations requiring approval should use
/// </summary>
[Authorize] // All authenticated users can initiate workflows
public record InitiateWorkflowCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    
    // Workflow Configuration
    public string WorkflowCode { get; init; } = string.Empty; // e.g., ACCOUNT_OPENING, LOAN_APPROVAL, LARGE_TRANSACTION
    public string WorkflowName { get; init; } = string.Empty;
    public WorkflowType WorkflowType { get; init; } = WorkflowType.MakerChecker;
    
    // Entity Being Approved
    public string EntityType { get; init; } = string.Empty; // Account, Loan, Transaction, etc.
    public Guid EntityId { get; init; }
    public string EntityReference { get; init; } = string.Empty; // Human-readable reference
    
    // Transaction Details (for amount-based approval routing)
    public decimal? Amount { get; init; }
    public string Currency { get; init; } = "KES";
    
    // Priority and SLA
    public Priority Priority { get; init; } = Priority.Normal;
    public int SLAHours { get; init; } = 24;
    
    // Approval Configuration
    public int RequiredApprovalLevels { get; init; } = 1;
    public List<ApprovalLevelConfig> ApprovalLevels { get; init; } = new();
    
    // Context Information
    public string BranchCode { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public string RequestData { get; init; } = "{}"; // JSON serialized request data
    public string InitiatorComments { get; init; } = string.Empty;
    
    // Auto-routing Configuration
    public bool UseAutoRouting { get; init; } = true; // Use approval matrix for routing
    public List<string>? SpecificApprovers { get; init; } // Override auto-routing
}

public record ApprovalLevelConfig
{
    public int Level { get; init; }
    public List<UserRole> RequiredRoles { get; init; } = new();
    public decimal? MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }
    public bool IsRequired { get; init; } = true;
    public bool AllowDelegation { get; init; } = true;
    public int TimeoutHours { get; init; } = 24;
}