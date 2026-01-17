using Wekeza.Core.Domain.Common;
///<summary>
/// 2. Money.cs (The DNA of the Bank)
/// This object handles all financial math. It prevents the "Currency Mismatch" bug by throwing an exception if you try to add USD to KES. It also enforces Banker's Rounding (MidpointRounding.ToEven).
///</summary>
namespace Wekeza.Core.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public Currency Currency { get; init; }

    public Money(decimal amount, Currency currency)
    {
        Amount = Math.Round(amount, currency.DecimalPlaces, MidpointRounding.ToEven);
        Currency = currency;
    }

    public static Money Zero(Currency currency) => new(0, currency);

    // Operator Overloading: The "Statement" of a Fullstack Architect
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot add amounts with different currencies.", "CURRENCY_MISMATCH");
        
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot subtract amounts with different currencies.", "CURRENCY_MISMATCH");
        
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public bool IsGreaterThan(Money other) => Amount > other.Amount;
    public bool IsNegative() => Amount < 0;
    public bool IsZero() => Amount == 0;

    public override string ToString() => $"{Currency.Symbol} {Amount:N2}";
}
