using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for SystemConfiguration aggregate
/// </summary>
public class SystemConfigurationRepository
{
    private readonly ApplicationDbContext _context;

    public SystemConfigurationRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<SystemConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SystemConfigurations
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<SystemConfiguration?> GetByConfigCodeAsync(string configCode, CancellationToken cancellationToken = default)
    {
        return await _context.SystemConfigurations
            .FirstOrDefaultAsync(c => c.ConfigCode == configCode, cancellationToken);
    }

    public async Task<List<SystemConfiguration>> SearchAsync(
        string? category = null, 
        string? configCode = null, 
        ConfigurationStatus? status = null,
        int page = 1, 
        int pageSize = 50, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.SystemConfigurations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(c => c.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(configCode))
        {
            query = query.Where(c => c.ConfigCode.Contains(configCode));
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SystemConfiguration>> GetActiveConfigurationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SystemConfigurations
            .Where(c => c.Status == ConfigurationStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SystemConfiguration>> GetConfigurationsByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.SystemConfigurations
            .Where(c => c.Category == category)
            .OrderBy(c => c.ConfigCode)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SystemConfiguration configuration, CancellationToken cancellationToken = default)
    {
        await _context.SystemConfigurations.AddAsync(configuration, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SystemConfiguration configuration, CancellationToken cancellationToken = default)
    {
        _context.SystemConfigurations.Update(configuration);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var configuration = await GetByIdAsync(id, cancellationToken);
        if (configuration != null)
        {
            _context.SystemConfigurations.Remove(configuration);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetPendingApprovalCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SystemConfigurations
            .CountAsync(c => c.Status == ConfigurationStatus.PendingApproval, cancellationToken);
    }
}
