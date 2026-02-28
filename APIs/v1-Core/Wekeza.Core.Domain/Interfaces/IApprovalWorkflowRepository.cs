using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for ApprovalWorkflow aggregate
/// </summary>
public interface IApprovalWorkflowRepository
{
    Task<ApprovalWorkflow?> GetByIdAsync(Guid id);
    Task<ApprovalWorkflow?> GetByWorkflowCodeAsync(string workflowCode);
    Task<IEnumerable<ApprovalWorkflow>> GetByEntityAsync(string entityType, Guid entityId);
    Task<IEnumerable<ApprovalWorkflow>> GetPendingForUserAsync(string userId);
    Task<IEnumerable<ApprovalWorkflow>> GetOverdueAsync();
    Task<IEnumerable<ApprovalWorkflow>> GetByStatusAsync(WorkflowStatus status);
    Task<IEnumerable<ApprovalWorkflow>> GetByBranchAsync(string branchCode);
    Task<IEnumerable<ApprovalWorkflow>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<bool> ExistsForEntityAsync(string entityType, Guid entityId);
    Task AddAsync(ApprovalWorkflow workflow);
    Task UpdateAsync(ApprovalWorkflow workflow);
    Task DeleteAsync(Guid id);
}