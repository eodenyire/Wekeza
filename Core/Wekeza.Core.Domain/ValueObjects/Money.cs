using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Exceptions;
///<summary>
/// 2. Money.cs (The DNA of the Bank)
/// This object handles all financial math. It prevents the "Currency Mismatch" bug by throwing an exception if you try to add USD to KES. It also enforces Banker's Rounding (MidpointRounding.ToEven).
///</summary>
namespace Wekeza.Core.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public Currency Currency { get; init; }

    // Parameterless constructor for EF Core
    private Money() 
    { 
        Currency = Currency.KES; // Default currency
    }

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
            throw new CurrencyMismatchException(left.Currency, right.Currency);
        
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new CurrencyMismatchException(left.Currency, right.Currency);
        
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static bool operator <(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new CurrencyMismatchException(left.Currency, right.Currency);
        return left.Amount < right.Amount;
    }

    public static bool operator >(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new CurrencyMismatchException(left.Currency, right.Currency);
        return left.Amount > right.Amount;
    }

    public static bool operator <=(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new CurrencyMismatchException(left.Currency, right.Currency);
        return left.Amount <= right.Amount;
    }

    public static bool operator >=(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new CurrencyMismatchException(left.Currency, right.Currency);
        return left.Amount >= right.Amount;
    }

    public bool IsGreaterThan(Money other) => this > other;
    public bool IsLessThan(Money other) => this < other;
    public bool IsNegative() => Amount < 0;
    public bool IsZero() => Amount == 0;
    public bool IsPositive() => Amount > 0;
    public bool IsLessThanOrEqualTo(Money other) => this <= other;
    public bool IsGreaterThanOrEqual(Money other) => this >= other;
    public bool HasValue => true; // Money always has a value
    public decimal Value => Amount; // For compatibility with nullable patterns

    // Static methods
    public static Money Min(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new CurrencyMismatchException(left.Currency, right.Currency);
        return left.Amount <= right.Amount ? left : right;
    }

    public static Money Max(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new CurrencyMismatchException(left.Currency, right.Currency);
        return left.Amount >= right.Amount ? left : right;
    }

    // Unary minus operator
    public static Money operator -(Money money)
    {
        return new Money(-money.Amount, money.Currency);
    }

    public override string ToString() => $"{Currency.Symbol} {Amount:N2}";
}
