using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Teller.Commands.EndSession;

public record EndSessionCommand : ICommand<Result<bool>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid SessionId { get; init; }
}
