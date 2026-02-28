using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wekeza.Core.Infrastructure.Caching;

namespace Wekeza.Core.Infrastructure.BackgroundServices;

public class CacheWarmupBackgroundService : BackgroundService
{
    private readonly ILogger<CacheWarmupBackgroundService> _logger;
    private readonly ICacheService _cacheService;

    public CacheWarmupBackgroundService(
        ILogger<CacheWarmupBackgroundService> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Warm up cache on startup
        try
        {
            _logger.LogInformation("Starting cache warmup");
            // Add cache warmup logic here
            _logger.LogInformation("Cache warmup completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during cache warmup");
        }

        // Keep service running but don't do anything else
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}