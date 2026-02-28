using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Infrastructure.Persistence;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class POSTransactionRepository : IPOSTransactionRepository
{
    private readonly ApplicationDbContext _context;

    public POSTransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<POSTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<POSTransaction?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber, cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetByCardIdAsync(Guid cardId, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.CardId == cardId)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.CustomerId == customerId)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.MerchantId == merchantId)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetByTerminalIdAsync(string terminalId, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.TerminalId == terminalId)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetByStatusAsync(POSTransactionStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetByTransactionTypeAsync(POSTransactionType transactionType, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.TransactionType == transactionType)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.TransactionDateTime >= fromDate && t.TransactionDateTime <= toDate)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetByMerchantCategoryAsync(string merchantCategory, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.MerchantCategory == merchantCategory)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetFailedTransactionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.Status == POSTransactionStatus.Failed || t.Status == POSTransactionStatus.Declined)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetSuspiciousTransactionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.IsSuspicious)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetUnsettledTransactionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => !t.IsSettled && t.Status == POSTransactionStatus.Completed)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetTransactionsForSettlementAsync(string merchantId, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.MerchantId == merchantId && !t.IsSettled && t.Status == POSTransactionStatus.Completed)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetTransactionsByBatchAsync(string batchNumber, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.BatchNumber == batchNumber)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<POSTransaction>> GetRefundableTransactionsAsync(Guid cardId, CancellationToken cancellationToken = default)
    {
        return await _context.POSTransactions
            .Where(t => t.CardId == cardId 
                     && t.Status == POSTransactionStatus.Completed 
                     && !t.IsRefunded 
                     && t.TransactionType == POSTransactionType.Purchase)
            .OrderByDescending(t => t.TransactionDateTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetDailyPurchaseAmountAsync(Guid cardId, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var totalAmount = await _context.POSTransactions
            .Where(t => t.CardId == cardId 
                     && t.TransactionDateTime >= startOfDay 
                     && t.TransactionDateTime < endOfDay
                     && t.Status == POSTransactionStatus.Completed
                     && (t.TransactionType == POSTransactionType.Purchase || t.TransactionType == POSTransactionType.Completion))
            .SumAsync(t => t.Amount.Amount, cancellationToken);

        return totalAmount;
    }

    public async Task<int> GetDailyTransactionCountAsync(Guid cardId, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.POSTransactions
            .Where(t => t.CardId == cardId 
                     && t.TransactionDateTime >= startOfDay 
                     && t.TransactionDateTime < endOfDay
                     && t.Status == POSTransactionStatus.Completed)
            .CountAsync(cancellationToken);
    }

    public async Task<decimal> GetMerchantDailyVolumeAsync(string merchantId, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var totalVolume = await _context.POSTransactions
            .Where(t => t.MerchantId == merchantId 
                     && t.TransactionDateTime >= startOfDay 
                     && t.TransactionDateTime < endOfDay
                     && t.Status == POSTransactionStatus.Completed)
            .SumAsync(t => t.Amount.Amount, cancellationToken);

        return totalVolume;
    }

    public async Task AddAsync(POSTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.POSTransactions.AddAsync(transaction, cancellationToken);
    }

    public void Update(POSTransaction transaction)
    {
        _context.POSTransactions.Update(transaction);
    }

    public async Task UpdateAsync(POSTransaction transaction, CancellationToken cancellationToken = default)
    {
        _context.POSTransactions.Update(transaction);
        await Task.CompletedTask;
    }
}