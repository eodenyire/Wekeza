using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin;

/// <summary>
/// Dashboard & Analytics Service - KPI Dashboards, Business Metrics, Real-time Analytics
/// Analytics portal providing unified dashboards for business, operations, and security KPIs with custom widgets
/// </summary>
public interface IDashboardAnalyticsService
{
    // ===== Executive Dashboard =====
    Task<ExecutiveDashboardDTO> GetExecutiveDashboardAsync(DateTime? asOfDate = null);
    Task<BusinessMetricsDTO> GetBusinessMetricsAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<FinancialPerformanceDTO> GetFinancialPerformanceAsync(DateTime? fromDate = null, DateTime? toDate = null);

    // ===== Operational Dashboard =====
    Task<OperationalDashboardDTO> GetOperationalDashboardAsync();
    Task<ProcessEfficiencyDTO> GetProcessEfficiencyAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<QueueAnalyticsDTO> GetQueueAnalyticsAsync();
    Task<SLAPerformanceDTO> GetSLAPerformanceAsync(DateTime? fromDate = null, DateTime? toDate = null);

    // ===== Security & Risk Dashboard =====
    Task<SecurityDashboardDTO> GetSecurityDashboardAsync(DateTime? asOfDate = null);
    Task<AnomalyDashboardDTO> GetAnomalyDashboardAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<ComplianceDashboardDTO> GetComplianceDashboardAsync(DateTime? asOfDate = null);

