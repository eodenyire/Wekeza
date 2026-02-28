using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class RecurringDepositRepository : IRecurringDepositRepository
{
    private readonly ApplicationDbContext _context;

    public RecurringDepositRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RecurringDeposit?> GetByIdAsync(Guid id) =>
        await _context.RecurringDeposits.FindAsync(id);

    public async Task<RecurringDeposit?> GetByDepositNumberAsync(string depositNumber) =>
        await _context.RecurringDeposits.FirstOrDefaultAsync(rd => rd.DepositNumber == depositNumber);

    public async Task<IEnumerable<RecurringDeposit>> GetByCustomerIdAsync(Guid customerId) =>
        await _context.RecurringDeposits.Where(rd => rd.CustomerId == customerId).ToListAsync();

    public async Task<IEnumerable<RecurringDeposit>> GetByAccountIdAsync(Guid accountId) =>
        await _context.RecurringDeposits.Where(rd => rd.AccountId == accountId).ToListAsync();

    public async Task<IEnumerable<RecurringDeposit>> GetActiveDepositsAsync(string? branchCode = null) =>
        await _context.RecurringDeposits.ToListAsync();

    public async Task<IEnumerable<RecurringDeposit>> GetMaturedDepositsAsync(DateTime? asOfDate = null) =>
        await _context.RecurringDeposits.ToListAsync();

    public async Task<IEnumerable<RecurringDeposit>> GetOverdueInstallmentsAsync(DateTime asOfDate) =>
        await _context.RecurringDeposits.ToListAsync();

    public async Task<IEnumerable<RecurringDeposit>> GetAutoDebitDepositsAsync(DateTime processingDate) =>
        await _context.RecurringDeposits.ToListAsync();

    public async Task<IEnumerable<RecurringDeposit>> GetDepositsByBranchAsync(string branchCode) =>
        await _context.RecurringDeposits.Where(rd => rd.BranchCode == branchCode).ToListAsync();

    public async Task<IEnumerable<RecurringDeposit>> GetDepositsByDateRangeAsync(DateTime fromDate, DateTime toDate) =>
        await _context.RecurringDeposits.Where(rd => rd.CreatedAt >= fromDate && rd.CreatedAt <= toDate).ToListAsync();

    public async Task AddAsync(RecurringDeposit recurringDeposit) =>
        await _context.RecurringDeposits.AddAsync(recurringDeposit);

    public async Task UpdateAsync(RecurringDeposit recurringDeposit) =>
        _context.RecurringDeposits.Update(recurringDeposit);

    public async Task DeleteAsync(RecurringDeposit recurringDeposit) =>
        _context.RecurringDeposits.Remove(recurringDeposit);
}
