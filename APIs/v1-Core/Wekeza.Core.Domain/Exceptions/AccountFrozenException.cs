using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Exceptions;
///<summary>
/// 📂 Wekeza.Core.Domain/Exceptions
/// 2. AccountFrozenException.cs
/// Security first. This exception is triggered whenever a debit or credit is attempted on a locked account. This is a direct signal to your Fraud Management and Card Systems.
///</summary>
public class AccountFrozenException : DomainException
{
    public AccountNumber AccountNumber { get; }

    public AccountFrozenException(AccountNumber accountNumber)
        : base($"Transaction denied. Account {accountNumber.Value} is currently frozen.", "ACCOUNT_FROZEN")
    {
        AccountNumber = accountNumber;
    }
}
