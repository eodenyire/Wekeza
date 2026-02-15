namespace Wekeza.Core.Domain.Enums;

/// <summary>
/// Report Type - Categorizes different types of reports
/// </summary>
public enum ReportType
{
    Financial,
    Regulatory,
    Operational,
    Risk,
    Compliance,
    Customer,
    Management,
    Audit,
    Statistical,
    Analytical,
    PrudentialReturn,
    StatutoryReturn,
    ComplianceReport,
    TaxReturn,
    AMLReport,
    RiskReport,
    FinancialStatement,
    RegulatoryFiling
}

/// <summary>
/// Report Category - Specific categories within report types
/// </summary>
public enum ReportCategory
{
    // Financial Reports
    BalanceSheet,
    ProfitAndLoss,
    CashFlow,
    TrialBalance,
    GeneralLedger,
    ConsolidatedFinancials,
    
    // Regulatory Reports
    RegulatoryReturn,
    PrudentialReturn,
    CapitalAdequacy,
    LiquidityReport,
    LargeExposure,
    CreditRiskReport,
    MarketRiskReport,
    OperationalRiskReport,
    
    // AML/Compliance Reports
    AMLReport,
    SARReport,
    CTRReport,
    SanctionsReport,
    KYCReport,
    
    // Operational Reports
    BranchPerformance,
    ProductPerformance,
    ChannelAnalytics,
    TransactionAnalytics,
    CustomerAnalytics,
    
    // Management Reports
    ExecutiveDashboard,
    KPIDashboard,
    PerformanceReport,
    TrendAnalysis,
    
    // Audit Reports
    InternalAudit,
    ExternalAudit,
    ComplianceAudit,
    ITAudit,
    
    // Risk Reports
    CreditPortfolio,
    MarketRisk,
    LiquidityRisk,
    ConcentrationRisk,
    StressTesting
}

/// <summary>
/// Report Format - Output formats for reports
/// </summary>
public enum ReportFormat
{
    PDF,
    Excel,
    CSV,
    JSON,
    XML,
    HTML,
    Word,
    PowerPoint
}

/// <summary>
/// Dashboard Type - Different types of dashboards
/// </summary>
public enum DashboardType
{
    Executive,
    Operational,
    Risk,
    Compliance,
    Branch,
    Customer,
    Product,
    Channel,
    Financial,
    Analytical,
    Custom
}

/// <summary>
/// Dashboard Status - Status of dashboards
/// </summary>
public enum DashboardStatus
{
    Active,
    Inactive,
    Draft,
    Published,
    Archived,
    Maintenance
}

/// <summary>
/// Dashboard Layout - Layout types for dashboards
/// </summary>
public enum DashboardLayout
{
    Grid,
    Masonry,
    Flexible,
    Fixed,
    Responsive,
    Custom
}

/// <summary>
/// Dashboard Visibility - Access control for dashboards
/// </summary>
public enum DashboardVisibility
{
    Private,
    Public,
    Shared,
    RoleRestricted,
    UserRestricted
}

/// <summary>
/// Widget Type - Types of dashboard widgets
/// </summary>
public enum WidgetType
{
    // Chart Widgets
    LineChart,
    BarChart,
    PieChart,
    AreaChart,
    ScatterChart,
    Histogram,
    Heatmap,
    
    // Data Widgets
    DataTable,
    DataGrid,
    MetricCard,
    KPICard,
    Scorecard,
    
    // Text Widgets
    TextWidget,
    MarkdownWidget,
    HTMLWidget,
    
    // Interactive Widgets
    FilterWidget,
    DatePicker,
    Dropdown,
    SearchBox,
    
    // Financial Widgets
    BalanceSheet,
    ProfitLoss,
    CashFlow,
    RatioAnalysis,
    
    // Risk Widgets
    RiskMeter,
    VaRChart,
    StressTest,
    LimitMonitor,
    
    // Operational Widgets
    TransactionVolume,
    ChannelMetrics,
    BranchMetrics,
    CustomerMetrics,
    
    // Custom Widgets
    CustomChart,
    CustomTable,
    CustomMetric,
    IFrameWidget
}

