using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetAccountStatistics;

public record GetAccountStatisticsQuery : IRequest<Result<AccountStatisticsDto>>
{
}

public class AccountStatisticsDto
{
    public int TotalAccounts { get; set; }
    public int ActiveAccounts { get; set; }
    public int DormantAccounts { get; set; }
    public int InactiveAccounts { get; set; }
    public int FrozenAccounts { get; set; }
    public decimal TotalBalance { get; set; }
    public decimal AverageBalance { get; set; }
    public Dictionary<string, int> AccountsByType { get; set; } = new();
    public Dictionary<string, int> AccountsByBranch { get; set; } = new();
    public List<TopAccountDto> TopAccountsByBalance { get; set; } = new();
}

public class TopAccountDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}

public class GetAccountStatisticsHandler : IRequestHandler<GetAccountStatisticsQuery, Result<AccountStatisticsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAccountStatisticsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AccountStatisticsDto>> Handle(GetAccountStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var stats = new AccountStatisticsDto();
            return Result<AccountStatisticsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            return Result<AccountStatisticsDto>.Failure($"Failed to get account statistics: {ex.Message}");
        }
    }
}
