using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetCardStatistics;

public record GetCardStatisticsQuery : IRequest<Result<CardStatisticsDto>>
{
}

public class CardStatisticsDto
{
    public int TotalCardsIssued { get; set; }
    public int ActiveCards { get; set; }
    public int BlockedCards { get; set; }
    public decimal TotalCardTransactions { get; set; }
    public decimal TotalCardSpending { get; set; }
}

public class GetCardStatisticsHandler : IRequestHandler<GetCardStatisticsQuery, Result<CardStatisticsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCardStatisticsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CardStatisticsDto>> Handle(GetCardStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var stats = new CardStatisticsDto
            {
                TotalCardsIssued = 0,
                ActiveCards = 0,
                BlockedCards = 0,
                TotalCardTransactions = 0,
                TotalCardSpending = 0
            };

            await Task.CompletedTask;
            return Result<CardStatisticsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            return Result<CardStatisticsDto>.Failure($"Failed to get card statistics: {ex.Message}");
        }
    }
}
