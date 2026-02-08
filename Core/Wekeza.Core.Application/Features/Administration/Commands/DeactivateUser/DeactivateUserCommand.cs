using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Administration.Commands.DeactivateUser;

public record DeactivateUserCommand : ICommand<Result<bool>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
