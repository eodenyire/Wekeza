using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// 📂 Wekeza.Core.Domain/Interfaces
/// 2. ICustomerRepository.cs (The Identity Port)
/// In a billion-dollar bank, the customer is the primary entity for KYC and Risk. We need fast lookup by National ID or Passport.
/// Defines the contract for persisting and retrieving Customer Aggregates.
/// </summary>
public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<Customer?> GetByIdentificationNumberAsync(string idNumber, CancellationToken cancellationToken = default);
    
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    
    void Update(Customer customer);
}
