using Wekeza.Nexus.Domain.Entities;
using Wekeza.Nexus.Domain.Interfaces;

namespace Wekeza.Nexus.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of transaction history repository
/// Stores transaction data in memory for fraud detection velocity analysis
/// In production, this would be backed by a time-series database or Redis
/// </summary>
public class InMemoryTransactionHistoryRepository : ITransactionHistoryRepository
{
    private readonly List<TransactionRecord> _transactions = new();
    private readonly Dictionary<string, AccountMetadata> _accountMetadata = new();
    private readonly object _lock = new();
    
    public Task AddTransactionAsync(
        TransactionRecord transaction, 
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _transactions.Add(transaction);
            
            // Automatically create account metadata if it doesn't exist
            if (!_accountMetadata.ContainsKey(transaction.FromAccountNumber))
            {
                _accountMetadata[transaction.FromAccountNumber] = new AccountMetadata
                {
                    AccountNumber = transaction.FromAccountNumber,
                    UserId = transaction.UserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-30) // Default to 30 days old
                };
            }
            
            if (!_accountMetadata.ContainsKey(transaction.ToAccountNumber))
            {
                _accountMetadata[transaction.ToAccountNumber] = new AccountMetadata
                {
                    AccountNumber = transaction.ToAccountNumber,
                    CreatedAt = DateTime.UtcNow.AddDays(-30) // Default to 30 days old
                };
            }
        }
        
        return Task.CompletedTask;
    }
    
    public Task<List<TransactionRecord>> GetUserTransactionsAsync(
        Guid userId, 
        DateTime since, 
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var results = _transactions
                .Where(t => t.UserId == userId && t.TransactionTime >= since)
                .OrderByDescending(t => t.TransactionTime)
                .ToList();
            
            return Task.FromResult(results);
        }
    }
    
    public Task<List<TransactionRecord>> GetTransactionsBetweenAccountsAsync(
        string fromAccount, 
        string toAccount, 
        DateTime since, 
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var results = _transactions
                .Where(t => t.FromAccountNumber == fromAccount 
                         && t.ToAccountNumber == toAccount 
                         && t.TransactionTime >= since)
                .OrderByDescending(t => t.TransactionTime)
                .ToList();
            
            return Task.FromResult(results);
        }
    }
    
    public Task<bool> HasTransactionHistoryWithBeneficiaryAsync(
        Guid userId, 
        string beneficiaryAccountNumber, 
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var hasHistory = _transactions.Any(t => 
                t.UserId == userId && 
                t.ToAccountNumber == beneficiaryAccountNumber);
            
            return Task.FromResult(hasHistory);
        }
    }
    
    public Task<AccountMetadata?> GetAccountMetadataAsync(
        string accountNumber, 
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(
                _accountMetadata.TryGetValue(accountNumber, out var metadata) 
                    ? metadata 
                    : null);
        }
    }
    
    public Task UpsertAccountMetadataAsync(
        AccountMetadata metadata, 
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _accountMetadata[metadata.AccountNumber] = metadata;
        }
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Helper method to get all transactions (useful for testing and debugging)
    /// </summary>
    public List<TransactionRecord> GetAllTransactions()
    {
        lock (_lock)
        {
            return _transactions.ToList();
        }
    }
    
    /// <summary>
    /// Helper method to clear all data (useful for testing)
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _transactions.Clear();
            _accountMetadata.Clear();
        }
    }
}
