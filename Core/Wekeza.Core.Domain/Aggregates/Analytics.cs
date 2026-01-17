using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Analytics Aggregate - Manages business analytics, insights, and forecasting
/// Supports descriptive, diagnostic, predictive, and prescriptive analytics
/// Industry Standard: Finacle Analytics & T24 Business Intelligence
/// </summary>
public class Analytics : AggregateRoot<Guid>
{
    // Core Properties
    public string AnalyticsCode { get; private set; }
    public string AnalyticsName { get; private set; }
    public string Description { get; private set; }
    public AnalyticsType Type { get; private set; }
    public AnalyticsCategory Category { get; private set; }
    public AnalyticsStatus Status { get; private set; }
    
    // Data & Time Period
    public DateTime AnalysisPeriodStart { get; private set; }
    public DateTime AnalysisPeriodEnd { get; private set; }
    public string DataSource { get; private set; }
    public Dictionary<string, object> DataFilters { get; private set; }
    
    // Metrics & KPIs
    public Dictionary<string, decimal> Metrics { get; private set; }
    public Dictionary<string, object> Dimensions { get; private set; }
    public List<KPIMetric> KPIs { get; private set; }
    public List<AnalyticsInsight> Insights { get; private set; }
    
    // Computation Details
    public DateTime ComputedAt { get; private set; }
    public string ComputedBy { get; private set; }
    public Dictionary<string, object> ComputationParameters { get; private set; }
    public TimeSpan ComputationDuration { get; private set; }
    
    // Trends & Forecasting
    public List<TrendData> TrendData { get; private set; }
    public List<ForecastData> Forecasts { get; private set; }
    public decimal ConfidenceLevel { get; private set; }
    
    // Comparison & Benchmarking
    public Dictionary<string, decimal> PreviousPeriodMetrics { get; private set; }
    public Dictionary<string, decimal> BenchmarkMetrics { get; private set; }
    public Dictionary<string, decimal> VarianceMetrics { get; private set; }
    
    // Metadata & Configuration
    public Dictionary<string, object> Metadata { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public bool IsStale { get; private set; }

    // Private constructor for EF Core
    private Analytics() 
    {
        DataFilters = new Dictionary<string, object>();
        Metrics = new Dictionary<string, decimal>();
        Dimensions = new Dictionary<string, object>();
        KPIs = new List<KPIMetric>();
        Insights = new List<AnalyticsInsight>();
        ComputationParameters = new Dictionary<string, object>();
        TrendData = new List<TrendData>();
        Forecasts = new List<ForecastData>();
        PreviousPeriodMetrics = new Dictionary<string, decimal>();
        BenchmarkMetrics = new Dictionary<string, decimal>();
        VarianceMetrics = new Dictionary<string, decimal>();
        Metadata = new Dictionary<string, object>();
    }

    // Factory method for creating new analytics
    public static Analytics Create(
        string analyticsCode,
        string analyticsName,
        AnalyticsType type,
        AnalyticsCategory category,
        DateTime periodStart,
        DateTime periodEnd,
        string computedBy,
        string dataSource = null,
        string description = null)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(analyticsCode))
            throw new ArgumentException("Analytics code cannot be empty", nameof(analyticsCode));
        
        if (string.IsNullOrWhiteSpace(analyticsName))
            throw new ArgumentException("Analytics name cannot be empty", nameof(analyticsName));
        
        if (string.IsNullOrWhiteSpace(computedBy))
            throw new ArgumentException("Computed by cannot be empty", nameof(computedBy));
        
        if (periodStart >= periodEnd)
            throw new ArgumentException("Period start must be before period end");

