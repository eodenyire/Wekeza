using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository contract for Card aggregate operations
/// </summary>
public interface ICardRepository
{
    Task<Card?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<Card?> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Card>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    
    Task AddAsync(Card card, CancellationToken cancellationToken = default);
    
    void Update(Card card);
}
