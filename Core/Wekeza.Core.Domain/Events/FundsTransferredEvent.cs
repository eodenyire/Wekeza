using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

/// <summary>
/// 📂 Wekeza.Core.Domain/Events
/// 1. FundsTransferredEvent.cs (The Fraud Engine Signal)
/// We will now implement the signals that keep Wekeza Bank reactive and secure. Every event inherits from our IDomainEvent interface to ensure consistency.
/// This event is critical. When money moves, the Python-based Fraud Engine needs to know immediately to update its risk models and check for suspicious patterns.
/// Signal raised when a transfer is finalized. 
/// Triggers the Fraud Engine and the Transaction Streamer.
/// </summary>
public record FundsTransferredEvent(
    Guid FromAccountId, 
    Guid ToAccountId, 
    Money Amount, 
    Guid CorrelationId) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