        var analytics = new Analytics
        {
            Id = Guid.NewGuid(),
            AnalyticsCode = analyticsCode,
            AnalyticsName = analyticsName,
            Description = description,
            Type = type,
            Category = category,
            Status = AnalyticsStatus.Pending,
            AnalysisPeriodStart = periodStart,
            AnalysisPeriodEnd = periodEnd,
            DataSource = dataSource,
            DataFilters = new Dictionary<string, object>(),
            Metrics = new Dictionary<string, decimal>(),
            Dimensions = new Dictionary<string, object>(),
            KPIs = new List<KPIMetric>(),
            Insights = new List<AnalyticsInsight>(),
            ComputedBy = computedBy,
            ComputationParameters = new Dictionary<string, object>(),
            TrendData = new List<TrendData>(),
            Forecasts = new List<ForecastData>(),
            PreviousPeriodMetrics = new Dictionary<string, decimal>(),
            BenchmarkMetrics = new Dictionary<string, decimal>(),
            VarianceMetrics = new Dictionary<string, decimal>(),
            Metadata = new Dictionary<string, object>(),
            ConfidenceLevel = 0.95m,
            IsStale = false
        };

        // Add creation event
        analytics.AddDomainEvent(new AnalyticsCreatedDomainEvent(
            analytics.Id,
            analytics.AnalyticsCode,
            analytics.AnalyticsName,
            analytics.Type,
            analytics.Category,
            analytics.ComputedBy));

