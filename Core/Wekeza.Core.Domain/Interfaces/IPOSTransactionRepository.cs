using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository contract for POS Transaction aggregate operations
/// </summary>
public interface IPOSTransactionRepository
{
    Task<POSTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<POSTransaction?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetByCardIdAsync(Guid cardId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetByTerminalIdAsync(string terminalId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetByStatusAsync(POSTransactionStatus status, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetByTransactionTypeAsync(POSTransactionType transactionType, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetByMerchantCategoryAsync(string merchantCategory, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetFailedTransactionsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetSuspiciousTransactionsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetUnsettledTransactionsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetTransactionsForSettlementAsync(string merchantId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetTransactionsByBatchAsync(string batchNumber, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<POSTransaction>> GetRefundableTransactionsAsync(Guid cardId, CancellationToken cancellationToken = default);
    
    Task<decimal> GetDailyPurchaseAmountAsync(Guid cardId, DateTime date, CancellationToken cancellationToken = default);
    
    Task<int> GetDailyTransactionCountAsync(Guid cardId, DateTime date, CancellationToken cancellationToken = default);
    
    Task<decimal> GetMerchantDailyVolumeAsync(string merchantId, DateTime date, CancellationToken cancellationToken = default);
    
    Task AddAsync(POSTransaction transaction, CancellationToken cancellationToken = default);
    
    void Update(POSTransaction transaction);
    
    Task UpdateAsync(POSTransaction transaction, CancellationToken cancellationToken = default);
}