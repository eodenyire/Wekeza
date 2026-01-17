using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Task Assignment aggregate - Complete task management system
/// Supports task creation, assignment, tracking, and completion
/// </summary>
public class TaskAssignment : AggregateRoot<Guid>
{
    public string TaskCode { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public TaskType TaskType { get; private set; }
    public TaskStatus Status { get; private set; }
    public Priority Priority { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? AssignedTo { get; private set; }
    public DateTime? AssignedAt { get; private set; }
    public string? AssignedBy { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? CompletedBy { get; private set; }
    public string? CompletionNotes { get; private set; }
    public string? CancellationReason { get; private set; }
    public string BranchCode { get; private set; }
    public string Department { get; private set; }
    public string? RelatedEntityType { get; private set; }
    public Guid? RelatedEntityId { get; private set; }
    public int EstimatedHours { get; private set; }
    public int? ActualHours { get; private set; }
    public bool IsEscalated { get; private set; }
    public DateTime? EscalatedAt { get; private set; }
    public string? EscalationReason { get; private set; }

    private readonly List<TaskComment> _comments = new();
    public IReadOnlyList<TaskComment> Comments => _comments.AsReadOnly();

    private readonly List<TaskAttachment> _attachments = new();
    public IReadOnlyList<TaskAttachment> Attachments => _attachments.AsReadOnly();

    private readonly List<TaskDependency> _dependencies = new();
    public IReadOnlyList<TaskDependency> Dependencies => _dependencies.AsReadOnly();

    private TaskAssignment() { } // EF Core

    public TaskAssignment(
        Guid id,
        string taskCode,
        string title,
        string description,
        TaskType taskType,
        Priority priority,
        string createdBy,
        string branchCode,
        string department,
        DateTime? dueDate = null,
        string? relatedEntityType = null,
        Guid? relatedEntityId = null,
        int estimatedHours = 1)
    {
        Id = id;
        TaskCode = taskCode;
        Title = title;
        Description = description;
        TaskType = taskType;
        Priority = priority;
        CreatedBy = createdBy;
        BranchCode = branchCode;
        Department = department;
        DueDate = dueDate;
        RelatedEntityType = relatedEntityType;
        RelatedEntityId = relatedEntityId;
        EstimatedHours = estimatedHours;
        
        Status = TaskStatus.Created;
        CreatedAt = DateTime.UtcNow;
        IsEscalated = false;

        AddDomainEvent(new TaskCreatedDomainEvent(Id, TaskCode, Title, CreatedBy));
    }

    public void AssignTo(string assigneeId, string assignedBy)
    {
        if (Status != TaskStatus.Created && Status != TaskStatus.Unassigned)
            throw new InvalidOperationException("Task can only be assigned when in Created or Unassigned status");

        AssignedTo = assigneeId;
        AssignedBy = assignedBy;
        AssignedAt = DateTime.UtcNow;
        Status = TaskStatus.Assigned;

        AddDomainEvent(new TaskAssignedDomainEvent(Id, TaskCode, assigneeId, assignedBy));
    }

    public void Reassign(string newAssigneeId, string reassignedBy, string? reason = null)
    {
        if (Status == TaskStatus.Completed || Status == TaskStatus.Cancelled)
            throw new InvalidOperationException("Cannot reassign completed or cancelled tasks");

        var previousAssignee = AssignedTo;
        AssignedTo = newAssigneeId;
        AssignedBy = reassignedBy;
        AssignedAt = DateTime.UtcNow;
        Status = TaskStatus.Assigned;

        if (!string.IsNullOrEmpty(reason))
        {
            AddComment(reassignedBy, $"Task reassigned from {previousAssignee} to {newAssigneeId}. Reason: {reason}");
        }

        AddDomainEvent(new TaskReassignedDomainEvent(Id, TaskCode, previousAssignee, newAssigneeId, reassignedBy));
    }

    public void Start(string startedBy)
    {
        if (Status != TaskStatus.Assigned)
            throw new InvalidOperationException("Task must be assigned before it can be started");

        if (AssignedTo != startedBy)
            throw new InvalidOperationException("Only the assigned user can start the task");

        Status = TaskStatus.InProgress;
        StartedAt = DateTime.UtcNow;

        AddDomainEvent(new TaskStartedDomainEvent(Id, TaskCode, startedBy));
    }

    public void Complete(string completedBy, string? completionNotes = null, int? actualHours = null)
    {
        if (Status != TaskStatus.InProgress)
            throw new InvalidOperationException("Task must be in progress to be completed");

        if (AssignedTo != completedBy)
            throw new InvalidOperationException("Only the assigned user can complete the task");

        // Check if all dependencies are completed
        var incompleteDependencies = _dependencies.Where(d => !d.IsCompleted);
        if (incompleteDependencies.Any())
            throw new InvalidOperationException("All task dependencies must be completed first");

        Status = TaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CompletedBy = completedBy;
        CompletionNotes = completionNotes;
        ActualHours = actualHours;

        if (!string.IsNullOrEmpty(completionNotes))
        {
            AddComment(completedBy, completionNotes);
        }

        AddDomainEvent(new TaskCompletedDomainEvent(Id, TaskCode, completedBy, ActualHours));
    }

    public void Cancel(string cancelledBy, string cancellationReason)
    {
        if (Status == TaskStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed tasks");

        Status = TaskStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
        CompletedBy = cancelledBy;
        CancellationReason = cancellationReason;

        AddComment(cancelledBy, $"Task cancelled: {cancellationReason}");

        AddDomainEvent(new TaskCancelledDomainEvent(Id, TaskCode, cancelledBy, cancellationReason));
    }

    public void UpdatePriority(Priority newPriority, string updatedBy, string? reason = null)
    {
        if (Status == TaskStatus.Completed || Status == TaskStatus.Cancelled)
            throw new InvalidOperationException("Cannot update priority of completed or cancelled tasks");

        var previousPriority = Priority;
        Priority = newPriority;

        var reasonText = !string.IsNullOrEmpty(reason) ? $" Reason: {reason}" : "";
        AddComment(updatedBy, $"Priority changed from {previousPriority} to {newPriority}.{reasonText}");

        AddDomainEvent(new TaskPriorityUpdatedDomainEvent(Id, TaskCode, previousPriority, newPriority, updatedBy));
    }

    public void UpdateDueDate(DateTime? newDueDate, string updatedBy, string? reason = null)
    {
        if (Status == TaskStatus.Completed || Status == TaskStatus.Cancelled)
            throw new InvalidOperationException("Cannot update due date of completed or cancelled tasks");

        var previousDueDate = DueDate;
        DueDate = newDueDate;

        var reasonText = !string.IsNullOrEmpty(reason) ? $" Reason: {reason}" : "";
        AddComment(updatedBy, $"Due date changed from {previousDueDate:yyyy-MM-dd} to {newDueDate:yyyy-MM-dd}.{reasonText}");

        AddDomainEvent(new TaskDueDateUpdatedDomainEvent(Id, TaskCode, previousDueDate, newDueDate, updatedBy));
    }

    public void Escalate(string escalationReason, string escalatedBy)
    {
        if (Status == TaskStatus.Completed || Status == TaskStatus.Cancelled)
            throw new InvalidOperationException("Cannot escalate completed or cancelled tasks");

        if (IsOverdue() || Priority == Priority.Critical)
        {
            IsEscalated = true;
            EscalatedAt = DateTime.UtcNow;
            EscalationReason = escalationReason;
            Priority = Priority.Critical;

            AddComment(escalatedBy, $"Task escalated: {escalationReason}");

            AddDomainEvent(new TaskEscalatedDomainEvent(Id, TaskCode, escalationReason, escalatedBy));
        }
    }

    public void AddComment(string userId, string comment)
    {
        var taskComment = new TaskComment(
            Guid.NewGuid(),
            Id,
            userId,
            comment,
            DateTime.UtcNow);

        _comments.Add(taskComment);
    }

    public void AttachFile(string fileName, string filePath, string uploadedBy, string? description = null)
    {
        var attachment = new TaskAttachment(
            Guid.NewGuid(),
            Id,
            fileName,
            filePath,
            uploadedBy,
            description,
            DateTime.UtcNow);

        _attachments.Add(attachment);

        AddDomainEvent(new TaskFileAttachedDomainEvent(Id, TaskCode, fileName, uploadedBy));
    }

    public void AddDependency(Guid dependentTaskId, DependencyType dependencyType, string addedBy)
    {
        if (_dependencies.Any(d => d.DependentTaskId == dependentTaskId))
            throw new InvalidOperationException("Dependency already exists");

        var dependency = new TaskDependency(
            Guid.NewGuid(),
            Id,
            dependentTaskId,
            dependencyType,
            addedBy,
            DateTime.UtcNow);

        _dependencies.Add(dependency);

        AddDomainEvent(new TaskDependencyAddedDomainEvent(Id, TaskCode, dependentTaskId, dependencyType));
    }

    public void CompleteDependency(Guid dependentTaskId, string completedBy)
    {
        var dependency = _dependencies.FirstOrDefault(d => d.DependentTaskId == dependentTaskId);
        if (dependency == null)
            throw new InvalidOperationException("Dependency not found");

        dependency.MarkCompleted(completedBy);
    }

    public bool IsOverdue()
    {
        return DueDate.HasValue && DateTime.UtcNow > DueDate.Value && Status != TaskStatus.Completed && Status != TaskStatus.Cancelled;
    }

    public TimeSpan? GetTimeRemaining()
    {
        if (!DueDate.HasValue || Status == TaskStatus.Completed || Status == TaskStatus.Cancelled)
            return null;

        var remaining = DueDate.Value - DateTime.UtcNow;
        return remaining.TotalMilliseconds > 0 ? remaining : TimeSpan.Zero;
    }

    public TimeSpan GetProcessingDuration()
    {
        if (!StartedAt.HasValue)
            return TimeSpan.Zero;

        var endTime = CompletedAt ?? DateTime.UtcNow;
        return endTime - StartedAt.Value;
    }

    public decimal GetCompletionPercentage()
    {
        return Status switch
        {
            TaskStatus.Created => 0,
            TaskStatus.Assigned => 10,
            TaskStatus.InProgress => 50,
            TaskStatus.Completed => 100,
            TaskStatus.Cancelled => 0,
            _ => 0
        };
    }

    public bool CanBeStarted()
    {
        return Status == TaskStatus.Assigned && _dependencies.All(d => d.IsCompleted);
    }

    public bool HasPendingDependencies()
    {
        return _dependencies.Any(d => !d.IsCompleted);
    }
}

/// <summary>
/// Task Comment
/// </summary>
public class TaskComment
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }
    public string UserId { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private TaskComment() { } // EF Core

    public TaskComment(Guid id, Guid taskId, string userId, string comment, DateTime createdAt)
    {
        Id = id;
        TaskId = taskId;
        UserId = userId;
        Comment = comment;
        CreatedAt = createdAt;
    }
}

/// <summary>
/// Task Attachment
/// </summary>
public class TaskAttachment
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public string UploadedBy { get; private set; }
    public string? Description { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private TaskAttachment() { } // EF Core

    public TaskAttachment(Guid id, Guid taskId, string fileName, string filePath, string uploadedBy, string? description, DateTime uploadedAt)
    {
        Id = id;
        TaskId = taskId;
        FileName = fileName;
        FilePath = filePath;
        UploadedBy = uploadedBy;
        Description = description;
        UploadedAt = uploadedAt;
    }
}

/// <summary>
/// Task Dependency
/// </summary>
public class TaskDependency
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }
    public Guid DependentTaskId { get; private set; }
    public DependencyType DependencyType { get; private set; }
    public bool IsCompleted { get; private set; }
    public string AddedBy { get; private set; }
    public DateTime AddedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? CompletedBy { get; private set; }

    private TaskDependency() { } // EF Core

    public TaskDependency(Guid id, Guid taskId, Guid dependentTaskId, DependencyType dependencyType, string addedBy, DateTime addedAt)
    {
        Id = id;
        TaskId = taskId;
        DependentTaskId = dependentTaskId;
        DependencyType = dependencyType;
        AddedBy = addedBy;
        AddedAt = addedAt;
        IsCompleted = false;
    }

    public void MarkCompleted(string completedBy)
    {
        IsCompleted = true;
        CompletedBy = completedBy;
        CompletedAt = DateTime.UtcNow;
    }
}

// Enums
public enum TaskType
{
    General = 1,
    Approval = 2,
    Review = 3,
    Investigation = 4,
    Maintenance = 5,
    Documentation = 6,
    Training = 7,
    Compliance = 8,
    CustomerService = 9,
    TechnicalSupport = 10
}

public enum TaskStatus
{
    Created = 1,
    Assigned = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5,
    OnHold = 6,
    Unassigned = 7
}

public enum DependencyType
{
    FinishToStart = 1,
    StartToStart = 2,
    FinishToFinish = 3,
    StartToFinish = 4
}

// Domain Events
public record TaskCreatedDomainEvent(
    Guid TaskId,
    string TaskCode,
    string Title,
    string CreatedBy) : IDomainEvent;

public record TaskAssignedDomainEvent(
    Guid TaskId,
    string TaskCode,
    string AssignedTo,
    string AssignedBy) : IDomainEvent;

public record TaskReassignedDomainEvent(
    Guid TaskId,
    string TaskCode,
    string? PreviousAssignee,
    string NewAssignee,
    string ReassignedBy) : IDomainEvent;

public record TaskStartedDomainEvent(
    Guid TaskId,
    string TaskCode,
    string StartedBy) : IDomainEvent;

public record TaskCompletedDomainEvent(
    Guid TaskId,
    string TaskCode,
    string CompletedBy,
    int? ActualHours) : IDomainEvent;

public record TaskCancelledDomainEvent(
    Guid TaskId,
    string TaskCode,
    string CancelledBy,
    string CancellationReason) : IDomainEvent;

public record TaskPriorityUpdatedDomainEvent(
    Guid TaskId,
    string TaskCode,
    Priority PreviousPriority,
    Priority NewPriority,
    string UpdatedBy) : IDomainEvent;

public record TaskDueDateUpdatedDomainEvent(
    Guid TaskId,
    string TaskCode,
    DateTime? PreviousDueDate,
    DateTime? NewDueDate,
    string UpdatedBy) : IDomainEvent;

public record TaskEscalatedDomainEvent(
    Guid TaskId,
    string TaskCode,
    string EscalationReason,
    string EscalatedBy) : IDomainEvent;

public record TaskFileAttachedDomainEvent(
    Guid TaskId,
    string TaskCode,
    string FileName,
    string UploadedBy) : IDomainEvent;

public record TaskDependencyAddedDomainEvent(
    Guid TaskId,
    string TaskCode,
    Guid DependentTaskId,
    DependencyType DependencyType) : IDomainEvent;