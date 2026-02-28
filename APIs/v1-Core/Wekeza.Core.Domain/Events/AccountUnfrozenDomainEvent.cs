using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

public record AccountUnfrozenDomainEvent(
    Guid AccountId,
    string UnfrozenBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}