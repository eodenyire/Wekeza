using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

public record AccountVerifiedDomainEvent(Guid AccountId, string VerifiedBy) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
