using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Queries.GetTransactions;

public record GetTransactionsQuery : IRequest<Result<List<TransactionDto>>>
{
    public string AccountNumber { get; init; } = string.Empty;
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public class TransactionDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}

public class GetTransactionsHandler : IRequestHandler<GetTransactionsQuery, Result<List<TransactionDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetTransactionsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TransactionDto>>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var transactions = new List<TransactionDto>();
            return Result<List<TransactionDto>>.Success(transactions);
        }
        catch (Exception ex)
        {
            return Result<List<TransactionDto>>.Failure($"Failed to get transactions: {ex.Message}");
        }
    }
}
