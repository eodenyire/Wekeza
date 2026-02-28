using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for BatchJob aggregate
/// </summary>
public class BatchJobRepository
{
    private readonly ApplicationDbContext _context;

    public BatchJobRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<BatchJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BatchJobs
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task<BatchJob?> GetByJobCodeAsync(string jobCode, CancellationToken cancellationToken = default)
    {
        return await _context.BatchJobs
            .FirstOrDefaultAsync(j => j.JobCode == jobCode, cancellationToken);
    }

    public async Task<List<BatchJob>> GetAllAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        return await _context.BatchJobs
            .OrderBy(j => j.JobCode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BatchJob>> GetRunningJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BatchJobs
            .Where(j => j.Status == BatchJobStatus.Running)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BatchJob>> GetScheduledJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BatchJobs
            .Where(j => j.IsEnabled && j.Status != BatchJobStatus.Running)
            .OrderBy(j => j.NextScheduledRun)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BatchJob>> GetFailedJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BatchJobs
            .Where(j => j.Status == BatchJobStatus.Error)
            .OrderByDescending(j => j.LastRunTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BatchJob>> GetJobsByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.BatchJobs
            .Where(j => j.Category == category)
            .OrderBy(j => j.JobName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BatchJob job, CancellationToken cancellationToken = default)
    {
        await _context.BatchJobs.AddAsync(job, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(BatchJob job, CancellationToken cancellationToken = default)
    {
        _context.BatchJobs.Update(job);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var job = await GetByIdAsync(id, cancellationToken);
        if (job != null)
        {
            _context.BatchJobs.Remove(job);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetRunningJobCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BatchJobs
            .CountAsync(j => j.Status == BatchJobStatus.Running, cancellationToken);
    }

    public async Task<int> GetFailedJobCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BatchJobs
            .CountAsync(j => j.Status == BatchJobStatus.Error, cancellationToken);
    }

    public async Task<int> GetTotalJobCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BatchJobs.CountAsync(cancellationToken);
    }
}
