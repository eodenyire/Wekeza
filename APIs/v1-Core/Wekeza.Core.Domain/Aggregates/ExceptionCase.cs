using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Exception Case - Manages operational exceptions, investigations, and resolutions
/// Supports case management, approval workflow, and root cause analysis
/// Industry standard: T24 Case Management, Finacle Exception Management
/// </summary>
public class ExceptionCase : AggregateRoot
{
    public string CaseNumber { get; private set; } // System-generated: EXC-YYYYMMDD-XXXXX
    public string CaseTitle { get; private set; }
    public string CaseDescription { get; private set; }
    public ExceptionType ExceptionType { get; private set; }
    public ExceptionCategory Category { get; private set; }
    public ExceptionPriority Priority { get; private set; }
    
    // Case Entity Details
    public string EntityType { get; private set; } // Account, Transaction, Loan, Customer, etc.
    public string EntityId { get; private set; }
    public Dictionary<string, object> EntityDetails { get; private set; }
    
    // Status & Lifecycle
    public ExceptionCaseStatus Status { get; private set; }
    public ExceptionSeverity Severity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public string? ResolvedBy { get; private set; }
    
    // Assignment & Routing
    public Guid? AssignedToUserId { get; private set; }
    public string? AssignedToUser { get; private set; }
    public DateTime? AssignedAt { get; private set; }
    public List<ExceptionCaseAssignment> AssignmentHistory { get; private set; }
    
    // Investigation & Resolution
    public List<ExceptionCaseComment> Comments { get; private set; }
    public List<ExceptionCaseAttachment> Attachments { get; private set; }
    public string? RootCauseAnalysis { get; private set; }
    public string? Resolution { get; private set; }
    public string? ResolutionAction { get; private set; }
    
    // Approval & Workflow
    public bool RequiresApproval { get; private set; }
    public List<ExceptionCaseApproval> ApprovalChain { get; private set; }
    public ExceptionCaseApprovalStatus ApprovalStatus { get; private set; }
    
    // Impact Assessment
    public decimal? FinancialImpact { get; private set; }
    public string? OperationalImpact { get; private set; }
    public string? RegulatoryImpact { get; private set; }
    public List<string> AffectedRecords { get; private set; }
    
    // Timeline & Escalation
    public DateTime? SLA_DueDate { get; private set; }
    public bool IsEscalated { get; private set; }
    public DateTime? EscalatedAt { get; private set; }
    public string? EscalationReason { get; private set; }
    public int EscalationLevel { get; private set; }
    
    // Metrics
    public TimeSpan ResolutionTime => ResolvedAt.HasValue ? ResolvedAt.Value - CreatedAt : DateTime.UtcNow - CreatedAt;
    public int CommentCount { get; private set; }
    public List<string> Tags { get; private set; }

    private ExceptionCase() : base(Guid.NewGuid())
    {
        EntityDetails = new Dictionary<string, object>();
        AssignmentHistory = new List<ExceptionCaseAssignment>();
        Comments = new List<ExceptionCaseComment>();
        Attachments = new List<ExceptionCaseAttachment>();
        ApprovalChain = new List<ExceptionCaseApproval>();
        AffectedRecords = new List<string>();
        Tags = new List<string>();
    }

    public ExceptionCase(
        string caseTitle,
        string caseDescription,
        ExceptionType exceptionType,
        ExceptionCategory category,
        ExceptionPriority priority,
        string entityType,
        string entityId,
        string createdBy,
        Dictionary<string, object> entityDetails) : this()
    {
        if (string.IsNullOrWhiteSpace(caseTitle))
            throw new ArgumentException("Case title cannot be empty", nameof(caseTitle));

        Id = Guid.NewGuid();
        CaseNumber = GenerateCaseNumber();
        CaseTitle = caseTitle;
        CaseDescription = caseDescription;
        ExceptionType = exceptionType;
        Category = category;
        Priority = priority;
        EntityType = entityType;
        EntityId = entityId;
        EntityDetails = entityDetails ?? new Dictionary<string, object>();
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        Status = ExceptionCaseStatus.Open;
        Severity = DetermineSeverity(priority, exceptionType);
        SLA_DueDate = CalculateSLADueDate(priority);
        ApprovalStatus = ExceptionCaseApprovalStatus.None;
    }

    public void AssignCase(Guid userId, string userName, string assignedBy)
    {
        if (Status == ExceptionCaseStatus.Closed || Status == ExceptionCaseStatus.Resolved)
            throw new InvalidOperationException("Cannot assign closed or resolved case");

        AssignedToUserId = userId;
        AssignedToUser = userName;
        AssignedAt = DateTime.UtcNow;

        AssignmentHistory.Add(new ExceptionCaseAssignment
        {
            UserId = userId,
            UserName = userName,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy
        });
    }

    public void AddComment(string commentText, string commentedBy, bool isInternalNote = false)
    {
        Comments.Add(new ExceptionCaseComment
        {
            CommentText = commentText,
            CommentedBy = commentedBy,
            CommentedAt = DateTime.UtcNow,
            IsInternalNote = isInternalNote
        });
        CommentCount++;
    }

    public void AddAttachment(string fileName, string fileUrl, string uploadedBy, long fileSizeBytes)
    {
        Attachments.Add(new ExceptionCaseAttachment
        {
            FileName = fileName,
            FileUrl = fileUrl,
            UploadedBy = uploadedBy,
            UploadedAt = DateTime.UtcNow,
            FileSizeBytes = fileSizeBytes
        });
    }

