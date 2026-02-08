using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
///📂 Wekeza.Core.Infrastructure/Persistence/Repositories
///1. AccountRepository.cs
///This repository manages the state of every account aggregate. We use eager loading to ensure we have the necessary data for business rule validation.
///
///
namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;

    public AccountRepository(ApplicationDbContext context) => _context = context;

    public async Task<Account?> GetByAccountNumberAsync(AccountNumber accountNumber, CancellationToken ct)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, ct);
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Accounts.FindAsync(new object[] { id }, ct);
    }

    public async Task AddAsync(Account account, CancellationToken ct)
    {
        await _context.Accounts.AddAsync(account, ct);
    }

    public void Add(Account account)
    {
        _context.Accounts.Add(account);
    }

    public void Update(Account account)
    {
        _context.Accounts.Update(account);
    }

    public async Task<bool> ExistsAsync(AccountNumber accountNumber, CancellationToken ct)
    {
        return await _context.Accounts
            .AnyAsync(a => a.AccountNumber == accountNumber, ct);
    }

    public async Task<Account?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct)
    {
        // Assuming phone number is stored in Customer entity
        // This requires navigation property or join
        return await _context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Customer.PhoneNumber == phoneNumber, ct);
    }

    public async Task<int> GetNextAccountSequenceAsync(string prefix)
    {
        var currentMonth = DateTime.UtcNow.ToString("yyyyMM");
        var pattern = $"{prefix}{currentMonth}";
        
        var lastAccount = await _context.Accounts
            .Where(a => a.AccountNumber.Value.StartsWith(pattern))
            .OrderByDescending(a => a.AccountNumber.Value)
            .FirstOrDefaultAsync();

        if (lastAccount == null)
            return 1;

        // Extract sequence number from account number
        var accountNumber = lastAccount.AccountNumber.Value;
        var sequencePart = accountNumber.Substring(pattern.Length);
        
        if (int.TryParse(sequencePart, out int sequence))
            return sequence + 1;
        
        return 1;
    }

    public async Task<IEnumerable<Account>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(a => a.Customer)
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.OpenedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Account>> GetAccountsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(a => a.Customer)
            .Where(a => a.OpenedDate.Date >= fromDate.Date && a.OpenedDate.Date <= toDate.Date)
            .OrderByDescending(a => a.OpenedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Update(account);
        await Task.CompletedTask;
    }
}
