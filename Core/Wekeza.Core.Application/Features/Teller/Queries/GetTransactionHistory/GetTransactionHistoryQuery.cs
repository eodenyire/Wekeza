using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Queries.GetTransactionHistory;

public record GetTransactionHistoryQuery : IRequest<Result<List<TransactionHistoryDto>>>
{
    public string AccountNumber { get; init; } = string.Empty;
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

public class TransactionHistoryDto
{
    public Guid TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
}

public class GetTransactionHistoryHandler : IRequestHandler<GetTransactionHistoryQuery, Result<List<TransactionHistoryDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetTransactionHistoryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TransactionHistoryDto>>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var transactions = new List<TransactionHistoryDto>();
            await Task.CompletedTask;
            return Result<List<TransactionHistoryDto>>.Success(transactions);
        }
        catch (Exception ex)
        {
            return Result<List<TransactionHistoryDto>>.Failure($"Failed to get transaction history: {ex.Message}");
        }
    }
}
