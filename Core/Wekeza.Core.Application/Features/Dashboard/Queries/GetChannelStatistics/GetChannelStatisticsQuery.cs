using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetChannelStatistics;

public record GetChannelStatisticsQuery : IRequest<Result<ChannelStatisticsDto>>
{
}

public class ChannelStatisticsDto
{
    public int TotalMobileBankingUsers { get; set; }
    public int TotalInternetBankingUsers { get; set; }
    public int TotalUSSDUsers { get; set; }
    public int ActiveMobileSessions { get; set; }
    public decimal MobileTransactionVolume { get; set; }
}

public class GetChannelStatisticsHandler : IRequestHandler<GetChannelStatisticsQuery, Result<ChannelStatisticsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetChannelStatisticsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ChannelStatisticsDto>> Handle(GetChannelStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var stats = new ChannelStatisticsDto();
            return Result<ChannelStatisticsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            return Result<ChannelStatisticsDto>.Failure($"Failed to get channel statistics: {ex.Message}");
        }
    }
}
