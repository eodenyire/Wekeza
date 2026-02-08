///📂 Wekeza.Core.Domain/Interfaces/
///1. ITransactionRepository.cs
///This contract defines how we interact with the Ledger. In 2026, we don't just "get" transactions; we need to be able to fetch matured instruments (like cheques) and high-speed historical data for your analytics engine.
///
///

using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Contract for the Bank's Ledger operations.
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Persists a new ledger entry (Debit/Credit).
    /// </summary>
    Task AddAsync(Transaction transaction, CancellationToken ct);

    /// <summary>
    /// Fetches all cheque deposits that have passed their clearing window 
    /// but haven't been marked as 'Cleared' yet.
    /// </summary>
    Task<IEnumerable<Transaction>> GetMaturedChequesAsync(CancellationToken ct);

    /// <summary>
    /// Retrieves the most recent transactions for a specific account.
    /// In the infrastructure implementation, this is often optimized with Dapper.
    /// </summary>
    Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(Guid accountId, int limit, CancellationToken ct);

    /// <summary>
    /// Gets transactions by account ID with optional filtering
    /// </summary>
    Task<IEnumerable<Transaction>> GetByAccountAsync(Guid accountId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);

    /// <summary>
    /// Gets transactions within a date range
    /// </summary>
    Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default);
}
