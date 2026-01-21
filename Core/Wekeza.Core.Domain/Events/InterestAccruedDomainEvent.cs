using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record InterestAccruedDomainEvent(
    Guid AccountId,
    string AccountNumber,
    Money InterestAmount,
    DateTime AccrualDate,
    string AccrualPeriod) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}