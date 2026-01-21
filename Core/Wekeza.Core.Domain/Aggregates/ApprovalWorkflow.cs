using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Approval Workflow aggregate - Comprehensive maker-checker and multi-level approval system
/// Supports configurable approval matrices, escalation rules, and SLA management
/// </summary>
public class ApprovalWorkflow : AggregateRoot
{
    public string WorkflowCode { get; private set; }
    public string WorkflowName { get; private set; }
    public WorkflowType WorkflowType { get; private set; }
    public string EntityType { get; private set; } // Transaction, Loan, Account, etc.
    public Guid EntityId { get; private set; }
    public Money? Amount { get; private set; }
    public WorkflowStatus Status { get; private set; }
    public int CurrentLevel { get; private set; }
    public int MaxLevels { get; private set; }
    public Priority Priority { get; private set; }
    public DateTime InitiatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? DueDate { get; private set; }
    public string InitiatedBy { get; private set; }
    public string? CompletedBy { get; private set; }
    public string? RejectionReason { get; private set; }
    public string BranchCode { get; private set; }
    public string Department { get; private set; }
    public bool RequiresMakerChecker { get; private set; }
    public bool IsEscalated { get; private set; }
    public DateTime? EscalatedAt { get; private set; }
    public string? EscalationReason { get; private set; }

    private readonly List<ApprovalStep> _approvalSteps = new();
    public IReadOnlyList<ApprovalStep> ApprovalSteps => _approvalSteps.AsReadOnly();

    private readonly List<WorkflowComment> _comments = new();
    public IReadOnlyList<WorkflowComment> Comments => _comments.AsReadOnly();

    private readonly List<WorkflowDocument> _documents = new();
    public IReadOnlyList<WorkflowDocument> Documents => _documents.AsReadOnly();

    private ApprovalWorkflow() : base(Guid.NewGuid()) { } // EF Core

    public ApprovalWorkflow(
        Guid id,
        string workflowCode,
        string workflowName,
        WorkflowType workflowType,
        string entityType,
        Guid entityId,
        Money? amount,
        Priority priority,
        string initiatedBy,
        string branchCode,
        string department,
        bool requiresMakerChecker = true) : base(id)
    {
        WorkflowCode = workflowCode;
        WorkflowName = workflowName;
        WorkflowType = workflowType;
        EntityType = entityType;
        EntityId = entityId;
        Amount = amount;
        Priority = priority;
        InitiatedBy = initiatedBy;
        BranchCode = branchCode;
        Department = department;
        RequiresMakerChecker = requiresMakerChecker;
        
        Status = WorkflowStatus.Initiated;
        CurrentLevel = 1;
        MaxLevels = 1; // Will be updated when approval steps are added
        InitiatedAt = DateTime.UtcNow;
        IsEscalated = false;

        // Set due date based on priority
        SetDueDate();

        AddDomainEvent(new ApprovalWorkflowInitiatedDomainEvent(Id, WorkflowCode, EntityType, EntityId, InitiatedBy));
    }

    private void SetDueDate()
    {
        var hoursToAdd = Priority switch
        {
            Priority.Critical => 2,
            Priority.Urgent => 4,
            Priority.High => 8,
            Priority.Normal => 24,
            _ => 48
        };

        DueDate = InitiatedAt.AddHours(hoursToAdd);
    }

    public void AddApprovalStep(int level, string approverRole, string? specificApprover, bool isRequired, Money? minimumAmount = null, Money? maximumAmount = null)
    {
        if (Status != WorkflowStatus.Initiated)
            throw new InvalidOperationException("Cannot add approval steps after workflow has started");

        var step = new ApprovalStep(
            Guid.NewGuid(),
            Id,
            level,
            approverRole,
            specificApprover,
            isRequired,
            minimumAmount,
            maximumAmount,
            DateTime.UtcNow);

        _approvalSteps.Add(step);
        MaxLevels = Math.Max(MaxLevels, level);
    }

    public void StartWorkflow()
    {
        if (Status != WorkflowStatus.Initiated)
            throw new InvalidOperationException("Workflow has already been started");

        if (!_approvalSteps.Any())
            throw new InvalidOperationException("Cannot start workflow without approval steps");

        Status = WorkflowStatus.InProgress;
        
        // Assign first level approvers
        AssignCurrentLevelApprovers();

        AddDomainEvent(new ApprovalWorkflowStartedDomainEvent(Id, WorkflowCode, CurrentLevel));
    }

