using Microsoft.EntityFrameworkCore;
using EnhancedWekezaApi.Domain.Entities;
using EnhancedWekezaApi.Domain.Interfaces;
using EnhancedWekezaApi.Infrastructure.Data;

namespace EnhancedWekezaApi.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly WekezaDbContext _context;

    public AccountRepository(WekezaDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(a => a.Customer)
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(a => a.Customer)
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, cancellationToken);
    }

    public async Task<IEnumerable<Account>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(a => a.Customer)
            .Include(a => a.Transactions)
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(a => a.Customer)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Account account, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync(cancellationToken);
    }
}