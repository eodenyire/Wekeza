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
    Conditional,        // Conditional routing
    PaymentApproval,    // Payment approval workflow
    LoanApproval,       // Loan approval workflow
    AccountOpening      // Account opening workflow
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
