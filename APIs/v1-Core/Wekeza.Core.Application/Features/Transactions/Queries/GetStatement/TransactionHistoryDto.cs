namespace Wekeza.Core.Application.Features.Transactions.Queries.GetStatement;

/// <summary>
/// Transaction history data transfer object
/// </summary>
public record TransactionHistoryDto
{
    public Guid TransactionId { get; init; }
    public DateTime TransactionDate { get; init; }
    public DateTime ValueDate { get; init; }
    public string TransactionType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Reference { get; init; } = string.Empty;
    public decimal DebitAmount { get; init; }
    public decimal CreditAmount { get; init; }
    public decimal Balance { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Channel { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? Remarks { get; init; }
}