using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record ATMTransactionInitiatedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    ATMTransactionType TransactionType,
    Money Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ATMTransactionAuthorizedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    Money Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ATMTransactionCompletedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    ATMTransactionType TransactionType,
    Money Amount,
    Money AccountBalance) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ATMTransactionDeclinedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    string ResponseCode,
    string ResponseMessage) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ATMTransactionFailedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    string FailureReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ATMTransactionTimeoutDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ATMTransactionReversedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    Money Amount,
    string ReversalReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ATMPINVerificationFailedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid CustomerId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ATMTransactionMarkedSuspiciousDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    string Reason,
    string FraudScore) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}