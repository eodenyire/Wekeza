using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace Wekeza.Core.Infrastructure.Caching;

/// <summary>
/// Redis-based cache service implementation
/// Provides high-performance distributed caching with enterprise features
/// </summary>
public class RedisCacheService : ICacheService, IDisposable
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly RedisCacheOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(
        IConnectionMultiplexer redis,
        ILogger<RedisCacheService> logger,
        IOptions<RedisCacheOptions> options)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        
        _database = _redis.GetDatabase(_options.DatabaseId);
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    // Basic Cache Operations
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var value = await _database.StringGetAsync(cacheKey);
            
            if (!value.HasValue)
            {
                _logger.LogDebug("Cache miss for key: {Key}", cacheKey);
                return default;
            }

            _logger.LogDebug("Cache hit for key: {Key}", cacheKey);
            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
            return default;
        }
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var value = await _database.StringGetAsync(cacheKey);
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting string cache value for key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            var expiry = expiration ?? _options.DefaultExpiration;
            
            await _database.StringSetAsync(cacheKey, serializedValue, expiry);
            _logger.LogDebug("Cache set for key: {Key} with expiration: {Expiration}", cacheKey, expiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var expiry = expiration ?? _options.DefaultExpiration;
            
            await _database.StringSetAsync(cacheKey, value, expiry);
            _logger.LogDebug("Cache string set for key: {Key}", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting string cache value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            await _database.KeyDeleteAsync(cacheKey);
            _logger.LogDebug("Cache removed for key: {Key}", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            return await _database.KeyExistsAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return false;
        }
    }

    // Batch Operations
    public async Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKeys = keys.Select(GetCacheKey).ToArray();
            var values = await _database.StringGetAsync(cacheKeys);
            
            var result = new Dictionary<string, T?>();
            for (int i = 0; i < cacheKeys.Length; i++)
            {
                var originalKey = keys.ElementAt(i);
                if (values[i].HasValue)
                {
                    result[originalKey] = JsonSerializer.Deserialize<T>(values[i]!, _jsonOptions);
                }
                else
                {
                    result[originalKey] = default;
                }
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting multiple cache values");
            return new Dictionary<string, T?>();
        }
    }

    public async Task SetManyAsync<T>(Dictionary<string, T> keyValuePairs, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var expiry = expiration ?? _options.DefaultExpiration;
            var tasks = keyValuePairs.Select(async kvp =>
            {
                var cacheKey = GetCacheKey(kvp.Key);
                var serializedValue = JsonSerializer.Serialize(kvp.Value, _jsonOptions);
                await _database.StringSetAsync(cacheKey, serializedValue, expiry);
            });
            
            await Task.WhenAll(tasks);
            _logger.LogDebug("Cache set for {Count} keys", keyValuePairs.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting multiple cache values");
        }
    }

    public async Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKeys = keys.Select(GetCacheKey).ToArray();
            await _database.KeyDeleteAsync(cacheKeys);
            _logger.LogDebug("Cache removed for {Count} keys", cacheKeys.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing multiple cache values");
        }
    }

    // Pattern-based Operations
    public async Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(_options.DatabaseId, GetCacheKey(pattern));
            return keys.Select(k => k.ToString().Replace(_options.KeyPrefix, ""));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting keys by pattern: {Pattern}", pattern);
            return Enumerable.Empty<string>();
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var keys = await GetKeysByPatternAsync(pattern, cancellationToken);
            if (keys.Any())
            {
                await RemoveManyAsync(keys, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing keys by pattern: {Pattern}", pattern);
        }
    }

    // Expiration Management
    public async Task<bool> ExpireAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            return await _database.KeyExpireAsync(cacheKey, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting expiration for key: {Key}", key);
            return false;
        }
    }

    public async Task<TimeSpan?> GetTtlAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            return await _database.KeyTimeToLiveAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting TTL for key: {Key}", key);
            return null;
        }
    }

    public async Task<bool> PersistAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            return await _database.KeyPersistAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error persisting key: {Key}", key);
            return false;
        }
    }

    // Hash Operations
    public async Task<T?> HashGetAsync<T>(string key, string field, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var value = await _database.HashGetAsync(cacheKey, field);
            
            if (!value.HasValue)
                return default;
                
            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hash field {Field} for key: {Key}", field, key);
            return default;
        }
    }

    public async Task HashSetAsync<T>(string key, string field, T value, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            await _database.HashSetAsync(cacheKey, field, serializedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting hash field {Field} for key: {Key}", field, key);
        }
    }

    public async Task<Dictionary<string, T?>> HashGetAllAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var hash = await _database.HashGetAllAsync(cacheKey);
            
            var result = new Dictionary<string, T?>();
            foreach (var item in hash)
            {
                result[item.Name!] = JsonSerializer.Deserialize<T>(item.Value!, _jsonOptions);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all hash fields for key: {Key}", key);
            return new Dictionary<string, T?>();
        }
    }

    public async Task HashDeleteAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            await _database.HashDeleteAsync(cacheKey, field);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hash field {Field} for key: {Key}", field, key);
        }
    }

    // Atomic Operations
    public async Task<long> IncrementAsync(string key, long value = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            return await _database.StringIncrementAsync(cacheKey, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing key: {Key}", key);
            return 0;
        }
    }

    public async Task<long> DecrementAsync(string key, long value = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            return await _database.StringDecrementAsync(cacheKey, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrementing key: {Key}", key);
            return 0;
        }
    }

    public async Task<double> IncrementFloatAsync(string key, double value, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            return await _database.StringIncrementAsync(cacheKey, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing float key: {Key}", key);
            return 0;
        }
    }

    // List Operations
    public async Task<long> ListPushAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            return await _database.ListLeftPushAsync(cacheKey, serializedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pushing to list key: {Key}", key);
            return 0;
        }
    }

    public async Task<T?> ListPopAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var value = await _database.ListLeftPopAsync(cacheKey);
            
            if (!value.HasValue)
                return default;
                
            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error popping from list key: {Key}", key);
            return default;
        }
    }

    public async Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var values = await _database.ListRangeAsync(cacheKey, start, stop);
            
            var result = new List<T>();
            foreach (var value in values)
            {
                if (value.HasValue)
                {
                    result.Add(JsonSerializer.Deserialize<T>(value!, _jsonOptions)!);
                }
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting list range for key: {Key}", key);
            return new List<T>();
        }
    }

    public async Task<long> ListLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            return await _database.ListLengthAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting list length for key: {Key}", key);
            return 0;
        }
    }

    // Set Operations
    public async Task<bool> SetAddAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            return await _database.SetAddAsync(cacheKey, serializedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to set key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> SetRemoveAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            return await _database.SetRemoveAsync(cacheKey, serializedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing from set key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> SetContainsAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            return await _database.SetContainsAsync(cacheKey, serializedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking set membership for key: {Key}", key);
            return false;
        }
    }

    public async Task<List<T>> SetMembersAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var values = await _database.SetMembersAsync(cacheKey);
            
            var result = new List<T>();
            foreach (var value in values)
            {
                if (value.HasValue)
                {
                    result.Add(JsonSerializer.Deserialize<T>(value!, _jsonOptions)!);
                }
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting set members for key: {Key}", key);
            return new List<T>();
        }
    }

    // Lock Operations
    public async Task<bool> AcquireLockAsync(string lockKey, TimeSpan expiration, string lockValue, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey($"lock:{lockKey}");
            return await _database.StringSetAsync(cacheKey, lockValue, expiration, When.NotExists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error acquiring lock: {LockKey}", lockKey);
            return false;
        }
    }

    public async Task<bool> ReleaseLockAsync(string lockKey, string lockValue, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey($"lock:{lockKey}");
            const string script = @"
                if redis.call('GET', KEYS[1]) == ARGV[1] then
                    return redis.call('DEL', KEYS[1])
                else
                    return 0
                end";
            
            var result = await _database.ScriptEvaluateAsync(script, new RedisKey[] { cacheKey }, new RedisValue[] { lockValue });
            return result.ToString() == "1";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing lock: {LockKey}", lockKey);
            return false;
        }
    }

    public async Task<bool> ExtendLockAsync(string lockKey, string lockValue, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey($"lock:{lockKey}");
            const string script = @"
                if redis.call('GET', KEYS[1]) == ARGV[1] then
                    return redis.call('EXPIRE', KEYS[1], ARGV[2])
                else
                    return 0
                end";
            
            var result = await _database.ScriptEvaluateAsync(script, 
                new RedisKey[] { cacheKey }, 
                new RedisValue[] { lockValue, (int)expiration.TotalSeconds });
            return result.ToString() == "1";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extending lock: {LockKey}", lockKey);
            return false;
        }
    }

    // Cache Statistics
    public async Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var info = await server.InfoAsync();
            
            var stats = new CacheStatistics();
            
            foreach (var section in info)
            {
                foreach (var item in section)
                {
                    switch (item.Key.ToLower())
                    {
                        case "used_memory":
                            if (long.TryParse(item.Value, out var usedMemory))
                                stats.UsedMemory = usedMemory;
                            break;
                        case "maxmemory":
                            if (long.TryParse(item.Value, out var maxMemory))
                                stats.MaxMemory = maxMemory;
                            break;
                        case "total_connections_received":
                            if (long.TryParse(item.Value, out var totalConnections))
                                stats.TotalConnections = totalConnections;
                            break;
                        case "total_commands_processed":
                            if (long.TryParse(item.Value, out var totalCommands))
                                stats.TotalCommandsProcessed = totalCommands;
                            break;
                        case "uptime_in_seconds":
                            if (int.TryParse(item.Value, out var uptime))
                                stats.Uptime = TimeSpan.FromSeconds(uptime);
                            break;
                    }
                }
            }
            
            stats.TotalKeys = await GetDatabaseSizeAsync(cancellationToken);
            stats.MemoryUsagePercentage = stats.MaxMemory > 0 ? (double)stats.UsedMemory / stats.MaxMemory * 100 : 0;
            
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache statistics");
            return new CacheStatistics();
        }
    }

    public async Task<long> GetDatabaseSizeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            return await server.DatabaseSizeAsync(_options.DatabaseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting database size");
            return 0;
        }
    }

    public async Task FlushDatabaseAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            await server.FlushDatabaseAsync(_options.DatabaseId);
            _logger.LogWarning("Cache database flushed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error flushing cache database");
        }
    }

    // Health Check
    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var ping = await PingAsync(cancellationToken);
            return ping == "PONG";
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> PingAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var latency = await _database.PingAsync();
            return "PONG";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pinging Redis");
            return "ERROR";
        }
    }

    // Helper Methods
    private string GetCacheKey(string key)
    {
        return $"{_options.KeyPrefix}{key}";
    }

    public void Dispose()
    {
        _redis?.Dispose();
    }
}

/// <summary>
/// Redis cache configuration options
/// </summary>
public class RedisCacheOptions
{
    public string ConnectionString { get; set; } = "localhost:6379";
    public int DatabaseId { get; set; } = 0;
    public string KeyPrefix { get; set; } = "wekeza:";
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromHours(1);
    public bool EnableLogging { get; set; } = true;
    public int CommandTimeout { get; set; } = 5000;
    public int ConnectTimeout { get; set; } = 5000;
    public int ConnectRetry { get; set; } = 3;
}