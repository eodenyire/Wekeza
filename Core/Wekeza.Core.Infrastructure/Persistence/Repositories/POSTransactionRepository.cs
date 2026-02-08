using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Infrastructure.Persistence;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class POSTransactionRepository : IPOSTransactionRepository
{
    private readonly ApplicationDbContext _context;

    public POSTransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<POSTransaction?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.POSTransactions.FindAsync(new object[] { id }, ct);
    }

    public async Task<POSTransaction?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken ct = default)
    {
        return await _context.POSTransactions
            .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber, ct);
    }

    public async Task<IEnumerable<POSTransaction>> GetByCardIdAsync(Guid cardId, CancellationToken ct = default)
    {
        return await _context.POSTransactions
            .Where(t => t.CardId == cardId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<POSTransaction>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
    {
        return await _context.POSTransactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<POSTransaction>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.POSTransactions
            .Where(t => t.CustomerId == customerId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<POSTransaction>> GetByCardNumberAsync(string cardNumber, CancellationToken ct = default)
    {
        return await _context.POSTransactions
            .Where(t => t.CardNumber == cardNumber)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<POSTransaction>> GetByMerchantIdAsync(string merchantId, CancellationToken ct = default)
    {
        return await _context.POSTransactions
            .Where(t => t.MerchantId == merchantId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<POSTransaction>> GetByTerminalIdAsync(string terminalId, CancellationToken ct = default)
    {
        return await _context.POSTransactions
            .Where(t => t.TerminalId == terminalId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<POSTransaction>> GetByStatusAsync(POSTransactionStatus status, CancellationToken ct = default)
    {
        return await _context.POSTransactions
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<POSTransaction>> GetByTransactionTypeAsync(POSTransactionType transactionType, CancellationToken ct = default)
    {
        return await _context.POSTransactions
            .Where(t => t.TransactionType == transactionType)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<POSTransaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.POSTransactions
            .Where(t => t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<POSTransaction>> GetByMerchantIdAsync(string merchantId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.POSTransactions
            .Where(t => t.MerchantId == merchantId && t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<int> GetDailyTransactionCountAsync(Guid cardId, DateTime date, CancellationToken ct = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);
        return await _context.POSTransactions
            .CountAsync(t => t.CardId == cardId && t.TransactionDate >= startOfDay && t.TransactionDate < endOfDay, ct);
    }

    public async Task<decimal> GetMerchantDailyVolumeAsync(string merchantId, DateTime date, CancellationToken ct = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);
        return await _context.POSTransactions
            .Where(t => t.MerchantId == merchantId && t.TransactionDate >= startOfDay && t.TransactionDate < endOfDay)
            .SumAsync(t => t.Amount, ct);
    }

    public async Task AddAsync(POSTransaction transaction, CancellationToken ct = default)
    {
        await _context.POSTransactions.AddAsync(transaction, ct);
    }

    public async Task UpdateAsync(POSTransaction transaction, CancellationToken ct = default)
    {
        _context.POSTransactions.Update(transaction);
        await Task.CompletedTask;
    }

    public void Update(POSTransaction transaction)
    {
        _context.POSTransactions.Update(transaction);
    }

    public void Delete(POSTransaction transaction)
    {
        _context.POSTransactions.Remove(transaction);
    }
}