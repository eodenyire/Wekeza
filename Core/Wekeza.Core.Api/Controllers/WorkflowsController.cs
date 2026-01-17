using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.Workflows.Commands.InitiateWorkflow;
using Wekeza.Core.Application.Features.Workflows.Commands.ApproveWorkflow;
using Wekeza.Core.Application.Features.Workflows.Commands.RejectWorkflow;
using Wekeza.Core.Application.Features.Workflows.Queries.GetPendingApprovals;
using Wekeza.Core.Application.Features.Workflows.Queries.GetWorkflowDetails;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Workflow & Maker-Checker Controller
/// Enterprise workflow engine similar to Finacle Workflow and T24 Maker-Checker
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkflowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkflowsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Initiate a new workflow for approval
    /// </summary>
    /// <param name="command">Workflow initiation details</param>
    /// <returns>Workflow ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InitiateWorkflow([FromBody] InitiateWorkflowCommand command)
    {
        var workflowId = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetWorkflowDetails),
            new { workflowId },
            new { workflowId, message = "Workflow initiated successfully" });
    }

    /// <summary>
    /// Get pending approvals for current user
    /// </summary>
    /// <returns>List of pending approvals</returns>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(List<PendingApprovalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingApprovals()
    {
        var query = new GetPendingApprovalsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get workflow details
    /// </summary>
    /// <param name="workflowId">Workflow ID</param>
    /// <returns>Complete workflow details</returns>
    [HttpGet("{workflowId}")]
    [ProducesResponseType(typeof(WorkflowDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkflowDetails(Guid workflowId)
    {
        var query = new GetWorkflowDetailsQuery { WorkflowId = workflowId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Approve a pending workflow
    /// </summary>
    /// <param name="workflowId">Workflow ID</param>
    /// <param name="command">Approval details</param>
    /// <returns>Success indicator</returns>
    [HttpPost("{workflowId}/approve")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveWorkflow(Guid workflowId, [FromBody] ApproveWorkflowCommand command)
    {
        command = command with { WorkflowId = workflowId };
        var result = await _mediator.Send(command);
        return Ok(new { success = result, message = "Workflow approved successfully" });
    }

    /// <summary>
    /// Reject a pending workflow
    /// </summary>
    /// <param name="workflowId">Workflow ID</param>
    /// <param name="command">Rejection details</param>
    /// <returns>Success indicator</returns>
    [HttpPost("{workflowId}/reject")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectWorkflow(Guid workflowId, [FromBody] RejectWorkflowCommand command)
    {
        command = command with { WorkflowId = workflowId };
        var result = await _mediator.Send(command);
        return Ok(new { success = result, message = "Workflow rejected successfully" });
    }
}
