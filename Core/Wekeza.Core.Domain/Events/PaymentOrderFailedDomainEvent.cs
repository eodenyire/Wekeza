using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

public record PaymentOrderFailedDomainEvent(
    Guid PaymentOrderId,
    string PaymentReference,
    string FailureReason) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}