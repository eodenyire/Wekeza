using MediatR;
using Microsoft.AspNetCore.Mvc;
using WekezaERMS.Application.Commands.Risks;
using WekezaERMS.Application.DTOs;
using WekezaERMS.Application.Queries.Risks;

namespace WekezaERMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RisksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RisksController> _logger;

    public RisksController(IMediator mediator, ILogger<RisksController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all risks
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<RiskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RiskDto>>> GetAllRisks()
    {
        _logger.LogInformation("Getting all risks");
        var query = new GetAllRisksQuery();
        var risks = await _mediator.Send(query);
        return Ok(risks);
    }

    /// <summary>
    /// Create a new risk
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RiskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RiskDto>> CreateRisk([FromBody] CreateRiskDto createRiskDto)
    {
        _logger.LogInformation("Creating new risk: {Title}", createRiskDto.Title);
        
        var command = new CreateRiskCommand
        {
            RiskData = createRiskDto,
            CreatedBy = Guid.NewGuid() // TODO: Get from authentication context
        };

        var risk = await _mediator.Send(command);
        
        return CreatedAtAction(nameof(GetAllRisks), new { id = risk.Id }, risk);
    }

    /// <summary>
    /// Get risk statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetStatistics()
    {
        var query = new GetAllRisksQuery();
        var risks = await _mediator.Send(query);
        
        var statistics = new
        {
            TotalRisks = risks.Count,
            ByCategory = risks.GroupBy(r => r.Category).Select(g => new { Category = g.Key.ToString(), Count = g.Count() }),
            ByStatus = risks.GroupBy(r => r.Status).Select(g => new { Status = g.Key.ToString(), Count = g.Count() }),
            ByLevel = risks.GroupBy(r => r.InherentRiskLevel).Select(g => new { Level = g.Key.ToString(), Count = g.Count() })
        };
        
        return Ok(statistics);
    }

    /// <summary>
    /// Get risk dashboard data
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetDashboard()
    {
        var query = new GetAllRisksQuery();
        var risks = await _mediator.Send(query);
        
        var dashboard = new
        {
            TotalRisks = risks.Count,
            CriticalRisks = risks.Count(r => r.InherentRiskLevel == Domain.Enums.RiskLevel.Critical),
            HighRisks = risks.Count(r => r.InherentRiskLevel == Domain.Enums.RiskLevel.High || r.InherentRiskLevel == Domain.Enums.RiskLevel.VeryHigh),
            ActiveRisks = risks.Count(r => r.Status == Domain.Enums.RiskStatus.Active),
            RisksByCategory = risks.GroupBy(r => r.Category).Select(g => new { Category = g.Key.ToString(), Count = g.Count() }).ToList(),
            RecentRisks = risks.OrderByDescending(r => r.IdentifiedDate).Take(5).Select(r => new { r.RiskCode, r.Title, r.InherentRiskLevel, r.IdentifiedDate }).ToList()
        };
        
        return Ok(dashboard);
    }
}
