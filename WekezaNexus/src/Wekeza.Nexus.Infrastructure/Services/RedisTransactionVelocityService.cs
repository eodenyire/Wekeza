using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Wekeza.Nexus.Domain.Interfaces;

namespace Wekeza.Nexus.Infrastructure.Services;

/// <summary>
/// Redis-backed implementation of transaction velocity tracking
/// Provides high-performance velocity metrics for fraud detection
/// Integrated with MVP4.0 Redis infrastructure
/// </summary>
public class RedisTransactionVelocityService : ITransactionVelocityService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly ILogger<RedisTransactionVelocityService> _logger;
    private const string KeyPrefix = "nexus:velocity:";

    public RedisTransactionVelocityService(
        IConnectionMultiplexer redis,
        ILogger<RedisTransactionVelocityService> logger)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _database = _redis.GetDatabase();
    }

    public async Task<int> GetTransactionCountAsync(
        Guid userId, 
        int minutes, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"{KeyPrefix}count:{userId}:{minutes}m";
            var value = await _database.StringGetAsync(key);
            
            if (!value.HasValue)
            {
                _logger.LogDebug("No velocity data found for user {UserId} in last {Minutes} minutes", userId, minutes);
                return 0;
            }

            return (int)value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction count for user {UserId}", userId);
            return 0; // Fail-safe: return 0 on error
        }
    }

    public async Task<decimal> GetTransactionAmountAsync(
        Guid userId, 
        int minutes, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"{KeyPrefix}amount:{userId}:{minutes}m";
            var value = await _database.StringGetAsync(key);
            
            if (!value.HasValue)
            {
                return 0m;
            }

            return decimal.Parse(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction amount for user {UserId}", userId);
            return 0m;
        }
    }

    public async Task<decimal> GetAverageTransactionAmountAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"{KeyPrefix}avg:{userId}";
            var value = await _database.StringGetAsync(key);
            
            if (!value.HasValue)
            {
                // Default baseline for new users
                return 5000m;
            }

            return decimal.Parse(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting average transaction amount for user {UserId}", userId);
            return 5000m; // Default baseline
        }
    }

    public async Task<bool> IsFirstTimeBeneficiaryAsync(
        Guid userId, 
        string beneficiaryAccountNumber, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"{KeyPrefix}beneficiaries:{userId}";
            return !await _database.SetContainsAsync(key, beneficiaryAccountNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking first-time beneficiary for user {UserId}", userId);
            return false; // Conservative: assume not first time
        }
    }

    public async Task<int?> GetAccountAgeDaysAsync(
        string accountNumber, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"{KeyPrefix}account_age:{accountNumber}";
            var value = await _database.StringGetAsync(key);
            
            if (!value.HasValue)
            {
                return null;
            }

            return (int)value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account age for account {AccountNumber}", accountNumber);
            return null;
        }
    }

    public async Task<bool> DetectCircularTransactionAsync(
        string fromAccount, 
        string toAccount, 
        int lookbackHours = 24, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if there's a recent transaction from toAccount to fromAccount
            var key = $"{KeyPrefix}graph:{toAccount}:{fromAccount}";
            var value = await _database.StringGetAsync(key);
            
            if (!value.HasValue)
            {
                return false;
            }

            // Check if transaction is within lookback window
            var timestamp = long.Parse(value!);
            var age = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - timestamp;
            return age < (lookbackHours * 3600);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting circular transaction from {FromAccount} to {ToAccount}", fromAccount, toAccount);
            return false;
        }
    }

    /// <summary>
    /// Record a transaction for velocity tracking
    /// Called after transaction is approved
    /// </summary>
    public async Task RecordTransactionAsync(
        Guid userId,
        string fromAccount,
        string toAccount,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTimeOffset.UtcNow;
            
            // Update 10-minute velocity counter
            var count10Key = $"{KeyPrefix}count:{userId}:10m";
            await _database.StringIncrementAsync(count10Key);
            await _database.KeyExpireAsync(count10Key, TimeSpan.FromMinutes(10));
            
            // Update 10-minute amount velocity
            var amount10Key = $"{KeyPrefix}amount:{userId}:10m";
            var current10 = await _database.StringGetAsync(amount10Key);
            var newAmount10 = (current10.HasValue ? decimal.Parse(current10!) : 0m) + amount;
            await _database.StringSetAsync(amount10Key, newAmount10.ToString(), TimeSpan.FromMinutes(10));
            
            // Update 24-hour velocity counter
            var count24Key = $"{KeyPrefix}count:{userId}:1440m";
            await _database.StringIncrementAsync(count24Key);
            await _database.KeyExpireAsync(count24Key, TimeSpan.FromHours(24));
            
            // Update 24-hour amount velocity
            var amount24Key = $"{KeyPrefix}amount:{userId}:1440m";
            var current24 = await _database.StringGetAsync(amount24Key);
            var newAmount24 = (current24.HasValue ? decimal.Parse(current24!) : 0m) + amount;
            await _database.StringSetAsync(amount24Key, newAmount24.ToString(), TimeSpan.FromHours(24));
            
            // Add beneficiary to set
            var beneficiaryKey = $"{KeyPrefix}beneficiaries:{userId}";
            await _database.SetAddAsync(beneficiaryKey, toAccount);
            await _database.KeyExpireAsync(beneficiaryKey, TimeSpan.FromDays(30));
            
            // Record transaction for circular detection
            var graphKey = $"{KeyPrefix}graph:{fromAccount}:{toAccount}";
            await _database.StringSetAsync(graphKey, now.ToUnixTimeSeconds().ToString(), TimeSpan.FromHours(24));
            
            _logger.LogDebug("Recorded transaction velocity for user {UserId}: Amount={Amount}", userId, amount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording transaction velocity for user {UserId}", userId);
            // Non-critical error, don't throw
        }
    }

    /// <summary>
    /// Update user's average transaction amount
    /// Called periodically or after each transaction
    /// </summary>
    public async Task UpdateAverageTransactionAmountAsync(
        Guid userId,
        decimal average,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"{KeyPrefix}avg:{userId}";
            await _database.StringSetAsync(key, average.ToString(), TimeSpan.FromDays(30));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating average transaction amount for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Set account age for new accounts
    /// Called when account is created
    /// </summary>
    public async Task SetAccountAgeAsync(
        string accountNumber,
        int ageDays,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"{KeyPrefix}account_age:{accountNumber}";
            await _database.StringSetAsync(key, ageDays.ToString(), TimeSpan.FromDays(30));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting account age for account {AccountNumber}", accountNumber);
        }
    }
}
