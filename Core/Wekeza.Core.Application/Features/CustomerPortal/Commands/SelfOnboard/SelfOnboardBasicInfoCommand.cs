using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.SelfOnboard;

public record SelfOnboardBasicInfoCommand : ICommand<Result<SelfOnboardResponse>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string IdNumber { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
}

public class SelfOnboardResponse
{
    public Guid OnboardingId { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CIFNumber { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string TempPassword { get; set; } = string.Empty;
}

public class SelfOnboardBasicInfoHandler : IRequestHandler<SelfOnboardBasicInfoCommand, Result<SelfOnboardResponse>>
{
    public async Task<Result<SelfOnboardResponse>> Handle(SelfOnboardBasicInfoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = new SelfOnboardResponse
            {
                OnboardingId = Guid.NewGuid(),
                Status = "BasicInfoReceived"
            };
            return Result<SelfOnboardResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<SelfOnboardResponse>.Failure($"Failed to process basic info: {ex.Message}");
        }
    }
}
