using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wekeza.Core.Infrastructure.Monitoring;

namespace Wekeza.Core.Infrastructure.BackgroundServices;

public class PerformanceMonitoringBackgroundService : BackgroundService
{
    private readonly ILogger<PerformanceMonitoringBackgroundService> _logger;
    private readonly IPerformanceMonitoringService _performanceMonitoringService;

    public PerformanceMonitoringBackgroundService(
        ILogger<PerformanceMonitoringBackgroundService> logger,
        IPerformanceMonitoringService performanceMonitoringService)
    {
        _logger = logger;
        _performanceMonitoringService = performanceMonitoringService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Monitor performance metrics every 30 seconds
                // TODO: Implement LogPerformanceMetricAsync on IPerformanceMonitoringService
                // await _performanceMonitoringService.LogPerformanceMetricAsync("system.health", 1.0);
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in performance monitoring background service");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}