    // ===== Product Analytics =====
    Task<ProductAnalyticsDTO> GetProductAnalyticsAsync(string? productCode = null);
    Task<ProductGrowthDTO> GetProductGrowthAsync(string productCode, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ProductProfitabilityDTO> GetProductProfitabilityAsync(string productCode, DateTime? fromDate = null, DateTime? toDate = null);

    // ===== Customer Analytics =====
    Task<CustomerAnalyticsDTO> GetCustomerAnalyticsAsync();
    Task<CustomerSegmentationDTO> GetCustomerSegmentationAsync();
    Task<CustomerLifetimeValueAnalyticsDTO> GetCustomerLifetimeValueAsync();

    // ===== Transaction Analytics =====
    Task<TransactionAnalyticsDTO> GetTransactionAnalyticsAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<TransactionVolumeDTO> GetTransactionVolumeAsync(string? granularity = null);
    Task<TransactionTrendDTO> GetTransactionTrendAsync(DateTime? fromDate = null, DateTime? toDate = null);

    // ===== Custom Dashboard Management =====
    Task<CustomDashboardDTO> GetCustomDashboardAsync(Guid dashboardId);
    Task<List<CustomDashboardDTO>> GetUserDashboardsAsync(Guid userId);
    Task<CustomDashboardDTO> CreateCustomDashboardAsync(CreateCustomDashboardRequest request, Guid createdByUserId);
    Task<CustomDashboardDTO> UpdateCustomDashboardAsync(Guid dashboardId, UpdateCustomDashboardRequest request, Guid updatedByUserId);
    Task<CustomDashboardDTO> SaveDashboardLayoutAsync(Guid dashboardId, List<DashboardWidgetLayoutDTO> layout);
    Task DeleteCustomDashboardAsync(Guid dashboardId);

    // ===== Widget Management =====
    Task<List<AvailableWidgetDTO>> GetAvailableWidgetsAsync();
    Task<WidgetDataDTO> GetWidgetDataAsync(Guid widgetId, Dictionary<string, object> parameters);
    Task<List<RecentReportsDTO>> GetRecentReportsAsync(int limit = 10);

    // ===== KPI Configuration =====
    Task<KPIDefinitionDTO> GetKPIAsync(Guid kpiId);
    Task<List<KPIDefinitionDTO>> GetAllKPIsAsync();
    Task<KPIDefinitionDTO> CreateKPIAsync(CreateKPIRequest request, Guid createdByUserId);
    Task<KPIDefinitionDTO> UpdateKPIAsync(Guid kpiId, UpdateKPIRequest request, Guid updatedByUserId);
    Task<List<KPITrendDTO>> GetKPITrendAsync(Guid kpiId, DateTime? fromDate = null, DateTime? toDate = null);

    // ===== Report Builder =====
    Task<ReportDTO> GetReportAsync(Guid reportId);
    Task<List<ReportDTO>> SearchReportsAsync(string? reportType = null, int page = 1, int pageSize = 50);
    Task<ReportDTO> CreateReportAsync(CreateReportRequest request, Guid createdByUserId);
    Task<ReportDTO> UpdateReportAsync(Guid reportId, UpdateReportRequest request, Guid updatedByUserId);
    Task<ReportExecutionResultDTO> ExecuteReportAsync(Guid reportId, Dictionary<string, object> parameters);
    Task<byte[]> ExportReportAsync(Guid reportId, string format);
    Task ScheduleReportAsync(Guid reportId, string cronExpression, List<string> recipients, Guid scheduledByUserId);

    // ===== Real-time Metrics =====
    Task<RealTimeMetricsDTO> GetRealTimeMetricsAsync();
    Task<AlertSummaryDTO> GetAlertSummaryAsync();
    Task<SystemHealthDTO> GetSystemHealthAsync();

    // ===== Analytics Export =====
    Task<byte[]> ExportDashboardAsync(Guid dashboardId, string format);
    Task<byte[]> GenerateAnalyticsReportAsync(AnalyticsReportRequest request);
    Task<List<SavedAnalysisDTO>> GetSavedAnalysesAsync(Guid userId);
    Task<SavedAnalysisDTO> SaveAnalysisAsync(SaveAnalysisRequest request, Guid savedByUserId);
}

// DTOs
public class ExecutiveDashboardDTO
{
    public DateTime AsOfDate { get; set; }
    public FinancialHealthDTO FinancialHealth { get; set; }
    public KeyMetricsDTO KeyMetrics { get; set; }
    public List<StrategicAlertDTO> StrategicAlerts { get; set; }
    public Dictionary<string, decimal> TopMetrics { get; set; }
}

// FinancialHealthDTO defined in IFinanceAdminService.cs (primary location)

public class KeyMetricsDTO
{
    public int ActiveCustomers { get; set; }
    public int ActiveAccounts { get; set; }
    public decimal TotalDeposits { get; set; }
    public decimal TotalLoans { get; set; }
    public decimal NII { get; set; }
}

public class StrategicAlertDTO
{
    public string AlertType { get; set; }
    public string Message { get; set; }
    public string Priority { get; set; }
}

public class BusinessMetricsDTO
{
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetProfit { get; set; }
    public double GrowthRate { get; set; }
    public List<MonthlyMetricDTO> MonthlyTrend { get; set; }
}

public class MonthlyMetricDTO
{
    public DateTime Month { get; set; }
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
}

public class FinancialPerformanceDTO
{
    public decimal TotalRevenue { get; set; }
    public decimal InterestIncome { get; set; }
    public decimal NonInterestIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetIncome { get; set; }
    public double ProfitMargin { get; set; }
}

public class OperationalDashboardDTO
{
    public int PendingTransactions { get; set; }
    public int QueuedRequests { get; set; }
    public double AverageTurnaroundTime { get; set; }
    public List<OperationalAlertDTO> Alerts { get; set; }
}

public class OperationalAlertDTO
{
    public string AlertType { get; set; }
    public string Message { get; set; }
    public int AffectedItems { get; set; }
}

public class ProcessEfficiencyDTO
{
    public List<ProcessMetricDTO> ProcessMetrics { get; set; }
    public double OverallEfficiency { get; set; }
}

public class ProcessMetricDTO
{
    public string ProcessName { get; set; }
    public int ItemsProcessed { get; set; }
    public double AverageTime { get; set; }
    public int Errors { get; set; }
}

public class QueueAnalyticsDTO
{
    public List<QueueMetricDTO> QueueMetrics { get; set; }
    public int TotalQueued { get; set; }
    public double AverageWaitTime { get; set; }
}

public class QueueMetricDTO
{
    public string QueueName { get; set; }
    public int Count { get; set; }
    public double AverageWaitTime { get; set; }
}

public class SLAPerformanceDTO
{
    public int TotalSLAs { get; set; }
    public int MetSLAs { get; set; }
    public int BreachedSLAs { get; set; }
    public double SLACompliance { get; set; }
}

// SecurityDashboardDTO and SecurityAlertDTO defined in ISecurityAdminService.cs (primary locations)

public class AnomalyDashboardDTO
{
    public int AnomaliesDetected { get; set; }
    public int OpenAnomalies { get; set; }
    public List<DetectedAnomalyDTO> RecentAnomalies { get; set; }
}

// DetectedAnomalyDTO defined in IRiskManagementService.cs (primary location)

// ComplianceDashboardDTO and ComplianceIssueDTO defined in IComplianceAdminService.cs (primary locations)

public class ProductAnalyticsDTO
{
    public List<ProductMetricDTO> Products { get; set; }
}

// ProductMetricDTO defined in IProductAdminService.cs (primary location)

public class ProductGrowthDTO
{
    public string ProductCode { get; set; }
    public List<GrowthTrendDTO> Trend { get; set; }
    public double YoYGrowth { get; set; }
}

public class GrowthTrendDTO
{
    public DateTime Period { get; set; }
    public decimal Value { get; set; }
}

public class ProductProfitabilityDTO
{
    public string ProductCode { get; set; }
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetProfits { get; set; }
    public double RoI { get; set; }
}

public class CustomerAnalyticsDTO
{
    public int TotalCustomers { get; set; }
    public int NewCustomersThisMonth { get; set; }
    public int ChurnedCustomers { get; set; }
    public decimal AverageLifetimeValue { get; set; }
}

public class CustomerSegmentationDTO
{
    public List<SegmentMetricDTO> Segments { get; set; }
}

public class SegmentMetricDTO
{
    public string SegmentName { get; set; }
    public int CustomerCount { get; set; }
    public decimal TotalValue { get; set; }
}

public class CustomerLifetimeValueAnalyticsDTO
{
    public decimal AverageLTV { get; set; }
    public List<LTVTierDTO> LTVTiers { get; set; }
}

public class LTVTierDTO
{
    public string Tier { get; set; }
    public int CustomerCount { get; set; }
    public decimal AverageLTV { get; set; }
}

public class TransactionAnalyticsDTO
{
    public int TotalTransactions { get; set; }
    public decimal TotalVolume { get; set; }
    public double AverageTransactionSize { get; set; }
    public List<TransactionTypeMetricDTO> ByType { get; set; }
}

public class TransactionTypeMetricDTO
{
    public string TransactionType { get; set; }
    public int Count { get; set; }
    public decimal Volume { get; set; }
}

public class TransactionVolumeDTO
{
    public List<VolumeTrendDTO> Trend { get; set; }
}

public class VolumeTrendDTO
{
    public DateTime Period { get; set; }
    public int TransactionCount { get; set; }
    public decimal Volume { get; set; }
}

public class TransactionTrendDTO
{
    public List<TransactionTrendLineDTO> TrendLines { get; set; }
}

public class TransactionTrendLineDTO
{
    public DateTime Date { get; set; }
    public decimal Volume { get; set; }
    public int Count { get; set; }
}

public class CustomDashboardDTO
{
    public Guid Id { get; set; }
    public string DashboardName { get; set; }
    public string Description { get; set; }
    public List<DashboardWidgetLayoutDTO> Widgets { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DashboardWidgetLayoutDTO
{
    public Guid WidgetId { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class CreateCustomDashboardRequest
{
    public string DashboardName { get; set; }
    public string Description { get; set; }
}

public class UpdateCustomDashboardRequest
{
    public string DashboardName { get; set; }
    public string Description { get; set; }
}

public class AvailableWidgetDTO
{
    public Guid WidgetId { get; set; }
    public string WidgetName { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
}

public class WidgetDataDTO
{
    public Guid WidgetId { get; set; }
    public object Data { get; set; }
    public DateTime RefreshedAt { get; set; }
}

public class RecentReportsDTO
{
    public Guid ReportId { get; set; }
    public string ReportName { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class KPIDefinitionDTO
{
    public Guid Id { get; set; }
    public string KPICode { get; set; }
    public string KPIName { get; set; }
    public string Formula { get; set; }
    public decimal Target { get; set; }
    public string Status { get; set; }
}

public class CreateKPIRequest
{
    public string KPICode { get; set; }
    public string KPIName { get; set; }
    public string Formula { get; set; }
    public decimal Target { get; set; }
}

public class UpdateKPIRequest
{
    public string KPIName { get; set; }
    public string Formula { get; set; }
    public decimal Target { get; set; }
}

public class KPITrendDTO
{
    public DateTime Period { get; set; }
    public decimal Value { get; set; }
    public decimal Target { get; set; }
    public double PercentageOfTarget { get; set; }
}

public class ReportDTO
{
    public Guid Id { get; set; }
    public string ReportName { get; set; }
    public string ReportType { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReportRequest
{
    public string ReportName { get; set; }
    public string ReportType { get; set; }
    public List<string> Columns { get; set; }
    public Dictionary<string, object> Filters { get; set; }
}

public class UpdateReportRequest
{
    public string ReportName { get; set; }
    public List<string> Columns { get; set; }
    public Dictionary<string, object> Filters { get; set; }
}

public class ReportExecutionResultDTO
{
    public Guid ExecutionId { get; set; }
    public int RowCount { get; set; }
    public DateTime ExecutedAt { get; set; }
}

public class RealTimeMetricsDTO
{
    public int ActiveUsers { get; set; }
    public int TransactionsPerSecond { get; set; }
    public double SystemCPUUsage { get; set; }
    public double SystemMemoryUsage { get; set; }
}

public class AlertSummaryDTO
{
    public int CriticalAlerts { get; set; }
    public int HighPriorityAlerts { get; set; }
    public int MediumPriorityAlerts { get; set; }
}

public class SystemHealthDTO
{
    public string OverallStatus { get; set; }
    public List<ComponentHealthDTO> Components { get; set; }
}

public class ComponentHealthDTO
{
    public string ComponentName { get; set; }
    public string Status { get; set; }
    public double HealthPercentage { get; set; }
}

public class AnalyticsReportRequest
{
    public string ReportType { get; set; }
    public string Format { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class SavedAnalysisDTO
{
    public Guid AnalysisId { get; set; }
    public string AnalysisName { get; set; }
    public DateTime SavedAt { get; set; }
}

public class SaveAnalysisRequest
{
    public string AnalysisName { get; set; }
    public Dictionary<string, object> AnalysisData { get; set; }
}
