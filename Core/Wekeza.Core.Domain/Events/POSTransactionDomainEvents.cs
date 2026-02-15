using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Events;

public record POSTransactionInitiatedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    string MerchantId,
    POSTransactionType TransactionType,
    Money Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record POSTransactionAuthorizedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    string MerchantId,
    Money Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record POSTransactionCompletedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    string MerchantId,
    POSTransactionType TransactionType,
    Money Amount,
    Money AccountBalance) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record POSTransactionDeclinedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    string MerchantId,
    string ResponseCode,
    string ResponseMessage) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record POSTransactionFailedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    string MerchantId,
    string FailureReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record POSTransactionReversedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    string MerchantId,
    Money Amount,
    string ReversalReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record POSTransactionRefundedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    string MerchantId,
    Money RefundAmount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record POSTransactionSettledDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    string MerchantId,
    Money Amount,
    string SettlementBatchId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record POSPINVerificationFailedDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid CustomerId,
    string MerchantId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record POSTransactionMarkedSuspiciousDomainEvent(
    Guid TransactionId,
    Guid CardId,
    Guid AccountId,
    string MerchantId,
    string Reason,
    string FraudScore) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}