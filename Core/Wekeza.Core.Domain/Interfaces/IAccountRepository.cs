using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// 📂 Wekeza.Core.Domain/Interfaces
/// 1. IAccountRepository.cs (The Vault Gatekeeper)
/// This is not a generic CRUD interface. It is a specialized port for the Account Aggregate. It focuses on finding the account by its unique banking identifiers.
/// Defines the contract for persisting and retrieving Account Aggregates.
/// </summary>
public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<Account?> GetByAccountNumberAsync(AccountNumber accountNumber, CancellationToken cancellationToken = default);
    
    Task AddAsync(Account account, CancellationToken cancellationToken = default);
    
    void Add(Account account);
    
    void Update(Account account);
    
    // Financial systems rarely "Delete". We deactivate or close.
    Task<bool> ExistsAsync(AccountNumber accountNumber, CancellationToken cancellationToken = default);
    
    Task<int> GetNextAccountSequenceAsync(string prefix);
    
    Task<IEnumerable<Account>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
