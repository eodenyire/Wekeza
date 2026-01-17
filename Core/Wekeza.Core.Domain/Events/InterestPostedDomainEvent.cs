using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record InterestPostedDomainEvent(
    Guid AccountId,
    Money InterestAmount) : IDomainEvent;