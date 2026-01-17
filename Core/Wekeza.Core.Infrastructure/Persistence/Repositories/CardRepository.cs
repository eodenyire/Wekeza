using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class CardRepository : ICardRepository
{
    private readonly ApplicationDbContext _context;

    public CardRepository(ApplicationDbContext context) => _context = context;

    public async Task<Card?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Cards.FindAsync(new object[] { id }, ct);
    }

    public async Task<Card?> GetByCardNumberAsync(string cardNumber, CancellationToken ct)
    {
        return await _context.Cards
            .FirstOrDefaultAsync(c => c.CardNumber == cardNumber, ct);
    }

    public async Task<IEnumerable<Card>> GetByAccountIdAsync(Guid accountId, CancellationToken ct)
    {
        return await _context.Cards
            .Where(c => c.AccountId == accountId && !c.IsCancelled)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Card card, CancellationToken ct)
    {
        await _context.Cards.AddAsync(card, ct);
    }

    public void Update(Card card)
    {
        _context.Cards.Update(card);
    }
}
