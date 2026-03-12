using WekezaERMS.Domain.Entities;

namespace WekezaERMS.Application.Commands.Risks;

public interface IRiskRepository
{
    Task<Risk?> GetByIdAsync(Guid id);
    Task<List<Risk>> GetAllAsync();
    Task<int> GetCountAsync();
    Task AddAsync(Risk risk);
    Task UpdateAsync(Risk risk);
    Task SaveChangesAsync();
}
