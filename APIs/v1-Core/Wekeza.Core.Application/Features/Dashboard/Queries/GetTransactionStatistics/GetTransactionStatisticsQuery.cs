using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetTransactionStatistics;

public record GetTransactionStatisticsQuery : IRequest<Result<TransactionStatisticsDto>>
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
}

public class TransactionStatisticsDto
{
    public decimal TotalVolume { get; set; }
    public int TotalCount { get; set; }
    public Dictionary<string, int> TransactionsByType { get; set; } = new();
    public Dictionary<string, decimal> VolumeByType { get; set; } = new();
}

public class GetTransactionStatisticsHandler : IRequestHandler<GetTransactionStatisticsQuery, Result<TransactionStatisticsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTransactionStatisticsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TransactionStatisticsDto>> Handle(GetTransactionStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var statistics = new TransactionStatisticsDto();
            return Result<TransactionStatisticsDto>.Success(statistics);
        }
        catch (Exception ex)
        {
            return Result<TransactionStatisticsDto>.Failure($"Failed to get transaction statistics: {ex.Message}");
        }
    }
}
