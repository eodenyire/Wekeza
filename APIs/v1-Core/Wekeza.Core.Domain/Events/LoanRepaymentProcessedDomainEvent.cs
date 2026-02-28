using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record LoanRepaymentProcessedDomainEvent(
    Guid LoanId,
    string LoanNumber,
    Money PaymentAmount,
    DateTime PaymentDate,
    string? PaymentReference) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}