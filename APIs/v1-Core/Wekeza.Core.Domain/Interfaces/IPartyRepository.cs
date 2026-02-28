using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository for Party/CIF management
/// Inspired by Finacle CIF and T24 CUSTOMER module
/// </summary>
public interface IPartyRepository
{
    // Basic CRUD
    Task<Party?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Party?> GetByPartyNumberAsync(string partyNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Party party, CancellationToken cancellationToken = default);
    void Update(Party party);
    
    // Search & Query
    Task<IEnumerable<Party>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Party>> SearchByIdentificationAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task<Party?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Party?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<Party?> GetByRegistrationNumberAsync(string registrationNumber, CancellationToken cancellationToken = default);
    Task<Party?> GetLastPartyByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
    
    // KYC & Compliance
    Task<IEnumerable<Party>> GetPendingKYCAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Party>> GetExpiredKYCAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Party>> GetHighRiskPartiesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Party>> GetPEPPartiesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Party>> GetSanctionedPartiesAsync(CancellationToken cancellationToken = default);
    
    // Segmentation
    Task<IEnumerable<Party>> GetBySegmentAsync(CustomerSegment segment, CancellationToken cancellationToken = default);
    Task<IEnumerable<Party>> GetByPartyTypeAsync(PartyType partyType, CancellationToken cancellationToken = default);
    
    // Relationships
    Task<IEnumerable<Party>> GetRelatedPartiesAsync(Guid partyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Party>> GetCorporateGroupAsync(Guid parentPartyId, CancellationToken cancellationToken = default);
    
    // Analytics
    Task<int> GetTotalPartiesCountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<CustomerSegment, int>> GetPartiesBySegmentCountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<RiskRating, int>> GetPartiesByRiskRatingCountAsync(CancellationToken cancellationToken = default);
    
    // Validation
    Task<bool> ExistsByPartyNumberAsync(string partyNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdentificationAsync(string documentNumber, CancellationToken cancellationToken = default);
}
