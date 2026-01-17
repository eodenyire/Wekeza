using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Workflow Definition aggregate - Defines reusable business process workflows
/// Supports maker-checker, multi-level approvals, and complex business processes
/// </summary>
public class WorkflowDefinition : AggregateRoot
{
    public string WorkflowCode { get; private set; }
    public string WorkflowName { get; private set; }
    public string Description { get; private set; }
    public WorkflowCategory Category { get; private set; }
    public WorkflowType WorkflowType { get; private set; }
    public int Version { get; private set; }
    public WorkflowStatus Status { get; private set; }
    public bool IsActive { get; private set; }
    public string TriggerEvent { get; private set; }
    public string? TriggerConditions { get; private set; } // JSON conditions
    public int MaxApprovalLevels { get; private set; }
    public bool RequiresMakerChecker { get; private set; }
    public bool AllowParallelApproval { get; private set; }
    public bool AllowDelegation { get; private set; }
    public TimeSpan? SLADuration { get; private set; }
    public TimeSpan? EscalationDuration { get; private set; }
    public string? EscalationRules { get; private set; } // JSON rules
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    public string? ActivatedBy { get; private set; }

    private readonly List<WorkflowStep> _steps = new();
    public IReadOnlyList<WorkflowStep> Steps => _steps.AsReadOnly();

    private readonly List<WorkflowApprovalLevel> _approvalLevels = new();
    public IReadOnlyList<WorkflowApprovalLevel> ApprovalLevels => _approvalLevels.AsReadOnly();

    private WorkflowDefinition() { } // EF Core

