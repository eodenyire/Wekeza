using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for RegulatoryReport aggregate
/// </summary>
public interface IRegulatoryReportRepository
{
    Task<RegulatoryReport?> GetByIdAsync(Guid id);
    Task<RegulatoryReport?> GetByCodeAndPeriodAsync(string reportCode, DateTime periodStart, DateTime periodEnd);
    Task<IEnumerable<RegulatoryReport>> GetByAuthorityAsync(RegulatoryAuthority authority);
    Task<IEnumerable<RegulatoryReport>> GetByStatusAsync(Enums.ReportStatus status);
    Task<IEnumerable<RegulatoryReport>> GetOverdueReportsAsync();
    Task<IEnumerable<RegulatoryReport>> GetReportsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<RegulatoryReport>> GetReportsByFrequencyAsync(ReportFrequency frequency);
    Task AddAsync(RegulatoryReport report);
    Task UpdateAsync(RegulatoryReport report);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(string reportCode, DateTime periodStart, DateTime periodEnd);
}