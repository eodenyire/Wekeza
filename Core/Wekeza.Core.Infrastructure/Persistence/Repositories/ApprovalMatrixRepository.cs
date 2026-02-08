using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Enums;
using DomainWorkflowType = Wekeza.Core.Domain.Enums.WorkflowType;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class ApprovalMatrixRepository : IApprovalMatrixRepository
{
    private readonly ApplicationDbContext _context;

    public ApprovalMatrixRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApprovalMatrix?> GetByIdAsync(Guid id)
    {
        return await _context.ApprovalMatrices
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<ApprovalMatrix>> GetByWorkflowTypeAsync(DomainWorkflowType workflowType)
    {
        // Note: ApprovalMatrix uses EntityType, not WorkflowType
        // This filters by EntityType matching the workflow type's string representation
        var entityType = workflowType.ToString();
        return await _context.ApprovalMatrices
            .Where(a => a.EntityType == entityType && a.Status == MatrixStatus.Active)
            .OrderBy(a => a.MatrixCode)
            .ToListAsync();
    }

    public async Task<ApprovalMatrix?> GetApplicableMatrixAsync(DomainWorkflowType workflowType, decimal amount, string currency)
    {
        // Note: ApprovalMatrix uses EntityType, not WorkflowType
        // Rules contain MinAmount/MaxAmount/Currency information
        var entityType = workflowType.ToString();
        var matrices = await _context.ApprovalMatrices
            .Where(a => a.EntityType == entityType && a.Status == MatrixStatus.Active)
            .ToListAsync();
        
        // Filter in memory by checking rules
        return matrices.FirstOrDefault(a => a.Rules.Any(r => 
            r.MinAmount.HasValue && r.MaxAmount.HasValue &&
            amount >= r.MinAmount.Value && amount <= r.MaxAmount.Value));
    }

    public async Task<IEnumerable<ApprovalMatrix>> GetAllActiveAsync()
    {
        return await _context.ApprovalMatrices
            .Where(a => a.Status == MatrixStatus.Active)
            .OrderBy(a => a.EntityType)
            .ThenBy(a => a.MatrixCode)
            .ToListAsync();
    }

    public void Add(ApprovalMatrix approvalMatrix)
    {
        _context.ApprovalMatrices.Add(approvalMatrix);
    }

    public void Update(ApprovalMatrix approvalMatrix)
    {
        _context.ApprovalMatrices.Update(approvalMatrix);
    }

    public void Remove(ApprovalMatrix approvalMatrix)
    {
        _context.ApprovalMatrices.Remove(approvalMatrix);
    }
}