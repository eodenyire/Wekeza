using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetLoanPortfolioStats;

public record GetLoanPortfolioStatsQuery : IRequest<Result<LoanPortfolioStatsDto>>
{
}

public class LoanPortfolioStatsDto
{
    public decimal TotalLoanBalance { get; set; }
    public int TotalLoans { get; set; }
    public int ActiveLoans { get; set; }
    public decimal NPLRatio { get; set; }
    public decimal AverageInterestRate { get; set; }
    public decimal TotalDisbursedThisMonth { get; set; }
}

public class GetLoanPortfolioStatsHandler : IRequestHandler<GetLoanPortfolioStatsQuery, Result<LoanPortfolioStatsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLoanPortfolioStatsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<LoanPortfolioStatsDto>> Handle(GetLoanPortfolioStatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var stats = new LoanPortfolioStatsDto();
            return Result<LoanPortfolioStatsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            return Result<LoanPortfolioStatsDto>.Failure($"Failed to get loan portfolio stats: {ex.Message}");
        }
    }
}
