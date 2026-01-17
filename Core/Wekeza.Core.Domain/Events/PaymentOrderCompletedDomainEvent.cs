using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record PaymentOrderCompletedDomainEvent(
    Guid PaymentOrderId,
    string PaymentReference,
    Money Amount,
    DateTime SettledDate) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}