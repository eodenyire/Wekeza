namespace Wekeza.Core.Application.Features.Transactions.Queries.GetStatement;

/// <summary>
/// Statement data transfer object
/// </summary>
public record StatementDto
{
    public Guid AccountId { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public string AccountName { get; init; } = string.Empty;
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public decimal OpeningBalance { get; init; }
    public decimal ClosingBalance { get; init; }
    public List<TransactionHistoryDto> Transactions { get; init; } = new();
    public int TotalTransactions { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}