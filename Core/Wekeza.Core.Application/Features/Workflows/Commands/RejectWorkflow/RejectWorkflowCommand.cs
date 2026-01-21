using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Workflows.Commands.RejectWorkflow;

[Authorize(UserRole.RiskOfficer, UserRole.Administrator)]
public record RejectWorkflowCommand : ICommand<bool>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid WorkflowId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
