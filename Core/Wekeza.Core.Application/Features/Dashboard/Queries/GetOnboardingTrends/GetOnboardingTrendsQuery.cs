using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetOnboardingTrends;

public record GetOnboardingTrendsQuery : IRequest<Result<List<OnboardingTrendDto>>>
{
    public int Months { get; init; } = 12;
}

public class OnboardingTrendDto
{
    public DateTime Month { get; set; }
    public int NewCustomers { get; set; }
    public int CompletedOnboardings { get; set; }
    public int PendingOnboardings { get; set; }
}

public class GetOnboardingTrendsHandler : IRequestHandler<GetOnboardingTrendsQuery, Result<List<OnboardingTrendDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetOnboardingTrendsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<OnboardingTrendDto>>> Handle(GetOnboardingTrendsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var trends = new List<OnboardingTrendDto>();
            return Result<List<OnboardingTrendDto>>.Success(trends);
        }
        catch (Exception ex)
        {
            return Result<List<OnboardingTrendDto>>.Failure($"Failed to get onboarding trends: {ex.Message}");
        }
    }
}
