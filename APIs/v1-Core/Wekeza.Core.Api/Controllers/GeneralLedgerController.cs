using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.GeneralLedger.Commands.CreateGLAccount;
using Wekeza.Core.Application.Features.GeneralLedger.Commands.PostJournalEntry;
using Wekeza.Core.Application.Features.GeneralLedger.Queries.GetChartOfAccounts;
using Wekeza.Core.Application.Features.GeneralLedger.Queries.GetTrialBalance;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// General Ledger Controller
/// Financial backbone similar to Finacle GL and T24 ACCOUNT
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GeneralLedgerController : ControllerBase
{
    private readonly IMediator _mediator;

    public GeneralLedgerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new GL account in Chart of Accounts
    /// </summary>
    /// <param name="command">GL account details</param>
    /// <returns>GL Code</returns>
    [HttpPost("accounts")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGLAccount([FromBody] CreateGLAccountCommand command)
    {
        var glCode = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetChartOfAccounts),
            new { },
            new { glCode, message = "GL account created successfully" });
    }

    /// <summary>
    /// Get Chart of Accounts
    /// </summary>
    /// <returns>Complete chart of accounts</returns>
    [HttpGet("accounts")]
    [ProducesResponseType(typeof(List<GLAccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChartOfAccounts()
    {
        var query = new GetChartOfAccountsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Post a journal entry to GL
    /// </summary>
    /// <param name="command">Journal entry details</param>
    /// <returns>Journal number</returns>
    [HttpPost("journal-entries")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostJournalEntry([FromBody] PostJournalEntryCommand command)
    {
        var journalNumber = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetTrialBalance),
            new { asOfDate = DateTime.UtcNow.Date },
            new { journalNumber, message = "Journal entry posted successfully" });
    }

    /// <summary>
    /// Get Trial Balance
    /// </summary>
    /// <param name="asOfDate">As of date</param>
    /// <returns>Trial balance</returns>
    [HttpGet("trial-balance")]
    [ProducesResponseType(typeof(TrialBalanceDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrialBalance([FromQuery] DateTime? asOfDate = null)
    {
        var query = new GetTrialBalanceQuery
        {
            AsOfDate = asOfDate ?? DateTime.UtcNow.Date
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}