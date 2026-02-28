using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetLoanPerformance;

public record GetLoanPerformanceQuery : IRequest<Result<LoanPerformanceDto>>
{
}

public class LoanPerformanceDto
{
    public decimal NPLRatio { get; set; }
    public decimal ProvisionCoverage { get; set; }
    public decimal CollectionRate { get; set; }
    public int OverdueCount { get; set; }
    public decimal OverdueAmount { get; set; }
}

public class GetLoanPerformanceHandler : IRequestHandler<GetLoanPerformanceQuery, Result<LoanPerformanceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLoanPerformanceHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<LoanPerformanceDto>> Handle(GetLoanPerformanceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var performance = new LoanPerformanceDto();
            return Result<LoanPerformanceDto>.Success(performance);
        }
        catch (Exception ex)
        {
            return Result<LoanPerformanceDto>.Failure($"Failed to get loan performance: {ex.Message}");
        }
    }
}
