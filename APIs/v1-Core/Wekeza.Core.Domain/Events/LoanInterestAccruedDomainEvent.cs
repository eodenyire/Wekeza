using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record LoanInterestAccruedDomainEvent(
    Guid LoanId,
    string LoanNumber,
    Money InterestAmount,
    DateTime CalculationDate) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}