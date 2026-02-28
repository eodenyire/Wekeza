using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for AuditLog aggregate
/// </summary>
public class AuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<AuditLog>> SearchAsync(
        string? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? action = null,
        int page = 1,
        int pageSize = 100,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(a => a.UserId == userId);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(a => a.Timestamp >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(a => a.Timestamp <= toDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(a => a.Action.Contains(action));
        }

        return await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetUserChangeHistoryAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetEntityChangeHistoryAsync(string entityType, string entityId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(a => a.Resource == entityType && a.ResourceId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetRecentAuditLogsAsync(int limit = 100, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetAuditCountByUserAsync(string userId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .CountAsync(a => a.UserId == userId && a.Timestamp >= fromDate && a.Timestamp <= toDate, cancellationToken);
    }
}
