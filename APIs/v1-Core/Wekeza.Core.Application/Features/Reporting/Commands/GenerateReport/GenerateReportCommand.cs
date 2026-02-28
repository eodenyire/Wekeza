using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Reporting.Commands.GenerateReport;

/// <summary>
/// Command to generate various types of reports
/// Supports regulatory, MIS, operational, and custom reports
/// </summary>
[Authorize(UserRole.BranchManager, UserRole.Administrator, UserRole.ComplianceOfficer, UserRole.Auditor)]
public record GenerateReportCommand : ICommand<Result<ReportResult>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    
    public string ReportType { get; init; } = string.Empty;
    public string ReportCode { get; init; } = string.Empty;
    public string ReportName { get; init; } = string.Empty;
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public string OutputFormat { get; init; } = "PDF"; // PDF, EXCEL, CSV, JSON
    
    // Filters
    public Guid? BranchId { get; init; }
    public List<Guid>? CustomerIds { get; init; }
    public List<Guid>? AccountIds { get; init; }
    public List<string>? ProductCodes { get; init; }
    public Dictionary<string, object> CustomFilters { get; init; } = new();
    
    // Report Configuration
    public bool IncludeCharts { get; init; } = true;
    public bool IncludeSummary { get; init; } = true;
    public bool IncludeDetails { get; init; } = true;
    public string? GroupBy { get; init; }
    public string? SortBy { get; init; }
    public string SortOrder { get; init; } = "ASC";
    
    // Delivery Options
    public bool EmailReport { get; init; } = false;
    public List<string> EmailRecipients { get; init; } = new();
    public bool SaveToRepository { get; init; } = true;
    public string? ReportDescription { get; init; }
}

public record ReportResult
{
    public Guid ReportId { get; init; }
    public string ReportName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime GeneratedAt { get; init; }
    public string OutputFormat { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;
    public string DownloadUrl { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public int RecordCount { get; init; }
    public TimeSpan GenerationTime { get; init; }
    public string Message { get; init; } = string.Empty;
    public Dictionary<string, object> ReportMetadata { get; init; } = new();
}