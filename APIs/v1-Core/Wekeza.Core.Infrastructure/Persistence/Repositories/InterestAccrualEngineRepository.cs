using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class InterestAccrualEngineRepository : IInterestAccrualEngineRepository
{
    private readonly ApplicationDbContext _context;

    public InterestAccrualEngineRepository(ApplicationDbContext context) => _context = context;

    public async Task<InterestAccrualEngine?> GetByIdAsync(Guid id) =>
        await _context.InterestAccrualEngines.FindAsync(id);

    public async Task<InterestAccrualEngine?> GetByProcessingDateAsync(DateTime processingDate) =>
        await _context.InterestAccrualEngines.FirstOrDefaultAsync(e => e.ProcessingDate.Date == processingDate.Date);

    public async Task<IEnumerable<InterestAccrualEngine>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate) =>
        await _context.InterestAccrualEngines.Where(e => e.ProcessingDate >= fromDate && e.ProcessingDate <= toDate).ToListAsync();

    public async Task<IEnumerable<InterestAccrualEngine>> GetFailedAccrualsAsync() =>
        await _context.InterestAccrualEngines.ToListAsync();

    public async Task<InterestAccrualEngine?> GetLatestAccrualAsync() =>
        await _context.InterestAccrualEngines.OrderByDescending(e => e.ProcessingDate).FirstOrDefaultAsync();

    public async Task<IEnumerable<InterestAccrualEngine>> GetAccrualHistoryAsync(int pageSize = 50, int pageNumber = 1) =>
        await _context.InterestAccrualEngines.OrderByDescending(e => e.ProcessingDate)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

    public async Task AddAsync(InterestAccrualEngine accrualEngine) =>
        await _context.InterestAccrualEngines.AddAsync(accrualEngine);

    public async Task UpdateAsync(InterestAccrualEngine accrualEngine) =>
        _context.InterestAccrualEngines.Update(accrualEngine);

    public async Task DeleteAsync(InterestAccrualEngine accrualEngine) =>
        _context.InterestAccrualEngines.Remove(accrualEngine);
}
