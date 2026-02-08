using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.Administration.Commands.CreateUser;
using Wekeza.Core.Application.Features.Administration.Commands.UpdateUser;
using Wekeza.Core.Application.Features.Administration.Commands.DeactivateUser;
using Wekeza.Core.Application.Features.Administration.Commands.CreateRole;
using Wekeza.Core.Application.Features.Administration.Commands.AssignRole;
using Wekeza.Core.Application.Features.Administration.Commands.CreateBranch;
using Wekeza.Core.Application.Features.Administration.Commands.UpdateSystemParameter;
using Wekeza.Core.Application.Features.Administration.Queries.GetUsers;
using Wekeza.Core.Application.Features.Administration.Queries.GetRoles;
using Wekeza.Core.Application.Features.Administration.Queries.GetBranches;
using Wekeza.Core.Application.Features.Administration.Queries.GetSystemStats;
using Wekeza.Core.Application.Features.Administration.Queries.GetAuditLogs;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Administrator Portal Controller - Complete system administration
/// Handles user management, role management, branch management, and system oversight
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator,ITAdministrator")]
public class AdministratorController : BaseApiController
{
    public AdministratorController(IMediator mediator) : base(mediator) { }

    #region User Management

    /// <summary>
    /// Get all users with filtering and pagination
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetUser), new { id = result.Value }, result);
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var query = new GetUserQuery { UserId = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update user details
    /// </summary>
    [HttpPut("users/{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.UserId)
            return BadRequest("User ID mismatch");

        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deactivate user
    /// </summary>
    [HttpPost("users/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id, [FromBody] string reason)
    {
        var command = new DeactivateUserCommand { UserId = id, Reason = reason };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Reset user password
    /// </summary>
    [HttpPost("users/{id:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(Guid id)
    {
        var command = new ResetPasswordCommand { UserId = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Role Management

    /// <summary>
    /// Get all roles
    /// </summary>
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var query = new GetRolesQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Assign role to user
    /// </summary>
    [HttpPost("users/{userId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> AssignRole(Guid userId, Guid roleId)
    {
        var command = new AssignRoleCommand { UserId = userId, RoleId = roleId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Branch Management

    /// <summary>
    /// Get all branches
    /// </summary>
    [HttpGet("branches")]
    public async Task<IActionResult> GetBranches()
    {
        var query = new GetBranchesQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new branch
    /// </summary>
    [HttpPost("branches")]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region System Monitoring

    /// <summary>
    /// Get system statistics and KPIs
    /// </summary>
    [HttpGet("system/stats")]
    public async Task<IActionResult> GetSystemStats()
    {
        var query = new GetSystemStatsQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get audit logs
    /// </summary>
    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs([FromQuery] GetAuditLogsQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get pending approvals across all modules
    /// </summary>
    [HttpGet("pending-approvals")]
    public async Task<IActionResult> GetPendingApprovals()
    {
        var query = new GetPendingApprovalsQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region System Configuration

    /// <summary>
    /// Get system parameters
    /// </summary>
    [HttpGet("system/parameters")]
    public async Task<IActionResult> GetSystemParameters()
    {
        var query = new GetSystemParametersQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update system parameter
    /// </summary>
    [HttpPut("system/parameters/{key}")]
    public async Task<IActionResult> UpdateSystemParameter(string key, [FromBody] UpdateSystemParameterCommand command)
    {
        command.Key = key;
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion
}