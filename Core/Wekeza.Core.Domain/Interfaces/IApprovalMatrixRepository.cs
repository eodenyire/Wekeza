using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

public interface IApprovalMatrixRepository
{
    Task<ApprovalMatrix?> GetByIdAsync(Guid id);
    Task<IEnumerable<ApprovalMatrix>> GetByWorkflowTypeAsync(Enums.WorkflowType workflowType);
    Task<ApprovalMatrix?> GetApplicableMatrixAsync(Enums.WorkflowType workflowType, decimal amount, string currency);
    Task<IEnumerable<ApprovalMatrix>> GetAllActiveAsync();
    Task<ApprovalMatrix?> GetByWorkflowCodeAsync(string workflowCode, CancellationToken cancellationToken = default);
    Task AddAsync(ApprovalMatrix approvalMatrix, CancellationToken cancellationToken = default);
    void Add(ApprovalMatrix approvalMatrix);
    void Update(ApprovalMatrix approvalMatrix);
    void Remove(ApprovalMatrix approvalMatrix);
}