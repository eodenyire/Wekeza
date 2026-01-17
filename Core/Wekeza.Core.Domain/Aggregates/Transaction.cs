using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
namespace Wekeza.Core.Domain.Aggregates;
///<summary>
/// 3. Transaction.cs (The Immutable Record)
/// A transaction is a historical fact. In Wekeza Bank, once a transaction is recorded, it is never modified. Corrections are handled by Reversal Transactions, just like in Finacle.
///</summary>
public class Transaction : Entity
{
    public Guid CorrelationId { get; private set; } // Links Dr/Cr pairs
    public Guid AccountId { get; private set; }
    public Money Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public string Description { get; private set; }
    public DateTime Timestamp { get; private set; }

    private Transaction() : base(Guid.NewGuid()) { }

    public Transaction(Guid id, Guid correlationId, Guid accountId, Money amount, TransactionType type, string description) 
        : base(id)
    {
        CorrelationId = correlationId;
        AccountId = accountId;
        Amount = amount;
        Type = type;
        Description = description;
        Timestamp = DateTime.UtcNow;
    }
}

public enum TransactionType { Deposit, Withdrawal, Transfer, Fee, Interest }
