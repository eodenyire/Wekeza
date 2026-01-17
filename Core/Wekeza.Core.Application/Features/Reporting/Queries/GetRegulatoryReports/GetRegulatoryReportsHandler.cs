using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Reporting.Queries.GetRegulatoryReports;

/// <summary>
/// Handler for getting regulatory reports
/// </summary>
public class GetRegulatoryReportsHandler : IRequestHandler<GetRegulatoryReportsQuery, Result<IEnumerable<RegulatoryReportDto>>>
{
    private readonly IRegulatoryReportRepository _regulatoryReportRepository;

    public GetRegulatoryReportsHandler(IRegulatoryReportRepository regulatoryReportRepository)
    {
        _regulatoryReportRepository = regulatoryReportRepository;
    }

    public async Task<Result<IEnumerable<RegulatoryReportDto>>> Handle(
        GetRegulatoryReportsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Domain.Aggregates.RegulatoryReport> reports;

            // Apply filters
            if (request.OverdueOnly)
            {
                reports = await _regulatoryReportRepository.GetOverdueReportsAsync();
            }
            else if (request.Authority.HasValue)
            {
                reports = await _regulatoryReportRepository.GetByAuthorityAsync(request.Authority.Value);
            }
            else if (request.Status.HasValue)
            {
                reports = await _regulatoryReportRepository.GetByStatusAsync(request.Status.Value);
            }
            else if (request.FromDate.HasValue && request.ToDate.HasValue)
            {
                reports = await _regulatoryReportRepository.GetReportsByDateRangeAsync(
                    request.FromDate.Value, 
                    request.ToDate.Value);
            }
            else
            {
                // Get recent reports if no specific filter
                var endDate = DateTime.UtcNow;
                var startDate = endDate.AddMonths(-3); // Last 3 months
                reports = await _regulatoryReportRepository.GetReportsByDateRangeAsync(startDate, endDate);
            }

            // Apply additional filters
            if (request.Authority.HasValue && !request.OverdueOnly)
            {
                reports = reports.Where(r => r.Authority == request.Authority.Value);
            }

            if (request.Status.HasValue && !request.OverdueOnly)
            {
                reports = reports.Where(r => r.Status == request.Status.Value);
            }

            if (request.FromDate.HasValue)
            {
                reports = reports.Where(r => r.GeneratedAt >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                reports = reports.Where(r => r.GeneratedAt <= request.ToDate.Value);
            }

            // Apply pagination
            var pagedReports = reports
                .OrderByDescending(r => r.GeneratedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize);

            // Map to DTOs
            var reportDtos = pagedReports.Select(r => new RegulatoryReportDto
            {
                Id = r.Id,
                ReportCode = r.ReportCode,
                ReportName = r.ReportName,
                Authority = r.Authority.ToString(),
                ReportType = r.ReportType.ToString(),
                Frequency = r.Frequency.ToString(),
                ReportingPeriodStart = r.ReportingPeriodStart,
                ReportingPeriodEnd = r.ReportingPeriodEnd,
                DueDate = r.DueDate,
                SubmissionDate = r.SubmissionDate,
                Status = r.Status.ToString(),
                RecordCount = r.RecordCount,
                TotalAmount = r.TotalAmount?.Amount,
                Currency = r.TotalAmount?.Currency.Code,
                GeneratedBy = r.GeneratedBy,
                GeneratedAt = r.GeneratedAt,
                SubmissionReference = r.SubmissionReference,
                IsOverdue = r.IsOverdue(),
                DaysUntilDue = r.GetDaysUntilDue()
            });

            return Result<IEnumerable<RegulatoryReportDto>>.Success(reportDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<RegulatoryReportDto>>.Failure($"Failed to retrieve regulatory reports: {ex.Message}");
        }
    }
}