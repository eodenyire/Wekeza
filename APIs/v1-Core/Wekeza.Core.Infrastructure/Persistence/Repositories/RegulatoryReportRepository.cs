using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for RegulatoryReport aggregate
/// </summary>
public class RegulatoryReportRepository : IRegulatoryReportRepository
{
    private readonly ApplicationDbContext _context;

    public RegulatoryReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RegulatoryReport?> GetByIdAsync(Guid id)
    {
        return await _context.RegulatoryReports
            .Include(r => r.DataItems)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<RegulatoryReport?> GetByCodeAndPeriodAsync(string reportCode, DateTime periodStart, DateTime periodEnd)
    {
        return await _context.RegulatoryReports
            .Include(r => r.DataItems)
            .FirstOrDefaultAsync(r => r.ReportCode == reportCode && 
                                    r.ReportingPeriodStart == periodStart && 
                                    r.ReportingPeriodEnd == periodEnd);
    }

    public async Task<IEnumerable<RegulatoryReport>> GetByAuthorityAsync(RegulatoryAuthority authority)
    {
        return await _context.RegulatoryReports
            .Include(r => r.DataItems)
            .Where(r => r.Authority == authority)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<RegulatoryReport>> GetByStatusAsync(ReportStatus status)
    {
        return await _context.RegulatoryReports
            .Include(r => r.DataItems)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<RegulatoryReport>> GetOverdueReportsAsync()
    {
        var currentDate = DateTime.UtcNow;
        return await _context.RegulatoryReports
            .Include(r => r.DataItems)
            .Where(r => r.Status != ReportStatus.Submitted && r.DueDate < currentDate)
            .OrderBy(r => r.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<RegulatoryReport>> GetReportsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.RegulatoryReports
            .Include(r => r.DataItems)
            .Where(r => r.GeneratedAt >= startDate && r.GeneratedAt <= endDate)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<RegulatoryReport>> GetReportsByFrequencyAsync(ReportFrequency frequency)
    {
        return await _context.RegulatoryReports
            .Include(r => r.DataItems)
            .Where(r => r.Frequency == frequency)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }

    public async Task AddAsync(RegulatoryReport report)
    {
        await _context.RegulatoryReports.AddAsync(report);
    }

    public async Task UpdateAsync(RegulatoryReport report)
    {
        _context.RegulatoryReports.Update(report);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var report = await _context.RegulatoryReports.FindAsync(id);
        if (report != null)
        {
            _context.RegulatoryReports.Remove(report);
        }
    }

    public async Task<bool> ExistsAsync(string reportCode, DateTime periodStart, DateTime periodEnd)
    {
        return await _context.RegulatoryReports
            .AnyAsync(r => r.ReportCode == reportCode && 
                          r.ReportingPeriodStart == periodStart && 
                          r.ReportingPeriodEnd == periodEnd);
    }
}