using MediatR;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Loans.Queries.GetLoanPortfolio;

/// <summary>
/// Get Loan Portfolio Query - Portfolio analytics and loan listing
/// Returns loan portfolio with filtering, pagination, and analytics
/// </summary>
public record GetLoanPortfolioQuery : IRequest<LoanPortfolioDto>
{
    // Filtering parameters
    public Guid? CustomerId { get; init; }
    public LoanStatus? Status { get; init; }
    public LoanSubStatus? SubStatus { get; init; }
    public CreditRiskGrade? RiskGrade { get; init; }
    public ProductType? ProductType { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? SearchTerm { get; init; }
    
    // Pagination
    public int PageSize { get; init; } = 50;
    public int PageNumber { get; init; } = 1;
    
    // Analytics flags
    public bool IncludeAnalytics { get; init; } = true;
    public bool IncludeRiskBreakdown { get; init; } = true;
    public bool IncludeStatusBreakdown { get; init; } = true;
}

public record LoanPortfolioDto
{
    public List<LoanSummaryDto> Loans { get; init; } = new();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    
    // Portfolio analytics
    public LoanPortfolioAnalyticsDto? Analytics { get; init; }
    public List<RiskGradeBreakdownDto>? RiskBreakdown { get; init; }
    public List<StatusBreakdownDto>? StatusBreakdown { get; init; }
}

public record LoanSummaryDto
{
    public Guid Id { get; init; }
    public string LoanNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal PrincipalAmount { get; init; }
    public decimal OutstandingPrincipal { get; init; }
    public string Currency { get; init; } = string.Empty;
    public decimal InterestRate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string SubStatus { get; init; } = string.Empty;
    public string? RiskGrade { get; init; }
    public DateTime ApplicationDate { get; init; }
    public DateTime? DisbursementDate { get; init; }
    public DateTime? MaturityDate { get; init; }
    public int DaysPastDue { get; init; }
    public decimal PastDueAmount { get; init; }
    public decimal ProvisionAmount { get; init; }
}

public record LoanPortfolioAnalyticsDto
{
    public int TotalLoans { get; init; }
    public decimal TotalPrincipal { get; init; }
    public decimal TotalOutstanding { get; init; }
    public decimal TotalProvisions { get; init; }
    public decimal TotalInterestPaid { get; init; }
    public decimal AverageInterestRate { get; init; }
    public decimal PortfolioAtRiskRate { get; init; } // PAR rate
    public decimal NonPerformingLoanRate { get; init; } // NPL rate
    public int ActiveLoans { get; init; }
    public int PastDueLoans { get; init; }
    public int NonPerformingLoans { get; init; }
}

public record RiskGradeBreakdownDto
{
    public string RiskGrade { get; init; } = string.Empty;
    public int Count { get; init; }
    public decimal TotalOutstanding { get; init; }
    public decimal Percentage { get; init; }
}

public record StatusBreakdownDto
{
    public string Status { get; init; } = string.Empty;
    public int Count { get; init; }
    public decimal TotalOutstanding { get; init; }
    public decimal Percentage { get; init; }
}