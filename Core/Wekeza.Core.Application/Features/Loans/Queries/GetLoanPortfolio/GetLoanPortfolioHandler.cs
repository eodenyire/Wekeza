using MediatR;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Loans.Queries.GetLoanPortfolio;

/// <summary>
/// Get Loan Portfolio Handler - Retrieves loan portfolio with analytics
/// Provides comprehensive portfolio view with risk and performance metrics
/// </summary>
public class GetLoanPortfolioHandler : IRequestHandler<GetLoanPortfolioQuery, LoanPortfolioDto>
{
    private readonly ILoanRepository _loanRepository;

    public GetLoanPortfolioHandler(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<LoanPortfolioDto> Handle(GetLoanPortfolioQuery request, CancellationToken cancellationToken)
    {
        // Get filtered loans
        var loans = await _loanRepository.SearchLoansAsync(
            searchTerm: request.SearchTerm,
            status: request.Status,
            riskGrade: request.RiskGrade,
            fromDate: request.FromDate,
            toDate: request.ToDate,
            pageSize: request.PageSize,
            pageNumber: request.PageNumber,
            ct: cancellationToken);

        var loanList = loans.ToList();

        // Map to summary DTOs
        var loanSummaries = loanList.Select(loan => new LoanSummaryDto
        {
            Id = loan.Id,
            LoanNumber = loan.LoanNumber,
            CustomerId = loan.CustomerId,
            CustomerName = loan.Customer?.FullName ?? "Unknown",
            ProductName = loan.Product?.Name ?? "Unknown",
            PrincipalAmount = loan.Principal.Amount,
            OutstandingPrincipal = loan.OutstandingPrincipal.Amount,
            Currency = loan.Principal.Currency.Code,
            InterestRate = loan.InterestRate,
            Status = loan.Status.ToString(),
            SubStatus = loan.SubStatus.ToString(),
            RiskGrade = loan.RiskGrade?.ToString(),
            ApplicationDate = loan.ApplicationDate,
            DisbursementDate = loan.DisbursementDate,
            MaturityDate = loan.MaturityDate,
            DaysPastDue = loan.DaysPastDue,
            PastDueAmount = loan.PastDueAmount.Amount,
            ProvisionAmount = loan.ProvisionAmount.Amount
        }).ToList();

        // Calculate pagination info
        var totalCount = loanList.Count; // This would need to be from a separate count query in real implementation
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var result = new LoanPortfolioDto
        {
            Loans = loanSummaries,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages
        };

        // Add analytics if requested
        if (request.IncludeAnalytics)
        {
            result = result with { Analytics = await CalculatePortfolioAnalyticsAsync(cancellationToken) };
        }

        // Add risk breakdown if requested
        if (request.IncludeRiskBreakdown)
        {
            result = result with { RiskBreakdown = await CalculateRiskBreakdownAsync(cancellationToken) };
        }

        // Add status breakdown if requested
        if (request.IncludeStatusBreakdown)
        {
            result = result with { StatusBreakdown = await CalculateStatusBreakdownAsync(cancellationToken) };
        }

        return result;
    }

    private async Task<LoanPortfolioAnalyticsDto> CalculatePortfolioAnalyticsAsync(CancellationToken cancellationToken)
    {
        // Get all active loans for analytics
        var activeLoans = await _loanRepository.GetByStatusAsync(LoanStatus.Active, cancellationToken);
        var activeLoansList = activeLoans.ToList();

        var totalOutstanding = await _loanRepository.GetTotalOutstandingPrincipalAsync(cancellationToken);
        var totalProvisions = await _loanRepository.GetTotalProvisionAmountAsync(cancellationToken);
        
        var pastDueLoans = await _loanRepository.GetPastDueLoansAsync(1, cancellationToken);
        var pastDueLoansList = pastDueLoans.ToList();
        
        var nonPerformingLoans = await _loanRepository.GetNonPerformingLoansAsync(cancellationToken);
        var nplList = nonPerformingLoans.ToList();

        var totalInterestPaid = activeLoansList.Sum(l => l.TotalInterestPaid.Amount);
        var averageInterestRate = activeLoansList.Any() ? activeLoansList.Average(l => l.InterestRate) : 0;

        // Calculate PAR (Portfolio at Risk) rate
        var pastDueAmount = pastDueLoansList.Sum(l => l.OutstandingPrincipal.Amount);
        var parRate = totalOutstanding > 0 ? (pastDueAmount / totalOutstanding) * 100 : 0;

        // Calculate NPL rate
        var nplAmount = nplList.Sum(l => l.OutstandingPrincipal.Amount);
        var nplRate = totalOutstanding > 0 ? (nplAmount / totalOutstanding) * 100 : 0;

        return new LoanPortfolioAnalyticsDto
        {
            TotalLoans = activeLoansList.Count,
            TotalPrincipal = activeLoansList.Sum(l => l.Principal.Amount),
            TotalOutstanding = totalOutstanding,
            TotalProvisions = totalProvisions,
            TotalInterestPaid = totalInterestPaid,
            AverageInterestRate = averageInterestRate,
            PortfolioAtRiskRate = parRate,
            NonPerformingLoanRate = nplRate,
            ActiveLoans = activeLoansList.Count,
            PastDueLoans = pastDueLoansList.Count,
            NonPerformingLoans = nplList.Count
        };
    }

    private async Task<List<RiskGradeBreakdownDto>> CalculateRiskBreakdownAsync(CancellationToken cancellationToken)
    {
        var breakdown = new List<RiskGradeBreakdownDto>();
        var totalOutstanding = await _loanRepository.GetTotalOutstandingPrincipalAsync(cancellationToken);

        // Get breakdown for each risk grade
        foreach (CreditRiskGrade riskGrade in Enum.GetValues<CreditRiskGrade>())
        {
            var loans = await _loanRepository.GetByRiskGradeAsync(riskGrade, cancellationToken);
            var loansList = loans.ToList();

            if (loansList.Any())
            {
                var gradeOutstanding = loansList.Sum(l => l.OutstandingPrincipal.Amount);
                var percentage = totalOutstanding > 0 ? (gradeOutstanding / totalOutstanding) * 100 : 0;

                breakdown.Add(new RiskGradeBreakdownDto
                {
                    RiskGrade = riskGrade.ToString(),
                    Count = loansList.Count,
                    TotalOutstanding = gradeOutstanding,
                    Percentage = percentage
                });
            }
        }

        return breakdown.OrderBy(b => b.RiskGrade).ToList();
    }

    private async Task<List<StatusBreakdownDto>> CalculateStatusBreakdownAsync(CancellationToken cancellationToken)
    {
        var breakdown = new List<StatusBreakdownDto>();
        var totalOutstanding = await _loanRepository.GetTotalOutstandingPrincipalAsync(cancellationToken);

        // Get breakdown for each status
        foreach (LoanStatus status in Enum.GetValues<LoanStatus>())
        {
            var count = await _loanRepository.GetLoanCountByStatusAsync(status, cancellationToken);
            
            if (count > 0)
            {
                var loans = await _loanRepository.GetByStatusAsync(status, cancellationToken);
                var statusOutstanding = loans.Sum(l => l.OutstandingPrincipal.Amount);
                var percentage = totalOutstanding > 0 ? (statusOutstanding / totalOutstanding) * 100 : 0;

                breakdown.Add(new StatusBreakdownDto
                {
                    Status = status.ToString(),
                    Count = count,
                    TotalOutstanding = statusOutstanding,
                    Percentage = percentage
                });
            }
        }

        return breakdown.OrderBy(b => b.Status).ToList();
    }
}