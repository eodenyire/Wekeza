using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record LoanCollateralAddedDomainEvent(
    Guid LoanId,
    string LoanNumber,
    string CollateralType,
    Money Value) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}