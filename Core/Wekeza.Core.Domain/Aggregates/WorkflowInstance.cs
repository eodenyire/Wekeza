using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Exceptions;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// WorkflowInstance Aggregate - Represents a running workflow
/// Inspired by Finacle Workflow Engine and T24 Maker-Checker
/// Implements dual authorization and multi-level approval
/// </summary>
public class WorkflowInstance : AggregateRoot
{
    public string WorkflowCode { get; private set; } // e.g., PRODUCT_APPROVAL, LOAN_APPROVAL
    public string WorkflowName { get; private set; }
    public WorkflowType Type { get; private set; }
    public WorkflowStatus Status { get; private set; }
    
    // Entity being approved
    public string EntityType { get; private set; } // Product, Loan, Transaction, etc.
    public Guid EntityId { get; private set; }
    public string EntityReference { get; private set; } // Human-readable reference
    
    // Workflow metadata
    public int CurrentLevel { get; private set; }
    public int RequiredLevels { get; private set; }
    public DateTime InitiatedDate { get; private set; }
    public string InitiatedBy { get; private set; } // Maker
    public DateTime? CompletedDate { get; private set; }
    public string? CompletedBy { get; private set; }
    
    // Approval chain
    private readonly List<ApprovalStep> _approvalSteps = new();
    public IReadOnlyCollection<ApprovalStep> ApprovalSteps => _approvalSteps.AsReadOnly();
    
    // Comments and notes
    private readonly List<WorkflowComment> _comments = new();
    public IReadOnlyCollection<WorkflowComment> Comments => _comments.AsReadOnly();
    
    // SLA tracking
    public DateTime? DueDate { get; private set; }
    public bool IsOverdue => DueDate.HasValue && DateTime.UtcNow > DueDate && Status == WorkflowStatus.Pending;
    public int Priority { get; private set; }
    
    // Escalation
    public bool IsEscalated { get; private set; }
    public DateTime? EscalatedDate { get; private set; }
    public string? EscalatedTo { get; private set; }
    
    // Request data (JSON)
    public string RequestData { get; private set; } = string.Empty;

    private WorkflowInstance() : base(Guid.NewGuid()) { }

    public static WorkflowInstance Create(
        string workflowCode,
        string workflowName,
        WorkflowType type,
        string entityType,
        Guid entityId,
        string entityReference,
        int requiredLevels,
        string initiatedBy,
        string requestData,
        int slaHours = 24)
    {
        var workflow = new WorkflowInstance
        {
            Id = Guid.NewGuid(),
            WorkflowCode = workflowCode,
            WorkflowName = workflowName,
            Type = type,
            Status = WorkflowStatus.Pending,
            EntityType = entityType,
            EntityId = entityId,
            EntityReference = entityReference,
            CurrentLevel = 0,
            RequiredLevels = requiredLevels,
            InitiatedDate = DateTime.UtcNow,
            InitiatedBy = initiatedBy,
            RequestData = requestData,
            DueDate = DateTime.UtcNow.AddHours(slaHours),
            IsEscalated = false,
            Priority = 1
        };

        // Create initial approval step
        workflow._approvalSteps.Add(new ApprovalStep(
            Guid.NewGuid(),
            workflow.Id,
            1,
            "APPROVER", // Default role
            null, // No specific approver
            true, // Required
            null, // No minimum amount
            null, // No maximum amount
            DateTime.UtcNow));

        return workflow;
    }

    public void Approve(string approvedBy, string comments, UserRole approverRole)
    {
        if (Status != WorkflowStatus.Pending)
            throw new WorkflowException($"Cannot approve workflow in {Status} status.");

        var currentStep = _approvalSteps.FirstOrDefault(s => s.Level == CurrentLevel + 1);
        if (currentStep == null)
            throw new WorkflowException("No pending approval step found.");

        // Check if approver is the maker (prevent self-approval)
        if (approvedBy == InitiatedBy)
            throw new WorkflowException("Maker cannot approve their own request (Maker-Checker violation).");

        // Update current step
        currentStep.Approve(approvedBy, comments);

        // Add comment
        AddComment(approvedBy, $"Approved at Level {CurrentLevel + 1}: {comments}");

        CurrentLevel++;

        // Check if all levels are approved
        if (CurrentLevel >= RequiredLevels)
        {
            Status = WorkflowStatus.Approved;
            CompletedDate = DateTime.UtcNow;
            CompletedBy = approvedBy;
        }
        else
        {
            // Create next approval step
            _approvalSteps.Add(new ApprovalStep(
                Guid.NewGuid(),
                Id,
                CurrentLevel + 1,
                "APPROVER", // Default role
                null, // No specific approver
                true, // Required
                null, // No minimum amount
                null, // No maximum amount
                DateTime.UtcNow));
        }
    }

    public void Reject(string rejectedBy, string reason)
    {
        if (Status != WorkflowStatus.Pending)
            throw new WorkflowException($"Cannot reject workflow in {Status} status.");

        var currentStep = _approvalSteps.FirstOrDefault(s => s.Level == CurrentLevel + 1);
        if (currentStep != null)
        {
            currentStep.Reject(rejectedBy, reason);
        }

        Status = WorkflowStatus.Rejected;
        CompletedDate = DateTime.UtcNow;
        CompletedBy = rejectedBy;

        AddComment(rejectedBy, $"Rejected: {reason}");
    }

    public void Cancel(string cancelledBy, string reason)
    {
        if (Status != WorkflowStatus.Pending)
            throw new WorkflowException($"Cannot cancel workflow in {Status} status.");

        Status = WorkflowStatus.Cancelled;
        CompletedDate = DateTime.UtcNow;
        CompletedBy = cancelledBy;

        AddComment(cancelledBy, $"Cancelled: {reason}");
    }

    public void Escalate(string escalatedTo, string reason)
    {
        if (Status != WorkflowStatus.Pending)
            throw new WorkflowException($"Cannot escalate workflow in {Status} status.");

        IsEscalated = true;
        EscalatedDate = DateTime.UtcNow;
        EscalatedTo = escalatedTo;

        AddComment("System", $"Escalated to {escalatedTo}: {reason}");
    }

    public void AssignToApprover(int level, string approverId)
    {
        var step = _approvalSteps.FirstOrDefault(s => s.Level == level);
        if (step == null)
            throw new WorkflowException($"Approval step at level {level} not found.");

        step.Assign();
    }

    public void AddComment(string commentBy, string comment)
    {
        _comments.Add(new WorkflowComment(
            Guid.NewGuid(),
            Id,
            commentBy,
            comment,
            WorkflowCommentType.General,
            DateTime.UtcNow));
    }

    public void ExtendDueDate(int additionalHours, string reason)
    {
        if (DueDate.HasValue)
        {
            DueDate = DueDate.Value.AddHours(additionalHours);
            AddComment("System", $"Due date extended by {additionalHours} hours: {reason}");
        }
    }

    public void AddApprovalStep(int level, string approverRole, string? specificApprover = null, bool isRequired = true)
    {
        _approvalSteps.Add(new ApprovalStep(
            Guid.NewGuid(),
            Id,
            level,
            approverRole,
            specificApprover,
            isRequired,
            null,
            null,
            DateTime.UtcNow));
    }

    public void SetPriority(int priority)
    {
        Priority = priority;
    }

    public void SetDueDate(DateTime dueDate)
    {
        DueDate = dueDate;
    }
}