    private void AssignCurrentLevelApprovers()
    {
        var currentSteps = _approvalSteps.Where(s => s.Level == CurrentLevel);
        
        foreach (var step in currentSteps)
        {
            step.Assign();
        }

        AddDomainEvent(new ApprovalStepAssignedDomainEvent(Id, WorkflowCode, CurrentLevel, currentSteps.Count()));
    }

    public void ApproveStep(int level, string approverUserId, string? comments = null)
    {
        if (Status != WorkflowStatus.InProgress)
            throw new InvalidOperationException("Workflow is not in progress");

        var step = _approvalSteps.FirstOrDefault(s => s.Level == level && 
            (s.SpecificApprover == approverUserId || string.IsNullOrEmpty(s.SpecificApprover)));

        if (step == null)
            throw new InvalidOperationException("No approval step found for this approver at this level");

        if (step.Status != ApprovalStepStatus.Assigned)
            throw new InvalidOperationException("Approval step is not in assigned state");

        // Maker-checker validation
        if (RequiresMakerChecker && approverUserId == InitiatedBy)
            throw new InvalidOperationException("Maker cannot approve their own transaction");

        step.Approve(approverUserId, comments);

        if (!string.IsNullOrEmpty(comments))
        {
            AddComment(approverUserId, comments, WorkflowCommentType.Approval);
        }

        AddDomainEvent(new ApprovalStepApprovedDomainEvent(Id, WorkflowCode, level, approverUserId));

        // Check if current level is complete
        CheckLevelCompletion();
    }

    public void RejectStep(int level, string approverUserId, string rejectionReason)
    {
        if (Status != WorkflowStatus.InProgress)
            throw new InvalidOperationException("Workflow is not in progress");

        var step = _approvalSteps.FirstOrDefault(s => s.Level == level && 
            (s.SpecificApprover == approverUserId || string.IsNullOrEmpty(s.SpecificApprover)));

        if (step == null)
            throw new InvalidOperationException("No approval step found for this approver at this level");

        step.Reject(approverUserId, rejectionReason);
        
        Status = WorkflowStatus.Rejected;
        RejectionReason = rejectionReason;
        CompletedAt = DateTime.UtcNow;
        CompletedBy = approverUserId;

        AddComment(approverUserId, rejectionReason, WorkflowCommentType.Rejection);

        AddDomainEvent(new ApprovalWorkflowRejectedDomainEvent(Id, WorkflowCode, level, approverUserId, rejectionReason));
    }

    private void CheckLevelCompletion()
    {
        var currentLevelSteps = _approvalSteps.Where(s => s.Level == CurrentLevel);
        var requiredSteps = currentLevelSteps.Where(s => s.IsRequired);
        var approvedRequiredSteps = requiredSteps.Where(s => s.Status == ApprovalStepStatus.Approved);

        // Check if all required steps are approved
        if (approvedRequiredSteps.Count() == requiredSteps.Count())
        {
            // Move to next level or complete workflow
            if (CurrentLevel < MaxLevels)
            {
                CurrentLevel++;
                AssignCurrentLevelApprovers();
                AddDomainEvent(new ApprovalLevelCompletedDomainEvent(Id, WorkflowCode, CurrentLevel - 1));
            }
            else
            {
                CompleteWorkflow();
            }
        }
    }

    private void CompleteWorkflow()
    {
        Status = WorkflowStatus.Approved;
        CompletedAt = DateTime.UtcNow;
        CompletedBy = GetLastApprover();

        AddDomainEvent(new ApprovalWorkflowCompletedDomainEvent(Id, WorkflowCode, EntityType, EntityId, CompletedBy!));
    }

    private string? GetLastApprover()
    {
        return _approvalSteps
            .Where(s => s.Status == ApprovalStepStatus.Approved)
            .OrderByDescending(s => s.ProcessedAt)
            .FirstOrDefault()?.ProcessedBy;
    }

