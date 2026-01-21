using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Events;

/// <summary>
/// Domain event raised when a card transaction is processed
/// </summary>
public record CardTransactionProcessedDomainEvent(
    Guid CardId,
    Guid CustomerId,
    Guid AccountId,
    decimal Amount,
    TransactionType TransactionType,
    string TransactionReference) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}