        return analytics;
    }

    // Compute analytics metrics
    public void ComputeMetrics(
        Dictionary<string, decimal> metrics,
        Dictionary<string, object> parameters = null,
        TimeSpan? duration = null)
    {
        if (metrics == null || !metrics.Any())
            throw new ArgumentException("Metrics cannot be null or empty", nameof(metrics));

        if (Status != AnalyticsStatus.Pending && Status != AnalyticsStatus.Computing)
            throw new InvalidOperationException($"Cannot compute metrics in status: {Status}");

        Status = AnalyticsStatus.Computing;
        ComputedAt = DateTime.UtcNow;
        Metrics = new Dictionary<string, decimal>(metrics);
        ComputationParameters = parameters ?? new Dictionary<string, object>();
        ComputationDuration = duration ?? TimeSpan.Zero;
        
        // Calculate variance metrics if previous period data exists
        CalculateVarianceMetrics();
        
        // Set expiration (analytics typically valid for 24 hours)
        ExpiresAt = DateTime.UtcNow.AddHours(24);
        
        Status = AnalyticsStatus.Completed;

        // Update metadata
        Metadata["ComputationCompletedAt"] = ComputedAt;
        Metadata["MetricCount"] = Metrics.Count;
        Metadata["ComputationDurationMs"] = ComputationDuration.TotalMilliseconds;

        AddDomainEvent(new AnalyticsComputedDomainEvent(
            Id,
            AnalyticsCode,
            Type,
            Metrics.Count,
            ComputedAt));
    }

    // Add insight to analytics
    public void AddInsight(AnalyticsInsight insight)
    {
        if (insight == null)
            throw new ArgumentNullException(nameof(insight));

        if (Status != AnalyticsStatus.Completed)
            throw new InvalidOperationException($"Cannot add insight to analytics in status: {Status}");

        // Check for duplicate insights
        if (Insights.Any(i => i.InsightCode == insight.InsightCode))
            throw new InvalidOperationException($"Insight with code {insight.InsightCode} already exists");

        Insights.Add(insight);

        // Update metadata
        Metadata["InsightCount"] = Insights.Count;
        Metadata["LastInsightAddedAt"] = DateTime.UtcNow;

        AddDomainEvent(new AnalyticsInsightAddedDomainEvent(
            Id,
            AnalyticsCode,
            insight.InsightCode,
            insight.Type,
            insight.Severity));
    }

    // Update trend data
    public void UpdateTrendData(List<TrendData> trends)
    {
        if (trends == null)
            throw new ArgumentNullException(nameof(trends));

        TrendData = new List<TrendData>(trends);

        // Update metadata
        Metadata["TrendDataCount"] = TrendData.Count;
        Metadata["TrendDataUpdatedAt"] = DateTime.UtcNow;

        AddDomainEvent(new AnalyticsTrendUpdatedDomainEvent(
            Id,
            AnalyticsCode,
            TrendData.Count));
    }

    // Generate forecast
    public void GenerateForecast(List<ForecastData> forecasts, decimal confidenceLevel = 0.95m)
    {
        if (forecasts == null || !forecasts.Any())
            throw new ArgumentException("Forecasts cannot be null or empty", nameof(forecasts));

        if (confidenceLevel <= 0 || confidenceLevel >= 1)
            throw new ArgumentException("Confidence level must be between 0 and 1", nameof(confidenceLevel));

        Forecasts = new List<ForecastData>(forecasts);
        ConfidenceLevel = confidenceLevel;

        // Update metadata
        Metadata["ForecastCount"] = Forecasts.Count;
        Metadata["ForecastGeneratedAt"] = DateTime.UtcNow;
        Metadata["ConfidenceLevel"] = ConfidenceLevel;

        AddDomainEvent(new AnalyticsForecastGeneratedDomainEvent(
            Id,
            AnalyticsCode,
            Forecasts.Count,
            ConfidenceLevel));
    }

    // Add KPI metric
    public void AddKPI(KPIMetric kpi)
    {
        if (kpi == null)
            throw new ArgumentNullException(nameof(kpi));

        // Check for duplicate KPI codes
        if (KPIs.Any(k => k.MetricCode == kpi.MetricCode))
            throw new InvalidOperationException($"KPI with code {kpi.MetricCode} already exists");

        KPIs.Add(kpi);

        // Update metadata
        Metadata["KPICount"] = KPIs.Count;
        Metadata["LastKPIAddedAt"] = DateTime.UtcNow;

        AddDomainEvent(new AnalyticsKPIAddedDomainEvent(
            Id,
            AnalyticsCode,
            kpi.MetricCode,
            kpi.CurrentValue,
            kpi.TargetValue));
    }

    // Set previous period metrics for comparison
    public void SetPreviousPeriodMetrics(Dictionary<string, decimal> previousMetrics)
    {
        if (previousMetrics == null)
            throw new ArgumentNullException(nameof(previousMetrics));

        PreviousPeriodMetrics = new Dictionary<string, decimal>(previousMetrics);
        
        // Recalculate variance metrics
        CalculateVarianceMetrics();

        // Update metadata
        Metadata["PreviousPeriodMetricsCount"] = PreviousPeriodMetrics.Count;
        Metadata["PreviousPeriodMetricsUpdatedAt"] = DateTime.UtcNow;
    }

    // Set benchmark metrics for comparison
    public void SetBenchmarkMetrics(Dictionary<string, decimal> benchmarkMetrics)
    {
        if (benchmarkMetrics == null)
            throw new ArgumentNullException(nameof(benchmarkMetrics));

        BenchmarkMetrics = new Dictionary<string, decimal>(benchmarkMetrics);

        // Update metadata
        Metadata["BenchmarkMetricsCount"] = BenchmarkMetrics.Count;
        Metadata["BenchmarkMetricsUpdatedAt"] = DateTime.UtcNow;
    }

    // Add data filter
    public void AddDataFilter(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Filter key cannot be empty", nameof(key));

        DataFilters[key] = value;
    }

    // Add dimension
    public void AddDimension(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Dimension key cannot be empty", nameof(key));

        Dimensions[key] = value;
    }

    // Mark as stale
    public void MarkAsStale()
    {
        IsStale = true;
        Status = AnalyticsStatus.Stale;

        // Update metadata
        Metadata["MarkedStaleAt"] = DateTime.UtcNow;

        AddDomainEvent(new AnalyticsMarkedStaleDomainEvent(
            Id,
            AnalyticsCode,
            DateTime.UtcNow));
    }

    // Refresh analytics
    public void Refresh(string refreshedBy)
    {
        if (string.IsNullOrWhiteSpace(refreshedBy))
            throw new ArgumentException("Refreshed by cannot be empty", nameof(refreshedBy));

        Status = AnalyticsStatus.Pending;
        IsStale = false;
        ExpiresAt = null;

        // Update metadata
        Metadata["RefreshedAt"] = DateTime.UtcNow;
        Metadata["RefreshedBy"] = refreshedBy;
        Metadata["RefreshCount"] = Metadata.ContainsKey("RefreshCount") 
            ? (int)Metadata["RefreshCount"] + 1 
            : 1;

        AddDomainEvent(new AnalyticsRefreshedDomainEvent(
            Id,
            AnalyticsCode,
            refreshedBy,
            DateTime.UtcNow));
    }

    // Check if analytics is expired
    public bool IsExpired(DateTime currentTime)
    {
        return ExpiresAt.HasValue && currentTime > ExpiresAt.Value;
    }

    // Get metric value
    public decimal GetMetric(string metricCode, decimal defaultValue = 0)
    {
        return Metrics.ContainsKey(metricCode) ? Metrics[metricCode] : defaultValue;
    }

    // Get variance percentage for a metric
    public decimal GetVariancePercentage(string metricCode)
    {
        return VarianceMetrics.ContainsKey(metricCode) ? VarianceMetrics[metricCode] : 0;
    }

    // Get KPI by code
    public KPIMetric GetKPI(string kpiCode)
    {
        return KPIs.FirstOrDefault(k => k.MetricCode == kpiCode);
    }

    // Get insights by type
    public List<AnalyticsInsight> GetInsightsByType(InsightType type)
    {
        return Insights.Where(i => i.Type == type).ToList();
    }

    // Get insights by severity
    public List<AnalyticsInsight> GetInsightsBySeverity(InsightSeverity severity)
    {
        return Insights.Where(i => i.Severity == severity).ToList();
    }

    // Private method to calculate variance metrics
    private void CalculateVarianceMetrics()
    {
        VarianceMetrics.Clear();

        foreach (var metric in Metrics)
        {
            if (PreviousPeriodMetrics.ContainsKey(metric.Key))
            {
                var currentValue = metric.Value;
                var previousValue = PreviousPeriodMetrics[metric.Key];
                
                if (previousValue != 0)
                {
                    var variance = ((currentValue - previousValue) / previousValue) * 100;
                    VarianceMetrics[metric.Key] = Math.Round(variance, 2);
                }
            }
        }
    }
}

