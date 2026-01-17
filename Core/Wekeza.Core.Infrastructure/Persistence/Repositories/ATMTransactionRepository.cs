using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Infrastructure.Persistence;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class ATMTransactionRepository : IATMTransactionRepository
{
    private readonly ApplicationDbContext _context;

    public ATMTransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ATMTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ATMTransactions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<ATMTransaction?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        return await _context.ATMTransactions
            .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber, cancellationToken);
    }

    public async Task<IEnumerable<ATMTransaction>> GetByCardIdAsync(Guid cardId, CancellationToken cancellationToken = default)
    {
        return await _context.ATMTransactions
            .Where(t => t.CardId == cardId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ATMTransaction>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.ATMTransactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ATMTransaction>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.ATMTransactions
            .Where(t => t.CustomerId == customerId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ATMTransaction>> GetByATMIdAsync(string atmId, CancellationToken cancellationToken = default)
    {
        return await _context.ATMTransactions
            .Where(t => t.ATMId == atmId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ATMTransaction>> GetByStatusAsync(ATMTransactionStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.ATMTransactions
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ATMTransaction>> GetByTransactionTypeAsync(ATMTransactionType transactionType, CancellationToken cancellationToken = default)
    {
        return await _context.ATMTransactions
            .Where(t => t.TransactionType == transactionType)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ATMTransaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.ATMTransactions
            .Where(t => t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ATMTransaction>> GetFailedTransactionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ATMTransactions
            .Where(t => t.Status == ATMTransactionStatus.Failed)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ATMTransaction>> GetSuspiciousTransactionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ATMTransactions
            .Where(t => t.IsSuspicious == true)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ATMTransaction>> GetTransactionsForReversalAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ATMTransactions
            .Where(t => t.Status == ATMTransactionStatus.PendingReversal)
            .OrderBy(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ATMTransaction>> GetTransactionsByCardAndDateAsync(Guid cardId, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.ATMTransactions
            .Where(t => t.CardId == cardId && t.TransactionDate >= startOfDay && t.TransactionDate < endOfDay)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetDailyWithdrawalAmountAsync(Guid cardId, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.ATMTransactions
            .Where(t => t.CardId == cardId 
                && t.TransactionDate >= startOfDay 
                && t.TransactionDate < endOfDay
                && t.TransactionType == ATMTransactionType.Withdrawal
                && t.Status == ATMTransactionStatus.Completed)
            .SumAsync(t => t.Amount, cancellationToken);
    }

    public async Task<int> GetDailyTransactionCountAsync(Guid cardId, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.ATMTransactions
            .CountAsync(t => t.CardId == cardId 
                && t.TransactionDate >= startOfDay 
                && t.TransactionDate < endOfDay, cancellationToken);
    }

    public async Task AddAsync(ATMTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.ATMTransactions.AddAsync(transaction, cancellationToken);
    }

    public void Update(ATMTransaction transaction)
    {
        _context.ATMTransactions.Update(transaction);
    }

    public async Task UpdateAsync(ATMTransaction transaction, CancellationToken cancellationToken = default)
    {
        _context.ATMTransactions.Update(transaction);
        await Task.CompletedTask;
    }
}