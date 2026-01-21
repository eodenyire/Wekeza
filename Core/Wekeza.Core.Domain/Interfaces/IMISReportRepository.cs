using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for MISReport aggregate
/// </summary>
public interface IMISReportRepository
{
    Task<MISReport?> GetByIdAsync(Guid id);
    Task<MISReport?> GetByCodeAndPeriodAsync(string reportCode, DateTime periodStart, DateTime periodEnd);
    Task<IEnumerable<MISReport>> GetByReportTypeAsync(MISReportType reportType);
    Task<IEnumerable<MISReport>> GetByStatusAsync(Enums.ReportStatus status);
    Task<IEnumerable<MISReport>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<MISReport>> GetByDepartmentAsync(string department);
    Task<IEnumerable<MISReport>> GetExecutiveReportsAsync();
    Task AddAsync(MISReport report);
    Task UpdateAsync(MISReport report);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(string reportCode, DateTime periodStart, DateTime periodEnd);
}