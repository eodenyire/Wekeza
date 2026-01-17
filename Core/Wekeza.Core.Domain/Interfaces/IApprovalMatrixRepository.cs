using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

public interface IApprovalMatrixRepository
{
    Task<ApprovalMatrix?> GetByIdAsync(Guid id);
    Task<IEnumerable<ApprovalMatrix>> GetByWorkflowTypeAsync(WorkflowType workflowType);
    Task<ApprovalMatrix?> GetApplicableMatrixAsync(WorkflowType workflowType, decimal amount, string currency);
    Task<IEnumerable<ApprovalMatrix>> GetAllActiveAsync();
    void Add(ApprovalMatrix approvalMatrix);
    void Update(ApprovalMatrix approvalMatrix);
    void Remove(ApprovalMatrix approvalMatrix);
}