    public void SubmitForApproval(List<Guid> approverIds, string submittedBy, string? approvalComments = null)
    {
        if (Status != ExceptionCaseStatus.Open)
            throw new InvalidOperationException("Can only submit open cases for approval");

        RequiresApproval = true;
        ApprovalStatus = ExceptionCaseApprovalStatus.Pending;

        foreach (var approverId in approverIds)
        {
            ApprovalChain.Add(new ExceptionCaseApproval
            {
                ApproverId = approverId,
                Status = ExceptionCaseApprovalStatus.Pending,
                RequestedAt = DateTime.UtcNow,
                RequestedBy = submittedBy,
                Comments = approvalComments
            });
        }
    }

    public void ApproveCase(Guid approverId, string approverName, string? approvalComments = null)
    {
        var approval = ApprovalChain.FirstOrDefault(a => a.ApproverId == approverId);
        if (approval == null)
            throw new InvalidOperationException("User is not in approval chain");

        approval.Status = ExceptionCaseApprovalStatus.Approved;
        approval.ApprovedAt = DateTime.UtcNow;
        approval.ApprovedBy = approverName;
        approval.Comments = approvalComments;

        // Mark as approved if all approvers have approved
        if (ApprovalChain.All(a => a.Status == ExceptionCaseApprovalStatus.Approved))
            ApprovalStatus = ExceptionCaseApprovalStatus.Approved;
    }

    public void RejectCase(Guid rejecterId, string rejectionReason)
    {
        var approval = ApprovalChain.FirstOrDefault(a => a.ApproverId == rejecterId);
        if (approval == null)
            throw new InvalidOperationException("User is not in approval chain");

        approval.Status = ExceptionCaseApprovalStatus.Rejected;
        approval.RejectedAt = DateTime.UtcNow;
        approval.RejectionReason = rejectionReason;
        ApprovalStatus = ExceptionCaseApprovalStatus.Rejected;

        Status = ExceptionCaseStatus.Open;
    }

    public void ResolveCase(string rootCause, string resolution, string resolutionAction, 
        Guid? approverId = null, string? approverName = null, bool requiresApproval = false)
    {
        if (requiresApproval && ApprovalStatus != ExceptionCaseApprovalStatus.Approved)
            throw new InvalidOperationException("Resolution requires approval before closing");

        RootCauseAnalysis = rootCause;
        Resolution = resolution;
        ResolutionAction = resolutionAction;
        Status = ExceptionCaseStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = approverName ?? "System";
    }

    public void CloseCase(string closedBy)
    {
        if (Status != ExceptionCaseStatus.Resolved)
            throw new InvalidOperationException("Can only close resolved cases");

        Status = ExceptionCaseStatus.Closed;
    }

    public void EscalateCase(string escalationReason, string escalatedBy)
    {
        if (IsEscalated && EscalationLevel >= 3)
            throw new InvalidOperationException("Case is at maximum escalation level");

        IsEscalated = true;
        EscalatedAt = DateTime.UtcNow;
        EscalationReason = escalationReason;
        EscalationLevel++;

        AddComment($"Case escalated: {escalationReason}", escalatedBy, true);
    }

    public void RecordImpact(decimal? financialImpact, string? operationalImpact, string? regulatoryImpact)
    {
        FinancialImpact = financialImpact;
        OperationalImpact = operationalImpact;
        RegulatoryImpact = regulatoryImpact;
    }

    private static string GenerateCaseNumber()
    {
        return $"EXC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}";
    }

    private static ExceptionSeverity DetermineSeverity(ExceptionPriority priority, ExceptionType exceptionType)
    {
        return (priority, exceptionType) switch
        {
            (ExceptionPriority.Critical, _) => ExceptionSeverity.Critical,
            (ExceptionPriority.High, ExceptionType.SystemError) => ExceptionSeverity.High,
            (ExceptionPriority.High, _) => ExceptionSeverity.Moderate,
            _ => ExceptionSeverity.Low
        };
    }

    private static DateTime CalculateSLADueDate(ExceptionPriority priority) =>
        DateTime.UtcNow.AddHours(priority switch
        {
            ExceptionPriority.Critical => 1,
            ExceptionPriority.High => 4,
            ExceptionPriority.Medium => 8,
            _ => 24
        });
}

public class ExceptionCaseComment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CommentText { get; set; }
    public string CommentedBy { get; set; }
    public DateTime CommentedAt { get; set; }
    public bool IsInternalNote { get; set; }
}

public class ExceptionCaseAttachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public string UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
    public long FileSizeBytes { get; set; }
}

public class ExceptionCaseAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public DateTime AssignedAt { get; set; }
    public string AssignedBy { get; set; }
    public DateTime? UnassignedAt { get; set; }
}

public class ExceptionCaseApproval
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApproverId { get; set; }
    public ExceptionCaseApprovalStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public string RequestedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string? Comments { get; set; }
}

public enum ExceptionType
{
    SystemError = 1,
    DataInconsistency = 2,
    ProcessException = 3,
    ValidationFailure = 4,
    IntegrationError = 5,
    SecurityIssue = 6,
    ComplianceViolation = 7,
    OperationalIssue = 8
}

public enum ExceptionCategory
{
    Technical = 1,
    Operational = 2,
    Financial = 3,
    Compliance = 4,
    Security = 5,
    Data = 6
}

public enum ExceptionPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum ExceptionCaseStatus
{
    Open = 1,
    InProgress = 2,
    OnHold = 3,
    Resolved = 4,
    Closed = 5,
    Escalated = 6
}

public enum ExceptionSeverity
{
    Low = 1,
    Moderate = 2,
    High = 3,
    Critical = 4
}

public enum ExceptionCaseApprovalStatus
{
    None = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Escalated = 4
}
