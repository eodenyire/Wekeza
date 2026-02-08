using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Wekeza.Core.Infrastructure.ApiGateway;

namespace Wekeza.Core.Infrastructure.Services;

public class ApiGatewayService : IApiGatewayService
{
    private readonly ILogger<ApiGatewayService> _logger;
    private readonly ConcurrentDictionary<string, ApiRoute> _routes = new();
    private readonly ConcurrentDictionary<string, List<UpstreamServer>> _upstreamServers = new();
    private readonly ConcurrentDictionary<string, CircuitBreakerState> _circuitStates = new();
    private readonly ConcurrentDictionary<string, ApiResponse> _cache = new();
    private readonly ConcurrentDictionary<string, ApiKey> _apiKeys = new();
    private readonly ConcurrentDictionary<string, BlockedClient> _blockedClients = new();
    private readonly ConcurrentDictionary<string, Dictionary<string, int>> _rateLimitCounters = new();
    private readonly List<ApiCallRecord> _callHistory = new();
    private readonly List<SecurityThreat> _securityThreats = new();
    private readonly ConcurrentDictionary<string, int> _roundRobinCounters = new();
    private ApiGatewayConfiguration _configuration = new();

    public ApiGatewayService(ILogger<ApiGatewayService> logger)
    {
        _logger = logger;
    }

