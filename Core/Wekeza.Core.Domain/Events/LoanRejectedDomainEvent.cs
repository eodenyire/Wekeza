using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

public record LoanRejectedDomainEvent(
    Guid LoanId,
    string LoanNumber,
    string Reason,
    string RejectedBy) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}