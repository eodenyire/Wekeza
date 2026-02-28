using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Party Repository Implementation
/// High-performance repository for CIF/Party management
/// </summary>
public class PartyRepository : IPartyRepository
{
    private readonly ApplicationDbContext _context;

    public PartyRepository(ApplicationDbContext context) => _context = context;

    public async Task<Party?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Parties
            .Include(p => p.Addresses)
            .Include(p => p.Identifications)
            .Include(p => p.Relationships)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Party?> GetByPartyNumberAsync(string partyNumber, CancellationToken ct = default)
    {
        return await _context.Parties
            .Include(p => p.Addresses)
            .Include(p => p.Identifications)
            .Include(p => p.Relationships)
            .FirstOrDefaultAsync(p => p.PartyNumber == partyNumber, ct);
    }

    public async Task AddAsync(Party party, CancellationToken ct = default)
    {
        await _context.Parties.AddAsync(party, ct);
    }

    public void Update(Party party)
    {
        _context.Parties.Update(party);
    }

    public async Task<IEnumerable<Party>> SearchByNameAsync(string name, CancellationToken ct = default)
    {
        var searchTerm = name.ToLower();
        return await _context.Parties
            .Where(p => 
                (p.FirstName != null && p.FirstName.ToLower().Contains(searchTerm)) ||
                (p.LastName != null && p.LastName.ToLower().Contains(searchTerm)) ||
                (p.CompanyName != null && p.CompanyName.ToLower().Contains(searchTerm)))
            .Take(100)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Party>> SearchByIdentificationAsync(string documentNumber, CancellationToken ct = default)
    {
        return await _context.Parties
            .Where(p => p.Identifications.Any(i => i.DocumentNumber == documentNumber))
            .ToListAsync(ct);
    }

    public async Task<Party?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Parties
            .FirstOrDefaultAsync(p => p.PrimaryEmail == email, ct);
    }

    public async Task<Party?> GetByPhoneAsync(string phone, CancellationToken ct = default)
    {
        return await _context.Parties
            .FirstOrDefaultAsync(p => p.PrimaryPhone == phone, ct);
    }

    public async Task<Party?> GetByRegistrationNumberAsync(string registrationNumber, CancellationToken ct = default)
    {
        return await _context.Parties
            .FirstOrDefaultAsync(p => p.RegistrationNumber == registrationNumber, ct);
    }

    public async Task<Party?> GetLastPartyByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        return await _context.Parties
            .Where(p => p.PartyNumber.StartsWith(prefix))
            .OrderByDescending(p => p.PartyNumber)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<Party>> GetPendingKYCAsync(CancellationToken ct = default)
    {
        return await _context.Parties
            .Where(p => p.KYCStatus == KYCStatus.Pending || p.KYCStatus == KYCStatus.InProgress)
            .OrderBy(p => p.CreatedDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Party>> GetExpiredKYCAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Parties
            .Where(p => p.KYCExpiryDate != null && p.KYCExpiryDate < today)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Party>> GetHighRiskPartiesAsync(CancellationToken ct = default)
    {
        return await _context.Parties
            .Where(p => p.RiskRating == RiskRating.High || p.RiskRating == RiskRating.VeryHigh)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Party>> GetPEPPartiesAsync(CancellationToken ct = default)
    {
        return await _context.Parties
            .Where(p => p.IsPEP)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Party>> GetSanctionedPartiesAsync(CancellationToken ct = default)
    {
        return await _context.Parties
            .Where(p => p.IsSanctioned)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Party>> GetBySegmentAsync(CustomerSegment segment, CancellationToken ct = default)
    {
        return await _context.Parties
            .Where(p => p.Segment == segment)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Party>> GetByPartyTypeAsync(PartyType partyType, CancellationToken ct = default)
    {
        return await _context.Parties
            .Where(p => p.PartyType == partyType)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Party>> GetRelatedPartiesAsync(Guid partyId, CancellationToken ct = default)
    {
        var party = await GetByIdAsync(partyId, ct);
        if (party == null) return Enumerable.Empty<Party>();

        var relatedPartyIds = party.Relationships.Select(r => r.RelatedPartyId).ToList();
        
        return await _context.Parties
            .Where(p => relatedPartyIds.Contains(p.Id))
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Party>> GetCorporateGroupAsync(Guid parentPartyId, CancellationToken ct = default)
    {
        // Get all parties that have a relationship with the parent
        return await _context.Parties
            .Where(p => p.Relationships.Any(r => 
                r.RelatedPartyId == parentPartyId && 
                (r.RelationshipType == "Subsidiary" || r.RelationshipType == "Parent")))
            .ToListAsync(ct);
    }

    public async Task<int> GetTotalPartiesCountAsync(CancellationToken ct = default)
    {
        return await _context.Parties.CountAsync(ct);
    }

    public async Task<Dictionary<CustomerSegment, int>> GetPartiesBySegmentCountAsync(CancellationToken ct = default)
    {
        return await _context.Parties
            .GroupBy(p => p.Segment)
            .Select(g => new { Segment = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Segment, x => x.Count, ct);
    }

    public async Task<Dictionary<RiskRating, int>> GetPartiesByRiskRatingCountAsync(CancellationToken ct = default)
    {
        return await _context.Parties
            .GroupBy(p => p.RiskRating)
            .Select(g => new { Rating = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Rating, x => x.Count, ct);
    }

    public async Task<bool> ExistsByPartyNumberAsync(string partyNumber, CancellationToken ct = default)
    {
        return await _context.Parties.AnyAsync(p => p.PartyNumber == partyNumber, ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Parties.AnyAsync(p => p.PrimaryEmail == email, ct);
    }

    public async Task<bool> ExistsByIdentificationAsync(string documentNumber, CancellationToken ct = default)
    {
        return await _context.Parties
            .AnyAsync(p => p.Identifications.Any(i => i.DocumentNumber == documentNumber), ct);
    }
}
