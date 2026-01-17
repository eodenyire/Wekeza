using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record FundsWithdrawnDomainEvent(
    Guid AccountId,
    Money Amount,
    string TransactionReference,
    string Description) : IDomainEvent;