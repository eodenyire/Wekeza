using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record InterestAccruedDomainEvent(
    Guid AccountId,
    Money InterestAmount,
    DateTime CalculationDate) : IDomainEvent;