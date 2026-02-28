using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Exceptions;
/// <summary>
/// 📂 Wekeza.Core.Domain/Exceptions
/// We will now generate the specific exceptions that protect the integrity of the Wekeza Ledger. All of these inherit from the DomainException base class we built in the Common folder.
/// 1. InsufficientFundsException.cs
/// The most common banking error. We provide context (Account Number and Attempted Amount) so the UI can show a helpful message without needing to query the balance again.
/// </summary>
public class InsufficientFundsException : DomainException
{
    public AccountNumber AccountNumber { get; }
    public Money AttemptedAmount { get; }

    public InsufficientFundsException(AccountNumber accountNumber, Money attemptedAmount)
        : base($"Account {accountNumber.Value} has insufficient funds for a transaction of {attemptedAmount}.", "INSUFFICIENT_FUNDS")
    {
        AccountNumber = accountNumber;
        AttemptedAmount = attemptedAmount;
    }
}
