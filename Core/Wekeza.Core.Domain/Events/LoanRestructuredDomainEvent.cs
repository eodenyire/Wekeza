using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

public record LoanRestructuredDomainEvent(
    Guid LoanId,
    string LoanNumber,
    string Reason,
    string RestructuredBy) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}