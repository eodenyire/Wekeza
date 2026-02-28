namespace EnhancedWekezaApi.Application.Features.Accounts.Queries.GetAccount;

/// <summary>
/// The public-facing representation of a Wekeza Bank Account.
/// Designed for high-speed serialization and zero data leakage.
/// </summary>
public record AccountDto
{
    public Guid Id { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsFrozen { get; init; }
    public string AccountType { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    
    // Customer information
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerNumber { get; init; } = string.Empty;
}