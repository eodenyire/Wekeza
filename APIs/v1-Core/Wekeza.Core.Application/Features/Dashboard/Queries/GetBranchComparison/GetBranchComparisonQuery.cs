using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetBranchComparison;

public record GetBranchComparisonQuery : IRequest<Result<List<BranchComparisonDto>>>
{
    public string Metric { get; init; } = string.Empty;
}

public class BranchComparisonDto
{
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Metric { get; set; } = string.Empty;
}

public class GetBranchComparisonHandler : IRequestHandler<GetBranchComparisonQuery, Result<List<BranchComparisonDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetBranchComparisonHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<BranchComparisonDto>>> Handle(GetBranchComparisonQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var comparison = new List<BranchComparisonDto>();
            await Task.CompletedTask;
            return Result<List<BranchComparisonDto>>.Success(comparison);
        }
        catch (Exception ex)
        {
            return Result<List<BranchComparisonDto>>.Failure($"Failed to get branch comparison: {ex.Message}");
        }
    }
}
