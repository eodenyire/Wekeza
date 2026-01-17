using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

public record AccountClosedDomainEvent(
    Guid AccountId,
    string Reason,
    string ClosedBy) : IDomainEvent;