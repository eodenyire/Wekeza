using WekezaERMS.Domain.Entities;

namespace WekezaERMS.Application.Commands.Controls;

public interface IControlRepository
{
    Task<RiskControl?> GetByIdAsync(Guid id);
    Task<List<RiskControl>> GetByRiskIdAsync(Guid riskId);
    Task AddAsync(RiskControl control);
    Task UpdateAsync(RiskControl control);
    Task DeleteAsync(RiskControl control);
    Task SaveChangesAsync();
}
