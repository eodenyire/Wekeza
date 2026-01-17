using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Workflows.Commands.ApproveWorkflow;

/// <summary>
/// Command to approve a pending workflow
/// Implements Checker role in Maker-Checker pattern
/// </summary>
[Authorize(UserRole.RiskOfficer, UserRole.Administrator)]
public record ApproveWorkflowCommand : ICommand<bool>
{
    public Guid WorkflowId { get; init; }
    public string Comments { get; init; } = string.Empty;
}
