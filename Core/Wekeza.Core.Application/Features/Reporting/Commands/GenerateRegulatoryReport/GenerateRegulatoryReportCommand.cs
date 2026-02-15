using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Reporting.Commands.GenerateRegulatoryReport;

/// <summary>
/// Command to generate regulatory reports for compliance authorities
/// </summary>
public record GenerateRegulatoryReportCommand : IRequest<Result<Guid>>
{
    public Guid ReportId { get; init; }
    public string ReportCode { get; init; } = string.Empty;
    public string ReportName { get; init; } = string.Empty;
    public RegulatoryAuthority Authority { get; init; }
    public ReportType ReportType { get; init; }
    public ReportFrequency Frequency { get; init; }
    public DateTime ReportingPeriodStart { get; init; }
    public DateTime ReportingPeriodEnd { get; init; }
    public DateTime DueDate { get; init; }
    public string GeneratedBy { get; init; } = string.Empty;
    public bool AutoSubmit { get; init; } = false;
    public string? BranchCode { get; init; }
    public string? ProductCode { get; init; }
}