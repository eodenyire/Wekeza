using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for SystemMonitor aggregate
/// Provides data access methods for system monitoring operations
/// </summary>
public interface ISystemMonitorRepository
{
    // Basic CRUD Operations
    Task<SystemMonitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SystemMonitor?> GetByCodeAsync(string monitorCode, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SystemMonitor> AddAsync(SystemMonitor monitor, CancellationToken cancellationToken = default);
    Task<SystemMonitor> UpdateAsync(SystemMonitor monitor, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Query Operations by Type and Status
    Task<List<SystemMonitor>> GetByTypeAsync(MonitorType type, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetByStatusAsync(MonitorStatus status, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetByHealthAsync(MonitorHealth health, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetEnabledAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetDisabledAsync(CancellationToken cancellationToken = default);

    // Health and Status Queries
    Task<List<SystemMonitor>> GetHealthyAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetUnhealthyAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetDegradedAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetInMaintenanceAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetWithUnknownHealthAsync(CancellationToken cancellationToken = default);

    // Target Resource Queries
    Task<List<SystemMonitor>> GetByTargetResourceAsync(string targetResource, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetByResourcePatternAsync(string resourcePattern, CancellationToken cancellationToken = default);
    Task<SystemMonitor?> GetMonitorForResourceAsync(string targetResource, MonitorType? type = null, CancellationToken cancellationToken = default);

    // Performance and Metrics Queries
    Task<List<SystemMonitor>> GetWithHighFailureRateAsync(decimal minFailureRate = 10m, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetWithSlowResponseTimeAsync(TimeSpan minResponseTime, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetWithConsecutiveFailuresAsync(int minFailures = 3, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetWithLowSuccessRateAsync(decimal maxSuccessRate = 90m, CancellationToken cancellationToken = default);

    // Alert and Notification Queries
    Task<List<SystemMonitor>> GetWithActiveAlertsAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetWithAlertRulesAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetWithNotificationChannelsAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetByAlertSeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default);

    // Time-based Queries
    Task<List<SystemMonitor>> GetDueForCheckAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetOverdueForCheckAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetRecentlyCheckedAsync(TimeSpan within, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetNotCheckedSinceAsync(DateTime since, CancellationToken cancellationToken = default);

    // Search Operations
    Task<List<SystemMonitor>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> SearchByTargetResourceAsync(string resource, CancellationToken cancellationToken = default);

    // Pagination Support
    Task<(List<SystemMonitor> Monitors, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        MonitorType? type = null,
        MonitorStatus? status = null,
        MonitorHealth? health = null,
        CancellationToken cancellationToken = default);

    // Configuration and Settings
    Task<List<SystemMonitor>> GetByCheckIntervalAsync(TimeSpan minInterval, TimeSpan maxInterval, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetWithTimeoutAsync(TimeSpan minTimeout, TimeSpan maxTimeout, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetWithRetryAttemptsAsync(int minAttempts, int maxAttempts, CancellationToken cancellationToken = default);

    // Threshold Management
    Task<List<SystemMonitor>> GetWithThresholdsAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetWithThresholdAsync(string metric, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetExceedingThresholdAsync(string metric, decimal value, CancellationToken cancellationToken = default);

    // Reporting & Analytics
    Task<Dictionary<MonitorType, int>> GetCountByTypeAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<MonitorStatus, int>> GetCountByStatusAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<MonitorHealth, int>> GetCountByHealthAsync(CancellationToken cancellationToken = default);
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetHealthyCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetUnhealthyCountAsync(CancellationToken cancellationToken = default);

    // Performance Statistics
    Task<decimal> GetAverageSuccessRateAsync(MonitorType? type = null, CancellationToken cancellationToken = default);
    Task<TimeSpan> GetAverageResponseTimeAsync(MonitorType? type = null, CancellationToken cancellationToken = default);
    Task<Dictionary<string, decimal>> GetSuccessRateByResourceAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, TimeSpan>> GetResponseTimeByResourceAsync(CancellationToken cancellationToken = default);

    // Health Summary Operations
    Task<Dictionary<string, object>> GetSystemHealthSummaryAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<MonitorType, Dictionary<string, object>>> GetHealthSummaryByTypeAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetCriticalMonitorsAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetRequiringAttentionAsync(CancellationToken cancellationToken = default);

    // Bulk Operations
    Task<List<SystemMonitor>> AddRangeAsync(List<SystemMonitor> monitors, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> UpdateRangeAsync(List<SystemMonitor> monitors, CancellationToken cancellationToken = default);
    Task BulkUpdateStatusAsync(List<Guid> monitorIds, MonitorStatus status, string updatedBy, CancellationToken cancellationToken = default);
    Task BulkEnableAsync(List<Guid> monitorIds, string enabledBy, CancellationToken cancellationToken = default);
    Task BulkDisableAsync(List<Guid> monitorIds, string disabledBy, CancellationToken cancellationToken = default);
    Task BulkUpdateCheckIntervalAsync(List<Guid> monitorIds, TimeSpan interval, string updatedBy, CancellationToken cancellationToken = default);

    // Validation Support
    Task<bool> IsCodeAvailableAsync(string monitorCode, Guid? excludeMonitorId = null, CancellationToken cancellationToken = default);
    Task<bool> CanDeleteMonitorAsync(Guid monitorId, CancellationToken cancellationToken = default);
    Task<bool> HasDependentMonitorsAsync(Guid monitorId, CancellationToken cancellationToken = default);

    // Existence Checks
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string monitorCode, CancellationToken cancellationToken = default);
    Task<bool> ExistsForResourceAsync(string targetResource, CancellationToken cancellationToken = default);

    // Alert History and Management
    Task<List<MonitorAlert>> GetAlertsAsync(Guid monitorId, CancellationToken cancellationToken = default);
    Task<List<MonitorAlert>> GetActiveAlertsAsync(Guid? monitorId = null, CancellationToken cancellationToken = default);
    Task<List<MonitorAlert>> GetRecentAlertsAsync(TimeSpan within, Guid? monitorId = null, CancellationToken cancellationToken = default);
    Task<List<MonitorAlert>> GetAlertsByTypeAsync(string monitorCode, AlertSeverity? severity = null, CancellationToken cancellationToken = default);

    // Check History and Trends
    Task<List<Dictionary<string, object>>> GetCheckHistoryAsync(Guid monitorId, int count = 100, CancellationToken cancellationToken = default);
    Task<Dictionary<string, object>> GetLatestCheckResultAsync(Guid monitorId, CancellationToken cancellationToken = default);
    Task<List<Dictionary<string, object>>> GetFailedChecksAsync(Guid monitorId, int count = 50, CancellationToken cancellationToken = default);

    // Maintenance Operations
    Task<List<SystemMonitor>> GetForMaintenanceAsync(CancellationToken cancellationToken = default);
    Task SetMaintenanceModeAsync(Guid monitorId, bool maintenanceMode, string updatedBy, CancellationToken cancellationToken = default);
    Task BulkSetMaintenanceModeAsync(List<Guid> monitorIds, bool maintenanceMode, string updatedBy, CancellationToken cancellationToken = default);

    // Cleanup Operations
    Task<int> CleanupOldCheckResultsAsync(TimeSpan olderThan, CancellationToken cancellationToken = default);
    Task<int> CleanupResolvedAlertsAsync(TimeSpan olderThan, CancellationToken cancellationToken = default);
    Task<int> ArchiveInactiveMonitorsAsync(TimeSpan inactiveSince, CancellationToken cancellationToken = default);

    // Export and Import
    Task<List<SystemMonitor>> ExportConfigurationAsync(MonitorType? type = null, CancellationToken cancellationToken = default);
    Task ImportConfigurationAsync(List<SystemMonitor> monitors, string importedBy, bool overwriteExisting = false, CancellationToken cancellationToken = default);

    // Dependency Management
    Task<List<SystemMonitor>> GetDependentMonitorsAsync(string targetResource, CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetMonitorDependenciesAsync(Guid monitorId, CancellationToken cancellationToken = default);
    Task<Dictionary<string, List<string>>> GetResourceDependencyMapAsync(CancellationToken cancellationToken = default);

    // Notification Channel Management
    Task<List<string>> GetAllNotificationChannelsAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetNotificationChannelUsageAsync(CancellationToken cancellationToken = default);
    Task<List<SystemMonitor>> GetByNotificationChannelAsync(string channel, CancellationToken cancellationToken = default);
}