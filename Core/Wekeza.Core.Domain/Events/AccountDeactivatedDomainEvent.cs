using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

public record AccountDeactivatedDomainEvent(Guid AccountId, string Reason) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
