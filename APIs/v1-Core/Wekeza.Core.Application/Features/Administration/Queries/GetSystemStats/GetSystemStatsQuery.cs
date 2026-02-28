using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Administration.Queries.GetSystemStats;

public record GetSystemStatsQuery : IRequest<Result<SystemStatsDto>>
{
}

public class SystemStatsDto
{
    public int TotalCustomers { get; set; }
    public int TotalAccounts { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalDeposits { get; set; }
    public decimal TotalLoans { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalBranches { get; set; }
}

public class GetSystemStatsHandler : IRequestHandler<GetSystemStatsQuery, Result<SystemStatsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSystemStatsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SystemStatsDto>> Handle(GetSystemStatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var stats = new SystemStatsDto
            {
                TotalCustomers = 0,
                TotalAccounts = 0,
                TotalTransactions = 0,
                TotalDeposits = 0,
                TotalLoans = 0,
                ActiveUsers = 0,
                TotalBranches = 0
            };

            await Task.CompletedTask;
            return Result<SystemStatsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            return Result<SystemStatsDto>.Failure($"Failed to get system stats: {ex.Message}");
        }
    }
}
