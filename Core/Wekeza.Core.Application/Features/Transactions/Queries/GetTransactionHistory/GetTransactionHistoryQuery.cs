using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Transactions.Queries.GetTransactionHistory;
///
/// 2. The Request: GetTransactionHistoryQuery.cs
/// We include PageNumber and PageSize. This is Future-Proofing 101. You never return an unbounded list of transactions in a production bank.
public record GetTransactionHistoryQuery : IQuery<PaginatedList<TransactionDto>>
{
    public string AccountNumber { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}
