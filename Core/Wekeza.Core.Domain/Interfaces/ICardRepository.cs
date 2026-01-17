using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository contract for Card aggregate operations
/// Enhanced for enterprise card management capabilities
/// </summary>
public interface ICardRepository
{
    Task<Card?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<Card?> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Card>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Card>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Card>> GetActiveCardsByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Card>> GetActiveCardsByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Card>> GetCardsByStatusAsync(CardStatus status, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Card>> GetExpiringCardsAsync(DateTime expiryDate, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Card>> GetBlockedCardsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Card>> GetHotlistedCardsAsync(CancellationToken cancellationToken = default);
    
    Task<Card?> GetByActivationCodeAsync(string activationCode, CancellationToken cancellationToken = default);
    
    Task<int> GetActiveCardCountByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Card>> GetCardsByTypeAsync(CardType cardType, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Card>> GetCardsForRenewalAsync(int daysBeforeExpiry, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Card>> GetCardsByDeliveryStatusAsync(CardDeliveryStatus deliveryStatus, CancellationToken cancellationToken = default);
    
    Task AddAsync(Card card, CancellationToken cancellationToken = default);
    
    void Update(Card card);
    
    Task UpdateAsync(Card card, CancellationToken cancellationToken = default);
}
