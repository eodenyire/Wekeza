using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Application.Features.Transactions.Queries.GetStatement;
///<summary>
/// 📂 Wekeza.Core.Infrastructure/Persistence/Repositories
/// TransactionRepository.cs (The High-Performance Ledger)
/// This repository handles the historical record of every cent moved. We include specialized methods for Statements—this is the end-to-end visibility you demanded.
///</summary>

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IDbConnection _dbConnection; // Injected for Dapper performance

    public TransactionRepository(ApplicationDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    // WRITE: Standard EF Core persistence
    public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        await _context.Transactions.AddAsync(transaction, ct);
    }

    // READ: High-Performance Dapper Query for Statements
    // This is how we "run the streets" of Nairobi with sub-millisecond lookups.
    public async Task<IEnumerable<TransactionHistoryDto>> GetStatementAsync(
        Guid accountId, 
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken ct = default)
    {
        const string sql = @"
            SELECT 
                Id, 
                Amount, 
                Type, 
                Description, 
                Timestamp 
            FROM Transactions 
            WHERE AccountId = @AccountId 
              AND Timestamp >= @FromDate 
              AND Timestamp <= @ToDate 
            ORDER BY Timestamp DESC";

        return await _dbConnection.QueryAsync<TransactionHistoryDto>(sql, new { 
            AccountId = accountId, 
            FromDate = fromDate, 
            ToDate = toDate 
        });
    }

    // ANALYTICS: Summary for the Risk Engine
    public async Task<decimal> GetTotalVolumeAsync(Guid accountId, DateTime since, CancellationToken ct = default)
    {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId && t.Timestamp >= since)
            .SumAsync(t => t.Amount.Amount, ct);
    }

    public async Task<IEnumerable<Transaction>> GetMaturedChequesAsync(CancellationToken ct = default)
    {
        // Cheques typically clear in 3 business days
        var clearingDate = DateTime.UtcNow.AddDays(-3);
        
        return await _context.Transactions
            .Where(t => t.Type == TransactionType.Deposit 
                     && t.Description.Contains("Cheque")
                     && t.Timestamp <= clearingDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(Guid accountId, int limit, CancellationToken ct = default)
    {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .Take(limit)
            .ToListAsync(ct);
    }
}
