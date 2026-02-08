namespace Wekeza.Core.Infrastructure.ApiGateway;

/// <summary>
/// Cache statistics for API Gateway
/// </summary>
public class CacheStatistics
{
    public long TotalRequests { get; set; }
    public long CacheHits { get; set; }
    public long CacheMisses { get; set; }
    public double HitRate => TotalRequests > 0 ? (double)CacheHits / TotalRequests * 100 : 0;
    public long TotalCachedItems { get; set; }
    public long TotalMemoryUsed { get; set; }
    public DateTime LastUpdated { get; set; }
}
