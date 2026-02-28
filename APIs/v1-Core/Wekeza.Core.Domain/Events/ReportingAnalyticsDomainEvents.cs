using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Events;

// ============================================================================
// REPORT DOMAIN EVENTS
// ============================================================================

/// <summary>
/// Report Created Domain Event - Triggered when a new report is created
/// </summary>
public record ReportCreatedDomainEvent(
    Guid ReportId,
    string ReportCode,
    ReportType ReportType,
    ReportCategory Category,
    string GeneratedBy,
    bool IsRegulatory) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Report Generated Domain Event - Triggered when report generation is completed
/// </summary>
public record ReportGeneratedDomainEvent(
    Guid ReportId,
    string ReportCode,
    ReportType ReportType,
    ReportFormat Format,
    long FileSizeBytes) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Report Submitted Domain Event - Triggered when regulatory report is submitted
/// </summary>
public record ReportSubmittedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string RegulatoryReference,
    string SubmittedBy,
    DateTime SubmittedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Report Archived Domain Event - Triggered when report is archived
/// </summary>
public record ReportArchivedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string ArchiveLocation,
    DateTime ArchivedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Report Regenerated Domain Event - Triggered when report is regenerated
/// </summary>
public record ReportRegeneratedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string RegeneratedBy,
    DateTime RegeneratedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Report Failed Domain Event - Triggered when report generation fails
/// </summary>
public record ReportFailedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string ErrorMessage,
    string FailedBy,
    DateTime FailedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Report Approved Domain Event - Triggered when report is approved
/// </summary>
public record ReportApprovedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string ApprovedBy,
    DateTime ApprovedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Report Rejected Domain Event - Triggered when report is rejected
/// </summary>
public record ReportRejectedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string RejectedBy,
    string RejectionReason,
    DateTime RejectedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ============================================================================
// DASHBOARD DOMAIN EVENTS
// ============================================================================

/// <summary>
/// Dashboard Created Domain Event - Triggered when a new dashboard is created
/// </summary>
public record DashboardCreatedDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    string DashboardName,
    DashboardType Type,
    string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Widget Added Domain Event - Triggered when widget is added to dashboard
/// </summary>
public record DashboardWidgetAddedDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    Guid WidgetId,
    WidgetType WidgetType,
    string WidgetTitle) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Widget Removed Domain Event - Triggered when widget is removed from dashboard
/// </summary>
public record DashboardWidgetRemovedDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    Guid WidgetId,
    string WidgetTitle) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Widget Updated Domain Event - Triggered when widget is updated
/// </summary>
public record DashboardWidgetUpdatedDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    Guid WidgetId,
    string WidgetTitle) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Layout Updated Domain Event - Triggered when dashboard layout changes
/// </summary>
public record DashboardLayoutUpdatedDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    DashboardLayout NewLayout) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Configuration Updated Domain Event - Triggered when dashboard config changes
/// </summary>
public record DashboardConfigurationUpdatedDomainEvent(
    Guid DashboardId,
    string DashboardCode) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Refreshed Domain Event - Triggered when dashboard data is refreshed
/// </summary>
public record DashboardRefreshedDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    string RefreshedBy,
    DateTime RefreshedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Shared With User Domain Event - Triggered when dashboard is shared with user
/// </summary>
public record DashboardSharedWithUserDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    string UserId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Shared With Role Domain Event - Triggered when dashboard is shared with role
/// </summary>
public record DashboardSharedWithRoleDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    string Role) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard User Access Removed Domain Event - Triggered when user access is removed
/// </summary>
public record DashboardUserAccessRemovedDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    string UserId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Role Access Removed Domain Event - Triggered when role access is removed
/// </summary>
public record DashboardRoleAccessRemovedDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    string Role) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Viewed Domain Event - Triggered when dashboard is viewed
/// </summary>
public record DashboardViewedDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    string ViewedBy,
    DateTime ViewedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Auto Refresh Updated Domain Event - Triggered when auto refresh settings change
/// </summary>
public record DashboardAutoRefreshUpdatedDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    bool AutoRefresh,
    int IntervalMinutes) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Theme Updated Domain Event - Triggered when dashboard theme changes
/// </summary>
public record DashboardThemeUpdatedDomainEvent(
    Guid DashboardId,
    string DashboardCode,
    string Theme) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Activated Domain Event - Triggered when dashboard is activated
/// </summary>
public record DashboardActivatedDomainEvent(
    Guid DashboardId,
    string DashboardCode) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Dashboard Deactivated Domain Event - Triggered when dashboard is deactivated
/// </summary>
public record DashboardDeactivatedDomainEvent(
    Guid DashboardId,
    string DashboardCode) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ============================================================================
// ANALYTICS DOMAIN EVENTS
// ============================================================================

