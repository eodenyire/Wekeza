using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record FeeAppliedDomainEvent(
    Guid AccountId,
    Money FeeAmount,
    string FeeType,
    string Description) : IDomainEvent;