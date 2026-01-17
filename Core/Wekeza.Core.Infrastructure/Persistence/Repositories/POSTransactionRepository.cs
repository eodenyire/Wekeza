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

    public async Task<POSTransaction?> GetByIdAsync(Guid id)
    {
        return await _context.POSTransactions.FindAsync(id);
    }

    public async Task<IEnumerable<POSTransaction>> GetByCardNumberAsync(string cardNumber)
    {
        return await _context.POSTransactions
            .Where(t => t.CardNumber == cardNumber)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<POSTransaction>> GetByMerchantIdAsync(string merchantId, DateTime fromDate, DateTime toDate)
    {
        return await _context.POSTransactions
            .Where(t => t.MerchantId == merchantId && t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task AddAsync(POSTransaction transaction)
    {
        await _context.POSTransactions.AddAsync(transaction);
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