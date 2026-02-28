namespace Wekeza.Core.Infrastructure.Caching;

/// <summary>
/// Cache service interface for distributed caching operations
/// Provides enterprise-grade caching capabilities with Redis backend
/// </summary>
public interface ICacheService
{
    // Basic Cache Operations
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task SetStringAsync(string key, string value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    // Batch Operations
    Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default);
    Task SetManyAsync<T>(Dictionary<string, T> keyValuePairs, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    // Pattern-based Operations
    Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    // Expiration Management
    Task<bool> ExpireAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);
    Task<TimeSpan?> GetTtlAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> PersistAsync(string key, CancellationToken cancellationToken = default);

    // Hash Operations (for complex objects)
    Task<T?> HashGetAsync<T>(string key, string field, CancellationToken cancellationToken = default);
    Task HashSetAsync<T>(string key, string field, T value, CancellationToken cancellationToken = default);
    Task<Dictionary<string, T?>> HashGetAllAsync<T>(string key, CancellationToken cancellationToken = default);
    Task HashDeleteAsync(string key, string field, CancellationToken cancellationToken = default);

    // List Operations (for collections)
    Task<long> ListPushAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    Task<T?> ListPopAsync<T>(string key, CancellationToken cancellationToken = default);
    Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default);
    Task<long> ListLengthAsync(string key, CancellationToken cancellationToken = default);

    // Set Operations (for unique collections)
    Task<bool> SetAddAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    Task<bool> SetRemoveAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    Task<bool> SetContainsAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    Task<List<T>> SetMembersAsync<T>(string key, CancellationToken cancellationToken = default);

    // Atomic Operations
    Task<long> IncrementAsync(string key, long value = 1, CancellationToken cancellationToken = default);
    Task<long> DecrementAsync(string key, long value = 1, CancellationToken cancellationToken = default);
    Task<double> IncrementFloatAsync(string key, double value, CancellationToken cancellationToken = default);

    // Lock Operations (for distributed locking)
    Task<bool> AcquireLockAsync(string lockKey, TimeSpan expiration, string lockValue, CancellationToken cancellationToken = default);
    Task<bool> ReleaseLockAsync(string lockKey, string lockValue, CancellationToken cancellationToken = default);
    Task<bool> ExtendLockAsync(string lockKey, string lockValue, TimeSpan expiration, CancellationToken cancellationToken = default);

    // Cache Statistics
    Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
    Task<long> GetDatabaseSizeAsync(CancellationToken cancellationToken = default);
    Task FlushDatabaseAsync(CancellationToken cancellationToken = default);

    // Health Check
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
    Task<string> PingAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Cache statistics for monitoring and optimization
/// </summary>
public class CacheStatistics
{
    public long TotalKeys { get; set; }
    public long UsedMemory { get; set; }
    public long MaxMemory { get; set; }
    public double MemoryUsagePercentage { get; set; }
    public long TotalConnections { get; set; }
    public long TotalCommandsProcessed { get; set; }
    public double HitRate { get; set; }
    public double MissRate { get; set; }
    public TimeSpan Uptime { get; set; }
    public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
}