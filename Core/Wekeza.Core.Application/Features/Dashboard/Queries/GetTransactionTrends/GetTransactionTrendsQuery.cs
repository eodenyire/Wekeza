using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetTransactionTrends;

public record GetTransactionTrendsQuery : IRequest<Result<TransactionTrendsDto>>
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string Period { get; init; } = "daily";
    public int Days { get; init; } = 30;
}

public class TransactionTrendsDto
{
    public decimal TotalVolume { get; set; }
    public int TotalCount { get; set; }
    public List<DailyTrend> DailyTrends { get; set; } = new();
}

public class DailyTrend
{
    public DateTime Date { get; set; }
    public decimal Volume { get; set; }
    public int Count { get; set; }
}

public class GetTransactionTrendsHandler : IRequestHandler<GetTransactionTrendsQuery, Result<TransactionTrendsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTransactionTrendsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TransactionTrendsDto>> Handle(GetTransactionTrendsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var trends = new TransactionTrendsDto();
            return Result<TransactionTrendsDto>.Success(trends);
        }
        catch (Exception ex)
        {
            return Result<TransactionTrendsDto>.Failure($"Failed to get transaction trends: {ex.Message}");
        }
    }
}
