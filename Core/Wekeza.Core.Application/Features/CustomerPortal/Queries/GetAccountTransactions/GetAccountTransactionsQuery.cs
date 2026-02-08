using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Queries.GetAccountTransactions;

public record GetAccountTransactionsQuery : IRequest<Result<PaginatedList<AccountTransactionDto>>>
{
    public Guid AccountId { get; init; }
    public int PageSize { get; init; } = 20;
    public int PageNumber { get; init; } = 1;
}

public class AccountTransactionDto
{
    public Guid TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }
    public string Reference { get; set; } = string.Empty;
}

public class GetAccountTransactionsHandler : IRequestHandler<GetAccountTransactionsQuery, Result<PaginatedList<AccountTransactionDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAccountTransactionsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<AccountTransactionDto>>> Handle(GetAccountTransactionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var transactions = new List<AccountTransactionDto>();
            var paginatedResult = new PaginatedList<AccountTransactionDto>(transactions, transactions.Count, request.PageNumber, request.PageSize);
            return Result<PaginatedList<AccountTransactionDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<AccountTransactionDto>>.Failure($"Failed to get account transactions: {ex.Message}");
        }
    }
}
