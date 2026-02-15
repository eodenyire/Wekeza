using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wekeza.MVP4._0.Models;
using Wekeza.MVP4._0.Services;

namespace Wekeza.MVP4._0.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApprovalQueueController : ControllerBase
{
    private readonly IMakerCheckerService _makerCheckerService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ApprovalQueueController> _logger;

    public ApprovalQueueController(
        IMakerCheckerService makerCheckerService,
        INotificationService notificationService,
        ILogger<ApprovalQueueController> logger)
    {
        _makerCheckerService = makerCheckerService;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Get approval queue for the current user
    /// </summary>
    [HttpGet("queue")]
    public async Task<ActionResult<List<PendingApproval>>> GetApprovalQueue()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user ID");
            }

            var approvalQueue = await _makerCheckerService.GetApprovalQueueAsync(userId);
            return Ok(approvalQueue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving approval queue");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get approval queue by role
    /// </summary>
    [HttpGet("queue/role/{roleName}")]
    public async Task<ActionResult<List<PendingApproval>>> GetApprovalQueueByRole(string roleName)
    {
        try
        {
            var approvalQueue = await _makerCheckerService.GetApprovalQueueByRoleAsync(roleName);
            return Ok(approvalQueue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving approval queue for role {RoleName}", roleName);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Approve a workflow
    /// </summary>
    [HttpPost("{workflowId}/approve")]
    public async Task<ActionResult<ApprovalResult>> ApproveWorkflow(
        Guid workflowId, 
        [FromBody] ApprovalRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user ID");
            }

            var result = await _makerCheckerService.SubmitForApprovalAsync(
                workflowId, userId, request.Comments);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Workflow {WorkflowId} approved by user {UserId}", workflowId, userId);
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("Failed to approve workflow {WorkflowId}: {Message}", workflowId, result.Message);
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving workflow {WorkflowId}", workflowId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Reject a workflow
    /// </summary>
    [HttpPost("{workflowId}/reject")]
    public async Task<ActionResult<ApprovalResult>> RejectWorkflow(
        Guid workflowId, 
        [FromBody] RejectionRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user ID");
            }

            if (string.IsNullOrEmpty(request.Reason))
            {
                return BadRequest("Rejection reason is required");
            }

            var result = await _makerCheckerService.RejectWorkflowAsync(
                workflowId, userId, request.Reason);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Workflow {WorkflowId} rejected by user {UserId} with reason: {Reason}", 
                    workflowId, userId, request.Reason);
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("Failed to reject workflow {WorkflowId}: {Message}", workflowId, result.Message);
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting workflow {WorkflowId}", workflowId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Escalate a workflow
    /// </summary>
    [HttpPost("{workflowId}/escalate")]
    public async Task<ActionResult<EscalationResult>> EscalateWorkflow(
        Guid workflowId, 
        [FromBody] EscalationRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Reason))
            {
                return BadRequest("Escalation reason is required");
            }

            var result = await _makerCheckerService.EscalateApprovalAsync(workflowId, request.Reason);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Workflow {WorkflowId} escalated with reason: {Reason}", 
                    workflowId, request.Reason);
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("Failed to escalate workflow {WorkflowId}: {Message}", workflowId, result.Message);
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error escalating workflow {WorkflowId}", workflowId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get workflow details
    /// </summary>
    [HttpGet("{workflowId}")]
    public async Task<ActionResult<WorkflowInstance>> GetWorkflowDetails(Guid workflowId)
    {
        try
        {
            var workflow = await _makerCheckerService.GetWorkflowInstanceAsync(workflowId);
            if (workflow == null)
            {
                return NotFound("Workflow not found");
            }

            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow {WorkflowId}", workflowId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get workflow steps
    /// </summary>
    [HttpGet("{workflowId}/steps")]
    public async Task<ActionResult<List<ApprovalStep>>> GetWorkflowSteps(Guid workflowId)
    {
        try
        {
            var steps = await _makerCheckerService.GetWorkflowStepsAsync(workflowId);
            return Ok(steps);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow steps for {WorkflowId}", workflowId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get user notifications
    /// </summary>
    [HttpGet("notifications")]
    public async Task<ActionResult<List<Notification>>> GetNotifications([FromQuery] bool unreadOnly = false)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user ID");
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPost("notifications/{notificationId}/read")]
    public async Task<ActionResult> MarkNotificationAsRead(Guid notificationId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user ID");
            }

            var result = await _notificationService.MarkNotificationAsReadAsync(notificationId, userId);
            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound("Notification not found or not accessible");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
            return StatusCode(500, "Internal server error");
        }
    }
}

// Request DTOs
public class ApprovalRequest
{
    public string? Comments { get; set; }
}

public class RejectionRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class EscalationRequest
{
    public string Reason { get; set; } = string.Empty;
}