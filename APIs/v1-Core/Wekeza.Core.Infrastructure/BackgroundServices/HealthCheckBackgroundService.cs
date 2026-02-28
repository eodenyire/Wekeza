using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Wekeza.Core.Infrastructure.BackgroundServices;

public class HealthCheckBackgroundService : BackgroundService
{
    private readonly ILogger<HealthCheckBackgroundService> _logger;
    private readonly HealthCheckService _healthCheckService;

    public HealthCheckBackgroundService(
        ILogger<HealthCheckBackgroundService> logger,
        HealthCheckService healthCheckService)
    {
        _logger = logger;
        _healthCheckService = healthCheckService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var healthReport = await _healthCheckService.CheckHealthAsync(stoppingToken);
                
                if (healthReport.Status != HealthStatus.Healthy)
                {
                    _logger.LogWarning("Health check failed: {Status}", healthReport.Status);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in health check background service");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}