/// <summary>
/// Analytics Created Domain Event - Triggered when new analytics is created
/// </summary>
public record AnalyticsCreatedDomainEvent(
    Guid AnalyticsId,
    string AnalyticsCode,
    string AnalyticsName,
    AnalyticsType Type,
    AnalyticsCategory Category,
    string ComputedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Analytics Computed Domain Event - Triggered when analytics computation is completed
/// </summary>
public record AnalyticsComputedDomainEvent(
    Guid AnalyticsId,
    string AnalyticsCode,
    AnalyticsType Type,
    int MetricCount,
    DateTime ComputedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Analytics Insight Added Domain Event - Triggered when insight is added to analytics
/// </summary>
public record AnalyticsInsightAddedDomainEvent(
    Guid AnalyticsId,
    string AnalyticsCode,
    string InsightCode,
    InsightType InsightType,
    InsightSeverity Severity) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Analytics Trend Updated Domain Event - Triggered when trend data is updated
/// </summary>
public record AnalyticsTrendUpdatedDomainEvent(
    Guid AnalyticsId,
    string AnalyticsCode,
    int TrendDataCount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Analytics Forecast Generated Domain Event - Triggered when forecast is generated
/// </summary>
public record AnalyticsForecastGeneratedDomainEvent(
    Guid AnalyticsId,
    string AnalyticsCode,
    int ForecastCount,
    decimal ConfidenceLevel) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Analytics KPI Added Domain Event - Triggered when KPI is added to analytics
/// </summary>
public record AnalyticsKPIAddedDomainEvent(
    Guid AnalyticsId,
    string AnalyticsCode,
    string KPICode,
    decimal CurrentValue,
    decimal TargetValue) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Analytics Marked Stale Domain Event - Triggered when analytics is marked as stale
/// </summary>
public record AnalyticsMarkedStaleDomainEvent(
    Guid AnalyticsId,
    string AnalyticsCode,
    DateTime MarkedStaleAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Analytics Refreshed Domain Event - Triggered when analytics is refreshed
/// </summary>
public record AnalyticsRefreshedDomainEvent(
    Guid AnalyticsId,
    string AnalyticsCode,
    string RefreshedBy,
    DateTime RefreshedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Analytics Failed Domain Event - Triggered when analytics computation fails
/// </summary>
public record AnalyticsFailedDomainEvent(
    Guid AnalyticsId,
    string AnalyticsCode,
    string ErrorMessage,
    DateTime FailedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ============================================================================
// KPI DOMAIN EVENTS
// ============================================================================

/// <summary>
/// KPI Target Exceeded Domain Event - Triggered when KPI exceeds target
/// </summary>
public record KPITargetExceededDomainEvent(
    string KPICode,
    string KPIName,
    decimal CurrentValue,
    decimal TargetValue,
    decimal ExcessPercentage,
    DateTime MeasurementDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// KPI Target Missed Domain Event - Triggered when KPI misses target significantly
/// </summary>
public record KPITargetMissedDomainEvent(
    string KPICode,
    string KPIName,
    decimal CurrentValue,
    decimal TargetValue,
    decimal MissPercentage,
    DateTime MeasurementDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// KPI Trend Changed Domain Event - Triggered when KPI trend changes significantly
/// </summary>
public record KPITrendChangedDomainEvent(
    string KPICode,
    string KPIName,
    KPITrend OldTrend,
    KPITrend NewTrend,
    decimal CurrentValue,
    DateTime MeasurementDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// KPI Status Changed Domain Event - Triggered when KPI status changes
/// </summary>
public record KPIStatusChangedDomainEvent(
    string KPICode,
    string KPIName,
    KPIStatus OldStatus,
    KPIStatus NewStatus,
    decimal CurrentValue,
    DateTime MeasurementDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ============================================================================
// REGULATORY REPORTING DOMAIN EVENTS
// ============================================================================

/// <summary>
/// Regulatory Report Due Domain Event - Triggered when regulatory report is due
/// </summary>
public record RegulatoryReportDueDomainEvent(
    Guid ReportId,
    string ReportCode,
    string RegulatoryAuthority,
    DateTime DueDate,
    int DaysUntilDue) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Regulatory Report Overdue Domain Event - Triggered when regulatory report is overdue
/// </summary>
public record RegulatoryReportOverdueDomainEvent(
    Guid ReportId,
    string ReportCode,
    string RegulatoryAuthority,
    DateTime DueDate,
    int DaysOverdue) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Regulatory Submission Acknowledged Domain Event - Triggered when submission is acknowledged
/// </summary>
public record RegulatorySubmissionAcknowledgedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string RegulatoryAuthority,
    string AcknowledgmentReference,
    DateTime AcknowledgedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Regulatory Submission Rejected Domain Event - Triggered when submission is rejected by regulator
/// </summary>
public record RegulatorySubmissionRejectedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string RegulatoryAuthority,
    string RejectionReason,
    DateTime RejectedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ============================================================================
// DATA QUALITY DOMAIN EVENTS
// ============================================================================

/// <summary>
/// Data Quality Issue Detected Domain Event - Triggered when data quality issues are found
/// </summary>
public record DataQualityIssueDetectedDomainEvent(
    string DataSource,
    string IssueType,
    string Description,
    string Severity,
    DateTime DetectedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Data Refresh Completed Domain Event - Triggered when data refresh is completed
/// </summary>
public record DataRefreshCompletedDomainEvent(
    string DataSource,
    int RecordsProcessed,
    TimeSpan Duration,
    DateTime CompletedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Data Refresh Failed Domain Event - Triggered when data refresh fails
/// </summary>
public record DataRefreshFailedDomainEvent(
    string DataSource,
    string ErrorMessage,
    DateTime FailedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}