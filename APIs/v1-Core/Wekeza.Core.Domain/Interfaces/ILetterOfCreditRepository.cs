using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

public interface ILetterOfCreditRepository
{
    Task<LetterOfCredit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LetterOfCredit?> GetByLCNumberAsync(string lcNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<LetterOfCredit>> GetByApplicantIdAsync(Guid applicantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LetterOfCredit>> GetByBeneficiaryIdAsync(Guid beneficiaryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LetterOfCredit>> GetByIssuingBankIdAsync(Guid issuingBankId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LetterOfCredit>> GetExpiringLCsAsync(DateTime expiryDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<LetterOfCredit>> GetOutstandingLCsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<LetterOfCredit>> GetLCsByStatusAsync(LCStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<LetterOfCredit>> GetLCsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalLCExposureAsync(Guid applicantId, CancellationToken cancellationToken = default);
    Task<int> GetLCCountByStatusAsync(LCStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(LetterOfCredit letterOfCredit, CancellationToken cancellationToken = default);
    Task UpdateAsync(LetterOfCredit letterOfCredit, CancellationToken cancellationToken = default);
    Task DeleteAsync(LetterOfCredit letterOfCredit, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string lcNumber, CancellationToken cancellationToken = default);
}