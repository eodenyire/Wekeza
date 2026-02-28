using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Exceptions;
///<summary>
/// 📂 Wekeza.Core.Domain/Exceptions
/// 3. CurrencyMismatchException.cs
/// This protects the DNA of Wekeza. It is thrown by the Money Value Object if someone tries to add, for example, KES to USD without a conversion service.
///</summary>
public class CurrencyMismatchException : DomainException
{
    public CurrencyMismatchException(Currency expected, Currency actual)
        : base($"Cannot perform operations on different currencies. Expected: {expected.Code}, but got: {actual.Code}.", "CURRENCY_MISMATCH")
    {
    }
}
