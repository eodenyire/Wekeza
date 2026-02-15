using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

/// <summary>
/// Domain event raised when a transfer is completed
/// </summary>
public class TransferCompletedEvent : DomainEvent
{
    public Guid TransferId { get; }
    public Guid FromAccountId { get; }
    public Guid ToAccountId { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public string TransferType { get; }
    public string Reference { get; }
    public DateTime CompletedAt { get; }

    public TransferCompletedEvent(
        Guid transferId,
        Guid fromAccountId,
        Guid toAccountId,
        decimal amount,
        string currency,
        string transferType,
        string reference,
        DateTime completedAt)
    {
        TransferId = transferId;
        FromAccountId = fromAccountId;
        ToAccountId = toAccountId;
        Amount = amount;
        Currency = currency;
        TransferType = transferType;
        Reference = reference;
        CompletedAt = completedAt;
    }
}