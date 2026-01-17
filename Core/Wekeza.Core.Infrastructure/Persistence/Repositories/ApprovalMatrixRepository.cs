using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

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

    public async Task<IEnumerable<ApprovalMatrix>> GetByWorkflowTypeAsync(WorkflowType workflowType)
    {
        return await _context.ApprovalMatrices
            .Where(a => a.WorkflowType == workflowType && a.IsActive)
            .OrderBy(a => a.MinAmount)
            .ToListAsync();
    }

    public async Task<ApprovalMatrix?> GetApplicableMatrixAsync(WorkflowType workflowType, decimal amount, string currency)
    {
        return await _context.ApprovalMatrices
            .Where(a => a.WorkflowType == workflowType 
                       && a.IsActive 
                       && a.Currency == currency
                       && amount >= a.MinAmount 
                       && amount <= a.MaxAmount)
            .OrderBy(a => a.MinAmount)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ApprovalMatrix>> GetAllActiveAsync()
    {
        return await _context.ApprovalMatrices
            .Where(a => a.IsActive)
            .OrderBy(a => a.WorkflowType)
            .ThenBy(a => a.MinAmount)
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