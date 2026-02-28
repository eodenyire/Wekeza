using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record LoanApprovedDomainEvent(
    Guid LoanId,
    string LoanNumber,
    Money Principal,
    string ApprovedBy) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}