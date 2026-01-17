using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Events;

public record CardIssuedDomainEvent(
    Guid CardId,
    Guid CustomerId,
    Guid AccountId,
    CardType CardType) : IDomainEvent;

public record CardActivatedDomainEvent(
    Guid CardId,
    Guid CustomerId,
    Guid AccountId) : IDomainEvent;

public record CardPINSetDomainEvent(
    Guid CardId,
    Guid CustomerId) : IDomainEvent;

public record CardPINBlockedDomainEvent(
    Guid CardId,
    Guid CustomerId,
    int FailedAttempts) : IDomainEvent;

public record CardBlockedDomainEvent(
    Guid CardId,
    Guid CustomerId,
    string Reason) : IDomainEvent;

public record CardUnblockedDomainEvent(
    Guid CardId,
    Guid CustomerId,
    string Reason) : IDomainEvent;

public record CardCancelledDomainEvent(
    Guid CardId,
    Guid CustomerId,
    Guid AccountId,
    string Reason) : IDomainEvent;

public record CardReplacedDomainEvent(
    Guid OldCardId,
    Guid NewCardId,
    Guid CustomerId,
    string Reason) : IDomainEvent;

public record CardChannelControlsUpdatedDomainEvent(
    Guid CardId,
    Guid CustomerId) : IDomainEvent;

public record CardLimitsUpdatedDomainEvent(
    Guid CardId,
    Guid CustomerId) : IDomainEvent;

public record CardHotlistedDomainEvent(
    Guid CardId,
    Guid CustomerId,
    string Reason) : IDomainEvent;

public record CardRemovedFromHotlistDomainEvent(
    Guid CardId,
    Guid CustomerId) : IDomainEvent;

public record CardDeliveredDomainEvent(
    Guid CardId,
    Guid CustomerId) : IDomainEvent;