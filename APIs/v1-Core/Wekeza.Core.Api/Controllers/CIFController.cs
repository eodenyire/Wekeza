using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.CIF.Commands.CreateIndividualParty;
using Wekeza.Core.Application.Features.CIF.Commands.CreateCorporateParty;
using Wekeza.Core.Application.Features.CIF.Commands.PerformAMLScreening;
using Wekeza.Core.Application.Features.CIF.Commands.UpdateKYCStatus;
using Wekeza.Core.Application.Features.CIF.Queries.GetCustomer360View;
using Wekeza.Core.Application.Features.CIF.Queries.SearchParties;
using Wekeza.Core.Application.Features.CIF.Queries.GetPendingKYC;
using Wekeza.Core.Application.Features.CIF.Queries.GetHighRiskParties;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Customer Information File (CIF) / Party Management Controller
/// Enterprise-grade customer management similar to Finacle CIF and T24 CUSTOMER
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CIFController : ControllerBase
{
    private readonly IMediator _mediator;

    public CIFController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new individual party (retail customer)
    /// </summary>
    /// <param name="command">Individual party details</param>
    /// <returns>Party Number (CIF Number)</returns>
    [HttpPost("individual")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIndividualParty([FromBody] CreateIndividualPartyCommand command)
    {
        var partyNumber = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetCustomer360View), 
            new { partyNumber }, 
            new { partyNumber, message = "Individual party created successfully" });
    }

    /// <summary>
    /// Create a new corporate party (business customer)
    /// </summary>
    /// <param name="command">Corporate party details</param>
    /// <returns>Party Number (CIF Number)</returns>
    [HttpPost("corporate")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCorporateParty([FromBody] CreateCorporatePartyCommand command)
    {
        var partyNumber = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetCustomer360View), 
            new { partyNumber }, 
            new { partyNumber, message = "Corporate party created successfully" });
    }

    /// <summary>
    /// Get complete 360° view of a customer
    /// Includes accounts, loans, cards, transactions, and relationships
    /// </summary>
    /// <param name="partyNumber">Party Number (CIF Number)</param>
    /// <returns>Complete customer profile</returns>
    [HttpGet("{partyNumber}/360-view")]
    [ProducesResponseType(typeof(Customer360ViewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomer360View(string partyNumber)
    {
        var query = new GetCustomer360ViewQuery { PartyNumber = partyNumber };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Perform AML/CFT screening on a party
    /// Checks against sanctions lists, PEP databases, and adverse media
    /// </summary>
    /// <param name="command">Screening parameters</param>
    /// <returns>Screening results</returns>
    [HttpPost("aml-screening")]
    [ProducesResponseType(typeof(AMLScreeningResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PerformAMLScreening([FromBody] PerformAMLScreeningCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Update KYC status after verification
    /// </summary>
    /// <param name="command">KYC status update</param>
    /// <returns>Success indicator</returns>
    [HttpPut("kyc-status")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateKYCStatus([FromBody] UpdateKYCStatusCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { success = result, message = "KYC status updated successfully" });
    }

    /// <summary>
    /// Search parties by name
    /// </summary>
    /// <param name="name">Search term</param>
    /// <returns>List of matching parties</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<PartySearchResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchParties([FromQuery] string name)
    {
        var query = new SearchPartiesQuery { SearchTerm = name };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get pending KYC parties
    /// </summary>
    /// <returns>List of parties with pending KYC</returns>
    [HttpGet("pending-kyc")]
    [ProducesResponseType(typeof(List<PendingKYCDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingKYC()
    {
        var query = new GetPendingKYCQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get high-risk parties
    /// </summary>
    /// <returns>List of high-risk parties</returns>
    [HttpGet("high-risk")]
    [ProducesResponseType(typeof(List<HighRiskPartyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHighRiskParties()
    {
        var query = new GetHighRiskPartiesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
