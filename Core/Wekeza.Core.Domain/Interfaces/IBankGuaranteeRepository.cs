using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

public interface IBankGuaranteeRepository
{
    Task<BankGuarantee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BankGuarantee?> GetByBGNumberAsync(string bgNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<BankGuarantee>> GetByPrincipalIdAsync(Guid principalId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BankGuarantee>> GetByBeneficiaryIdAsync(Guid beneficiaryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BankGuarantee>> GetByIssuingBankIdAsync(Guid issuingBankId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BankGuarantee>> GetExpiringBGsAsync(DateTime expiryDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<BankGuarantee>> GetOutstandingBGsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<BankGuarantee>> GetBGsByStatusAsync(BGStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<BankGuarantee>> GetBGsByTypeAsync(GuaranteeType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<BankGuarantee>> GetBGsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<BankGuarantee>> GetBGsWithClaimsAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalBGExposureAsync(Guid principalId, CancellationToken cancellationToken = default);
    Task<int> GetBGCountByStatusAsync(BGStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(BankGuarantee bankGuarantee, CancellationToken cancellationToken = default);
    Task UpdateAsync(BankGuarantee bankGuarantee, CancellationToken cancellationToken = default);
    Task DeleteAsync(BankGuarantee bankGuarantee, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string bgNumber, CancellationToken cancellationToken = default);
}