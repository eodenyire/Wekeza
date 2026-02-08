using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class WorkflowRepository : IWorkflowRepository
{
    private readonly ApplicationDbContext _context;

    public WorkflowRepository(ApplicationDbContext context) => _context = context;

    public async Task<WorkflowInstance?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.WorkflowInstances.FirstOrDefaultAsync(w => w.Id == id, ct);
    }

    public async Task<WorkflowInstance?> GetByEntityAsync(string entityType, Guid entityId, CancellationToken ct = default)
    {
        return await _context.WorkflowInstances
            .Where(w => w.EntityType == entityType && w.EntityId == entityId)
            .OrderByDescending(w => w.InitiatedDate)
            .FirstOrDefaultAsync(ct);
    }

    public async Task AddAsync(WorkflowInstance workflow, CancellationToken ct = default)
    {
        await _context.WorkflowInstances.AddAsync(workflow, ct);
    }

    public void Update(WorkflowInstance workflow)
    {
        _context.WorkflowInstances.Update(workflow);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetPendingWorkflowsAsync(CancellationToken ct = default)
    {
        return await _context.WorkflowInstances
            .Where(w => w.Status == WorkflowStatus.Pending)
            .OrderBy(w => w.DueDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetByStatusAsync(WorkflowStatus status, CancellationToken ct = default)
    {
        return await _context.WorkflowInstances
            .Where(w => w.Status == status)
            .OrderByDescending(w => w.InitiatedDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetPendingForApproverAsync(string approverId, CancellationToken ct = default)
    {
        // This is simplified - in production, you'd check approval matrix and user roles
        return await _context.WorkflowInstances
            .Where(w => w.Status == WorkflowStatus.Pending)
            .OrderBy(w => w.DueDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetInitiatedByUserAsync(string userId, CancellationToken ct = default)
    {
        return await _context.WorkflowInstances
            .Where(w => w.InitiatedBy == userId)
            .OrderByDescending(w => w.InitiatedDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetByEntityTypeAsync(string entityType, CancellationToken ct = default)
    {
        return await _context.WorkflowInstances
            .Where(w => w.EntityType == entityType)
            .OrderByDescending(w => w.InitiatedDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetOverdueWorkflowsAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await _context.WorkflowInstances
            .Where(w => w.Status == WorkflowStatus.Pending && 
                       w.DueDate.HasValue && 
                       w.DueDate < now)
            .OrderBy(w => w.DueDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetEscalatedWorkflowsAsync(CancellationToken ct = default)
    {
        return await _context.WorkflowInstances
            .Where(w => w.IsEscalated && w.Status == WorkflowStatus.Pending)
            .OrderBy(w => w.EscalatedDate)
            .ToListAsync(ct);
    }

    public async Task<int> GetPendingCountAsync(CancellationToken ct = default)
    {
        return await _context.WorkflowInstances
            .CountAsync(w => w.Status == WorkflowStatus.Pending, ct);
    }

    public async Task<Dictionary<WorkflowStatus, int>> GetCountByStatusAsync(CancellationToken ct = default)
    {
        return await _context.WorkflowInstances
            .GroupBy(w => w.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, ct);
    }

    public async Task<Dictionary<string, int>> GetCountByEntityTypeAsync(CancellationToken ct = default)
    {
        return await _context.WorkflowInstances
            .GroupBy(w => w.EntityType)
            .Select(g => new { EntityType = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EntityType, x => x.Count, ct);
    }
}
