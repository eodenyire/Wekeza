namespace EnhancedWekezaApi.Domain.ValueObjects;

public class AccountNumber
{
    public string Value { get; private set; }

    public AccountNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Account number cannot be empty", nameof(value));
        
        Value = value.Trim().ToUpper();
    }

    public static AccountNumber Generate()
    {
        return new AccountNumber($"WKZ-{Guid.NewGuid().ToString()[..8].ToUpper()}");
    }

    public override string ToString() => Value;
    
    public static implicit operator string(AccountNumber accountNumber) => accountNumber.Value;
    public static implicit operator AccountNumber(string value) => new(value);
}