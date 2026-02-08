using MediatR;
using Microsoft.AspNetCore.Mvc;
using WekezaERMS.Application.Commands.Controls;
using WekezaERMS.Application.DTOs;
using WekezaERMS.Application.Queries.Controls;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WekezaERMS.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class ControlsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ControlsController> _logger;

    public ControlsController(IMediator mediator, ILogger<ControlsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all controls for a specific risk
    /// </summary>
    [HttpGet("risks/{riskId}/controls")]
    [Authorize(Policy = "RiskViewer")]
    [ProducesResponseType(typeof(List<ControlDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ControlDto>>> GetControlsByRiskId(Guid riskId)
    {
        _logger.LogInformation("Getting controls for risk: {RiskId}", riskId);
        var query = new GetControlsByRiskIdQuery { RiskId = riskId };
        var controls = await _mediator.Send(query);
        return Ok(controls);
    }

    /// <summary>
    /// Get a single control by ID
    /// </summary>
    [HttpGet("controls/{id}")]
    [Authorize(Policy = "RiskViewer")]
    [ProducesResponseType(typeof(ControlDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ControlDto>> GetControlById(Guid id)
    {
        _logger.LogInformation("Getting control by ID: {Id}", id);
        var query = new GetControlByIdQuery { Id = id };
        var control = await _mediator.Send(query);
        
        if (control == null)
        {
            return NotFound(new { message = $"Control with ID {id} not found" });
        }
        
        return Ok(control);
    }

    /// <summary>
    /// Create a new control for a risk
    /// </summary>
    [HttpPost("risks/{riskId}/controls")]
    [Authorize(Policy = "RiskOfficer")]
    [ProducesResponseType(typeof(ControlDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ControlDto>> CreateControl(Guid riskId, [FromBody] CreateControlDto createControlDto)
    {
        try
        {
            _logger.LogInformation("Creating control for risk: {RiskId}", riskId);
            
            var userId = GetCurrentUserId();
            
            var command = new CreateControlCommand
            {
                RiskId = riskId,
                ControlData = createControlDto,
                CreatedBy = userId
            };

            var control = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetControlById), new { id = control.Id }, control);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed for CreateControl: {Errors}", ex.Errors);
            return BadRequest(new { errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Invalid operation: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing control
    /// </summary>
    [HttpPut("controls/{id}")]
    [Authorize(Policy = "RiskOfficer")]
    [ProducesResponseType(typeof(ControlDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ControlDto>> UpdateControl(Guid id, [FromBody] UpdateControlDto updateControlDto)
    {
        try
        {
            _logger.LogInformation("Updating control: {Id}", id);
            
            var userId = GetCurrentUserId();
            
            var command = new UpdateControlCommand
            {
                Id = id,
                ControlData = updateControlDto,
                UpdatedBy = userId
            };

            var control = await _mediator.Send(command);
            
            if (control == null)
            {
                return NotFound(new { message = $"Control with ID {id} not found" });
            }
            
            return Ok(control);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed for UpdateControl: {Errors}", ex.Errors);
            return BadRequest(new { errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }
    }

    /// <summary>
    /// Delete a control
    /// </summary>
    [HttpDelete("controls/{id}")]
    [Authorize(Policy = "RiskManager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteControl(Guid id)
    {
        _logger.LogInformation("Deleting control: {Id}", id);
        
        var userId = GetCurrentUserId();
        
        var command = new DeleteControlCommand
        {
            Id = id,
            DeletedBy = userId
        };

        var success = await _mediator.Send(command);
        
        if (!success)
        {
            return NotFound(new { message = $"Control with ID {id} not found" });
        }
        
        return NoContent();
    }

    /// <summary>
    /// Update control effectiveness
    /// </summary>
    [HttpPut("controls/{id}/effectiveness")]
    [Authorize(Policy = "RiskOfficer")]
    [ProducesResponseType(typeof(ControlDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ControlDto>> UpdateControlEffectiveness(Guid id, [FromBody] ControlEffectivenessDto effectivenessDto)
    {
        try
        {
            _logger.LogInformation("Updating control effectiveness: {Id}", id);
            
            var userId = GetCurrentUserId();
            
            var command = new UpdateControlEffectivenessCommand
            {
                Id = id,
                EffectivenessData = effectivenessDto,
                UpdatedBy = userId
            };

            var control = await _mediator.Send(command);
            
            if (control == null)
            {
                return NotFound(new { message = $"Control with ID {id} not found" });
            }
            
            return Ok(control);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed for UpdateControlEffectiveness: {Errors}", ex.Errors);
            return BadRequest(new { errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }
    }

    /// <summary>
    /// Record control test result
    /// </summary>
    [HttpPost("controls/{id}/test")]
    [Authorize(Policy = "RiskOfficer")]
    [ProducesResponseType(typeof(ControlDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ControlDto>> RecordControlTest(Guid id, [FromBody] ControlTestDto testDto)
    {
        try
        {
            _logger.LogInformation("Recording control test: {Id}", id);
            
            var userId = GetCurrentUserId();
            
            var command = new RecordControlTestCommand
            {
                Id = id,
                TestData = testDto,
                UpdatedBy = userId
            };

            var control = await _mediator.Send(command);
            
            if (control == null)
            {
                return NotFound(new { message = $"Control with ID {id} not found" });
            }
            
            return Ok(control);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed for RecordControlTest: {Errors}", ex.Errors);
            return BadRequest(new { errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
