using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// MIS Report aggregate - Management Information System reports for internal decision making
/// Provides executive dashboards, performance metrics, and business intelligence
/// </summary>
public class MISReport : AggregateRoot
{
    public string ReportCode { get; private set; }
    public string ReportName { get; private set; }
    public MISReportType ReportType { get; private set; }
    public string Department { get; private set; }
    public ReportFrequency Frequency { get; private set; }
    public DateTime ReportingPeriodStart { get; private set; }
    public DateTime ReportingPeriodEnd { get; private set; }
    public ReportStatus Status { get; private set; }
    public string ReportData { get; private set; } // JSON data
    public string? ReportFilePath { get; private set; }
    public int RecordCount { get; private set; }
    public Money? TotalAmount { get; private set; }
    public string GeneratedBy { get; private set; }
    public DateTime GeneratedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? Comments { get; private set; }
    public bool IsExecutiveReport { get; private set; }
    public int Priority { get; private set; } // 1 = High, 2 = Medium, 3 = Low

    private readonly List<MISReportMetric> _metrics = new();
    public IReadOnlyList<MISReportMetric> Metrics => _metrics.AsReadOnly();

    private readonly List<MISReportChart> _charts = new();
    public IReadOnlyList<MISReportChart> Charts => _charts.AsReadOnly();

    private MISReport() : base(Guid.NewGuid()) { } // EF Core

    public MISReport(
        Guid id,
        string reportCode,
        string reportName,
        MISReportType reportType,
        string department,
        ReportFrequency frequency,
        DateTime reportingPeriodStart,
        DateTime reportingPeriodEnd,
        bool isExecutiveReport,
        int priority,
        string generatedBy) : base(id) {
        Id = id;
        ReportCode = reportCode;
        ReportName = reportName;
        ReportType = reportType;
        Department = department;
        Frequency = frequency;
        ReportingPeriodStart = reportingPeriodStart;
        ReportingPeriodEnd = reportingPeriodEnd;
        IsExecutiveReport = isExecutiveReport;
        Priority = priority;
        GeneratedBy = generatedBy;
        Status = ReportStatus.Draft;
        ReportData = "{}";
        RecordCount = 0;
        GeneratedAt = DateTime.UtcNow;

        AddDomainEvent(new MISReportCreatedDomainEvent(Id, ReportCode, ReportType, Department));
    }

    public void AddMetric(string metricName, string category, decimal value, string unit, string? target = null)
    {
        if (Status != ReportStatus.Draft)
            throw new InvalidOperationException("Cannot modify report metrics after generation");

        var metric = new MISReportMetric(
            Guid.NewGuid(),
            Id,
            metricName,
            category,
            value,
            unit,
            target,
            DateTime.UtcNow);

        _metrics.Add(metric);
        RecordCount = _metrics.Count + _charts.Count;
    }

    public void AddChart(string chartName, string chartType, string dataSource, string configuration)
    {
        if (Status != ReportStatus.Draft)
            throw new InvalidOperationException("Cannot modify report charts after generation");

        var chart = new MISReportChart(
            Guid.NewGuid(),
            Id,
            chartName,
            chartType,
            dataSource,
            configuration,
            DateTime.UtcNow);

        _charts.Add(chart);
        RecordCount = _metrics.Count + _charts.Count;
    }

    public void GenerateReport(string reportData)
    {
        if (Status != ReportStatus.Draft)
            throw new InvalidOperationException("Report has already been generated");

        ReportData = reportData;
        Status = ReportStatus.Generated;
        GeneratedAt = DateTime.UtcNow;

        // Calculate total amount from metrics
        var totalValue = _metrics.Sum(m => m.Value);
        TotalAmount = new Money(totalValue, Currency.KES);

        AddDomainEvent(new MISReportGeneratedDomainEvent(Id, ReportCode, RecordCount, TotalAmount));
    }

    public void ValidateReport()
    {
        if (Status != ReportStatus.Generated)
            throw new InvalidOperationException("Report must be generated before validation");

        var errors = new List<string>();

        // Basic validation rules
        if (RecordCount == 0)
            errors.Add("Report contains no data");

        if (string.IsNullOrEmpty(ReportData) || ReportData == "{}")
            errors.Add("Report data is empty");

        if (ReportingPeriodStart >= ReportingPeriodEnd)
            errors.Add("Invalid reporting period");

        // Executive report specific validations
        if (IsExecutiveReport && _charts.Count == 0)
            errors.Add("Executive reports must contain at least one chart");

        // Department-specific validations
        ValidateDepartmentSpecificRules(errors);

        if (errors.Any())
        {
            Status = ReportStatus.ValidationFailed;
            Comments = string.Join("; ", errors);
            AddDomainEvent(new MISReportValidationFailedDomainEvent(Id, ReportCode, Comments));
        }
        else
        {
            Status = ReportStatus.Validated;
            AddDomainEvent(new MISReportValidatedDomainEvent(Id, ReportCode));
        }
    }

    private void ValidateDepartmentSpecificRules(List<string> errors)
    {
        switch (Department.ToUpper())
        {
            case "FINANCE":
                if (ReportType == MISReportType.FinancialPerformance && TotalAmount?.Amount == 0)
                    errors.Add("Financial performance reports must have non-zero amounts");
                break;
            case "RISK":
                if (ReportType == MISReportType.RiskAnalysis && _metrics.Count < 3)
                    errors.Add("Risk analysis reports must have at least 3 metrics");
                break;
            case "OPERATIONS":
                if (ReportType == MISReportType.OperationalMetrics && _metrics.Count == 0)
                    errors.Add("Operational reports must contain metrics");
                break;
        }
    }

