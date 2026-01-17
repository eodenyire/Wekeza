using Wekeza.Core.Infrastructure.Monitoring;

namespace Wekeza.Core.Infrastructure.Services;

public class PerformanceMonitoringService : IPerformanceMonitoringService
{
    public Task<bool> IsHealthyAsync()
    {
        return Task.FromResult(true);
    }

    public Task LogPerformanceMetricAsync(string metricName, double value, Dictionary<string, string>? tags = null)
    {
        // Implementation for logging performance metrics
        return Task.CompletedTask;
    }

    public Task<Dictionary<string, object>> GetPerformanceMetricsAsync()
    {
        return Task.FromResult(new Dictionary<string, object>());
    }
}