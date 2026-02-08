using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class CardRepository : ICardRepository
{
    private readonly ApplicationDbContext _context;

    public CardRepository(ApplicationDbContext context) => _context = context;

    public async Task<Card?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Cards.FindAsync(new object[] { id }, ct);
    }

    public async Task<Card?> GetByCardNumberAsync(string cardNumber, CancellationToken ct = default)
    {
        return await _context.Cards
            .FirstOrDefaultAsync(c => c.CardNumber == cardNumber, ct);
    }

    public async Task<IEnumerable<Card>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
    {
        return await _context.Cards
            .Where(c => c.AccountId == accountId && !c.IsCancelled)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Card>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.Cards
            .Where(c => c.CustomerId == customerId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Card>> GetActiveCardsByAccountIdAsync(Guid accountId, CancellationToken ct = default)
    {
        return await _context.Cards
            .Where(c => c.AccountId == accountId && c.Status == CardStatus.Active)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Card>> GetActiveCardsByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.Cards
            .Where(c => c.CustomerId == customerId && c.Status == CardStatus.Active)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Card>> GetCardsByStatusAsync(CardStatus status, CancellationToken ct = default)
    {
        return await _context.Cards
            .Where(c => c.Status == status)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Card>> GetExpiringCardsAsync(DateTime expiryDate, CancellationToken ct = default)
    {
        return await _context.Cards
            .Where(c => c.ExpiryDate <= expiryDate && c.Status == CardStatus.Active)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Card>> GetBlockedCardsAsync(CancellationToken ct = default)
    {
        return await _context.Cards
            .Where(c => c.Status == CardStatus.Blocked)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Card>> GetHotlistedCardsAsync(CancellationToken ct = default)
    {
        return await _context.Cards
            .Where(c => c.IsHotlisted)
            .ToListAsync(ct);
    }

    public async Task<Card?> GetByActivationCodeAsync(string activationCode, CancellationToken ct = default)
    {
        return await _context.Cards
            .FirstOrDefaultAsync(c => c.ActivationCode == activationCode, ct);
    }

    public async Task<int> GetActiveCardCountByCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.Cards
            .CountAsync(c => c.CustomerId == customerId && c.Status == CardStatus.Active, ct);
    }

    public async Task<IEnumerable<Card>> GetCardsByTypeAsync(CardType cardType, CancellationToken ct = default)
    {
        return await _context.Cards
            .Where(c => c.CardType == cardType)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Card>> GetCardsForRenewalAsync(int daysBeforeExpiry, CancellationToken ct = default)
    {
        var expiryThreshold = DateTime.UtcNow.AddDays(daysBeforeExpiry);
        return await _context.Cards
            .Where(c => c.ExpiryDate <= expiryThreshold && c.Status == CardStatus.Active)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Card>> GetCardsByDeliveryStatusAsync(CardDeliveryStatus deliveryStatus, CancellationToken ct = default)
    {
        return await _context.Cards
            .Where(c => c.DeliveryStatus == deliveryStatus)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Card card, CancellationToken ct = default)
    {
        await _context.Cards.AddAsync(card, ct);
    }

    public void Update(Card card)
    {
        _context.Cards.Update(card);
    }

    public async Task UpdateAsync(Card card, CancellationToken ct = default)
    {
        _context.Cards.Update(card);
        await Task.CompletedTask;
    }
}
