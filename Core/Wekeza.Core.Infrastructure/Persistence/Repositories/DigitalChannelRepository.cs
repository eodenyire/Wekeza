using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for DigitalChannel aggregate
/// </summary>
public class DigitalChannelRepository : IDigitalChannelRepository
{
    private readonly ApplicationDbContext _context;

    public DigitalChannelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DigitalChannel?> GetByIdAsync(Guid id)
    {
        return await _context.DigitalChannels
            .Include(c => c.Services)
            .Include(c => c.Sessions)
            .Include(c => c.Transactions)
            .Include(c => c.Alerts)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<DigitalChannel?> GetByCodeAsync(string channelCode)
    {
        return await _context.DigitalChannels
            .Include(c => c.Services)
            .Include(c => c.Sessions)
            .Include(c => c.Transactions)
            .Include(c => c.Alerts)
            .FirstOrDefaultAsync(c => c.ChannelCode == channelCode);
    }

    public async Task<IEnumerable<DigitalChannel>> GetAllAsync()
    {
        return await _context.DigitalChannels
            .Include(c => c.Services)
            .Include(c => c.Sessions.Where(s => s.Status == SessionStatus.Active))
            .OrderBy(c => c.ChannelName)
            .ToListAsync();
    }

    public async Task<IEnumerable<DigitalChannel>> GetByTypeAsync(ChannelType channelType)
    {
        return await _context.DigitalChannels
            .Include(c => c.Services)
            .Include(c => c.Sessions.Where(s => s.Status == SessionStatus.Active))
            .Where(c => c.ChannelType == channelType)
            .OrderBy(c => c.ChannelName)
            .ToListAsync();
    }

    public async Task<IEnumerable<DigitalChannel>> GetByStatusAsync(ChannelStatus status)
    {
        return await _context.DigitalChannels
            .Include(c => c.Services)
            .Include(c => c.Sessions.Where(s => s.Status == SessionStatus.Active))
            .Where(c => c.Status == status)
            .OrderBy(c => c.ChannelName)
            .ToListAsync();
    }

    public async Task<IEnumerable<DigitalChannel>> GetOperationalAsync()
    {
        return await _context.DigitalChannels
            .Include(c => c.Services)
            .Include(c => c.Sessions.Where(s => s.Status == SessionStatus.Active))
            .Where(c => c.Status == ChannelStatus.Active)
            .OrderBy(c => c.ChannelName)
            .ToListAsync();
    }

    public async Task<bool> ExistsByCodeAsync(string channelCode)
    {
        return await _context.DigitalChannels.AnyAsync(c => c.ChannelCode == channelCode);
    }

    public async Task AddAsync(DigitalChannel channel)
    {
        await _context.DigitalChannels.AddAsync(channel);
    }

    public async Task UpdateAsync(DigitalChannel channel)
    {
        _context.DigitalChannels.Update(channel);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var channel = await _context.DigitalChannels.FindAsync(id);
        if (channel != null)
        {
            _context.DigitalChannels.Remove(channel);
        }
    }
}