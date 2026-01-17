using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Transactions.Queries.GetStatement;

/// <summary>
/// Principal-Grade Query for retrieving paginated account statements.
/// 📂 Wekeza.Core.Application/Features/Transactions/Queries/GetStatement
/// 1. GetStatementQuery.cs (The Request)
/// We include pagination parameters (PageNumber, PageSize) and a date range. This allows the UI to load "infinite scroll" statements without overloading the server.
/// </summary>
public record GetStatementQuery : IQuery<StatementResponseDto>
{
    public string AccountNumber { get; init; } = string.Empty;
    public DateTime FromDate { get; init; } = DateTime.UtcNow.AddMonths(-1);
    public DateTime ToDate { get; init; } = DateTime.UtcNow;
    
    // Pagination for the "Beast" scale
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    
    // Optional: Filter by specific type (Deposit, Withdrawal, etc.)
    public string? TransactionType { get; init; }
}
