using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class CardRepository : ICardRepository
{
    private readonly ApplicationDbContext _context;

    public CardRepository(ApplicationDbContext context) => _context = context;

    public async Task<Card?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Cards.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Card?> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .FirstOrDefaultAsync(c => c.CardNumber == cardNumber, cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.AccountId == accountId && !c.IsCancelled)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetActiveCardsByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.AccountId == accountId && c.Status == CardStatus.Active)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetActiveCardsByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.CustomerId == customerId && c.Status == CardStatus.Active)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetCardsByStatusAsync(CardStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetExpiringCardsAsync(DateTime expiryDate, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.ExpiryDate <= expiryDate && c.Status == CardStatus.Active)
            .OrderBy(c => c.ExpiryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetBlockedCardsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.Status == CardStatus.Blocked)
            .OrderByDescending(c => c.BlockedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetHotlistedCardsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.IsHotlisted)
            .OrderByDescending(c => c.HotlistedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Card?> GetByActivationCodeAsync(string activationCode, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .FirstOrDefaultAsync(c => c.ActivationCode == activationCode, cancellationToken);
    }

    public async Task<int> GetActiveCardCountByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .CountAsync(c => c.CustomerId == customerId && c.Status == CardStatus.Active, cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetCardsByTypeAsync(CardType cardType, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.CardType == cardType)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetCardsForRenewalAsync(int daysBeforeExpiry, CancellationToken cancellationToken = default)
    {
        var renewalDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);
        return await _context.Cards
            .Where(c => c.ExpiryDate <= renewalDate 
                     && c.ExpiryDate > DateTime.UtcNow
                     && c.Status == CardStatus.Active)
            .OrderBy(c => c.ExpiryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetCardsByDeliveryStatusAsync(CardDeliveryStatus deliveryStatus, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.DeliveryStatus == deliveryStatus)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Card card, CancellationToken cancellationToken = default)
    {
        await _context.Cards.AddAsync(card, cancellationToken);
    }

    public void Update(Card card)
    {
        _context.Cards.Update(card);
    }

    public async Task UpdateAsync(Card card, CancellationToken cancellationToken = default)
    {
        _context.Cards.Update(card);
        await Task.CompletedTask;
    }
}
