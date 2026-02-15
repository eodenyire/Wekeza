using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.ValueObjects;

/// <summary>
/// KPI Metric Value Object - Represents Key Performance Indicators
/// Immutable value object for tracking business metrics and performance
/// Industry Standard: Finacle KPI Framework & T24 Performance Metrics
/// </summary>
public class KPIMetric : ValueObject
{
    public string MetricCode { get; private set; } = string.Empty;
    public string MetricName { get; private set; } = string.Empty;
    public decimal CurrentValue { get; private set; }
    public decimal TargetValue { get; private set; }
    public decimal PreviousValue { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public KPITrend Trend { get; private set; }
    public decimal VariancePercentage { get; private set; }
    public KPIStatus Status { get; private set; }
    public DateTime MeasurementDate { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public int Priority { get; private set; }

    // Parameterless constructor for EF Core
    private KPIMetric() { }

    public KPIMetric(
        string metricCode,
        string metricName,
        decimal currentValue,
        decimal targetValue,
        decimal previousValue = 0,
        string unit = null,
        DateTime? measurementDate = null,
        string category = null,
        int priority = 1)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(metricCode))
            throw new ArgumentException("Metric code cannot be empty", nameof(metricCode));
        
        if (string.IsNullOrWhiteSpace(metricName))
            throw new ArgumentException("Metric name cannot be empty", nameof(metricName));
        
        if (priority < 1 || priority > 5)
            throw new ArgumentException("Priority must be between 1 and 5", nameof(priority));

        MetricCode = metricCode;
        MetricName = metricName;
        CurrentValue = currentValue;
        TargetValue = targetValue;
        PreviousValue = previousValue;
        Unit = unit ?? string.Empty;
        MeasurementDate = measurementDate ?? DateTime.UtcNow;
        Category = category ?? "General";
        Priority = priority;
        
        // Calculate derived properties
        Trend = CalculateTrend();
        VariancePercentage = CalculateVariance();
        Status = CalculateStatus();
    }

    /// <summary>
    /// Calculate trend based on current vs previous value
    /// </summary>
    public KPITrend CalculateTrend()
    {
        if (PreviousValue == 0)
            return KPITrend.Stable;

        var changePercentage = Math.Abs((CurrentValue - PreviousValue) / PreviousValue * 100);
        
        // Consider changes less than 2% as stable
        if (changePercentage < 2)
            return KPITrend.Stable;

        // Determine if improvement depends on metric type
        // For most metrics, higher is better, but some metrics (like NPL ratio) lower is better
        var isImproving = IsImprovingMetric() ? CurrentValue > PreviousValue : CurrentValue < PreviousValue;
        
        // Check for volatility (large swings)
        if (changePercentage > 50)
            return KPITrend.Volatile;

        return isImproving ? KPITrend.Improving : KPITrend.Declining;
    }

    /// <summary>
    /// Calculate variance percentage from target
    /// </summary>
    public decimal CalculateVariance()
    {
        if (TargetValue == 0)
            return 0;

        return Math.Round(((CurrentValue - TargetValue) / TargetValue) * 100, 2);
    }

    /// <summary>
    /// Calculate KPI status based on performance against target
    /// </summary>
    public KPIStatus CalculateStatus()
    {
        var variance = Math.Abs(VariancePercentage);
        
        // Within 5% of target is considered on track
        if (variance <= 5)
            return KPIStatus.OnTrack;
        
        // Within 15% of target but trending in right direction
        if (variance <= 15 && Trend == KPITrend.Improving)
            return KPIStatus.AtRisk;
        
        // More than 15% off target or declining trend
        if (variance > 15 || Trend == KPITrend.Declining)
            return KPIStatus.OffTrack;
        
        // Volatile metrics need attention
        if (Trend == KPITrend.Volatile)
            return KPIStatus.AtRisk;

        return KPIStatus.OnTrack;
    }

    /// <summary>
    /// Get performance rating (1-5 stars)
    /// </summary>
    public int GetPerformanceRating()
    {
        return Status switch
        {
            KPIStatus.Excellent => 5,
            KPIStatus.OnTrack => 4,
            KPIStatus.AtRisk => 3,
            KPIStatus.OffTrack => 2,
            KPIStatus.Critical => 1,
            _ => 3
        };
    }

    /// <summary>
    /// Get variance from previous period
    /// </summary>
    public decimal GetPeriodVariance()
    {
        if (PreviousValue == 0)
            return 0;

        return Math.Round(((CurrentValue - PreviousValue) / PreviousValue) * 100, 2);
    }

