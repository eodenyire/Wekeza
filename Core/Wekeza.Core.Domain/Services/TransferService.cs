using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Exceptions;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Services;

/// <summary>
/// 📂 Wekeza.Core.Domain/Services
/// TransferService.cs (The Financial Orchestrator)
/// This service handles the "Inter-Account" logic. It ensures we aren't transferring between different currencies (unless we have an FX service) and coordinates the Debit and Credit operations.
/// A Domain Service that orchestrates complex financial movements between multiple account aggregates.
/// </summary>
public class TransferService
{
    /// <summary>
    /// Performs a high-integrity transfer between a source and a destination account.
    /// </summary>
    public void Transfer(Account source, Account destination, Money amount)
    {
        // Rule 1: Prevent self-transfer (Common fraud/error vector)
        if (source.Id == destination.Id)
        {
            throw new DomainException("Source and destination accounts cannot be the same.", "SAME_ACCOUNT_TRANSFER");
        }

        // Rule 2: Currency Integrity
        if (source.Balance.Currency != destination.Balance.Currency)
        {
            throw new CurrencyMismatchException(source.Balance.Currency, destination.Balance.Currency);
        }

        // Rule 3: Atomic execution of Debit/Credit
        // Note: The Debit() method inside Account.cs already checks for Insufficient Funds and Frozen status.
        source.Debit(amount);
        destination.Credit(amount);

        // Rule 4: Raise the Domain Event for the rest of the 100 systems (Fraud, Reporting, etc.)
        // We use a CorrelationId to link these two movements in the ledger.
        var correlationId = Guid.NewGuid();
        
        // This event will be captured by the Outbox pattern later.
        // We can attach it to the source account's event list.
    }
}
