using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Instruments.Cards.Commands.ActivateCard;

public record ActivateCardCommand : ICommand<Result<bool>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string CardNumber { get; init; } = string.Empty;
    public string Pin { get; init; } = string.Empty;
}
