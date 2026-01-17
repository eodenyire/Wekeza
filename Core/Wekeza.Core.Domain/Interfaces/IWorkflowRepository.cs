using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository for Workflow management
/// Inspired by Finacle Workflow Engine and T24 Maker-Checker
/// </summary>
public interface IWorkflowRepository
{
    // Basic CRUD
    Task<WorkflowInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkflowInstance?> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
    Task AddAsync(WorkflowInstance workflow, CancellationToken cancellationToken = default);
    void Update(WorkflowInstance workflow);
    
    // Query by status
    Task<IEnumerable<WorkflowInstance>> GetPendingWorkflowsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkflowInstance>> GetByStatusAsync(WorkflowStatus status, CancellationToken cancellationToken = default);
    
    // Query by user
    Task<IEnumerable<WorkflowInstance>> GetPendingForApproverAsync(string approverId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkflowInstance>> GetInitiatedByUserAsync(string userId, CancellationToken cancellationToken = default);
    
    // Query by entity
    Task<IEnumerable<WorkflowInstance>> GetByEntityTypeAsync(string entityType, CancellationToken cancellationToken = default);
    
    // SLA monitoring
    Task<IEnumerable<WorkflowInstance>> GetOverdueWorkflowsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkflowInstance>> GetEscalatedWorkflowsAsync(CancellationToken cancellationToken = default);
    
    // Analytics
    Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<WorkflowStatus, int>> GetCountByStatusAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetCountByEntityTypeAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for Approval Matrix management
/// </summary>
public interface IApprovalMatrixRepository
{
    Task<ApprovalMatrix?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApprovalMatrix?> GetByMatrixCodeAsync(string matrixCode, CancellationToken cancellationToken = default);
    Task<ApprovalMatrix?> GetByEntityTypeAsync(string entityType, CancellationToken cancellationToken = default);
    Task AddAsync(ApprovalMatrix matrix, CancellationToken cancellationToken = default);
    void Update(ApprovalMatrix matrix);
    Task<IEnumerable<ApprovalMatrix>> GetActiveMatricesAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByMatrixCodeAsync(string matrixCode, CancellationToken cancellationToken = default);
}
