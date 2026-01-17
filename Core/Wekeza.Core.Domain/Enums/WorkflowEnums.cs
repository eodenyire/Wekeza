namespace Wekeza.Core.Domain.Enums;

/// <summary>
/// Workflow type classification
/// </summary>
public enum WorkflowType
{
    MakerChecker,       // Simple dual authorization
    MultiLevel,         // Multi-level approval
    Sequential,         // Sequential approval chain
    Parallel,           // Parallel approval (all must approve)
    Conditional         // Conditional routing
}

/// <summary>
/// Workflow instance status
/// </summary>
public enum WorkflowStatus
{
    Pending,            // Awaiting approval
    Approved,           // Fully approved
    Rejected,           // Rejected by approver
    Cancelled,          // Cancelled by maker
    Expired             // SLA exceeded
}

/// <summary>
/// Approval step status
/// </summary>
public enum ApprovalStepStatus
{
    Pending,            // Awaiting approval
    Approved,           // Approved
    Rejected,           // Rejected
    Skipped             // Skipped (conditional)
}

/// <summary>
/// Approval matrix status
/// </summary>
public enum MatrixStatus
{
    Draft,              // Being configured
    Active,             // In use
    Inactive            // Disabled
}

/// <summary>
/// Task priority
/// </summary>
public enum TaskPriority
{
    Low,
    Normal,
    High,
    Critical
}

/// <summary>
/// General priority classification
/// </summary>
public enum Priority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Urgent = 4,
    Critical = 5
}

/// <summary>
/// Task status
/// </summary>
public enum TaskStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled,
    Overdue
}
