using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Events;

public record PaymentOrderCreatedDomainEvent(
    Guid PaymentOrderId,
    string PaymentReference,
    PaymentType PaymentType) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}