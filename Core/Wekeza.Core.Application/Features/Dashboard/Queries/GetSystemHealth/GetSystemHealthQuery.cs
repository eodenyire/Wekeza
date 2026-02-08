using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetSystemHealth;

public record GetSystemHealthQuery : IRequest<Result<SystemHealthDto>>
{
}

public class SystemHealthDto
{
    public string Status { get; set; } = string.Empty;
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public int ActiveSessions { get; set; }
    public DateTime LastChecked { get; set; }
}

public class GetSystemHealthHandler : IRequestHandler<GetSystemHealthQuery, Result<SystemHealthDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSystemHealthHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SystemHealthDto>> Handle(GetSystemHealthQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var health = new SystemHealthDto
            {
                Status = "Healthy",
                CpuUsage = 45.5,
                MemoryUsage = 60.2,
                DiskUsage = 70.8,
                ActiveSessions = 150,
                LastChecked = DateTime.UtcNow
            };

            await Task.CompletedTask;
            return Result<SystemHealthDto>.Success(health);
        }
        catch (Exception ex)
        {
            return Result<SystemHealthDto>.Failure($"Failed to get system health: {ex.Message}");
        }
    }
}
