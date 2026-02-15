///2. CancelCard (The Emergency Brake)
///In Nairobi, if a customer loses their phone or wallet, this API is the first line of defense. It must be fast, permanent, and audited.
///
///
using MediatR;

namespace Wekeza.Core.Application.Features.Instruments.Cards.Commands.CancelCard;

public record CancelCardCommand(Guid CardId, string Reason) : IRequest<bool>;
