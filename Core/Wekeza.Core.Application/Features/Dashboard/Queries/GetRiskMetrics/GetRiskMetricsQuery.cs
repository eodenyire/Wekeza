using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetRiskMetrics;

public record GetRiskMetricsQuery : IRequest<Result<RiskMetricsDto>>
{
}

public class RiskMetricsDto
{
    public decimal RiskWeightedAssets { get; set; }
    public decimal CapitalAdequacyRatio { get; set; }
    public int HighRiskAccounts { get; set; }
    public int PendingAMLCases { get; set; }
    public decimal LimitUtilization { get; set; }
}

public class GetRiskMetricsHandler : IRequestHandler<GetRiskMetricsQuery, Result<RiskMetricsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRiskMetricsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RiskMetricsDto>> Handle(GetRiskMetricsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var metrics = new RiskMetricsDto();
            return Result<RiskMetricsDto>.Success(metrics);
        }
        catch (Exception ex)
        {
            return Result<RiskMetricsDto>.Failure($"Failed to get risk metrics: {ex.Message}");
        }
    }
}
