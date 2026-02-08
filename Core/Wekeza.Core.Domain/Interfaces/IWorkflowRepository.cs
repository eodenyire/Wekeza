using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository for Workflow management
/// Inspired by Finacle Workflow Engine and T24 Maker-Checker
/// </summary>
public interface IWorkflowRepository
{
    Task<WorkflowInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkflowInstance?> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
    Task<WorkflowInstance?> GetByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default);
    Task AddAsync(WorkflowInstance workflow, CancellationToken cancellationToken = default);
    Task AddWorkflowAsync(WorkflowInstance workflow, CancellationToken cancellationToken = default);
    Task<ApprovalMatrix?> GetApprovalMatrixAsync(string workflowType, decimal amount, string currency, CancellationToken cancellationToken = default);
    void Update(WorkflowInstance workflow);
    void UpdateWorkflow(WorkflowInstance workflow);
    
    Task<IEnumerable<WorkflowInstance>> GetPendingWorkflowsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkflowInstance>> GetByStatusAsync(Enums.WorkflowStatus status, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<WorkflowInstance>> GetPendingForApproverAsync(string approverId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkflowInstance>> GetInitiatedByUserAsync(string userId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<WorkflowInstance>> GetByEntityTypeAsync(string entityType, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<WorkflowInstance>> GetOverdueWorkflowsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkflowInstance>> GetEscalatedWorkflowsAsync(CancellationToken cancellationToken = default);
    
    Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<Enums.WorkflowStatus, int>> GetCountByStatusAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetCountByEntityTypeAsync(CancellationToken cancellationToken = default);
}
