using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Reporting.Queries.GetRegulatoryReports;

/// <summary>
/// Query to get regulatory reports with filtering options
/// </summary>
public record GetRegulatoryReportsQuery : IRequest<Result<IEnumerable<RegulatoryReportDto>>>
{
    public RegulatoryAuthority? Authority { get; init; }
    public ReportStatus? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public bool OverdueOnly { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

/// <summary>
/// DTO for regulatory report information
/// </summary>
public record RegulatoryReportDto
{
    public Guid Id { get; init; }
    public string ReportCode { get; init; } = string.Empty;
    public string ReportName { get; init; } = string.Empty;
    public string Authority { get; init; } = string.Empty;
    public string ReportType { get; init; } = string.Empty;
    public string Frequency { get; init; } = string.Empty;
    public DateTime ReportingPeriodStart { get; init; }
    public DateTime ReportingPeriodEnd { get; init; }
    public DateTime DueDate { get; init; }
    public DateTime? SubmissionDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public int RecordCount { get; init; }
    public decimal? TotalAmount { get; init; }
    public string? Currency { get; init; }
    public string GeneratedBy { get; init; } = string.Empty;
    public DateTime GeneratedAt { get; init; }
    public string? SubmissionReference { get; init; }
    public bool IsOverdue { get; init; }
    public int DaysUntilDue { get; init; }
}