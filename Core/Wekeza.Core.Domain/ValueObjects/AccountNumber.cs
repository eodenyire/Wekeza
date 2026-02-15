using System.Text.RegularExpressions;
///<summary>
/// 3. AccountNumber.cs (The Security Checksum)
/// To prevent typos, every account number in Wekeza Bank follows a validation pattern. We've future-proofed this to support both internal 10-digit formats and international IBANs.
///</summary>

namespace Wekeza.Core.Domain.ValueObjects;

public record AccountNumber
{
    public string Value { get; init; } = string.Empty;

    // Parameterless constructor for EF Core
    private AccountNumber() { }

    public AccountNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^[A-Z0-9]{10,34}$"))
        {
            throw new ArgumentException("Invalid Account Number format.");
        }
        // Future: Add Mod-97 checksum validation here
        Value = value;
    }

    public static implicit operator string(AccountNumber accountNumber) => accountNumber.Value;
}
