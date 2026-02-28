using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetOnboardingStatus;

public record GetOnboardingStatusQuery : IRequest<Result<OnboardingStatusDto>>
{
    public Guid OnboardingId { get; init; }
}

public class OnboardingStatusDto
{
    public Guid OnboardingId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public int PercentComplete { get; set; }
    public List<string> CompletedSteps { get; set; } = new();
    public List<string> PendingSteps { get; set; } = new();
}

public class GetOnboardingStatusHandler : IRequestHandler<GetOnboardingStatusQuery, Result<OnboardingStatusDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOnboardingStatusHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<OnboardingStatusDto>> Handle(GetOnboardingStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var status = new OnboardingStatusDto
            {
                OnboardingId = request.OnboardingId,
                Status = "Pending",
                CurrentStep = "Identity Verification",
                PercentComplete = 50
            };
            return Result<OnboardingStatusDto>.Success(status);
        }
        catch (Exception ex)
        {
            return Result<OnboardingStatusDto>.Failure($"Failed to get onboarding status: {ex.Message}");
        }
    }
}
