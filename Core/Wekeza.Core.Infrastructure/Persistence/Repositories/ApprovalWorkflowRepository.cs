using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ApprovalWorkflow aggregate
/// </summary>
public class ApprovalWorkflowRepository : IApprovalWorkflowRepository
{
    private readonly ApplicationDbContext _context;

    public ApprovalWorkflowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApprovalWorkflow?> GetByIdAsync(Guid id)
    {
        return await _context.ApprovalWorkflows
            .Include(w => w.ApprovalSteps)
            .Include(w => w.Comments)
            .Include(w => w.Documents)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<ApprovalWorkflow?> GetByWorkflowCodeAsync(string workflowCode)
    {
        return await _context.ApprovalWorkflows
            .Include(w => w.ApprovalSteps)
            .Include(w => w.Comments)
            .Include(w => w.Documents)
            .FirstOrDefaultAsync(w => w.WorkflowCode == workflowCode);
    }

    public async Task<IEnumerable<ApprovalWorkflow>> GetByEntityAsync(string entityType, Guid entityId)
    {
        return await _context.ApprovalWorkflows
            .Include(w => w.ApprovalSteps)
            .Include(w => w.Comments)
            .Where(w => w.EntityType == entityType && w.EntityId == entityId)
            .OrderByDescending(w => w.InitiatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ApprovalWorkflow>> GetPendingForUserAsync(string userId)
    {
        return await _context.ApprovalWorkflows
            .Include(w => w.ApprovalSteps)
            .Where(w => w.Status == WorkflowStatus.InProgress &&
                       w.ApprovalSteps.Any(s => s.Level == w.CurrentLevel &&
                                              s.Status == ApprovalStepStatus.Assigned &&
                                              (s.SpecificApprover == userId || string.IsNullOrEmpty(s.SpecificApprover))))
            .OrderBy(w => w.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ApprovalWorkflow>> GetOverdueAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.ApprovalWorkflows
            .Include(w => w.ApprovalSteps)
            .Where(w => w.Status == WorkflowStatus.InProgress && 
                       w.DueDate.HasValue && 
                       w.DueDate.Value < now)
            .OrderBy(w => w.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ApprovalWorkflow>> GetByStatusAsync(WorkflowStatus status)
    {
        return await _context.ApprovalWorkflows
            .Include(w => w.ApprovalSteps)
            .Where(w => w.Status == status)
            .OrderByDescending(w => w.InitiatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ApprovalWorkflow>> GetByBranchAsync(string branchCode)
    {
        return await _context.ApprovalWorkflows
            .Include(w => w.ApprovalSteps)
            .Where(w => w.BranchCode == branchCode)
            .OrderByDescending(w => w.InitiatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ApprovalWorkflow>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.ApprovalWorkflows
            .Include(w => w.ApprovalSteps)
            .Where(w => w.InitiatedAt >= fromDate && w.InitiatedAt <= toDate)
            .OrderByDescending(w => w.InitiatedAt)
            .ToListAsync();
    }

    public async Task<bool> ExistsForEntityAsync(string entityType, Guid entityId)
    {
        return await _context.ApprovalWorkflows
            .AnyAsync(w => w.EntityType == entityType && 
                          w.EntityId == entityId && 
                          w.Status == WorkflowStatus.InProgress);
    }

    public async Task AddAsync(ApprovalWorkflow workflow)
    {
        await _context.ApprovalWorkflows.AddAsync(workflow);
    }

    public async Task UpdateAsync(ApprovalWorkflow workflow)
    {
        _context.ApprovalWorkflows.Update(workflow);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var workflow = await _context.ApprovalWorkflows.FindAsync(id);
        if (workflow != null)
        {
            _context.ApprovalWorkflows.Remove(workflow);
        }
    }
}