using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services;

public interface IMakerCheckerService
{
    // Workflow Initiation
    Task<WorkflowInstance> InitiateMakerActionAsync(MakerAction action);
    Task<WorkflowInstance?> GetWorkflowInstanceAsync(Guid workflowId);
    Task<List<WorkflowInstance>> GetWorkflowInstancesByUserAsync(Guid userId);
    Task<List<WorkflowInstance>> GetWorkflowInstancesByStatusAsync(WorkflowStatus status);

    // Approval Processing
    Task<ApprovalResult> SubmitForApprovalAsync(Guid workflowId, Guid checkerId, string? comments = null);
    Task<ApprovalResult> RejectWorkflowAsync(Guid workflowId, Guid checkerId, string reason);
    Task<List<PendingApproval>> GetApprovalQueueAsync(Guid checkerId);
    Task<List<PendingApproval>> GetApprovalQueueByRoleAsync(string roleName);

    // Escalation Management
    Task<EscalationResult> EscalateApprovalAsync(Guid workflowId, string reason);
    Task<EscalationResult> AutoEscalateExpiredWorkflowsAsync();
    Task<List<WorkflowInstance>> GetExpiredWorkflowsAsync();

    // Workflow Management
    Task<bool> CancelWorkflowAsync(Guid workflowId, Guid cancelledBy, string reason);
    Task<bool> CompleteWorkflowAsync(Guid workflowId);
    Task<List<ApprovalStep>> GetWorkflowStepsAsync(Guid workflowId);

    // Validation
    Task<ValidationResult> ValidateMakerCheckerRulesAsync(Guid makerId, Guid checkerId, string actionType);
    Task<bool> CanUserApproveWorkflowAsync(Guid userId, Guid workflowId);

    // Reporting and Analytics
    Task<WorkflowMetrics> GetWorkflowMetricsAsync(DateTime fromDate, DateTime toDate);
    Task<List<WorkflowInstance>> GetWorkflowHistoryAsync(Guid userId, int pageSize = 50, int pageNumber = 1);
}

// DTOs and Models
public class MakerAction
{
    public string ActionType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty; // JSON serialized data
    public Guid MakerId { get; set; }
    public string BusinessJustification { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Critical
    public DateTime? RequestedCompletionDate { get; set; }
}

public class ApprovalResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public WorkflowInstance? WorkflowInstance { get; set; }
    public string? ErrorCode { get; set; }
}

public class PendingApproval
{
    public Guid WorkflowId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public string BusinessJustification { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public string Priority { get; set; } = string.Empty;
    public DateTime InitiatedAt { get; set; }
    public DateTime? ApprovalDeadline { get; set; }
    public string InitiatedByName { get; set; } = string.Empty;
    public string CurrentApproverRole { get; set; } = string.Empty;
    public int DaysRemaining { get; set; }
    public bool IsOverdue { get; set; }
}

public class EscalationResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<Guid> EscalatedWorkflowIds { get; set; } = new();
    public string? NewApproverRole { get; set; }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> ValidationErrors { get; set; } = new();
}

public class WorkflowMetrics
{
    public int TotalWorkflows { get; set; }
    public int PendingWorkflows { get; set; }
    public int ApprovedWorkflows { get; set; }
    public int RejectedWorkflows { get; set; }
    public int EscalatedWorkflows { get; set; }
    public double AverageProcessingTimeHours { get; set; }
    public int OverdueWorkflows { get; set; }
    public Dictionary<string, int> WorkflowsByType { get; set; } = new();
    public Dictionary<string, int> WorkflowsByPriority { get; set; } = new();
}

public enum WorkflowStatus
{
    Pending,
    InProgress,
    Approved,
    Rejected,
    Cancelled,
    Escalated,
    Completed,
    Expired
}

public enum ApprovalStepStatus
{
    Pending,
    Approved,
    Rejected,
    Skipped,
    Escalated
}

public enum WorkflowPriority
{
    Low,
    Normal,
    High,
    Critical
}