using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

public record AccountUnfrozenDomainEvent(
    Guid AccountId,
    string UnfrozenBy) : IDomainEvent;