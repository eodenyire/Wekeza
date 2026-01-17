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
}
