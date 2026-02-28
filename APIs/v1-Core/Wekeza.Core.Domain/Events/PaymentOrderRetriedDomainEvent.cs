using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

public record PaymentOrderRetriedDomainEvent(
    Guid PaymentOrderId,
    string PaymentReference,
    int RetryCount) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}