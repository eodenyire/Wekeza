using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

public interface ITransactionMonitoringRepository
{
    Task<TransactionMonitoring?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TransactionMonitoring?> GetByTransactionIdAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TransactionMonitoring>> GetByStatusAsync(MonitoringStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<TransactionMonitoring>> GetBySeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default);
    Task<IEnumerable<TransactionMonitoring>> GetByResultAsync(ScreeningResult result, CancellationToken cancellationToken = default);
    Task<IEnumerable<TransactionMonitoring>> GetPendingReviewAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TransactionMonitoring>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<TransactionMonitoring>> GetHighSeverityAlertsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TransactionMonitoring>> GetOverdueReviewsAsync(int daysOverdue, CancellationToken cancellationToken = default);
    Task<IEnumerable<TransactionMonitoring>> GetByReviewerAsync(string reviewerId, CancellationToken cancellationToken = default);
    Task<int> GetAlertCountByStatusAsync(MonitoringStatus status, CancellationToken cancellationToken = default);
    Task<int> GetAlertCountBySeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetAlertStatisticsByRuleAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task AddAsync(TransactionMonitoring monitoring, CancellationToken cancellationToken = default);
    Task UpdateAsync(TransactionMonitoring monitoring, CancellationToken cancellationToken = default);
    Task DeleteAsync(TransactionMonitoring monitoring, CancellationToken cancellationToken = default);
}