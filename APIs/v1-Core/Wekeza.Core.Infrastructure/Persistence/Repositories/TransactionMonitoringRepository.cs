using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TransactionMonitoring aggregate
/// </summary>
public class TransactionMonitoringRepository : ITransactionMonitoringRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionMonitoringRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TransactionMonitoring?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitorings
            .FirstOrDefaultAsync(tm => tm.Id == id, cancellationToken);
    }

    public async Task<TransactionMonitoring?> GetByTransactionIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitorings
            .FirstOrDefaultAsync(tm => tm.TransactionId == transactionId, cancellationToken);
    }

    public async Task<IEnumerable<TransactionMonitoring>> GetByStatusAsync(MonitoringStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitorings
            .Where(tm => tm.Status == status)
            .OrderByDescending(tm => tm.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TransactionMonitoring>> GetBySeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitorings
            .Where(tm => tm.Severity == severity)
            .OrderByDescending(tm => tm.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TransactionMonitoring>> GetByResultAsync(ScreeningResult result, CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitorings
            .Where(tm => tm.Result == result)
            .OrderByDescending(tm => tm.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TransactionMonitoring>> GetPendingReviewAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitorings
            .Where(tm => tm.Status == MonitoringStatus.Pending || tm.Status == MonitoringStatus.PendingInfo)
            .OrderByDescending(tm => tm.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TransactionMonitoring>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitorings
            .Where(tm => tm.CreatedAt >= fromDate && tm.CreatedAt <= toDate)
            .OrderByDescending(tm => tm.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TransactionMonitoring>> GetHighSeverityAlertsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitorings
            .Where(tm => tm.Severity == AlertSeverity.High || tm.Severity == AlertSeverity.Critical)
            .OrderByDescending(tm => tm.Severity)
            .ThenByDescending(tm => tm.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TransactionMonitoring>> GetOverdueReviewsAsync(int daysOverdue, CancellationToken cancellationToken = default)
    {
        var dueDate = DateTime.UtcNow.AddDays(-daysOverdue);
        return await _context.TransactionMonitorings
            .Where(tm => (tm.Status == MonitoringStatus.Pending || tm.Status == MonitoringStatus.PendingInfo) && tm.CreatedAt <= dueDate)
            .OrderBy(tm => tm.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TransactionMonitoring>> GetByReviewerAsync(string reviewerId, CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitorings
            .Where(tm => tm.ReviewedBy == reviewerId)
            .OrderByDescending(tm => tm.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetAlertCountByStatusAsync(MonitoringStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitorings
            .CountAsync(tm => tm.Status == status, cancellationToken);
    }

    public async Task<int> GetAlertCountBySeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitorings
            .CountAsync(tm => tm.Severity == severity, cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetAlertStatisticsByRuleAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitorings
            .Where(tm => tm.CreatedAt >= fromDate && tm.CreatedAt <= toDate)
            .SelectMany(tm => tm.AppliedRules.Select(rule => new { RuleName = rule }))
            .GroupBy(x => x.RuleName)
            .Select(g => new { RuleName = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RuleName, x => x.Count, cancellationToken);
    }

    public async Task AddAsync(TransactionMonitoring monitoring, CancellationToken cancellationToken = default)
    {
        await _context.TransactionMonitorings.AddAsync(monitoring, cancellationToken);
    }

    public async Task UpdateAsync(TransactionMonitoring monitoring, CancellationToken cancellationToken = default)
    {
        _context.TransactionMonitorings.Update(monitoring);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(TransactionMonitoring monitoring, CancellationToken cancellationToken = default)
    {
        _context.TransactionMonitorings.Remove(monitoring);
        await Task.CompletedTask;
    }
}
