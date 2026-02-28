using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.SelfOnboard;

public record SelfOnboardAccountSetupCommand : ICommand<Result<SelfOnboardResponse>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid OnboardingId { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string Pin { get; init; } = string.Empty;
}

public class SelfOnboardAccountSetupHandler : IRequestHandler<SelfOnboardAccountSetupCommand, Result<SelfOnboardResponse>>
{
    public async Task<Result<SelfOnboardResponse>> Handle(SelfOnboardAccountSetupCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = new SelfOnboardResponse
            {
                OnboardingId = request.OnboardingId,
                Status = "Complete"
            };
            return Result<SelfOnboardResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<SelfOnboardResponse>.Failure($"Failed to setup account: {ex.Message}");
        }
    }
}
