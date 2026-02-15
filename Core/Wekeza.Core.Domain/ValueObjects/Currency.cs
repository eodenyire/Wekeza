namespace Wekeza.Core.Domain.ValueObjects;

public record Currency
/// <summary>
/// 📂 Wekeza.Core.Domain/ValueObjects
/// 1. Currency.cs (The Smart Enum Pattern)
/// We don't use strings for currencies. We use a Strongly Typed class to ensure that only supported ISO-4217 currencies exist in our system.
///</summary>
{
    public string Code { get; init; }
    public string Symbol { get; init; }
    public int DecimalPlaces { get; init; }

    private Currency(string code, string symbol, int decimalPlaces)
    {
        Code = code;
        Symbol = symbol;
        DecimalPlaces = decimalPlaces;
    }

    // Public constructor for deserialization and other use cases
    public Currency(string code) : this(code, GetSymbolForCode(code), GetDecimalPlacesForCode(code))
    {
    }

    public static readonly Currency KES = new("KES", "KSh", 2);
    public static readonly Currency USD = new("USD", "$", 2);
    public static readonly Currency EUR = new("EUR", "€", 2);
    public static readonly Currency GBP = new("GBP", "£", 2);

    public static Currency FromCode(string code) =>
        code.ToUpper() switch
        {
            "KES" => KES,
            "USD" => USD,
            "EUR" => EUR,
            "GBP" => GBP,
            _ => throw new ArgumentException($"Currency {code} is not supported by Wekeza Bank.")
        };

    private static string GetSymbolForCode(string code) =>
        code.ToUpper() switch
        {
            "KES" => "KSh",
            "USD" => "$",
            "EUR" => "€",
            "GBP" => "£",
            _ => code
        };

    private static int GetDecimalPlacesForCode(string code) =>
        code.ToUpper() switch
        {
            "KES" => 2,
            "USD" => 2,
            "EUR" => 2,
            "GBP" => 2,
            _ => 2
        };

    // String comparison operators for compatibility
    public static bool operator ==(Currency currency, string code)
    {
        return currency?.Code == code;
    }

    public static bool operator !=(Currency currency, string code)
    {
        return currency?.Code != code;
    }

    public static bool operator ==(string code, Currency currency)
    {
        return currency?.Code == code;
    }

    public static bool operator !=(string code, Currency currency)
    {
        return currency?.Code != code;
    }

    public override string ToString() => Code;
}