    /// <summary>
    /// Check if metric is achieving target
    /// </summary>
    public bool IsAchievingTarget()
    {
        return Status == KPIStatus.OnTrack || Status == KPIStatus.Excellent;
    }

    /// <summary>
    /// Check if metric needs attention
    /// </summary>
    public bool NeedsAttention()
    {
        return Status == KPIStatus.AtRisk || Status == KPIStatus.OffTrack || Status == KPIStatus.Critical;
    }

    /// <summary>
    /// Get formatted value with unit
    /// </summary>
    public string GetFormattedValue()
    {
        var formattedValue = FormatValue(CurrentValue);
        return string.IsNullOrEmpty(Unit) ? formattedValue : $"{formattedValue} {Unit}";
    }

    /// <summary>
    /// Get formatted target with unit
    /// </summary>
    public string GetFormattedTarget()
    {
        var formattedValue = FormatValue(TargetValue);
        return string.IsNullOrEmpty(Unit) ? formattedValue : $"{formattedValue} {Unit}";
    }

    /// <summary>
    /// Get trend indicator symbol
    /// </summary>
    public string GetTrendIndicator()
    {
        return Trend switch
        {
            KPITrend.Improving => "↗",
            KPITrend.Declining => "↘",
            KPITrend.Stable => "→",
            KPITrend.Volatile => "↕",
            _ => "→"
        };
    }

    /// <summary>
    /// Get status color for UI display
    /// </summary>
    public string GetStatusColor()
    {
        return Status switch
        {
            KPIStatus.Excellent => "#28a745", // Green
            KPIStatus.OnTrack => "#20c997",   // Teal
            KPIStatus.AtRisk => "#ffc107",    // Yellow
            KPIStatus.OffTrack => "#fd7e14",  // Orange
            KPIStatus.Critical => "#dc3545",  // Red
            _ => "#6c757d"                    // Gray
        };
    }

    /// <summary>
    /// Create a copy with updated current value
    /// </summary>
    public KPIMetric WithCurrentValue(decimal newCurrentValue, DateTime? newMeasurementDate = null)
    {
        return new KPIMetric(
            MetricCode,
            MetricName,
            newCurrentValue,
            TargetValue,
            CurrentValue, // Previous value becomes current
            Unit,
            newMeasurementDate ?? DateTime.UtcNow,
            Category,
            Priority);
    }

    /// <summary>
    /// Create a copy with updated target value
    /// </summary>
    public KPIMetric WithTargetValue(decimal newTargetValue)
    {
        return new KPIMetric(
            MetricCode,
            MetricName,
            CurrentValue,
            newTargetValue,
            PreviousValue,
            Unit,
            MeasurementDate,
            Category,
            Priority);
    }

    /// <summary>
    /// Determine if this is an "improving" metric (higher is better)
    /// </summary>
    private bool IsImprovingMetric()
    {
        // Metrics where lower values are better
        var lowerIsBetterMetrics = new[]
        {
            "NPL", "NPLRATIO", "COSTINCOME", "OPERATIONALRISK", "CREDITLOSS",
            "DEFAULTRATE", "CHURNRATE", "ERRORRATE", "PROCESSINGTIME",
            "COMPLAINTRATE", "FRAUDRATE", "RISKEXPOSURE"
        };

        var upperMetricCode = MetricCode.ToUpperInvariant();
        return !lowerIsBetterMetrics.Any(metric => upperMetricCode.Contains(metric));
    }

    /// <summary>
    /// Format numeric value for display
    /// </summary>
    private string FormatValue(decimal value)
    {
        // Format based on magnitude
        if (Math.Abs(value) >= 1_000_000_000)
            return $"{value / 1_000_000_000:F1}B";
        
        if (Math.Abs(value) >= 1_000_000)
            return $"{value / 1_000_000:F1}M";
        
        if (Math.Abs(value) >= 1_000)
            return $"{value / 1_000:F1}K";
        
        // For percentages and ratios
        if (Unit?.Contains("%") == true || Unit?.Contains("ratio") == true)
            return $"{value:F2}";
        
        // For currency
        if (Unit?.Contains("KES") == true || Unit?.Contains("USD") == true)
            return $"{value:N0}";
        
        return $"{value:F2}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MetricCode;
        yield return MetricName;
        yield return CurrentValue;
        yield return TargetValue;
        yield return PreviousValue;
        yield return Unit;
        yield return MeasurementDate.Date; // Compare only date part
        yield return Category;
        yield return Priority;
    }

    public override string ToString()
    {
        return $"{MetricName}: {GetFormattedValue()} (Target: {GetFormattedTarget()}) [{Status}] {GetTrendIndicator()}";
    }
}