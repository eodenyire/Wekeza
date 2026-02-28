namespace Wekeza.Core.Domain.Common;

/// <summary> 
/// 1. IDomainEvent.cs
/// Represents a signal that something important happened within the Wekeza Domain.
/// Every significant change in Wekeza Bank (like a large withdrawal) will trigger a Domain Event. This interface ensures all events have a timestamp and a unique ID for auditability.
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}