    public void ApproveReport(string approvedBy, string? comments = null)
    {
        if (Status != ReportStatus.Validated)
            throw new InvalidOperationException("Report must be validated before approval");

        Status = ReportStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
        Comments = comments;

        AddDomainEvent(new MISReportApprovedDomainEvent(Id, ReportCode, approvedBy));
    }

    public void PublishReport(string publishedBy, string? filePath = null)
    {
        if (Status != ReportStatus.Approved)
            throw new InvalidOperationException("Report must be approved before publishing");

        Status = ReportStatus.Submitted; // Reusing enum value for published
        ReportFilePath = filePath;
        
        AddDomainEvent(new MISReportPublishedDomainEvent(Id, ReportCode, publishedBy, DateTime.UtcNow));
    }

    public void RejectReport(string reason, string rejectedBy)
    {
        if (Status == ReportStatus.Submitted)
            throw new InvalidOperationException("Cannot reject published report");

        Status = ReportStatus.Rejected;
        Comments = $"Rejected by {rejectedBy}: {reason}";

        AddDomainEvent(new MISReportRejectedDomainEvent(Id, ReportCode, reason, rejectedBy));
    }

    public void RegenerateReport(string regeneratedBy)
    {
        if (Status == ReportStatus.Submitted)
            throw new InvalidOperationException("Cannot regenerate published report");

        Status = ReportStatus.Draft;
        Comments = null;
        ApprovedBy = null;
        ApprovedAt = null;
        GeneratedBy = regeneratedBy;
        GeneratedAt = DateTime.UtcNow;

        AddDomainEvent(new MISReportRegeneratedDomainEvent(Id, ReportCode, regeneratedBy));
    }

    public decimal GetMetricValue(string metricName)
    {
        var metric = _metrics.FirstOrDefault(m => m.MetricName.Equals(metricName, StringComparison.OrdinalIgnoreCase));
        return metric?.Value ?? 0;
    }

    public IEnumerable<MISReportMetric> GetMetricsByCategory(string category)
    {
        return _metrics.Where(m => m.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasChart(string chartName)
    {
        return _charts.Any(c => c.ChartName.Equals(chartName, StringComparison.OrdinalIgnoreCase));
    }

    public MISReportChart? GetChart(string chartName)
    {
        return _charts.FirstOrDefault(c => c.ChartName.Equals(chartName, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Individual metric within an MIS report
/// </summary>
public class MISReportMetric
{
    public Guid Id { get; private set; }
    public Guid ReportId { get; private set; }
    public string MetricName { get; private set; }
    public string Category { get; private set; }
    public decimal Value { get; private set; }
    public string Unit { get; private set; }
    public string? Target { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private MISReportMetric() { Id = Guid.NewGuid(); } // EF Core

    public MISReportMetric(Guid id, Guid reportId, string metricName, string category, 
        decimal value, string unit, string? target, DateTime createdAt)
    {
        Id = id;
        ReportId = reportId;
        MetricName = metricName;
        Category = category;
        Value = value;
        Unit = unit;
        Target = target;
        CreatedAt = createdAt;
    }

    public bool IsTargetMet()
    {
        if (string.IsNullOrEmpty(Target) || !decimal.TryParse(Target, out var targetValue))
            return false;

        return Value >= targetValue;
    }

    public decimal GetVarianceFromTarget()
    {
        if (string.IsNullOrEmpty(Target) || !decimal.TryParse(Target, out var targetValue))
            return 0;

        return Value - targetValue;
    }

    public decimal GetVariancePercentage()
    {
        if (string.IsNullOrEmpty(Target) || !decimal.TryParse(Target, out var targetValue) || targetValue == 0)
            return 0;

        return ((Value - targetValue) / targetValue) * 100;
    }
}

/// <summary>
/// Chart configuration within an MIS report
/// </summary>
public class MISReportChart
{
    public Guid Id { get; private set; }
    public Guid ReportId { get; private set; }
    public string ChartName { get; private set; }
    public string ChartType { get; private set; } // Bar, Line, Pie, etc.
    public string DataSource { get; private set; }
    public string Configuration { get; private set; } // JSON configuration
    public DateTime CreatedAt { get; private set; }

    private MISReportChart() { Id = Guid.NewGuid(); } // EF Core

    public MISReportChart(Guid id, Guid reportId, string chartName, string chartType, 
        string dataSource, string configuration, DateTime createdAt)
    {
        Id = id;
        ReportId = reportId;
        ChartName = chartName;
        ChartType = chartType;
        DataSource = dataSource;
        Configuration = configuration;
        CreatedAt = createdAt;
    }
}

// Enums
public enum MISReportType
{
    ExecutiveDashboard = 1,
    FinancialPerformance = 2,
    OperationalMetrics = 3,
    CustomerAnalytics = 4,
    RiskAnalysis = 5,
    ProductPerformance = 6,
    BranchPerformance = 7,
    ChannelAnalytics = 8,
    ComplianceMetrics = 9,
    ProfitabilityAnalysis = 10
}

// Domain Events
public record MISReportCreatedDomainEvent(
    Guid ReportId,
    string ReportCode,
    MISReportType ReportType,
    string Department) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record MISReportGeneratedDomainEvent(
    Guid ReportId,
    string ReportCode,
    int RecordCount,
    Money? TotalAmount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record MISReportValidatedDomainEvent(
    Guid ReportId,
    string ReportCode) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record MISReportValidationFailedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string ValidationErrors) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record MISReportApprovedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string ApprovedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record MISReportPublishedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string PublishedBy,
    DateTime PublishedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record MISReportRejectedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string Reason,
    string RejectedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record MISReportRegeneratedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string RegeneratedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