/// <summary>
/// Analytics Insight - Represents business insights derived from analytics
/// </summary>
public class AnalyticsInsight
{
    public string InsightCode { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public InsightType Type { get; private set; }
    public InsightSeverity Severity { get; private set; }
    public decimal Impact { get; private set; }
    public string Recommendation { get; private set; }
    public Dictionary<string, object> SupportingData { get; private set; }
    public DateTime GeneratedAt { get; private set; }

    private AnalyticsInsight() 
    {
        SupportingData = new Dictionary<string, object>();
    }

    public static AnalyticsInsight Create(
        string insightCode,
        string title,
        string description,
        InsightType type,
        InsightSeverity severity,
        decimal impact = 0,
        string recommendation = null)
    {
        if (string.IsNullOrWhiteSpace(insightCode))
            throw new ArgumentException("Insight code cannot be empty", nameof(insightCode));
        
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        return new AnalyticsInsight
        {
            InsightCode = insightCode,
            Title = title,
            Description = description,
            Type = type,
            Severity = severity,
            Impact = impact,
            Recommendation = recommendation,
            SupportingData = new Dictionary<string, object>(),
            GeneratedAt = DateTime.UtcNow
        };
    }

    public void AddSupportingData(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty", nameof(key));

        SupportingData[key] = value;
    }
}

/// <summary>
/// Trend Data - Represents time-series data for trend analysis
/// </summary>
public class TrendData
{
    public DateTime Date { get; set; }
    public string MetricCode { get; set; }
    public decimal Value { get; set; }
    public Dictionary<string, object> Attributes { get; set; }

    public TrendData()
    {
        Attributes = new Dictionary<string, object>();
    }
}

/// <summary>
/// Forecast Data - Represents forecasted values
/// </summary>
public class ForecastData
{
    public DateTime Date { get; set; }
    public string MetricCode { get; set; }
    public decimal ForecastedValue { get; set; }
    public decimal LowerBound { get; set; }
    public decimal UpperBound { get; set; }
    public decimal Confidence { get; set; }
    public string Method { get; set; }
    public Dictionary<string, object> Parameters { get; set; }

    public ForecastData()
    {
        Parameters = new Dictionary<string, object>();
    }
}