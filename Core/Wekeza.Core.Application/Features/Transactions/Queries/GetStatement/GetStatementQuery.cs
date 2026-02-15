using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Transactions.Queries.GetStatement;

/// <summary>
/// Query to get account statement
/// </summary>
public record GetStatementQuery : IRequest<Result<StatementDto>>
{
    public Guid AccountId { get; init; }
    public string? AccountNumber { get; init; }
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
    public string? TransactionType { get; init; }
}