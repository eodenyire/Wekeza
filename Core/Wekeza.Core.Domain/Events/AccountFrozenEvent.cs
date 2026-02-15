using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

public record AccountFrozenEvent(
    Guid AccountId,
    string AccountNumber,
    string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}