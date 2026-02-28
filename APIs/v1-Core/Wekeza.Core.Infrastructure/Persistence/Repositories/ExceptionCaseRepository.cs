using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ExceptionCase aggregate
/// </summary>
public class ExceptionCaseRepository
{
    private readonly ApplicationDbContext _context;

    public ExceptionCaseRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ExceptionCase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ExceptionCases
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<ExceptionCase?> GetByCaseNumberAsync(string caseNumber, CancellationToken cancellationToken = default)
    {
        return await _context.ExceptionCases
            .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber, cancellationToken);
    }

    public async Task<List<ExceptionCase>> SearchAsync(
        ExceptionCaseStatus? status = null,
        ExceptionPriority? priority = null,
        ExceptionCategory? category = null,
        bool? isEscalated = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ExceptionCases.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(c => c.Priority == priority.Value);
        }

        if (category.HasValue)
        {
            query = query.Where(c => c.Category == category.Value);
        }

        if (isEscalated.HasValue)
        {
            query = query.Where(c => c.IsEscalated == isEscalated.Value);
        }

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ExceptionCase>> GetOpenCasesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ExceptionCases
            .Where(c => c.Status == ExceptionCaseStatus.Open || c.Status == ExceptionCaseStatus.InProgress)
            .OrderByDescending(c => c.Priority)
            .ThenBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetOpenCasesCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ExceptionCases
            .CountAsync(c => c.Status == ExceptionCaseStatus.Open || c.Status == ExceptionCaseStatus.InProgress, cancellationToken);
    }

    public async Task<int> GetEscalatedCasesCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ExceptionCases
            .CountAsync(c => c.IsEscalated, cancellationToken);
    }

    public async Task<List<ExceptionCase>> GetCasesByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.ExceptionCases
            .Where(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ExceptionCase>> GetCasesByAssigneeAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.ExceptionCases
            .Where(c => c.AssignedToUserId == userId && 
                       (c.Status == ExceptionCaseStatus.Open || c.Status == ExceptionCaseStatus.InProgress))
            .OrderBy(c => c.SLA_DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ExceptionCase>> GetOverdueCasesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.ExceptionCases
            .Where(c => c.SLA_DueDate.HasValue && c.SLA_DueDate.Value < now && 
                       (c.Status == ExceptionCaseStatus.Open || c.Status == ExceptionCaseStatus.InProgress))
            .OrderBy(c => c.SLA_DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ExceptionCase exceptionCase, CancellationToken cancellationToken = default)
    {
        await _context.ExceptionCases.AddAsync(exceptionCase, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ExceptionCase exceptionCase, CancellationToken cancellationToken = default)
    {
        _context.ExceptionCases.Update(exceptionCase);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exceptionCase = await GetByIdAsync(id, cancellationToken);
        if (exceptionCase != null)
        {
            _context.ExceptionCases.Remove(exceptionCase);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetTotalCaseCountAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.ExceptionCases
            .CountAsync(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate, cancellationToken);
    }

    public async Task<int> GetResolvedCaseCountAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.ExceptionCases
            .CountAsync(c => c.Status == ExceptionCaseStatus.Resolved && 
                            c.ResolvedAt.HasValue && 
                            c.ResolvedAt.Value >= fromDate && 
                            c.ResolvedAt.Value <= toDate, cancellationToken);
    }
}
