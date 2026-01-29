using Wekeza.Nexus.Domain.Entities;

namespace Wekeza.Nexus.Domain.Interfaces;

/// <summary>
/// Repository for storing and querying historical transaction data
/// Used for velocity analysis, pattern detection, and fraud analytics
/// </summary>
public interface ITransactionHistoryRepository
{
    /// <summary>
    /// Adds a transaction record to the history
    /// </summary>
    Task AddTransactionAsync(TransactionRecord transaction, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets transactions for a user within a time window
    /// </summary>
    Task<List<TransactionRecord>> GetUserTransactionsAsync(
        Guid userId, 
        DateTime since, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all transactions between two accounts within a time window
    /// Used for circular transaction detection
    /// </summary>
    Task<List<TransactionRecord>> GetTransactionsBetweenAccountsAsync(
        string fromAccount,
        string toAccount,
        DateTime since,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a user has previously transacted with a beneficiary
    /// </summary>
    Task<bool> HasTransactionHistoryWithBeneficiaryAsync(
        Guid userId,
        string beneficiaryAccountNumber,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets account metadata by account number
    /// </summary>
    Task<AccountMetadata?> GetAccountMetadataAsync(
        string accountNumber,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds or updates account metadata
    /// </summary>
    Task UpsertAccountMetadataAsync(
        AccountMetadata metadata,
        CancellationToken cancellationToken = default);
}
