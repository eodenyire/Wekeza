namespace Wekeza.Core.Application.Features.Accounts.Queries.GetBusinessProfile;

/// <summary>
/// Account summary data transfer object
/// </summary>
public record AccountSummaryDto
{
    public Guid AccountId { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public string AccountType { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public decimal AvailableBalance { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime OpenedDate { get; init; }
    public DateTime LastTransactionDate { get; init; }
    public string BranchCode { get; init; } = string.Empty;
    public string BranchName { get; init; } = string.Empty;
}