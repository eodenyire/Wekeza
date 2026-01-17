using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.ValueObjects;

public class ExchangeRate : ValueObject
{
    public string BaseCurrency { get; }
    public string QuoteCurrency { get; }
    public decimal Rate { get; }
    public decimal Spread { get; }
    public DateTime Timestamp { get; }
    public string Source { get; }

    public ExchangeRate(string baseCurrency, string quoteCurrency, decimal rate, decimal spread = 0, string source = "SYSTEM")
    {
        if (string.IsNullOrWhiteSpace(baseCurrency))
            throw new ArgumentException("Base currency cannot be empty", nameof(baseCurrency));

        if (string.IsNullOrWhiteSpace(quoteCurrency))
            throw new ArgumentException("Quote currency cannot be empty", nameof(quoteCurrency));

        if (baseCurrency == quoteCurrency)
            throw new ArgumentException("Base and quote currencies cannot be the same");

        if (rate <= 0)
            throw new ArgumentException("Exchange rate must be positive", nameof(rate));

        if (spread < 0)
            throw new ArgumentException("Spread cannot be negative", nameof(spread));

        BaseCurrency = baseCurrency.ToUpper();
        QuoteCurrency = quoteCurrency.ToUpper();
        Rate = rate;
        Spread = spread;
        Timestamp = DateTime.UtcNow;
        Source = source ?? "SYSTEM";
    }

    public decimal BidRate => Rate - (Spread / 2);
    public decimal OfferRate => Rate + (Spread / 2);
    public string CurrencyPair => $"{BaseCurrency}/{QuoteCurrency}";

    public ExchangeRate Invert()
    {
        if (Rate == 0)
            throw new InvalidOperationException("Cannot invert zero rate");

        var invertedRate = 1 / Rate;
        var invertedSpread = Spread / (Rate * Rate); // Spread adjustment for inverted rate

        return new ExchangeRate(QuoteCurrency, BaseCurrency, invertedRate, invertedSpread, Source);
    }

    public ExchangeRate Cross(ExchangeRate other)
    {
        if (QuoteCurrency != other.BaseCurrency)
            throw new ArgumentException($"Cannot cross {CurrencyPair} with {other.CurrencyPair}");

        var crossRate = Rate * other.Rate;
        var crossSpread = Spread + other.Spread; // Simplified spread calculation

        return new ExchangeRate(BaseCurrency, other.QuoteCurrency, crossRate, crossSpread, $"{Source}+{other.Source}");
    }

    public Money Convert(Money amount)
    {
        if (amount.Currency != BaseCurrency)
            throw new ArgumentException($"Amount currency {amount.Currency} does not match base currency {BaseCurrency}");

        var convertedAmount = amount.Amount * Rate;
        return new Money(convertedAmount, QuoteCurrency);
    }

    public bool IsStale(TimeSpan maxAge)
    {
        return DateTime.UtcNow - Timestamp > maxAge;
    }

    public bool IsWithinSpread(decimal testRate)
    {
        return testRate >= BidRate && testRate <= OfferRate;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return BaseCurrency;
        yield return QuoteCurrency;
        yield return Rate;
        yield return Spread;
        yield return Timestamp;
        yield return Source;
    }

    public override string ToString()
    {
        return $"{CurrencyPair} {Rate:F6} (±{Spread:F6}) @ {Timestamp:yyyy-MM-dd HH:mm:ss} [{Source}]";
    }

    // Common currency pairs
    public static ExchangeRate USDKES(decimal rate, decimal spread = 0.01m) => new("USD", "KES", rate, spread);
    public static ExchangeRate EURKES(decimal rate, decimal spread = 0.01m) => new("EUR", "KES", rate, spread);
    public static ExchangeRate GBPKES(decimal rate, decimal spread = 0.01m) => new("GBP", "KES", rate, spread);
    public static ExchangeRate EURUSD(decimal rate, decimal spread = 0.0001m) => new("EUR", "USD", rate, spread);
    public static ExchangeRate GBPUSD(decimal rate, decimal spread = 0.0001m) => new("GBP", "USD", rate, spread);
}