using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.Accounts.Commands.OpenAccount;
using Wekeza.Core.Application.Features.Accounts.Commands.OpenProductBasedAccount;
using Wekeza.Core.Application.Features.Accounts.Commands.RegisterBusiness;
using Wekeza.Core.Application.Features.Accounts.Commands.Management.FreezeAccount;
using Wekeza.Core.Application.Features.Accounts.Commands.Management.CloseAccount;
using Wekeza.Core.Application.Features.Accounts.Queries.GetAccountSummary;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
///📂 Wekeza.Core.Api/Controllers/AccountsController.cs
///This controller is designed for high-volume identity management. It handles Individual and Corporate onboarding with strict RESTful patterns.
/// The Identity & Lifecycle Gateway.
/// Manages account creation, corporate onboarding, and administrative controls.
/// </summary>
public class AccountsController : BaseApiController
{
    /// <summary>
    /// RETAIL: Opens a standard individual savings or current account.
    /// </summary>
    [HttpPost("individual")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> OpenIndividual(OpenAccountCommand command)
    {
        var accountId = await Mediator.Send(command);
        return CreatedAtAction(nameof(OpenIndividual), new { id = accountId }, accountId);
    }

    /// <summary>
    /// PRODUCT-BASED: Opens an account using Product Factory configuration with GL integration
    /// This is the new enterprise-grade account opening with automatic GL posting
    /// </summary>
    [HttpPost("product-based")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<OpenProductBasedAccountResult>> OpenProductBasedAccount(OpenProductBasedAccountCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(OpenProductBasedAccount), new { id = result.AccountId }, result);
    }

    /// <summary>
    /// CORPORATE: Onboards a business entity with UBO (Director) details.
    /// </summary>
    [HttpPost("business")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AccountDto>> RegisterBusiness(RegisterBusinessCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(RegisterBusiness), new { id = result.Id }, result);
    }

    /// <summary>
    /// SECURITY: Freezes an account immediately (Managerial/Risk Engine Action).
    /// </summary>
    [HttpPatch("{accountNumber}/freeze")]
    public async Task<ActionResult<bool>> Freeze(string accountNumber, [FromBody] string reason)
    {
        return Ok(await Mediator.Send(new FreezeAccountCommand(accountNumber, reason)));
    }

    /// <summary>
    /// LIFECYCLE: Permanently closes an account (Only if balance is zero).
    /// </summary>
    [HttpDelete("{accountNumber}")]
    public async Task<ActionResult<bool>> Close(string accountNumber)
    {
        return Ok(await Mediator.Send(new CloseAccountCommand(accountNumber)));
    }

    /// <summary>
    /// VISIBILITY: Fetches the real-time summary of an account.
    /// </summary>
    [HttpGet("{accountNumber}/summary")]
    public async Task<ActionResult<AccountSummaryDto>> GetSummary(string accountNumber)
    {
        return Ok(await Mediator.Send(new GetAccountSummaryQuery(accountNumber)));
    }
}
