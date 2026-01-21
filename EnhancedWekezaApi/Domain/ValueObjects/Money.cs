using System.ComponentModel.DataAnnotations.Schema;

namespace EnhancedWekezaApi.Domain.ValueObjects;

public class Money
{
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
        
        Amount = amount;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    public Money Add(Money other)
    {
        if (Currency.Code != other.Currency.Code)
            throw new InvalidOperationException("Cannot add money with different currencies");
        
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency.Code != other.Currency.Code)
            throw new InvalidOperationException("Cannot subtract money with different currencies");
        
        if (Amount < other.Amount)
            throw new InvalidOperationException("Insufficient funds");
        
        return new Money(Amount - other.Amount, Currency);
    }

    public bool IsZero() => Amount == 0;
    public bool IsNegative() => Amount < 0;

    public override string ToString() => $"{Amount:C} {Currency.Code}";
}

public class Currency
{
    public string Code { get; private set; }
    public string Name { get; private set; }

    public Currency(string code, string name = "")
    {
        Code = code?.ToUpper() ?? throw new ArgumentNullException(nameof(code));
        Name = string.IsNullOrEmpty(name) ? code : name;
    }

    public static Currency FromCode(string code) => code?.ToUpper() switch
    {
        "KES" => new Currency("KES", "Kenyan Shilling"),
        "USD" => new Currency("USD", "US Dollar"),
        "EUR" => new Currency("EUR", "Euro"),
        "GBP" => new Currency("GBP", "British Pound"),
        _ => new Currency(code ?? "KES", code ?? "Kenyan Shilling")
    };

    public override string ToString() => Code;
}