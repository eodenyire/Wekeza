using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

public record PaymentOrderAuthorizedDomainEvent(
    Guid PaymentOrderId,
    string PaymentReference,
    string ApprovedBy) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}