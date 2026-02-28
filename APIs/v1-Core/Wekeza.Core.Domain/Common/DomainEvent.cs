using MediatR;

namespace Wekeza.Core.Domain.Common;

/// <summary>
/// Base class for domain events - provides common implementation for IDomainEvent
/// </summary>
public abstract class DomainEvent : IDomainEvent, INotification
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}