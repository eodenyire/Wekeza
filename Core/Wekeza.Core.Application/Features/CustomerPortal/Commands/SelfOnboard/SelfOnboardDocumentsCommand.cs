using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.SelfOnboard;

public record SelfOnboardDocumentsCommand : ICommand<Result<SelfOnboardResponse>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid OnboardingId { get; init; }
    public string IdFrontImage { get; init; } = string.Empty;
    public string IdBackImage { get; init; } = string.Empty;
    public string SelfieImage { get; init; } = string.Empty;
}

public class SelfOnboardDocumentsHandler : IRequestHandler<SelfOnboardDocumentsCommand, Result<SelfOnboardResponse>>
{
    public async Task<Result<SelfOnboardResponse>> Handle(SelfOnboardDocumentsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = new SelfOnboardResponse
            {
                OnboardingId = request.OnboardingId,
                Status = "DocumentsReceived"
            };
            return Result<SelfOnboardResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<SelfOnboardResponse>.Failure($"Failed to process documents: {ex.Message}");
        }
    }
}
