using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

/// <summary>
/// Domain event raised when approval is required for a workflow
/// </summary>
public record ApprovalRequiredDomainEvent(
    Guid WorkflowId,
    Guid ApproverId,
    int Level,
    string EntityType,
    string EntityReference
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}