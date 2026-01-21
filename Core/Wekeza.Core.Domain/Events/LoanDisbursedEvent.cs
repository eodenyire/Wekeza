using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record LoanDisbursedEvent(
    Guid LoanId,
    string LoanNumber,
    Guid AccountId,
    Money DisbursedAmount,
    DateTime DisbursementDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}