    public WorkflowDefinition(
        Guid id,
        string workflowCode,
        string workflowName,
        string description,
        WorkflowCategory category,
        WorkflowType workflowType,
        string triggerEvent,
        bool requiresMakerChecker,
        string createdBy)
    {
        Id = id;
        WorkflowCode = workflowCode;
        WorkflowName = workflowName;
        Description = description;
        Category = category;
        WorkflowType = workflowType;
        TriggerEvent = triggerEvent;
        RequiresMakerChecker = requiresMakerChecker;
        CreatedBy = createdBy;
        Version = 1;
        Status = WorkflowStatus.Draft;
        IsActive = false;
        MaxApprovalLevels = 1;
        AllowParallelApproval = false;
        AllowDelegation = true;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkflowDefinitionCreatedDomainEvent(Id, WorkflowCode, WorkflowName, Category));
    }

    public void AddStep(
        string stepName,
        string stepType,
        int sequence,
        bool isMandatory,
        string? assigneeRole = null,
        string? assigneeUser = null,
        string? conditions = null)
    {
        if (Status != WorkflowStatus.Draft)
            throw new InvalidOperationException("Cannot modify steps after workflow is finalized");

        // Validate sequence uniqueness
        if (_steps.Any(s => s.Sequence == sequence))
            throw new InvalidOperationException($"Step with sequence {sequence} already exists");

        var step = new WorkflowStep(
            Guid.NewGuid(),
            Id,
            stepName,
            stepType,
            sequence,
            isMandatory,
            assigneeRole,
            assigneeUser,
            conditions,
            DateTime.UtcNow);

        _steps.Add(step);
    }

    public void AddApprovalLevel(
        int level,
        string approverRole,
        string? approverUser,
        Money? minimumAmount,
        Money? maximumAmount,
        bool isRequired,
        string? conditions = null)
    {
        if (Status != WorkflowStatus.Draft)
            throw new InvalidOperationException("Cannot modify approval levels after workflow is finalized");

        // Validate level uniqueness
        if (_approvalLevels.Any(a => a.Level == level))
            throw new InvalidOperationException($"Approval level {level} already exists");

        var approvalLevel = new WorkflowApprovalLevel(
            Guid.NewGuid(),
            Id,
            level,
            approverRole,
            approverUser,
            minimumAmount,
            maximumAmount,
            isRequired,
            conditions,
            DateTime.UtcNow);

        _approvalLevels.Add(approvalLevel);
        MaxApprovalLevels = Math.Max(MaxApprovalLevels, level);
    }

    public void SetSLA(TimeSpan slaDuration, TimeSpan? escalationDuration = null)
    {
        if (Status != WorkflowStatus.Draft)
            throw new InvalidOperationException("Cannot modify SLA after workflow is finalized");

        SLADuration = slaDuration;
        EscalationDuration = escalationDuration;
        ModifiedAt = DateTime.UtcNow;
    }

    public void SetEscalationRules(string escalationRules)
    {
        if (Status != WorkflowStatus.Draft)
            throw new InvalidOperationException("Cannot modify escalation rules after workflow is finalized");

        EscalationRules = escalationRules;
        ModifiedAt = DateTime.UtcNow;
    }

    public void SetTriggerConditions(string triggerConditions)
    {
        if (Status != WorkflowStatus.Draft)
            throw new InvalidOperationException("Cannot modify trigger conditions after workflow is finalized");

        TriggerConditions = triggerConditions;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ConfigureParallelApproval(bool allowParallelApproval)
    {
        if (Status != WorkflowStatus.Draft)
            throw new InvalidOperationException("Cannot modify configuration after workflow is finalized");

        AllowParallelApproval = allowParallelApproval;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ConfigureDelegation(bool allowDelegation)
    {
        if (Status != WorkflowStatus.Draft)
            throw new InvalidOperationException("Cannot modify configuration after workflow is finalized");

        AllowDelegation = allowDelegation;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ValidateWorkflow()
    {
        if (Status != WorkflowStatus.Draft)
            throw new InvalidOperationException("Workflow has already been validated");

        var validationErrors = new List<string>();

        // Validate steps
        if (!_steps.Any())
            validationErrors.Add("Workflow must have at least one step");

        // Validate step sequences
        var sequences = _steps.Select(s => s.Sequence).OrderBy(s => s).ToList();
        for (int i = 0; i < sequences.Count; i++)
        {
            if (sequences[i] != i + 1)
            {
                validationErrors.Add("Step sequences must be consecutive starting from 1");
                break;
            }
        }

        // Validate approval levels if required
        if (RequiresMakerChecker && !_approvalLevels.Any())
            validationErrors.Add("Maker-checker workflow must have at least one approval level");

        // Validate approval level sequences
        if (_approvalLevels.Any())
        {
            var levels = _approvalLevels.Select(a => a.Level).OrderBy(l => l).ToList();
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i] != i + 1)
                {
                    validationErrors.Add("Approval levels must be consecutive starting from 1");
                    break;
                }
            }
        }

        // Validate SLA configuration
        if (SLADuration.HasValue && EscalationDuration.HasValue)
        {
            if (EscalationDuration.Value >= SLADuration.Value)
                validationErrors.Add("Escalation duration must be less than SLA duration");
        }

        if (validationErrors.Any())
        {
            Status = WorkflowStatus.ValidationFailed;
            AddDomainEvent(new WorkflowDefinitionValidationFailedDomainEvent(Id, WorkflowCode, validationErrors));
        }
        else
        {
            Status = WorkflowStatus.Validated;
            AddDomainEvent(new WorkflowDefinitionValidatedDomainEvent(Id, WorkflowCode));
        }
    }

    public void ActivateWorkflow(string activatedBy)
    {
        if (Status != WorkflowStatus.Validated)
            throw new InvalidOperationException("Workflow must be validated before activation");

        Status = WorkflowStatus.Active;
        IsActive = true;
        ActivatedBy = activatedBy;
        ActivatedAt = DateTime.UtcNow;
        ModifiedBy = activatedBy;
        ModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkflowDefinitionActivatedDomainEvent(Id, WorkflowCode, activatedBy));
    }

    public void DeactivateWorkflow(string deactivatedBy, string reason)
    {
        if (Status != WorkflowStatus.Active)
            throw new InvalidOperationException("Only active workflows can be deactivated");

        Status = WorkflowStatus.Inactive;
        IsActive = false;
        ModifiedBy = deactivatedBy;
        ModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkflowDefinitionDeactivatedDomainEvent(Id, WorkflowCode, deactivatedBy, reason));
    }

    public void CreateNewVersion(string modifiedBy)
    {
        if (Status != WorkflowStatus.Active)
            throw new InvalidOperationException("Only active workflows can be versioned");

        Version++;
        Status = WorkflowStatus.Draft;
        IsActive = false;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
        ActivatedAt = null;
        ActivatedBy = null;

        AddDomainEvent(new WorkflowDefinitionVersionCreatedDomainEvent(Id, WorkflowCode, Version, modifiedBy));
    }

    public void ArchiveWorkflow(string archivedBy)
    {
        if (IsActive)
            throw new InvalidOperationException("Cannot archive active workflow");

        Status = WorkflowStatus.Archived;
        ModifiedBy = archivedBy;
        ModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkflowDefinitionArchivedDomainEvent(Id, WorkflowCode, archivedBy));
    }

    public WorkflowStep? GetStep(int sequence)
    {
        return _steps.FirstOrDefault(s => s.Sequence == sequence);
    }

    public WorkflowStep? GetNextStep(int currentSequence)
    {
        return _steps.Where(s => s.Sequence > currentSequence)
                    .OrderBy(s => s.Sequence)
                    .FirstOrDefault();
    }

    public WorkflowApprovalLevel? GetApprovalLevel(int level)
    {
        return _approvalLevels.FirstOrDefault(a => a.Level == level);
    }

    public IEnumerable<WorkflowApprovalLevel> GetApplicableApprovalLevels(Money? amount = null)
    {
        return _approvalLevels.Where(a => 
            (a.MinimumAmount == null || amount == null || amount.Amount >= a.MinimumAmount.Amount) &&
            (a.MaximumAmount == null || amount == null || amount.Amount <= a.MaximumAmount.Amount))
            .OrderBy(a => a.Level);
    }

    public bool RequiresApproval(Money? amount = null)
    {
        return RequiresMakerChecker && GetApplicableApprovalLevels(amount).Any();
    }

    public int GetTotalSteps()
    {
        return _steps.Count;
    }

    public int GetMandatorySteps()
    {
        return _steps.Count(s => s.IsMandatory);
    }
}

