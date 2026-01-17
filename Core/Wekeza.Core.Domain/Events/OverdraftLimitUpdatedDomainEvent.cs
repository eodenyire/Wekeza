using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record OverdraftLimitUpdatedDomainEvent(
    Guid AccountId,
    Money NewLimit,
    string UpdatedBy) : IDomainEvent;