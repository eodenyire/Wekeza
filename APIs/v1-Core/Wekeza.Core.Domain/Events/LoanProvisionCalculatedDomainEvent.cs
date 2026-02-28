using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record LoanProvisionCalculatedDomainEvent(
    Guid LoanId,
    string LoanNumber,
    Money ProvisionAmount,
    Money ProvisionChange) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}