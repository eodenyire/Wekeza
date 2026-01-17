using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Regulatory Report aggregate - Handles all regulatory reporting requirements
/// Supports CBK, Basel III, IFRS, AML, and other regulatory compliance reports
/// </summary>
public class RegulatoryReport : AggregateRoot<Guid>
{
    public string ReportCode { get; private set; }
    public string ReportName { get; private set; }
    public RegulatoryAuthority Authority { get; private set; }
    public ReportType ReportType { get; private set; }
    public ReportFrequency Frequency { get; private set; }
    public DateTime ReportingPeriodStart { get; private set; }
    public DateTime ReportingPeriodEnd { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? SubmissionDate { get; private set; }
    public ReportStatus Status { get; private set; }
    public string ReportData { get; private set; } // JSON data
    public string? ReportFilePath { get; private set; }
    public string? SubmissionReference { get; private set; }
    public string? ValidationErrors { get; private set; }
    public int RecordCount { get; private set; }
    public Money? TotalAmount { get; private set; }
    public string GeneratedBy { get; private set; }
    public DateTime GeneratedAt { get; private set; }
    public string? SubmittedBy { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? Comments { get; private set; }

    private readonly List<ReportDataItem> _dataItems = new();
    public IReadOnlyList<ReportDataItem> DataItems => _dataItems.AsReadOnly();

    private RegulatoryReport() { } // EF Core

    public RegulatoryReport(
        Guid id,
        string reportCode,
        string reportName,
        RegulatoryAuthority authority,
        ReportType reportType,
        ReportFrequency frequency,
        DateTime reportingPeriodStart,
        DateTime reportingPeriodEnd,
        DateTime dueDate,
        string generatedBy)
    {
        Id = id;
        ReportCode = reportCode;
        ReportName = reportName;
        Authority = authority;
        ReportType = reportType;
        Frequency = frequency;
        ReportingPeriodStart = reportingPeriodStart;
        ReportingPeriodEnd = reportingPeriodEnd;
        DueDate = dueDate;
        GeneratedBy = generatedBy;
        Status = ReportStatus.Draft;
        ReportData = "{}";
        RecordCount = 0;
        GeneratedAt = DateTime.UtcNow;

        AddDomainEvent(new RegulatoryReportCreatedDomainEvent(Id, ReportCode, Authority, DueDate));
    }

    public void AddDataItem(string category, string description, decimal value, string currency = "KES")
    {
        if (Status != ReportStatus.Draft)
            throw new InvalidOperationException("Cannot modify report data after generation");

        var dataItem = new ReportDataItem(
            Guid.NewGuid(),
            Id,
            category,
            description,
            new Money(value, new Currency(currency)),
            DateTime.UtcNow);

        _dataItems.Add(dataItem);
        RecordCount = _dataItems.Count;

        // Update total amount
        var totalValue = _dataItems.Sum(x => x.Amount.Amount);
        TotalAmount = new Money(totalValue, new Currency(currency));
    }

    public void GenerateReport(string reportData)
    {
        if (Status != ReportStatus.Draft)
            throw new InvalidOperationException("Report has already been generated");

        ReportData = reportData;
        Status = ReportStatus.Generated;
        GeneratedAt = DateTime.UtcNow;

        AddDomainEvent(new RegulatoryReportGeneratedDomainEvent(Id, ReportCode, RecordCount, TotalAmount));
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

        // Authority-specific validations
        switch (Authority)
        {
            case RegulatoryAuthority.CBK:
                ValidateCBKReport(errors);
                break;
            case RegulatoryAuthority.KRA:
                ValidateKRAReport(errors);
                break;
            case RegulatoryAuthority.CMA:
                ValidateCMAReport(errors);
                break;
        }

        if (errors.Any())
        {
            ValidationErrors = string.Join("; ", errors);
            Status = ReportStatus.ValidationFailed;
            AddDomainEvent(new RegulatoryReportValidationFailedDomainEvent(Id, ReportCode, ValidationErrors));
        }
        else
        {
            ValidationErrors = null;
            Status = ReportStatus.Validated;
            AddDomainEvent(new RegulatoryReportValidatedDomainEvent(Id, ReportCode));
        }
    }

    private void ValidateCBKReport(List<string> errors)
    {
        // CBK-specific validation rules
        if (ReportType == ReportType.PrudentialReturn && TotalAmount?.Amount < 0)
            errors.Add("Prudential returns cannot have negative totals");

        if (ReportType == ReportType.StatutoryReturn && RecordCount > 10000)
            errors.Add("Statutory returns exceed maximum record limit");
    }

    private void ValidateKRAReport(List<string> errors)
    {
        // KRA-specific validation rules
        if (ReportType == ReportType.TaxReturn && TotalAmount == null)
            errors.Add("Tax returns must have total amount");
    }

    private void ValidateCMAReport(List<string> errors)
    {
        // CMA-specific validation rules
        if (ReportType == ReportType.ComplianceReport && string.IsNullOrEmpty(Comments))
            errors.Add("CMA compliance reports require comments");
    }

    public void ApproveReport(string approvedBy, string? comments = null)
    {
        if (Status != ReportStatus.Validated)
            throw new InvalidOperationException("Report must be validated before approval");

        Status = ReportStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
        Comments = comments;

        AddDomainEvent(new RegulatoryReportApprovedDomainEvent(Id, ReportCode, approvedBy));
    }

    public void SubmitReport(string submittedBy, string submissionReference, string? filePath = null)
    {
        if (Status != ReportStatus.Approved)
            throw new InvalidOperationException("Report must be approved before submission");

        if (DateTime.UtcNow > DueDate)
            AddDomainEvent(new RegulatoryReportOverdueDomainEvent(Id, ReportCode, DueDate));

        Status = ReportStatus.Submitted;
        SubmittedBy = submittedBy;
        SubmissionDate = DateTime.UtcNow;
        SubmissionReference = submissionReference;
        ReportFilePath = filePath;

        AddDomainEvent(new RegulatoryReportSubmittedDomainEvent(Id, ReportCode, SubmissionReference, SubmissionDate.Value));
    }

    public void RejectReport(string reason, string rejectedBy)
    {
        if (Status == ReportStatus.Submitted)
            throw new InvalidOperationException("Cannot reject submitted report");

        Status = ReportStatus.Rejected;
        ValidationErrors = reason;
        Comments = $"Rejected by {rejectedBy}: {reason}";

        AddDomainEvent(new RegulatoryReportRejectedDomainEvent(Id, ReportCode, reason, rejectedBy));
    }

    public void RegenerateReport(string regeneratedBy)
    {
        if (Status == ReportStatus.Submitted)
            throw new InvalidOperationException("Cannot regenerate submitted report");

        Status = ReportStatus.Draft;
        ValidationErrors = null;
        Comments = null;
        ApprovedBy = null;
        ApprovedAt = null;
        GeneratedBy = regeneratedBy;
        GeneratedAt = DateTime.UtcNow;

        AddDomainEvent(new RegulatoryReportRegeneratedDomainEvent(Id, ReportCode, regeneratedBy));
    }

    public bool IsOverdue()
    {
        return Status != ReportStatus.Submitted && DateTime.UtcNow > DueDate;
    }

    public int GetDaysUntilDue()
    {
        return (DueDate - DateTime.UtcNow).Days;
    }

    public int GetDaysOverdue()
    {
        if (!IsOverdue()) return 0;
        return (DateTime.UtcNow - DueDate).Days;
    }

    public Money GetTotalByCategory(string category)
    {
        var categoryItems = _dataItems.Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        if (!categoryItems.Any()) return new Money(0, Currency.KES);

        var total = categoryItems.Sum(x => x.Amount.Amount);
        return new Money(total, categoryItems.First().Amount.Currency);
    }
}

/// <summary>
/// Individual data item within a regulatory report
/// </summary>
public class ReportDataItem
{
    public Guid Id { get; private set; }
    public Guid ReportId { get; private set; }
    public string Category { get; private set; }
    public string Description { get; private set; }
    public Money Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ReportDataItem() { } // EF Core

    public ReportDataItem(Guid id, Guid reportId, string category, string description, Money amount, DateTime createdAt)
    {
        Id = id;
        ReportId = reportId;
        Category = category;
        Description = description;
        Amount = amount;
        CreatedAt = createdAt;
    }
}

// Enums
public enum RegulatoryAuthority
{
    CBK = 1,    // Central Bank of Kenya
    KRA = 2,    // Kenya Revenue Authority
    CMA = 3,    // Capital Markets Authority
    IRA = 4,    // Insurance Regulatory Authority
    SASRA = 5,  // Sacco Societies Regulatory Authority
    FRC = 6     // Financial Reporting Centre
}

public enum ReportType
{
    PrudentialReturn = 1,
    StatutoryReturn = 2,
    ComplianceReport = 3,
    TaxReturn = 4,
    AMLReport = 5,
    RiskReport = 6,
    FinancialStatement = 7,
    RegulatoryFiling = 8
}

public enum ReportFrequency
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Quarterly = 4,
    HalfYearly = 5,
    Yearly = 6,
    AdHoc = 7
}

public enum ReportStatus
{
    Draft = 1,
    Generated = 2,
    Validated = 3,
    ValidationFailed = 4,
    Approved = 5,
    Rejected = 6,
    Submitted = 7,
    Acknowledged = 8
}

// Domain Events
public record RegulatoryReportCreatedDomainEvent(
    Guid ReportId,
    string ReportCode,
    RegulatoryAuthority Authority,
    DateTime DueDate) : IDomainEvent;

public record RegulatoryReportGeneratedDomainEvent(
    Guid ReportId,
    string ReportCode,
    int RecordCount,
    Money? TotalAmount) : IDomainEvent;

public record RegulatoryReportValidatedDomainEvent(
    Guid ReportId,
    string ReportCode) : IDomainEvent;

public record RegulatoryReportValidationFailedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string ValidationErrors) : IDomainEvent;

public record RegulatoryReportApprovedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string ApprovedBy) : IDomainEvent;

public record RegulatoryReportSubmittedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string SubmissionReference,
    DateTime SubmissionDate) : IDomainEvent;

public record RegulatoryReportRejectedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string Reason,
    string RejectedBy) : IDomainEvent;

public record RegulatoryReportOverdueDomainEvent(
    Guid ReportId,
    string ReportCode,
    DateTime DueDate) : IDomainEvent;

public record RegulatoryReportRegeneratedDomainEvent(
    Guid ReportId,
    string ReportCode,
    string RegeneratedBy) : IDomainEvent;