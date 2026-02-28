using Wekeza.Core.Domain.ValueObjects;

///📂 Phase 6: Treasury & FX (Foreign Exchange)
/// Since Wekeza Bank will handle different currencies, we need the Cross-Rate Engine.
/// 1. 📂 Common/Services/CurrencyConverter.cs
/// This is an Application-level service that calculates the conversion before a transfer is initiated.
///
///
///

namespace Wekeza.Core.Application.Common.Services;

public class CurrencyConverter
{
    // In a real system, rates are fetched from a Real-time FX Provider (Port)
    public Money Convert(Money source, Currency targetCurrency, decimal exchangeRate)
    {
        if (source.Currency == targetCurrency) return source;

        var convertedAmount = source.Amount * exchangeRate;
        return new Money(convertedAmount, targetCurrency);
    }
}
