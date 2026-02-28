using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

public interface IAMLCaseRepository
{
    Task<AMLCase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AMLCase?> GetByCaseNumberAsync(string caseNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<AMLCase>> GetByPartyIdAsync(Guid partyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AMLCase>> GetByTransactionIdAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AMLCase>> GetByInvestigatorIdAsync(string investigatorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AMLCase>> GetByStatusAsync(AMLCaseStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<AMLCase>> GetByAlertTypeAsync(AMLAlertType alertType, CancellationToken cancellationToken = default);
    Task<IEnumerable<AMLCase>> GetByRiskLevelAsync(RiskLevel riskLevel, CancellationToken cancellationToken = default);
    Task<IEnumerable<AMLCase>> GetOpenCasesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AMLCase>> GetOverdueCasesAsync(int daysOverdue, CancellationToken cancellationToken = default);
    Task<IEnumerable<AMLCase>> GetCasesByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<AMLCase>> GetHighRiskCasesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AMLCase>> GetCasesRequiringSARAsync(CancellationToken cancellationToken = default);
    Task<int> GetCaseCountByStatusAsync(AMLCaseStatus status, CancellationToken cancellationToken = default);
    Task<int> GetOpenCaseCountByInvestigatorAsync(string investigatorId, CancellationToken cancellationToken = default);
    Task AddAsync(AMLCase amlCase, CancellationToken cancellationToken = default);
    Task UpdateAsync(AMLCase amlCase, CancellationToken cancellationToken = default);
    Task DeleteAsync(AMLCase amlCase, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string caseNumber, CancellationToken cancellationToken = default);
}