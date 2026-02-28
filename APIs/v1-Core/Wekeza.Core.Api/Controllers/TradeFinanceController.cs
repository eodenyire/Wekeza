using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Wekeza.Core.Application.Features.TradeFinance.Commands.IssueLCCommand;
using Wekeza.Core.Application.Features.TradeFinance.Commands.IssueBGCommand;
using Wekeza.Core.Application.Features.TradeFinance.Queries.GetLCDetails;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Trade Finance operations including Letters of Credit, Bank Guarantees, and Documentary Collections
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TradeFinanceController : BaseApiController
{
    public TradeFinanceController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Issue a new Letter of Credit
    /// </summary>
    /// <param name="command">LC issuance details</param>
    /// <returns>LC details with SWIFT message</returns>
    [HttpPost("letters-of-credit")]
    [Authorize(Roles = "Administrator,LoanOfficer,RiskOfficer")]
    public async Task<ActionResult<IssueLCResponse>> IssueLCAsync([FromBody] IssueLCCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get Letter of Credit details
    /// </summary>
    /// <param name="lcId">LC ID</param>
    /// <param name="lcNumber">LC Number (alternative to ID)</param>
    /// <returns>LC details</returns>
    [HttpGet("letters-of-credit")]
    public async Task<ActionResult<LCDetailsDto>> GetLCDetailsAsync([FromQuery] Guid? lcId, [FromQuery] string? lcNumber)
    {
        if (!lcId.HasValue && string.IsNullOrEmpty(lcNumber))
        {
            return BadRequest("Either LC ID or LC Number must be provided");
        }

        var query = new GetLCDetailsQuery
        {
            LCId = lcId ?? Guid.Empty,
            LCNumber = lcNumber
        };

        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Issue a new Bank Guarantee
    /// </summary>
    /// <param name="command">BG issuance details</param>
    /// <returns>BG details with SWIFT message</returns>
    [HttpPost("bank-guarantees")]
    [Authorize(Roles = "Administrator,LoanOfficer,RiskOfficer")]
    public async Task<ActionResult<IssueBGResponse>> IssueBGAsync([FromBody] IssueBGCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Amend an existing Letter of Credit
    /// </summary>
    /// <param name="lcId">LC ID</param>
    /// <param name="request">Amendment details</param>
    /// <returns>Amendment confirmation</returns>
    [HttpPut("letters-of-credit/{lcId}/amend")]
    [Authorize(Roles = "Administrator,LoanOfficer,RiskOfficer")]
    public async Task<ActionResult> AmendLCAsync(Guid lcId, [FromBody] AmendLCRequest request)
    {
        // TODO: Implement AmendLCCommand
        return Ok(new { Message = "LC amendment functionality will be implemented", LCId = lcId });
    }

    /// <summary>
    /// Present documents for an LC
    /// </summary>
    /// <param name="lcId">LC ID</param>
    /// <param name="request">Document presentation details</param>
    /// <returns>Presentation confirmation</returns>
    [HttpPost("letters-of-credit/{lcId}/present-documents")]
    [Authorize(Roles = "Administrator,Teller,LoanOfficer")]
    public async Task<ActionResult> PresentDocumentsAsync(Guid lcId, [FromBody] PresentDocumentsRequest request)
    {
        // TODO: Implement PresentDocumentsCommand
        return Ok(new { Message = "Document presentation functionality will be implemented", LCId = lcId });
    }

    /// <summary>
    /// Invoke a Bank Guarantee
    /// </summary>
    /// <param name="bgId">BG ID</param>
    /// <param name="request">Invocation details</param>
    /// <returns>Invocation confirmation</returns>
    [HttpPost("bank-guarantees/{bgId}/invoke")]
    [Authorize(Roles = "Administrator,RiskOfficer")]
    public async Task<ActionResult> InvokeBGAsync(Guid bgId, [FromBody] InvokeBGRequest request)
    {
        // TODO: Implement InvokeBGCommand
        return Ok(new { Message = "BG invocation functionality will be implemented", BGId = bgId });
    }

    /// <summary>
    /// Get outstanding Letters of Credit
    /// </summary>
    /// <returns>List of outstanding LCs</returns>
    [HttpGet("letters-of-credit/outstanding")]
    public async Task<ActionResult> GetOutstandingLCsAsync()
    {
        // TODO: Implement GetOutstandingLCsQuery
        return Ok(new { Message = "Outstanding LCs query will be implemented" });
    }

    /// <summary>
    /// Get outstanding Bank Guarantees
    /// </summary>
    /// <returns>List of outstanding BGs</returns>
    [HttpGet("bank-guarantees/outstanding")]
    public async Task<ActionResult> GetOutstandingBGsAsync()
    {
        // TODO: Implement GetOutstandingBGsQuery
        return Ok(new { Message = "Outstanding BGs query will be implemented" });
    }

    /// <summary>
    /// Get trade finance exposure for a party
    /// </summary>
    /// <param name="partyId">Party ID</param>
    /// <returns>Exposure details</returns>
    [HttpGet("exposure/{partyId}")]
    [Authorize(Roles = "Administrator,RiskOfficer,LoanOfficer")]
    public async Task<ActionResult> GetTradeFinanceExposureAsync(Guid partyId)
    {
        // TODO: Implement GetTradeFinanceExposureQuery
        return Ok(new { Message = "Trade finance exposure query will be implemented", PartyId = partyId });
    }

    /// <summary>
    /// Create a Documentary Collection
    /// </summary>
    /// <param name="request">Collection details</param>
    /// <returns>Collection confirmation</returns>
    [HttpPost("documentary-collections")]
    [Authorize(Roles = "Administrator,Teller,LoanOfficer")]
    public async Task<ActionResult> CreateDocumentaryCollectionAsync([FromBody] CreateCollectionRequest request)
    {
        // TODO: Implement CreateDocumentaryCollectionCommand
        return Ok(new { Message = "Documentary collection creation will be implemented" });
    }

    /// <summary>
    /// Get trade finance dashboard data
    /// </summary>
    /// <returns>Dashboard metrics</returns>
    [HttpGet("dashboard")]
    [Authorize(Roles = "Administrator,RiskOfficer,LoanOfficer")]
    public async Task<ActionResult> GetTradeFinanceDashboardAsync()
    {
        // TODO: Implement GetTradeFinanceDashboardQuery
        return Ok(new
        {
            Message = "Trade finance dashboard will be implemented",
            Metrics = new
            {
                OutstandingLCs = 0,
                OutstandingBGs = 0,
                TotalExposure = 0,
                ExpiringInstruments = 0
            }
        });
    }
}

// Request DTOs (to be moved to separate files later)
public record AmendLCRequest
{
    public string AmendmentDetails { get; init; } = string.Empty;
    public decimal? NewAmount { get; init; }
    public DateTime? NewExpiryDate { get; init; }
}

public record PresentDocumentsRequest
{
    public List<DocumentUpload> Documents { get; init; } = new();
    public Guid PresentingBankId { get; init; }
    public string Notes { get; init; } = string.Empty;
}

public record DocumentUpload
{
    public string DocumentType { get; init; } = string.Empty;
    public string DocumentNumber { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;
}

public record InvokeBGRequest
{
    public decimal ClaimAmount { get; init; }
    public string ClaimReason { get; init; } = string.Empty;
    public List<DocumentUpload> SupportingDocuments { get; init; } = new();
}

public record CreateCollectionRequest
{
    public string CollectionNumber { get; init; } = string.Empty;
    public Guid DrawerId { get; init; }
    public Guid DraweeId { get; init; }
    public Guid RemittingBankId { get; init; }
    public Guid CollectingBankId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";
    public string Type { get; init; } = "DocumentsAgainstPayment";
    public string Terms { get; init; } = string.Empty;
    public DateTime? MaturityDate { get; init; }
    public List<DocumentUpload> Documents { get; init; } = new();
}