/// <summary>
/// Individual step within a workflow definition
/// </summary>
public class WorkflowStep
{
    public Guid Id { get; private set; }
    public Guid WorkflowDefinitionId { get; private set; }
    public string StepName { get; private set; }
    public string StepType { get; private set; }
    public int Sequence { get; private set; }
    public bool IsMandatory { get; private set; }
    public string? AssigneeRole { get; private set; }
    public string? AssigneeUser { get; private set; }
    public string? Conditions { get; private set; } // JSON conditions
    public DateTime CreatedAt { get; private set; }

    private WorkflowStep() { } // EF Core

    public WorkflowStep(
        Guid id,
        Guid workflowDefinitionId,
        string stepName,
        string stepType,
        int sequence,
        bool isMandatory,
        string? assigneeRole,
        string? assigneeUser,
        string? conditions,
        DateTime createdAt)
    {
        Id = id;
        WorkflowDefinitionId = workflowDefinitionId;
        StepName = stepName;
        StepType = stepType;
        Sequence = sequence;
        IsMandatory = isMandatory;
        AssigneeRole = assigneeRole;
        AssigneeUser = assigneeUser;
        Conditions = conditions;
        CreatedAt = createdAt;
    }
}

/// <summary>
/// Approval level within a workflow definition
/// </summary>
public class WorkflowApprovalLevel
{
    public Guid Id { get; private set; }
    public Guid WorkflowDefinitionId { get; private set; }
    public int Level { get; private set; }
    public string ApproverRole { get; private set; }
    public string? ApproverUser { get; private set; }
    public Money? MinimumAmount { get; private set; }
    public Money? MaximumAmount { get; private set; }
    public bool IsRequired { get; private set; }
    public string? Conditions { get; private set; } // JSON conditions
    public DateTime CreatedAt { get; private set; }

    private WorkflowApprovalLevel() { } // EF Core

    public WorkflowApprovalLevel(
        Guid id,
        Guid workflowDefinitionId,
        int level,
        string approverRole,
        string? approverUser,
        Money? minimumAmount,
        Money? maximumAmount,
        bool isRequired,
        string? conditions,
        DateTime createdAt)
    {
        Id = id;
        WorkflowDefinitionId = workflowDefinitionId;
        Level = level;
        ApproverRole = approverRole;
        ApproverUser = approverUser;
        MinimumAmount = minimumAmount;
        MaximumAmount = maximumAmount;
        IsRequired = isRequired;
        Conditions = conditions;
        CreatedAt = createdAt;
    }
}

// Enums
public enum WorkflowCategory
{
    AccountManagement = 1,
    LoanProcessing = 2,
    PaymentProcessing = 3,
    TradeFinance = 4,
    ComplianceReview = 5,
    RiskAssessment = 6,
    CustomerOnboarding = 7,
    ProductManagement = 8,
    SystemAdministration = 9
}

public enum WorkflowType
{
    Sequential = 1,
    Parallel = 2,
    Conditional = 3,
    MakerChecker = 4,
    Approval = 5,
    Review = 6,
    Notification = 7
}

public enum WorkflowStatus
{
    Draft = 1,
    Validated = 2,
    ValidationFailed = 3,
    Active = 4,
    Inactive = 5,
    Archived = 6
}

// Domain Events
public record WorkflowDefinitionCreatedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    string WorkflowName,
    WorkflowCategory Category) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record WorkflowDefinitionValidatedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record WorkflowDefinitionValidationFailedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    List<string> ValidationErrors) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record WorkflowDefinitionActivatedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    string ActivatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record WorkflowDefinitionDeactivatedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    string DeactivatedBy,
    string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record WorkflowDefinitionVersionCreatedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    int Version,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record WorkflowDefinitionArchivedDomainEvent(
    Guid WorkflowId,
    string WorkflowCode,
    string ArchivedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}