/// <summary>
/// Widget Size - Size options for widgets
/// </summary>
public enum WidgetSize
{
    Small,      // 1x1
    Medium,     // 2x2
    Large,      // 3x3
    Wide,       // 4x2
    Tall,       // 2x4
    ExtraLarge, // 4x4
    Custom
}

/// <summary>
/// Analytics Type - Types of analytics
/// </summary>
public enum AnalyticsType
{
    Descriptive,  // What happened?
    Diagnostic,   // Why did it happen?
    Predictive,   // What will happen?
    Prescriptive  // What should we do?
}

/// <summary>
/// Analytics Category - Categories of analytics
/// </summary>
public enum AnalyticsCategory
{
    // Financial Analytics
    Profitability,
    Liquidity,
    Capital,
    AssetQuality,
    
    // Customer Analytics
    CustomerSegmentation,
    CustomerLifetime,
    CustomerBehavior,
    CustomerSatisfaction,
    ChurnAnalysis,
    
    // Product Analytics
    ProductPerformance,
    ProductProfitability,
    ProductUsage,
    CrossSelling,
    
    // Risk Analytics
    CreditRisk,
    MarketRisk,
    OperationalRisk,
    LiquidityRisk,
    ConcentrationRisk,
    
    // Operational Analytics
    ProcessEfficiency,
    ChannelPerformance,
    BranchPerformance,
    StaffProductivity,
    
    // Compliance Analytics
    AMLAnalytics,
    FraudAnalytics,
    RegulatoryCompliance,
    
    // Market Analytics
    CompetitiveAnalysis,
    MarketShare,
    TrendAnalysis,
    
    // General Analytics
    BusinessIntelligence,
    PerformanceAnalytics,
    BenchmarkAnalysis
}

/// <summary>
/// Analytics Status - Status of analytics computations
/// </summary>
public enum AnalyticsStatus
{
    Pending,
    Computing,
    Completed,
    Failed,
    Stale,
    Refreshing,
    Cancelled
}

/// <summary>
/// KPI Trend - Trend direction for KPIs
/// </summary>
public enum KPITrend
{
    Improving,
    Stable,
    Declining,
    Volatile,
    Unknown
}

/// <summary>
/// KPI Status - Performance status of KPIs
/// </summary>
public enum KPIStatus
{
    Excellent,
    OnTrack,
    AtRisk,
    OffTrack,
    Critical,
    Unknown
}

/// <summary>
/// Insight Type - Types of analytical insights
/// </summary>
public enum InsightType
{
    Trend,
    Anomaly,
    Pattern,
    Correlation,
    Prediction,
    Recommendation,
    Alert,
    Opportunity,
    Risk,
    Performance
}

/// <summary>
/// Insight Severity - Severity levels for insights
/// </summary>
public enum InsightSeverity
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Frequency - Reporting and refresh frequencies
/// </summary>
public enum Frequency
{
    RealTime,
    Hourly,
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Annually,
    OnDemand,
    Custom
}

/// <summary>
/// Aggregation Type - Data aggregation methods
/// </summary>
public enum AggregationType
{
    Sum,
    Average,
    Count,
    Min,
    Max,
    Median,
    StandardDeviation,
    Variance,
    Percentile,
    Custom
}

/// <summary>
/// Time Period - Standard time periods for reporting
/// </summary>
public enum TimePeriod
{
    Today,
    Yesterday,
    ThisWeek,
    LastWeek,
    ThisMonth,
    LastMonth,
    ThisQuarter,
    LastQuarter,
    ThisYear,
    LastYear,
    YearToDate,
    MonthToDate,
    QuarterToDate,
    Last7Days,
    Last30Days,
    Last90Days,
    Last365Days,
    Custom
}

/// <summary>
/// Data Source Type - Types of data sources
/// </summary>
public enum DataSourceType
{
    Database,
    API,
    File,
    Stream,
    Cache,
    External,
    Calculated,
    Manual
}

/// <summary>
/// Export Format - Export formats for data
/// </summary>
public enum ExportFormat
{
    Excel,
    CSV,
    PDF,
    JSON,
    XML,
    PowerBI,
    Tableau,
    QlikView
}
