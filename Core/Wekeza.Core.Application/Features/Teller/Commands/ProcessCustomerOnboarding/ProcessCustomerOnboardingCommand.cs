using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Teller.Commands.ProcessCustomerOnboarding;

public record ProcessCustomerOnboardingCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string IdNumber { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
