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
    Task AddAsync(Transaction transaction, CancellationToken ct);
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Transaction>> GetMaturedChequesAsync(CancellationToken ct);
    Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(Guid accountId, int limit, CancellationToken ct);
    Task<IEnumerable<Transaction>> GetRecentByCustomerIdAsync(Guid customerId, int limit, CancellationToken ct = default);
    Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<IEnumerable<Transaction>> GetByAccountAsync(Guid accountId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
}
