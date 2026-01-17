using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record LoanGuarantorAddedDomainEvent(
    Guid LoanId,
    string LoanNumber,
    Guid GuarantorId,
    Money GuaranteeAmount) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}