    public void EscalateWorkflow(string escalationReason, string escalatedBy)
    {
        if (Status != WorkflowStatus.InProgress)
            throw new InvalidOperationException("Only in-progress workflows can be escalated");

        if (IsOverdue() || Priority == Priority.Critical)
        {
            IsEscalated = true;
            EscalatedAt = DateTime.UtcNow;
            EscalationReason = escalationReason;
            Priority = Priority.Critical;

            // Reset due date for escalated workflow
            DueDate = DateTime.UtcNow.AddHours(1);

            AddComment(escalatedBy, $"Workflow escalated: {escalationReason}", WorkflowCommentType.Escalation);

            AddDomainEvent(new ApprovalWorkflowEscalatedDomainEvent(Id, WorkflowCode, escalationReason, escalatedBy));
        }
    }

    public void CancelWorkflow(string cancellationReason, string cancelledBy)
    {
        if (Status == WorkflowStatus.Approved || Status == WorkflowStatus.Rejected)
            throw new InvalidOperationException("Cannot cancel completed workflow");

        Status = WorkflowStatus.Cancelled;
        RejectionReason = cancellationReason;
        CompletedAt = DateTime.UtcNow;
        CompletedBy = cancelledBy;

        AddComment(cancelledBy, cancellationReason, WorkflowCommentType.Cancellation);

        AddDomainEvent(new ApprovalWorkflowCancelledDomainEvent(Id, WorkflowCode, cancellationReason, cancelledBy));
    }

    public void AddComment(string userId, string comment, WorkflowCommentType commentType)
    {
        var workflowComment = new WorkflowComment(
            Guid.NewGuid(),
            Id,
            userId,
            comment,
            commentType,
            DateTime.UtcNow);

        _comments.Add(workflowComment);
    }

    public void AttachDocument(string fileName, string filePath, string uploadedBy, string? description = null)
    {
        var document = new WorkflowDocument(
            Guid.NewGuid(),
            Id,
            fileName,
            filePath,
            uploadedBy,
            description,
            DateTime.UtcNow);

        _documents.Add(document);

        AddDomainEvent(new WorkflowDocumentAttachedDomainEvent(Id, WorkflowCode, fileName, uploadedBy));
    }

    public bool IsOverdue()
    {
        return DueDate.HasValue && DateTime.UtcNow > DueDate.Value && Status == WorkflowStatus.InProgress;
    }

    public TimeSpan? GetTimeRemaining()
    {
        if (!DueDate.HasValue || Status != WorkflowStatus.InProgress)
            return null;

        var remaining = DueDate.Value - DateTime.UtcNow;
        return remaining.TotalMilliseconds > 0 ? remaining : TimeSpan.Zero;
    }

    public TimeSpan GetProcessingDuration()
    {
        var endTime = CompletedAt ?? DateTime.UtcNow;
        return endTime - InitiatedAt;
    }

    public IEnumerable<ApprovalStep> GetPendingSteps()
    {
        return _approvalSteps.Where(s => s.Status == ApprovalStepStatus.Assigned);
    }

    public IEnumerable<ApprovalStep> GetStepsForLevel(int level)
    {
        return _approvalSteps.Where(s => s.Level == level);
    }

    public bool CanUserApprove(string userId)
    {
        if (Status != WorkflowStatus.InProgress)
            return false;

        if (RequiresMakerChecker && userId == InitiatedBy)
            return false;

        return _approvalSteps.Any(s => s.Level == CurrentLevel && 
                                     s.Status == ApprovalStepStatus.Assigned &&
                                     (s.SpecificApprover == userId || string.IsNullOrEmpty(s.SpecificApprover)));
    }

    public decimal GetCompletionPercentage()
    {
        if (!_approvalSteps.Any())
            return 0;

        var completedSteps = _approvalSteps.Count(s => s.Status == ApprovalStepStatus.Approved);
        return (decimal)completedSteps / _approvalSteps.Count * 100;
    }
}

/// <summary>
/// Individual approval step within a workflow
/// </summary>
public class ApprovalStep
{
    public Guid Id { get; private set; }
    public Guid WorkflowId { get; private set; }
    public int Level { get; private set; }
    public string ApproverRole { get; private set; }
    public string? SpecificApprover { get; private set; }
    public bool IsRequired { get; private set; }
    public Money? MinimumAmount { get; private set; }
    public Money? MaximumAmount { get; private set; }
    public ApprovalStepStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? AssignedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? ProcessedBy { get; private set; }
    public string? Comments { get; private set; }

    private ApprovalStep() { } // EF Core

