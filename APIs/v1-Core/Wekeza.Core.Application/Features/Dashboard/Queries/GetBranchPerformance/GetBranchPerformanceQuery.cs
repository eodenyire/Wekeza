using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetBranchPerformance;

public record GetBranchPerformanceQuery : IRequest<Result<List<BranchPerformanceDto>>>
{
}

public class BranchPerformanceDto
{
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public decimal TransactionVolume { get; set; }
    public int CustomerCount { get; set; }
    public decimal DepositBalance { get; set; }
    public decimal LoanBalance { get; set; }
}

public class GetBranchPerformanceHandler : IRequestHandler<GetBranchPerformanceQuery, Result<List<BranchPerformanceDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetBranchPerformanceHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<BranchPerformanceDto>>> Handle(GetBranchPerformanceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var performance = new List<BranchPerformanceDto>();
            return Result<List<BranchPerformanceDto>>.Success(performance);
        }
        catch (Exception ex)
        {
            return Result<List<BranchPerformanceDto>>.Failure($"Failed to get branch performance: {ex.Message}");
        }
    }
}
