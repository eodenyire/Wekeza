using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Infrastructure.Persistence;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Teller Transaction Repository Implementation - Complete teller transaction management
/// Manages teller transactions with comprehensive querying capabilities
/// </summary>
public class TellerTransactionRepository : ITellerTransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TellerTransactionRepository(ApplicationDbContext context) => _context = context;

    // Basic CRUD operations
    public async Task<TellerTransaction?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<TellerTransaction?> GetByTransactionNumberAsync(string transactionNumber, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .FirstOrDefaultAsync(t => t.TransactionNumber == transactionNumber, ct);
    }

    public async Task AddAsync(TellerTransaction transaction, CancellationToken ct = default)
    {
        await _context.TellerTransactions.AddAsync(transaction, ct);
    }

    public void Add(TellerTransaction transaction)
    {
        _context.TellerTransactions.Add(transaction);
    }

    public void Update(TellerTransaction transaction)
    {
        _context.TellerTransactions.Update(transaction);
    }

    public void Remove(TellerTransaction transaction)
    {
        _context.TellerTransactions.Remove(transaction);
    }

    // Session-based queries
    public async Task<IEnumerable<TellerTransaction>> GetBySessionIdAsync(Guid sessionId, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.SessionId == sessionId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetBySessionNumberAsync(string sessionNumber, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.SessionNumber == sessionNumber)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    // Teller-based queries
    public async Task<IEnumerable<TellerTransaction>> GetByTellerIdAsync(Guid tellerId, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.TellerId == tellerId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetByTellerCodeAsync(string tellerCode, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.TellerCode == tellerCode)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    // Account-based queries
    public async Task<IEnumerable<TellerTransaction>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetByAccountNumberAsync(string accountNumber, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.AccountNumber == accountNumber)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.CustomerId == customerId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    // Branch-based queries
    public async Task<IEnumerable<TellerTransaction>> GetByBranchIdAsync(Guid branchId, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.BranchId == branchId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetByBranchCodeAsync(string branchCode, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.BranchCode == branchCode)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    // Transaction type queries
    public async Task<IEnumerable<TellerTransaction>> GetByTransactionTypeAsync(TellerTransactionType transactionType, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.TransactionType == transactionType)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetCashTransactionsAsync(CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.TransactionType == TellerTransactionType.CashDeposit || 
                       t.TransactionType == TellerTransactionType.CashWithdrawal)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetTransferTransactionsAsync(CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.TransactionType == TellerTransactionType.AccountTransfer)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    // Status-based queries
    public async Task<IEnumerable<TellerTransaction>> GetByStatusAsync(TellerTransactionStatus status, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetPendingTransactionsAsync(CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.Status == TellerTransactionStatus.Pending)
            .OrderBy(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetPendingApprovalTransactionsAsync(CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.Status == TellerTransactionStatus.PendingApproval)
            .OrderBy(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetCompletedTransactionsAsync(CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.Status == TellerTransactionStatus.Completed)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetReversibleTransactionsAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.TellerTransactions
            .Where(t => t.Status == TellerTransactionStatus.Completed && 
                       t.TransactionDate.Date == today)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    // Date-based queries
    public async Task<IEnumerable<TellerTransaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.TransactionDate.Date >= fromDate.Date && t.TransactionDate.Date <= toDate.Date)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetTodaysTransactionsAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.TellerTransactions
            .Where(t => t.TransactionDate.Date == today)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetTransactionsByValueDateAsync(DateTime valueDate, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.ValueDate.Date == valueDate.Date)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(ct);
    }

    // Amount-based queries
    public async Task<IEnumerable<TellerTransaction>> GetTransactionsAboveAmountAsync(decimal amount, string currencyCode, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.Amount.Amount > amount && t.Currency == currencyCode)
            .OrderByDescending(t => t.Amount.Amount)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerTransaction>> GetLargeTransactionsAsync(decimal threshold, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.Amount.Amount > threshold)
            .OrderByDescending(t => t.Amount.Amount)
            .ToListAsync(ct);
    }

    // Analytics and reporting
    public async Task<decimal> GetTotalTransactionAmountAsync(Guid tellerId, DateTime date, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.TellerId == tellerId && t.TransactionDate.Date == date.Date && 
                       t.Status == TellerTransactionStatus.Completed)
            .SumAsync(t => t.Amount.Amount, ct);
    }

    public async Task<decimal> GetTotalTransactionAmountByTypeAsync(TellerTransactionType transactionType, DateTime date, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .Where(t => t.TransactionType == transactionType && t.TransactionDate.Date == date.Date && 
                       t.Status == TellerTransactionStatus.Completed)
            .SumAsync(t => t.Amount.Amount, ct);
    }

    public async Task<int> GetTransactionCountAsync(Guid tellerId, DateTime date, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .CountAsync(t => t.TellerId == tellerId && t.TransactionDate.Date == date.Date && 
                           t.Status == TellerTransactionStatus.Completed, ct);
    }

    public async Task<int> GetTransactionCountByBranchAsync(Guid branchId, DateTime date, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .CountAsync(t => t.BranchId == branchId && t.TransactionDate.Date == date.Date && 
                           t.Status == TellerTransactionStatus.Completed, ct);
    }

    // Search and filtering
    public async Task<IEnumerable<TellerTransaction>> SearchTransactionsAsync(
        string? searchTerm = null,
        TellerTransactionType? transactionType = null,
        TellerTransactionStatus? status = null,
        Guid? tellerId = null,
        Guid? branchId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageSize = 50,
        int pageNumber = 1,
        CancellationToken ct = default)
    {
        var query = _context.TellerTransactions.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t => 
                t.TransactionNumber.Contains(searchTerm) ||
                (t.AccountNumber != null && t.AccountNumber.Contains(searchTerm)) ||
                (t.CustomerName != null && t.CustomerName.Contains(searchTerm)));
        }

        if (transactionType.HasValue)
        {
            query = query.Where(t => t.TransactionType == transactionType.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (tellerId.HasValue)
        {
            query = query.Where(t => t.TellerId == tellerId.Value);
        }

        if (branchId.HasValue)
        {
            query = query.Where(t => t.BranchId == branchId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate.Date >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate.Date <= toDate.Value.Date);
        }

        // Apply pagination
        return await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    // Validation helpers
    public async Task<bool> ExistsByTransactionNumberAsync(string transactionNumber, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .AnyAsync(t => t.TransactionNumber == transactionNumber, ct);
    }

    public async Task<bool> HasPendingTransactionsAsync(Guid sessionId, CancellationToken ct = default)
    {
        return await _context.TellerTransactions
            .AnyAsync(t => t.SessionId == sessionId && 
                          (t.Status == TellerTransactionStatus.Pending || 
                           t.Status == TellerTransactionStatus.PendingApproval), ct);
    }
}