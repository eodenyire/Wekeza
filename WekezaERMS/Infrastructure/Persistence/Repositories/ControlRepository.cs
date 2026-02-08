using Microsoft.EntityFrameworkCore;
using WekezaERMS.Application.Commands.Controls;
using WekezaERMS.Domain.Entities;
using WekezaERMS.Infrastructure.Persistence;

namespace WekezaERMS.Infrastructure.Persistence.Repositories;

public class ControlRepository : IControlRepository
{
    private readonly ERMSDbContext _context;

    public ControlRepository(ERMSDbContext context)
    {
        _context = context;
    }

    public async Task<RiskControl?> GetByIdAsync(Guid id)
    {
        return await _context.RiskControls.FindAsync(id);
    }

    public async Task<List<RiskControl>> GetByRiskIdAsync(Guid riskId)
    {
        return await _context.RiskControls
            .Where(c => c.RiskId == riskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(RiskControl control)
    {
        await _context.RiskControls.AddAsync(control);
    }

    public Task UpdateAsync(RiskControl control)
    {
        _context.RiskControls.Update(control);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(RiskControl control)
    {
        _context.RiskControls.Remove(control);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
