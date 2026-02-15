using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for DigitalChannel aggregate
/// </summary>
public interface IDigitalChannelRepository
{
    Task<DigitalChannel?> GetByIdAsync(Guid id);
    Task<DigitalChannel?> GetByCodeAsync(string channelCode);
    Task<IEnumerable<DigitalChannel>> GetAllAsync();
    Task<IEnumerable<DigitalChannel>> GetByTypeAsync(ChannelType channelType);
    Task<IEnumerable<DigitalChannel>> GetByStatusAsync(ChannelStatus status);
    Task<IEnumerable<DigitalChannel>> GetOperationalAsync();
    Task<bool> ExistsByCodeAsync(string channelCode);
    Task AddAsync(DigitalChannel channel);
    Task UpdateAsync(DigitalChannel channel);
    Task DeleteAsync(Guid id);
}