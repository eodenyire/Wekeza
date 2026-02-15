using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository contract for ATM Transaction aggregate operations
/// </summary>
public interface IATMTransactionRepository
{
    Task<ATMTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<ATMTransaction?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ATMTransaction>> GetByCardIdAsync(Guid cardId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ATMTransaction>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ATMTransaction>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ATMTransaction>> GetByATMIdAsync(string atmId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ATMTransaction>> GetByStatusAsync(ATMTransactionStatus status, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ATMTransaction>> GetByTransactionTypeAsync(ATMTransactionType transactionType, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ATMTransaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ATMTransaction>> GetFailedTransactionsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ATMTransaction>> GetSuspiciousTransactionsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ATMTransaction>> GetTransactionsForReversalAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ATMTransaction>> GetTransactionsByCardAndDateAsync(Guid cardId, DateTime date, CancellationToken cancellationToken = default);
    
    Task<decimal> GetDailyWithdrawalAmountAsync(Guid cardId, DateTime date, CancellationToken cancellationToken = default);
    
    Task<int> GetDailyTransactionCountAsync(Guid cardId, DateTime date, CancellationToken cancellationToken = default);
    
    Task AddAsync(ATMTransaction transaction, CancellationToken cancellationToken = default);
    
    void Update(ATMTransaction transaction);
    
    Task UpdateAsync(ATMTransaction transaction, CancellationToken cancellationToken = default);
}