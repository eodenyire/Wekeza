using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.Instruments.Cards.Commands.IssueCard;
using Wekeza.Core.Application.Features.Instruments.Cards.Commands.ActivateCard;
using Wekeza.Core.Application.Features.Instruments.Cards.Commands.CancelCard;
///📂 Wekeza.Core.Api/Controllers/
///4. CardsController.cs (The Instrument Gateway)
///This is critical for physical security. When a card is lost in the streets of Nairobi, this API must respond in milliseconds to hot-list it.
///
///
namespace Wekeza.Core.Api.Controllers;

public class CardsController : BaseApiController
{
    [HttpPost("issue")]
    public async Task<ActionResult<Guid>> Issue(IssueCardCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPatch("{cardId}/activate")]
    public async Task<ActionResult<bool>> Activate(Guid cardId, [FromBody] string code)
    {
        return Ok(await Mediator.Send(new ActivateCardCommand(cardId, code)));
    }

    [HttpDelete("{cardId}/cancel")]
    public async Task<ActionResult<bool>> Cancel(Guid cardId, [FromBody] string reason)
    {
        return Ok(await Mediator.Send(new CancelCardCommand(cardId, reason)));
    }
}