    public ApprovalStep(
        Guid id,
        Guid workflowId,
        int level,
        string approverRole,
        string? specificApprover,
        bool isRequired,
        Money? minimumAmount,
        Money? maximumAmount,
        DateTime createdAt)
    {
        Id = id;
        WorkflowId = workflowId;
        Level = level;
        ApproverRole = approverRole;
        SpecificApprover = specificApprover;
        IsRequired = isRequired;
        MinimumAmount = minimumAmount;
        MaximumAmount = maximumAmount;
        CreatedAt = createdAt;
        Status = ApprovalStepStatus.Pending;
    }

    public void Assign()
    {
        if (Status != ApprovalStepStatus.Pending)
            throw new InvalidOperationException("Step is not in pending state");

        Status = ApprovalStepStatus.Assigned;
        AssignedAt = DateTime.UtcNow;
    }

    public void Approve(string approverUserId, string? comments = null)
    {
        if (Status != ApprovalStepStatus.Assigned)
            throw new InvalidOperationException("Step is not in assigned state");

        Status = ApprovalStepStatus.Approved;
        ProcessedAt = DateTime.UtcNow;
        ProcessedBy = approverUserId;
        Comments = comments;
    }

    public void Reject(string approverUserId, string rejectionReason)
    {
        if (Status != ApprovalStepStatus.Assigned)
            throw new InvalidOperationException("Step is not in assigned state");

        Status = ApprovalStepStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
        ProcessedBy = approverUserId;
        Comments = rejectionReason;
    }

    public bool IsApplicableForAmount(Money? amount)
    {
        if (amount == null)
            return true;

        if (MinimumAmount != null && amount.Amount < MinimumAmount.Amount)
            return false;

        if (MaximumAmount != null && amount.Amount > MaximumAmount.Amount)
            return false;

        return true;
    }
}

/// <summary>
/// Comment within a workflow
/// </summary>
public class WorkflowComment
{
    public Guid Id { get; private set; }
    public Guid WorkflowId { get; private set; }
    public string UserId { get; private set; }
    public string Comment { get; private set; }
    public WorkflowCommentType CommentType { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private WorkflowComment() { } // EF Core

    public WorkflowComment(Guid id, Guid workflowId, string userId, string comment, WorkflowCommentType commentType, DateTime createdAt)
    {
        Id = id;
        WorkflowId = workflowId;
        UserId = userId;
        Comment = comment;
        CommentType = commentType;
        CreatedAt = createdAt;
    }
}

/// <summary>
/// Document attached to a workflow
/// </summary>
public class WorkflowDocument
{
    public Guid Id { get; private set; }
    public Guid WorkflowId { get; private set; }
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public string UploadedBy { get; private set; }
    public string? Description { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private WorkflowDocument() { } // EF Core

    public WorkflowDocument(Guid id, Guid workflowId, string fileName, string filePath, string uploadedBy, string? description, DateTime uploadedAt)
    {
        Id = id;
        WorkflowId = workflowId;
        FileName = fileName;
        FilePath = filePath;
        UploadedBy = uploadedBy;
        Description = description;
        UploadedAt = uploadedAt;
    }
}

// Enums
public enum WorkflowType
{
    Transaction = 1,
    LoanApproval = 2,
    AccountOpening = 3,
    LimitIncrease = 4,
    ProductApproval = 5,
    ComplianceReview = 6,
    ExceptionHandling = 7,
    DocumentVerification = 8,
    RiskAssessment = 9,
    AuditReview = 10
}

// Domain Events
public record ApprovalWorkflowInitiatedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    string EntityType,
    Guid EntityId,
    string InitiatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ApprovalWorkflowStartedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    int CurrentLevel) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ApprovalStepAssignedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    int Level,
    int StepCount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ApprovalStepApprovedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    int Level,
    string ApprovedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ApprovalLevelCompletedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    int CompletedLevel) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ApprovalWorkflowCompletedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    string EntityType,
    Guid EntityId,
    string CompletedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ApprovalWorkflowRejectedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    int Level,
    string RejectedBy,
    string RejectionReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ApprovalWorkflowEscalatedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    string EscalationReason,
    string EscalatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ApprovalWorkflowCancelledDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    string CancellationReason,
    string CancelledBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record WorkflowDocumentAttachedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    string FileName,
    string UploadedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

