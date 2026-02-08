using Microsoft.EntityFrameworkCore;
using WekezaERMS.Application.Commands.Risks;
using WekezaERMS.Domain.Entities;

namespace WekezaERMS.Infrastructure.Persistence.Repositories;

public class RiskRepository : IRiskRepository
{
    private readonly ERMSDbContext _context;

    public RiskRepository(ERMSDbContext context)
    {
        _context = context;
    }

    public async Task<Risk?> GetByIdAsync(Guid id)
    {
        return await _context.Risks
            .Include(r => r.Controls)
            .Include(r => r.MitigationActions)
            .Include(r => r.KeyRiskIndicators)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Risk>> GetAllAsync()
    {
        return await _context.Risks
            .Include(r => r.Controls)
            .Include(r => r.MitigationActions)
            .Include(r => r.KeyRiskIndicators)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Risks.CountAsync();
    }

    public async Task AddAsync(Risk risk)
    {
        await _context.Risks.AddAsync(risk);
    }

    public Task UpdateAsync(Risk risk)
    {
        _context.Risks.Update(risk);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
