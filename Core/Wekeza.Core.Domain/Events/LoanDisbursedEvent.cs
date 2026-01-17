///📂 Wekeza.Core.Domain/Events/LoanDisbursedEvent.cs
/// This is a "Notification" object. It carries just enough data so that other systems can react without needing to query the entire database again.
///
///
using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

/// <summary>
/// Triggered when funds are physically moved from the Bank to the Customer's account.
/// This event is the fuel for SMS notifications and Ledger Audit trails.
/// </summary>
public class LoanDisbursedEvent : DomainEvent
{
    public Guid LoanId { get; }
    public Guid AccountId { get; }
    public decimal PrincipalAmount { get; }
    public string Currency { get; }
    public DateTime OccurredOn { get; }

    public LoanDisbursedEvent(Guid loanId, Guid accountId, decimal amount, string currency)
    {
        LoanId = loanId;
        AccountId = accountId;
        PrincipalAmount = amount;
        Currency = currency;
        OccurredOn = DateTime.UtcNow;
    }
}
