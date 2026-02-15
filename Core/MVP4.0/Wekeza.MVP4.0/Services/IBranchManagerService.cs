using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services
{
    public interface IBranchManagerService
    {
        // Authorization Management
        Task<List<Authorization>> GetPendingAuthorizationsAsync();
        Task<bool> ApproveAuthorizationAsync(int authorizationId, string authorizedBy, string? reason = null);
        Task<bool> RejectAuthorizationAsync(int authorizationId, string authorizedBy, string reason);
        Task<Authorization> CreateAuthorizationRequestAsync(Authorization authorization);

        // Cash Management
        Task<CashPosition> GetCurrentCashPositionAsync();
        Task<bool> UpdateCashPositionAsync(CashPosition cashPosition);
        Task<List<CashPosition>> GetCashMovementHistoryAsync(int days = 7);

        // Risk & Compliance
        Task<List<RiskAlert>> GetActiveRiskAlertsAsync();
        Task<bool> UpdateRiskAlertStatusAsync(int alertId, string status, string? assignedTo = null, string? resolution = null);
        Task<RiskAlert> CreateRiskAlertAsync(RiskAlert alert);
        Task<int> GetRiskAlertCountByTypeAsync(string alertType);

        // Report Management
        Task<List<BranchReport>> GetRecentReportsAsync(int count = 10);
        Task<BranchReport> GenerateReportAsync(string reportType, string generatedBy);
        Task<byte[]?> GetReportFileAsync(int reportId);
        Task<bool> DeleteReportAsync(int reportId);

        // Staff Performance (using existing Users table)
        Task<List<ApplicationUser>> GetBranchStaffAsync();
        Task<Dictionary<string, object>> GetBranchPerformanceMetricsAsync();
        Task<bool> UpdateStaffPerformanceAsync(Guid staffId, decimal performanceScore, string comments);
    }
}