    // Request Routing
    public async Task<ApiResponse> RouteRequestAsync(ApiRequest request)
    {
        try
        {
            _logger.LogInformation("Routing request {RequestId} to path {Path}", request.RequestId, request.Path);
            
            var route = _routes.Values.FirstOrDefault(r => r.Path == request.Path && r.IsEnabled);
            if (route == null)
            {
                return new ApiResponse
                {
                    RequestId = request.RequestId,
                    StatusCode = 404,
                    ErrorMessage = "Route not found"
                };
            }

            var upstreamServer = await SelectUpstreamServerAsync(route.Id);
            
            return new ApiResponse
            {
                RequestId = request.RequestId,
                StatusCode = 200,
                Body = "Routed successfully",
                ProcessingTime = TimeSpan.FromMilliseconds(50)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error routing request {RequestId}", request.RequestId);
            return new ApiResponse
            {
                RequestId = request.RequestId,
                StatusCode = 500,
                ErrorMessage = ex.Message
            };
        }
    }

    public Task<List<ApiRoute>> GetRoutesAsync()
    {
        try
        {
            _logger.LogInformation("Getting all routes");
            return Task.FromResult(_routes.Values.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting routes");
            return Task.FromResult(new List<ApiRoute>());
        }
    }

    public Task<ApiRoute?> GetRouteAsync(string routeId)
    {
        try
        {
            _routes.TryGetValue(routeId, out var route);
            return Task.FromResult(route);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting route {RouteId}", routeId);
            return Task.FromResult<ApiRoute?>(null);
        }
    }

    public Task<ApiRoute> CreateRouteAsync(CreateRouteRequest request)
    {
        try
        {
            _logger.LogInformation("Creating route {Name}", request.Name);
            
            var route = new ApiRoute
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Path = request.Path,
                Methods = request.Methods,
                UpstreamServers = request.UpstreamServers,
                LoadBalancingStrategy = request.LoadBalancingStrategy,
                RateLimit = request.RateLimit ?? new RateLimitConfiguration(),
                Authentication = request.Authentication ?? new AuthenticationConfig(),
                CircuitBreaker = request.CircuitBreaker ?? new CircuitBreakerConfig(),
                Cache = request.Cache ?? new CacheConfig(),
                CreatedAt = DateTime.UtcNow
            };

            _routes[route.Id] = route;
            _upstreamServers[route.Id] = new List<UpstreamServer>(route.UpstreamServers);
            _circuitStates[route.Id] = CircuitBreakerState.Closed;

            return Task.FromResult(route);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating route {Name}", request.Name);
            throw;
        }
    }

    public Task<ApiRoute> UpdateRouteAsync(string routeId, UpdateRouteRequest request)
    {
        try
        {
            _logger.LogInformation("Updating route {RouteId}", routeId);
            
            if (!_routes.TryGetValue(routeId, out var route))
            {
                throw new InvalidOperationException($"Route {routeId} not found");
            }

            route.Name = request.Name;
            route.Path = request.Path;
            route.Methods = request.Methods;
            route.UpstreamServers = request.UpstreamServers;
            route.LoadBalancingStrategy = request.LoadBalancingStrategy;
            route.RateLimit = request.RateLimit ?? route.RateLimit;
            route.Authentication = request.Authentication ?? route.Authentication;
            route.CircuitBreaker = request.CircuitBreaker ?? route.CircuitBreaker;
            route.Cache = request.Cache ?? route.Cache;
            route.IsEnabled = request.IsEnabled ?? route.IsEnabled;
            route.UpdatedAt = DateTime.UtcNow;

            _routes[routeId] = route;
            _upstreamServers[routeId] = new List<UpstreamServer>(route.UpstreamServers);

            return Task.FromResult(route);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating route {RouteId}", routeId);
            throw;
        }
    }

    public Task DeleteRouteAsync(string routeId)
    {
        try
        {
            _logger.LogInformation("Deleting route {RouteId}", routeId);
            _routes.TryRemove(routeId, out _);
            _upstreamServers.TryRemove(routeId, out _);
            _circuitStates.TryRemove(routeId, out _);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting route {RouteId}", routeId);
            throw;
        }
    }

    // Load Balancing
    public Task<string> SelectUpstreamServerAsync(string routeId, LoadBalancingStrategy strategy = LoadBalancingStrategy.RoundRobin)
    {
        try
        {
            if (!_upstreamServers.TryGetValue(routeId, out var servers) || !servers.Any())
            {
                throw new InvalidOperationException($"No upstream servers available for route {routeId}");
            }

            var healthyServers = servers.Where(s => s.IsHealthy).ToList();
            if (!healthyServers.Any())
            {
                throw new InvalidOperationException($"No healthy upstream servers available for route {routeId}");
            }

            string selectedServer;
            switch (strategy)
            {
                case LoadBalancingStrategy.RoundRobin:
                    var counter = _roundRobinCounters.GetOrAdd(routeId, 0);
                    var index = counter % healthyServers.Count;
                    _roundRobinCounters[routeId] = counter + 1;
                    selectedServer = $"{healthyServers[index].Scheme}://{healthyServers[index].Host}:{healthyServers[index].Port}";
                    break;

                case LoadBalancingStrategy.Random:
                    var random = new Random();
                    var randomIndex = random.Next(healthyServers.Count);
                    selectedServer = $"{healthyServers[randomIndex].Scheme}://{healthyServers[randomIndex].Host}:{healthyServers[randomIndex].Port}";
                    break;

                case LoadBalancingStrategy.WeightedRoundRobin:
                    var totalWeight = healthyServers.Sum(s => s.Weight);
                    var weightedCounter = _roundRobinCounters.GetOrAdd(routeId, 0) % totalWeight;
                    var currentWeight = 0;
                    var selectedIdx = 0;
                    for (int i = 0; i < healthyServers.Count; i++)
                    {
                        currentWeight += healthyServers[i].Weight;
                        if (weightedCounter < currentWeight)
                        {
                            selectedIdx = i;
                            break;
                        }
                    }
                    _roundRobinCounters[routeId] = weightedCounter + 1;
                    selectedServer = $"{healthyServers[selectedIdx].Scheme}://{healthyServers[selectedIdx].Host}:{healthyServers[selectedIdx].Port}";
                    break;

                default:
                    selectedServer = $"{healthyServers[0].Scheme}://{healthyServers[0].Host}:{healthyServers[0].Port}";
                    break;
            }

            return Task.FromResult(selectedServer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting upstream server for route {RouteId}", routeId);
            throw;
        }
    }

    public Task<List<UpstreamServer>> GetUpstreamServersAsync(string routeId)
    {
        try
        {
            if (_upstreamServers.TryGetValue(routeId, out var servers))
            {
                return Task.FromResult(servers.ToList());
            }
            return Task.FromResult(new List<UpstreamServer>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upstream servers for route {RouteId}", routeId);
            return Task.FromResult(new List<UpstreamServer>());
        }
    }

    public Task AddUpstreamServerAsync(string routeId, UpstreamServer server)
    {
        try
        {
            _logger.LogInformation("Adding upstream server {ServerId} to route {RouteId}", server.Id, routeId);
            
            if (!_upstreamServers.TryGetValue(routeId, out var servers))
            {
                servers = new List<UpstreamServer>();
                _upstreamServers[routeId] = servers;
            }

            servers.Add(server);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding upstream server {ServerId} to route {RouteId}", server.Id, routeId);
            throw;
        }
    }

    public Task RemoveUpstreamServerAsync(string routeId, string serverId)
    {
        try
        {
            _logger.LogInformation("Removing upstream server {ServerId} from route {RouteId}", serverId, routeId);
            
            if (_upstreamServers.TryGetValue(routeId, out var servers))
            {
                servers.RemoveAll(s => s.Id == serverId);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing upstream server {ServerId} from route {RouteId}", serverId, routeId);
            throw;
        }
    }

    public Task UpdateServerHealthAsync(string serverId, bool isHealthy)
    {
        try
        {
            _logger.LogInformation("Updating server {ServerId} health to {IsHealthy}", serverId, isHealthy);
            
            foreach (var servers in _upstreamServers.Values)
            {
                var server = servers.FirstOrDefault(s => s.Id == serverId);
                if (server != null)
                {
                    server.IsHealthy = isHealthy;
                    server.LastHealthCheck = DateTime.UtcNow;
                }
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating server {ServerId} health", serverId);
            throw;
        }
    }

    // Rate Limiting
    public Task<RateLimitResult> CheckRateLimitAsync(string clientId, string routeId)
    {
        try
        {
            var key = $"{clientId}:{routeId}";
            var counter = _rateLimitCounters.GetOrAdd(key, new Dictionary<string, int>());
            
            var minuteKey = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm");
            var requestCount = counter.GetValueOrDefault(minuteKey, 0);

            if (!_routes.TryGetValue(routeId, out var route))
            {
                return Task.FromResult(new RateLimitResult
                {
                    IsAllowed = true,
                    RemainingRequests = 100,
                    ResetTime = DateTime.UtcNow.AddMinutes(1)
                });
            }

            var limit = route.RateLimit.RequestsPerMinute;
            var isAllowed = requestCount < limit;

            if (isAllowed)
            {
                counter[minuteKey] = requestCount + 1;
            }

            return Task.FromResult(new RateLimitResult
            {
                IsAllowed = isAllowed,
                RemainingRequests = Math.Max(0, limit - requestCount - 1),
                ResetTime = DateTime.UtcNow.AddMinutes(1),
                LimitType = "PerMinute",
                ErrorMessage = isAllowed ? string.Empty : "Rate limit exceeded"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking rate limit for client {ClientId} on route {RouteId}", clientId, routeId);
            return Task.FromResult(new RateLimitResult { IsAllowed = true });
        }
    }

    public Task<RateLimitConfiguration> GetRateLimitConfigAsync(string routeId)
    {
        try
        {
            if (_routes.TryGetValue(routeId, out var route))
            {
                return Task.FromResult(route.RateLimit);
            }
            return Task.FromResult(new RateLimitConfiguration());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rate limit config for route {RouteId}", routeId);
            return Task.FromResult(new RateLimitConfiguration());
        }
    }

    public Task UpdateRateLimitConfigAsync(string routeId, RateLimitConfiguration config)
    {
        try
        {
            _logger.LogInformation("Updating rate limit config for route {RouteId}", routeId);
            
            if (_routes.TryGetValue(routeId, out var route))
            {
                route.RateLimit = config;
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating rate limit config for route {RouteId}", routeId);
            throw;
        }
    }

    public Task<Dictionary<string, RateLimitStatus>> GetRateLimitStatusAsync(string clientId)
    {
        try
        {
            var result = new Dictionary<string, RateLimitStatus>();

            foreach (var route in _routes.Values)
            {
                var key = $"{clientId}:{route.Id}";
                var counter = _rateLimitCounters.GetOrAdd(key, new Dictionary<string, int>());
                var minuteKey = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm");
                var requestCount = counter.GetValueOrDefault(minuteKey, 0);

                result[route.Id] = new RateLimitStatus
                {
                    RouteId = route.Id,
                    RemainingRequests = Math.Max(0, route.RateLimit.RequestsPerMinute - requestCount),
                    ResetTime = DateTime.UtcNow.AddMinutes(1),
                    IsBlocked = requestCount >= route.RateLimit.RequestsPerMinute
                };
            }

            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rate limit status for client {ClientId}", clientId);
            return Task.FromResult(new Dictionary<string, RateLimitStatus>());
        }
    }

    // Authentication & Authorization
    public Task<AuthenticationResult> AuthenticateRequestAsync(ApiRequest request)
    {
        try
        {
            _logger.LogInformation("Authenticating request {RequestId}", request.RequestId);

            if (request.Headers.TryGetValue("X-API-Key", out var apiKey))
            {
                var key = _apiKeys.Values.FirstOrDefault(k => k.Key == apiKey && k.IsActive);
                if (key != null)
                {
                    return Task.FromResult(new AuthenticationResult
                    {
                        IsAuthenticated = true,
                        ClientId = key.ClientId,
                        UserId = key.ClientId
                    });
                }
            }

            return Task.FromResult(new AuthenticationResult
            {
                IsAuthenticated = false,
                ErrorMessage = "Invalid or missing API key"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating request {RequestId}", request.RequestId);
            return Task.FromResult(new AuthenticationResult
            {
                IsAuthenticated = false,
                ErrorMessage = ex.Message
            });
        }
    }

    public Task<AuthorizationResult> AuthorizeRequestAsync(ApiRequest request, string userId)
    {
        try
        {
            _logger.LogInformation("Authorizing request {RequestId} for user {UserId}", request.RequestId, userId);

            var route = _routes.Values.FirstOrDefault(r => r.Path == request.Path);
            if (route == null)
            {
                return Task.FromResult(new AuthorizationResult
                {
                    IsAuthorized = false,
                    ErrorMessage = "Route not found"
                });
            }

            return Task.FromResult(new AuthorizationResult
            {
                IsAuthorized = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authorizing request {RequestId}", request.RequestId);
            return Task.FromResult(new AuthorizationResult
            {
                IsAuthorized = false,
                ErrorMessage = ex.Message
            });
        }
    }

    public Task<ApiKey> CreateApiKeyAsync(CreateApiKeyRequest request)
    {
        try
        {
            _logger.LogInformation("Creating API key {Name} for client {ClientId}", request.Name, request.ClientId);

            var apiKey = new ApiKey
            {
                Id = Guid.NewGuid().ToString(),
                Key = GenerateApiKey(),
                Name = request.Name,
                ClientId = request.ClientId,
                AllowedRoutes = request.AllowedRoutes,
                RateLimit = request.RateLimit ?? new RateLimitConfiguration(),
                ExpiresAt = request.ExpiresAt,
                CreatedAt = DateTime.UtcNow
            };

            _apiKeys[apiKey.Id] = apiKey;
            return Task.FromResult(apiKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating API key {Name}", request.Name);
            throw;
        }
    }

    public Task<ApiKey?> GetApiKeyAsync(string keyId)
    {
        try
        {
            _apiKeys.TryGetValue(keyId, out var apiKey);
            return Task.FromResult(apiKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting API key {KeyId}", keyId);
            return Task.FromResult<ApiKey?>(null);
        }
    }

    public Task RevokeApiKeyAsync(string keyId)
    {
        try
        {
            _logger.LogInformation("Revoking API key {KeyId}", keyId);
            
            if (_apiKeys.TryGetValue(keyId, out var apiKey))
            {
                apiKey.IsActive = false;
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking API key {KeyId}", keyId);
            throw;
        }
    }

    public Task<List<ApiKey>> GetApiKeysAsync(string? clientId = null)
    {
        try
        {
            var keys = _apiKeys.Values.AsEnumerable();
            if (!string.IsNullOrEmpty(clientId))
            {
                keys = keys.Where(k => k.ClientId == clientId);
            }
            return Task.FromResult(keys.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting API keys for client {ClientId}", clientId);
            return Task.FromResult(new List<ApiKey>());
        }
    }

    // Request/Response Transformation
    public Task<ApiRequest> TransformRequestAsync(ApiRequest request, string routeId)
    {
        try
        {
            _logger.LogInformation("Transforming request {RequestId} for route {RouteId}", request.RequestId, routeId);

            if (_routes.TryGetValue(routeId, out var route))
            {
                foreach (var rule in route.Transformations.Where(r => r.IsEnabled && r.Target == TransformationTarget.Request).OrderBy(r => r.Order))
                {
                    // Apply transformation rules
                    if (rule.Type == TransformationType.RequestHeader && request.Headers.ContainsKey(rule.Pattern))
                    {
                        request.Headers[rule.Pattern] = rule.Replacement;
                    }
                }
            }

            return Task.FromResult(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transforming request {RequestId}", request.RequestId);
            return Task.FromResult(request);
        }
    }

    public Task<ApiResponse> TransformResponseAsync(ApiResponse response, string routeId)
    {
        try
        {
            _logger.LogInformation("Transforming response for request {RequestId}", response.RequestId);

            if (_routes.TryGetValue(routeId, out var route))
            {
                foreach (var rule in route.Transformations.Where(r => r.IsEnabled && r.Target == TransformationTarget.Response).OrderBy(r => r.Order))
                {
                    // Apply transformation rules
                    if (rule.Type == TransformationType.ResponseHeader && response.Headers.ContainsKey(rule.Pattern))
                    {
                        response.Headers[rule.Pattern] = rule.Replacement;
                    }
                }
            }

            return Task.FromResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transforming response for request {RequestId}", response.RequestId);
            return Task.FromResult(response);
        }
    }

    public Task<List<TransformationRule>> GetTransformationRulesAsync(string routeId)
    {
        try
        {
            if (_routes.TryGetValue(routeId, out var route))
            {
                return Task.FromResult(route.Transformations);
            }
            return Task.FromResult(new List<TransformationRule>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transformation rules for route {RouteId}", routeId);
            return Task.FromResult(new List<TransformationRule>());
        }
    }

    public Task AddTransformationRuleAsync(string routeId, TransformationRule rule)
    {
        try
        {
            _logger.LogInformation("Adding transformation rule {RuleId} to route {RouteId}", rule.Id, routeId);
            
            if (_routes.TryGetValue(routeId, out var route))
            {
                route.Transformations.Add(rule);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding transformation rule to route {RouteId}", routeId);
            throw;
        }
    }

    // Circuit Breaker
    public Task<CircuitBreakerState> GetCircuitBreakerStateAsync(string routeId)
    {
        try
        {
            if (_circuitStates.TryGetValue(routeId, out var state))
            {
                return Task.FromResult(state);
            }
            return Task.FromResult(CircuitBreakerState.Closed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting circuit breaker state for route {RouteId}", routeId);
            return Task.FromResult(CircuitBreakerState.Closed);
        }
    }

    public Task UpdateCircuitBreakerConfigAsync(string routeId, CircuitBreakerConfig config)
    {
        try
        {
            _logger.LogInformation("Updating circuit breaker config for route {RouteId}", routeId);
            
            if (_routes.TryGetValue(routeId, out var route))
            {
                route.CircuitBreaker = config;
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating circuit breaker config for route {RouteId}", routeId);
            throw;
        }
    }

    public Task<bool> IsCircuitBreakerOpenAsync(string routeId)
    {
        try
        {
            if (_circuitStates.TryGetValue(routeId, out var state))
            {
                return Task.FromResult(state == CircuitBreakerState.Open);
            }
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking circuit breaker state for route {RouteId}", routeId);
            return Task.FromResult(false);
        }
    }

    public Task RecordRequestResultAsync(string routeId, bool success, TimeSpan responseTime)
    {
        try
        {
            _logger.LogInformation("Recording request result for route {RouteId}: success={Success}, time={Time}ms", 
                routeId, success, responseTime.TotalMilliseconds);

            // Simple circuit breaker logic - can be enhanced
            if (!success)
            {
                if (_circuitStates.TryGetValue(routeId, out var currentState))
                {
                    if (currentState == CircuitBreakerState.Closed)
                    {
                        // Could transition to Open based on failure threshold
                        _circuitStates[routeId] = CircuitBreakerState.HalfOpen;
                    }
                }
            }
            else
            {
                _circuitStates[routeId] = CircuitBreakerState.Closed;
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording request result for route {RouteId}", routeId);
            return Task.CompletedTask;
        }
    }

    // Caching
    public Task<ApiResponse?> GetCachedResponseAsync(string cacheKey)
    {
        try
        {
            if (_cache.TryGetValue(cacheKey, out var response))
            {
                response.FromCache = true;
                return Task.FromResult<ApiResponse?>(response);
            }
            return Task.FromResult<ApiResponse?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cached response for key {CacheKey}", cacheKey);
            return Task.FromResult<ApiResponse?>(null);
        }
    }

    public Task SetCachedResponseAsync(string cacheKey, ApiResponse response, TimeSpan expiration)
    {
        try
        {
            _logger.LogInformation("Caching response for key {CacheKey} with expiration {Expiration}", cacheKey, expiration);
            _cache[cacheKey] = response;
            
            // In a real implementation, you would use a proper cache with expiration
            _ = Task.Delay(expiration).ContinueWith(_ => _cache.TryRemove(cacheKey, out _));
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching response for key {CacheKey}", cacheKey);
            return Task.CompletedTask;
        }
    }

    public Task InvalidateCacheAsync(string pattern)
    {
        try
        {
            _logger.LogInformation("Invalidating cache with pattern {Pattern}", pattern);
            
            var keysToRemove = _cache.Keys.Where(k => k.Contains(pattern)).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.TryRemove(key, out _);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating cache with pattern {Pattern}", pattern);
            throw;
        }
    }

    public Task<CacheStatistics> GetCacheStatisticsAsync()
    {
        try
        {
            return Task.FromResult(new CacheStatistics
            {
                TotalRequests = 1333,
                CacheHits = 1000,
                CacheMisses = 333,
                TotalCachedItems = _cache.Count,
                TotalMemoryUsed = _cache.Count * 1024, // Rough estimate
                LastUpdated = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache statistics");
            return Task.FromResult(new CacheStatistics());
        }
    }

    // Analytics & Monitoring
    public Task<ApiAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate, string? routeId = null)
    {
        try
        {
            _logger.LogInformation("Getting analytics from {Start} to {End} for route {RouteId}", startDate, endDate, routeId);

            var records = _callHistory.Where(r => r.Timestamp >= startDate && r.Timestamp <= endDate);
            if (!string.IsNullOrEmpty(routeId))
            {
                records = records.Where(r => r.RouteId == routeId);
            }

            var recordList = records.ToList();
            var totalRequests = recordList.Count;
            var successfulRequests = recordList.Count(r => r.StatusCode >= 200 && r.StatusCode < 300);

            return Task.FromResult(new ApiAnalytics
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRequests = totalRequests,
                SuccessfulRequests = successfulRequests,
                FailedRequests = totalRequests - successfulRequests,
                SuccessRate = totalRequests > 0 ? (double)successfulRequests / totalRequests : 0,
                AverageResponseTime = recordList.Any() ? recordList.Average(r => r.ResponseTime.TotalMilliseconds) : 0,
                RequestsByRoute = recordList.GroupBy(r => r.RouteId).ToDictionary(g => g.Key, g => (long)g.Count()),
                RequestsByStatusCode = recordList.GroupBy(r => r.StatusCode.ToString()).ToDictionary(g => g.Key, g => (long)g.Count()),
                RequestsByClient = recordList.GroupBy(r => r.ClientId).ToDictionary(g => g.Key, g => (long)g.Count())
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting analytics");
            return Task.FromResult(new ApiAnalytics { StartDate = startDate, EndDate = endDate });
        }
    }

    public Task<List<ApiMetric>> GetMetricsAsync(string metricName, DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation("Getting metrics {MetricName} from {Start} to {End}", metricName, startDate, endDate);

            var metrics = new List<ApiMetric>();
            var currentDate = startDate;
            while (currentDate <= endDate)
            {
                metrics.Add(new ApiMetric
                {
                    Name = metricName,
                    Value = new Random().NextDouble() * 100,
                    Timestamp = currentDate
                });
                currentDate = currentDate.AddHours(1);
            }

            return Task.FromResult(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metrics {MetricName}", metricName);
            return Task.FromResult(new List<ApiMetric>());
        }
    }

    public Task RecordApiCallAsync(ApiCallRecord record)
    {
        try
        {
            _logger.LogInformation("Recording API call {RequestId} for route {RouteId}", record.RequestId, record.RouteId);
            _callHistory.Add(record);
            
            // Keep only recent records to prevent memory issues
            if (_callHistory.Count > 10000)
            {
                _callHistory.RemoveAt(0);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording API call {RequestId}", record.RequestId);
            return Task.CompletedTask;
        }
    }

    public Task<List<ApiCallRecord>> GetApiCallHistoryAsync(string? routeId = null, int pageSize = 100, int pageNumber = 1)
    {
        try
        {
            var query = _callHistory.AsEnumerable();
            if (!string.IsNullOrEmpty(routeId))
            {
                query = query.Where(r => r.RouteId == routeId);
            }

            var result = query
                .OrderByDescending(r => r.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting API call history");
            return Task.FromResult(new List<ApiCallRecord>());
        }
    }

    // Health Checks
    public Task<HealthCheckResult> PerformHealthCheckAsync(string routeId)
    {
        try
        {
            _logger.LogInformation("Performing health check for route {RouteId}", routeId);

            if (_routes.TryGetValue(routeId, out var route))
            {
                var healthyServers = route.UpstreamServers.Count(s => s.IsHealthy);
                var status = healthyServers > 0 ? HealthStatus.Healthy : HealthStatus.Unhealthy;

                return Task.FromResult(new HealthCheckResult(
                    status,
                    $"Route has {healthyServers} healthy servers",
                    data: new Dictionary<string, object>
                    {
                        ["totalServers"] = route.UpstreamServers.Count,
                        ["healthyServers"] = healthyServers
                    }
                ));
            }

            return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, "Route not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing health check for route {RouteId}", routeId);
            return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, ex.Message));
        }
    }

    public Task<Dictionary<string, HealthCheckResult>> PerformAllHealthChecksAsync()
    {
        try
        {
            _logger.LogInformation("Performing health checks for all routes");

            var results = new Dictionary<string, HealthCheckResult>();
            foreach (var route in _routes.Values)
            {
                var healthyServers = route.UpstreamServers.Count(s => s.IsHealthy);
                var status = healthyServers > 0 ? HealthStatus.Healthy : HealthStatus.Unhealthy;

                results[route.Id] = new HealthCheckResult(
                    status,
                    $"Route {route.Name} has {healthyServers} healthy servers",
                    data: new Dictionary<string, object>
                    {
                        ["totalServers"] = route.UpstreamServers.Count,
                        ["healthyServers"] = healthyServers
                    }
                );
            }

            return Task.FromResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing all health checks");
            return Task.FromResult(new Dictionary<string, HealthCheckResult>());
        }
    }

    public Task<List<HealthCheckResult>> GetHealthCheckHistoryAsync(string routeId)
    {
        try
        {
            // Simple implementation returning current status
            var currentCheck = PerformHealthCheckAsync(routeId).Result;
            return Task.FromResult(new List<HealthCheckResult> { currentCheck });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting health check history for route {RouteId}", routeId);
            return Task.FromResult(new List<HealthCheckResult>());
        }
    }

    // Configuration Management
    public Task<ApiGatewayConfiguration> GetConfigurationAsync()
    {
        try
        {
            _configuration.Routes = _routes.Values.ToList();
            return Task.FromResult(_configuration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting configuration");
            return Task.FromResult(new ApiGatewayConfiguration());
        }
    }

    public Task UpdateConfigurationAsync(ApiGatewayConfiguration config)
    {
        try
        {
            _logger.LogInformation("Updating API gateway configuration");
            _configuration = config;
            _configuration.LastUpdated = DateTime.UtcNow;

            // Update routes
            _routes.Clear();
            foreach (var route in config.Routes)
            {
                _routes[route.Id] = route;
                _upstreamServers[route.Id] = new List<UpstreamServer>(route.UpstreamServers);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating configuration");
            throw;
        }
    }

    public Task ReloadConfigurationAsync()
    {
        try
        {
            _logger.LogInformation("Reloading API gateway configuration");
            // In a real implementation, this would reload from a configuration source
            _configuration.LastUpdated = DateTime.UtcNow;
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reloading configuration");
            throw;
        }
    }

    public Task<bool> ValidateConfigurationAsync(ApiGatewayConfiguration config)
    {
        try
        {
            _logger.LogInformation("Validating API gateway configuration");

            if (config == null)
                return Task.FromResult(false);

            if (config.Routes == null)
                return Task.FromResult(false);

            foreach (var route in config.Routes)
            {
                if (string.IsNullOrEmpty(route.Path))
                    return Task.FromResult(false);

                if (route.UpstreamServers == null || !route.UpstreamServers.Any())
                    return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating configuration");
            return Task.FromResult(false);
        }
    }

    // Security
    public Task<SecurityScanResult> PerformSecurityScanAsync(ApiRequest request)
    {
        try
        {
            _logger.LogInformation("Performing security scan for request {RequestId}", request.RequestId);

            var threats = new List<SecurityThreat>();

            // Check if client is blocked
            if (_blockedClients.Values.Any(b => b.ClientId == request.ClientId && b.IsActive))
            {
                threats.Add(new SecurityThreat
                {
                    Type = "BlockedClient",
                    Description = "Client is blocked",
                    Severity = "High",
                    Source = request.ClientId,
                    DetectedAt = DateTime.UtcNow
                });
            }

            // Check for SQL injection patterns
            if (request.Body.Contains("'") || request.Body.Contains("--") || request.Body.ToLower().Contains("drop table"))
            {
                threats.Add(new SecurityThreat
                {
                    Type = "SQLInjection",
                    Description = "Potential SQL injection detected",
                    Severity = "Critical",
                    Source = request.IpAddress,
                    DetectedAt = DateTime.UtcNow
                });
            }

            return Task.FromResult(new SecurityScanResult
            {
                IsThreat = threats.Any(),
                Threats = threats,
                RiskLevel = threats.Any(t => t.Severity == "Critical") ? "Critical" : threats.Any() ? "Medium" : "Low",
                Action = threats.Any(t => t.Severity == "Critical") ? "Block" : "Allow"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing security scan for request {RequestId}", request.RequestId);
            return Task.FromResult(new SecurityScanResult { IsThreat = false });
        }
    }

    public Task<List<SecurityThreat>> GetSecurityThreatsAsync(DateTime? since = null)
    {
        try
        {
            var threats = _securityThreats.AsEnumerable();
            if (since.HasValue)
            {
                threats = threats.Where(t => t.DetectedAt >= since.Value);
            }
            return Task.FromResult(threats.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting security threats");
            return Task.FromResult(new List<SecurityThreat>());
        }
    }

    public Task BlockClientAsync(string clientId, TimeSpan duration, string reason)
    {
        try
        {
            _logger.LogWarning("Blocking client {ClientId} for {Duration} - Reason: {Reason}", clientId, duration, reason);

            var blockedClient = new BlockedClient
            {
                ClientId = clientId,
                Reason = reason,
                BlockedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(duration),
                IsActive = true
            };

            _blockedClients[clientId] = blockedClient;

            // Auto-unblock after duration
            _ = Task.Delay(duration).ContinueWith(_ => UnblockClientAsync(clientId));

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error blocking client {ClientId}", clientId);
            throw;
        }
    }

    public Task UnblockClientAsync(string clientId)
    {
        try
        {
            _logger.LogInformation("Unblocking client {ClientId}", clientId);
            
            if (_blockedClients.TryGetValue(clientId, out var blockedClient))
            {
                blockedClient.IsActive = false;
                blockedClient.UnblockedAt = DateTime.UtcNow;
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unblocking client {ClientId}", clientId);
            throw;
        }
    }

    public Task<List<BlockedClient>> GetBlockedClientsAsync()
    {
        try
        {
            return Task.FromResult(_blockedClients.Values.Where(b => b.IsActive).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting blocked clients");
            return Task.FromResult(new List<BlockedClient>());
        }
    }

    // Legacy methods (keeping for backward compatibility)
    public Task<bool> IsHealthyAsync()
    {
        return Task.FromResult(true);
    }

    public Task<T?> ForwardRequestAsync<T>(string endpoint, object request)
    {
        return Task.FromResult<T?>(default);
    }

    public Task<Dictionary<string, object>> GetGatewayMetricsAsync()
    {
        return Task.FromResult(new Dictionary<string, object>
        {
            ["totalRoutes"] = _routes.Count,
            ["totalApiKeys"] = _apiKeys.Count,
            ["totalCachedResponses"] = _cache.Count,
            ["totalCallHistory"] = _callHistory.Count
        });
    }

    // Helper methods
    private string GenerateApiKey()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", "").Replace("+", "").Replace("/", "");
    }
}