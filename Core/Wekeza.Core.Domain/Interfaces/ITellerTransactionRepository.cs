using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Teller Transaction Repository - Complete teller transaction management
/// Contract for managing teller transactions with comprehensive querying capabilities
/// </summary>
public interface ITellerTransactionRepository
{
    // Basic CRUD operations
    Task<TellerTransaction?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TellerTransaction?> GetByTransactionNumberAsync(string transactionNumber, CancellationToken ct = default);
    Task AddAsync(TellerTransaction transaction, CancellationToken ct = default);
    void Add(TellerTransaction transaction);
    void Update(TellerTransaction transaction);
    void Remove(TellerTransaction transaction);

    // Session-based queries
    Task<IEnumerable<TellerTransaction>> GetBySessionIdAsync(Guid sessionId, CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetBySessionNumberAsync(string sessionNumber, CancellationToken ct = default);

    // Teller-based queries
    Task<IEnumerable<TellerTransaction>> GetByTellerIdAsync(Guid tellerId, CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetByTellerCodeAsync(string tellerCode, CancellationToken ct = default);

    // Account-based queries
    Task<IEnumerable<TellerTransaction>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetByAccountNumberAsync(string accountNumber, CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);

    // Branch-based queries
    Task<IEnumerable<TellerTransaction>> GetByBranchIdAsync(Guid branchId, CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetByBranchCodeAsync(string branchCode, CancellationToken ct = default);

    // Transaction type queries
    Task<IEnumerable<TellerTransaction>> GetByTransactionTypeAsync(TellerTransactionType transactionType, CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetCashTransactionsAsync(CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetTransferTransactionsAsync(CancellationToken ct = default);

    // Status-based queries
    Task<IEnumerable<TellerTransaction>> GetByStatusAsync(TellerTransactionStatus status, CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetPendingTransactionsAsync(CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetPendingApprovalTransactionsAsync(CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetCompletedTransactionsAsync(CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetReversibleTransactionsAsync(CancellationToken ct = default);

    // Date-based queries
    Task<IEnumerable<TellerTransaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetTodaysTransactionsAsync(CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetTransactionsByValueDateAsync(DateTime valueDate, CancellationToken ct = default);

    // Amount-based queries
    Task<IEnumerable<TellerTransaction>> GetTransactionsAboveAmountAsync(decimal amount, string currencyCode, CancellationToken ct = default);
    Task<IEnumerable<TellerTransaction>> GetLargeTransactionsAsync(decimal threshold, CancellationToken ct = default);

    // Analytics and reporting
    Task<decimal> GetTotalTransactionAmountAsync(Guid tellerId, DateTime date, CancellationToken ct = default);
    Task<decimal> GetTotalTransactionAmountByTypeAsync(TellerTransactionType transactionType, DateTime date, CancellationToken ct = default);
    Task<int> GetTransactionCountAsync(Guid tellerId, DateTime date, CancellationToken ct = default);
    Task<int> GetTransactionCountByBranchAsync(Guid branchId, DateTime date, CancellationToken ct = default);

    // Search and filtering
    Task<IEnumerable<TellerTransaction>> SearchTransactionsAsync(
        string? searchTerm = null,
        TellerTransactionType? transactionType = null,
        TellerTransactionStatus? status = null,
        Guid? tellerId = null,
        Guid? branchId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageSize = 50,
        int pageNumber = 1,
        CancellationToken ct = default);

    // Validation helpers
    Task<bool> ExistsByTransactionNumberAsync(string transactionNumber, CancellationToken ct = default);
    Task<bool> HasPendingTransactionsAsync(Guid sessionId, CancellationToken ct = default);
}