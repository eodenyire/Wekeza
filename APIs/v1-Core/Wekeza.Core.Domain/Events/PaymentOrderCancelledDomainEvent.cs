using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

public record PaymentOrderCancelledDomainEvent(
    Guid PaymentOrderId,
    string PaymentReference,
    string CancellationReason) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}