using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Integration Repository Interface - Data access for Integration aggregate
/// Supports third-party system integrations and API management
/// Industry Standard: Repository pattern for enterprise integration systems
/// </summary>
public interface IIntegrationRepository : IRepository<Integration>
{
    // ============================================================================
    // BASIC CRUD OPERATIONS
    // ============================================================================
    
    /// <summary>
    /// Get integration by unique code
    /// </summary>
    Task<Integration> GetByCodeAsync(string integrationCode);
    
    /// <summary>
    /// Check if integration code exists
    /// </summary>
    Task<bool> ExistsByCodeAsync(string integrationCode);

    // ============================================================================
    // QUERY BY TYPE AND STATUS
    // ============================================================================
    
    /// <summary>
    /// Get integrations by type
    /// </summary>
    Task<List<Integration>> GetByTypeAsync(IntegrationType type, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get integrations by status
    /// </summary>
    Task<List<Integration>> GetByStatusAsync(IntegrationStatus status, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get active integrations
    /// </summary>
    Task<List<Integration>> GetActiveIntegrationsAsync(int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get inactive integrations
    /// </summary>
    Task<List<Integration>> GetInactiveIntegrationsAsync(int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get failed integrations
    /// </summary>
    Task<List<Integration>> GetFailedIntegrationsAsync(int pageNumber = 1, int pageSize = 20);

    // ============================================================================
    // HEALTH AND MONITORING
    // ============================================================================
    
    /// <summary>
    /// Get healthy integrations
    /// </summary>
    Task<List<Integration>> GetHealthyIntegrationsAsync(int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get unhealthy integrations
    /// </summary>
    Task<List<Integration>> GetUnhealthyIntegrationsAsync(int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get integrations with circuit breaker open
    /// </summary>
    Task<List<Integration>> GetCircuitBreakerOpenIntegrationsAsync();
    
    /// <summary>
    /// Get integrations in maintenance mode
    /// </summary>
    Task<List<Integration>> GetMaintenanceModeIntegrationsAsync();
    
    /// <summary>
    /// Get integrations with consecutive failures above threshold
    /// </summary>
    Task<List<Integration>> GetIntegrationsWithHighFailuresAsync(int failureThreshold = 5);

    // ============================================================================
    // PERFORMANCE QUERIES
    // ============================================================================
    
    /// <summary>
    /// Get integrations by success rate range
    /// </summary>
    Task<List<Integration>> GetBySuccessRateRangeAsync(decimal minRate, decimal maxRate, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get slowest integrations by average response time
    /// </summary>
    Task<List<Integration>> GetSlowestIntegrationsAsync(int count = 10);
    
    /// <summary>
    /// Get fastest integrations by average response time
    /// </summary>
    Task<List<Integration>> GetFastestIntegrationsAsync(int count = 10);
    
    /// <summary>
    /// Get integrations with response time above threshold
    /// </summary>
    Task<List<Integration>> GetSlowResponseIntegrationsAsync(TimeSpan threshold);
    
    /// <summary>
    /// Get most used integrations by call count
    /// </summary>
    Task<List<Integration>> GetMostUsedIntegrationsAsync(int count = 10);

    // ============================================================================
    // TIME-BASED QUERIES
    // ============================================================================
    
    /// <summary>
    /// Get integrations created within date range
    /// </summary>
    Task<List<Integration>> GetCreatedWithinDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get integrations last called within date range
    /// </summary>
    Task<List<Integration>> GetLastCalledWithinDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get integrations not called since date
    /// </summary>
    Task<List<Integration>> GetNotCalledSinceAsync(DateTime cutoffDate, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get recently modified integrations
    /// </summary>
    Task<List<Integration>> GetRecentlyModifiedAsync(int hours = 24, int pageNumber = 1, int pageSize = 20);

    // ============================================================================
    // SEARCH AND FILTER
    // ============================================================================
    
    /// <summary>
    /// Search integrations by name, code, or description
    /// </summary>
    Task<List<Integration>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get integrations with advanced filters
    /// </summary>
    Task<List<Integration>> GetWithFiltersAsync(
        IntegrationType? type = null,
        IntegrationStatus? status = null,
        AuthenticationType? authType = null,
        bool? isActive = null,
        decimal? minSuccessRate = null,
        decimal? maxSuccessRate = null,
        TimeSpan? maxResponseTime = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        string createdBy = null,
        int pageNumber = 1,
        int pageSize = 20);

    // ============================================================================
    // AUTHENTICATION AND SECURITY
    // ============================================================================
    
    /// <summary>
    /// Get integrations by authentication type
    /// </summary>
    Task<List<Integration>> GetByAuthenticationTypeAsync(AuthenticationType authType, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get integrations with expired authentication
    /// </summary>
    Task<List<Integration>> GetWithExpiredAuthenticationAsync();
    
    /// <summary>
    /// Get integrations requiring authentication refresh
    /// </summary>
    Task<List<Integration>> GetRequiringAuthRefreshAsync(int daysBeforeExpiry = 7);

    // ============================================================================
    // STATISTICS AND ANALYTICS
    // ============================================================================
    
    /// <summary>
    /// Get integration count by type
    /// </summary>
    Task<Dictionary<IntegrationType, int>> GetCountByTypeAsync();
    
    /// <summary>
    /// Get integration count by status
    /// </summary>
    Task<Dictionary<IntegrationStatus, int>> GetCountByStatusAsync();
    
    /// <summary>
    /// Get integration count by authentication type
    /// </summary>
    Task<Dictionary<AuthenticationType, int>> GetCountByAuthTypeAsync();
    
    /// <summary>
    /// Get integration performance statistics
    /// </summary>
    Task<Dictionary<string, object>> GetPerformanceStatisticsAsync(DateTime fromDate, DateTime toDate);
    
    /// <summary>
    /// Get integration health statistics
    /// </summary>
    Task<Dictionary<string, object>> GetHealthStatisticsAsync();
    
    /// <summary>
    /// Get integration usage statistics
    /// </summary>
    Task<Dictionary<string, object>> GetUsageStatisticsAsync(DateTime fromDate, DateTime toDate);
    
    /// <summary>
    /// Get integration error statistics
    /// </summary>
    Task<Dictionary<string, object>> GetErrorStatisticsAsync(DateTime fromDate, DateTime toDate);

    // ============================================================================
    // MAINTENANCE AND CLEANUP
    // ============================================================================
    
    /// <summary>
    /// Get deprecated integrations
    /// </summary>
    Task<List<Integration>> GetDeprecatedIntegrationsAsync();
    
    /// <summary>
    /// Get unused integrations (not called for specified days)
    /// </summary>
    Task<List<Integration>> GetUnusedIntegrationsAsync(int unusedDays = 90);
    
    /// <summary>
    /// Get integrations eligible for cleanup
    /// </summary>
    Task<List<Integration>> GetEligibleForCleanupAsync(DateTime cutoffDate);
    
    /// <summary>
    /// Get integrations requiring configuration update
    /// </summary>
    Task<List<Integration>> GetRequiringConfigUpdateAsync();

    // ============================================================================
    // BULK OPERATIONS
    // ============================================================================
    
    /// <summary>
    /// Bulk update integration status
    /// </summary>
    Task BulkUpdateStatusAsync(List<Guid> integrationIds, IntegrationStatus newStatus);
    
    /// <summary>
    /// Bulk activate integrations
    /// </summary>
    Task BulkActivateAsync(List<Guid> integrationIds, string activatedBy);
    
    /// <summary>
    /// Bulk deactivate integrations
    /// </summary>
    Task BulkDeactivateAsync(List<Guid> integrationIds, string deactivatedBy, string reason = null);
    
    /// <summary>
    /// Bulk reset circuit breakers
    /// </summary>
    Task BulkResetCircuitBreakersAsync(List<Guid> integrationIds);

    // ============================================================================
    // MONITORING AND ALERTING
    // ============================================================================
    
    /// <summary>
    /// Get integrations requiring attention (health issues, high failures, etc.)
    /// </summary>
    Task<List<Integration>> GetRequiringAttentionAsync();
    
    /// <summary>
    /// Get integration health summary
    /// </summary>
    Task<Dictionary<string, object>> GetHealthSummaryAsync();
    
    /// <summary>
    /// Get integrations with recent errors
    /// </summary>
    Task<List<Integration>> GetWithRecentErrorsAsync(int hours = 1);
    
    /// <summary>
    /// Get integration uptime statistics
    /// </summary>
    Task<Dictionary<Guid, decimal>> GetUptimeStatisticsAsync(DateTime fromDate, DateTime toDate);

    // ============================================================================
    // CONFIGURATION MANAGEMENT
    // ============================================================================
    
    /// <summary>
    /// Get integrations by endpoint pattern
    /// </summary>
    Task<List<Integration>> GetByEndpointPatternAsync(string pattern);
    
    /// <summary>
    /// Get integrations with specific configuration key
    /// </summary>
    Task<List<Integration>> GetWithConfigurationKeyAsync(string configKey);
    
    /// <summary>
    /// Get integrations by protocol
    /// </summary>
    Task<List<Integration>> GetByProtocolAsync(string protocol);
    
    /// <summary>
    /// Get integrations by data format
    /// </summary>
    Task<List<Integration>> GetByDataFormatAsync(string dataFormat);

    // ============================================================================
    // VALIDATION AND INTEGRITY
    // ============================================================================
    
    /// <summary>
    /// Validate integration configuration
    /// </summary>
    Task<List<string>> ValidateConfigurationAsync(Guid integrationId);
    
    /// <summary>
    /// Check for duplicate integrations
    /// </summary>
    Task<List<Integration>> FindDuplicateIntegrationsAsync(string endpointUrl);
    
    /// <summary>
    /// Get integrations with invalid configuration
    /// </summary>
    Task<List<Integration>> GetWithInvalidConfigurationAsync();
    
    /// <summary>
    /// Get integrations with missing required fields
    /// </summary>
    Task<List<Integration>> GetWithMissingRequiredFieldsAsync();

    // ============================================================================
    // CUSTOM QUERIES
    // ============================================================================
    
    /// <summary>
    /// Execute custom query with parameters
    /// </summary>
    Task<List<Integration>> ExecuteCustomQueryAsync(string query, Dictionary<string, object> parameters);
    
    /// <summary>
    /// Get integration summary data
    /// </summary>
    Task<Dictionary<string, object>> GetIntegrationSummaryAsync(Guid integrationId);
    
    /// <summary>
    /// Get recommended integrations for optimization
    /// </summary>
    Task<List<Integration>> GetRecommendedForOptimizationAsync(int count = 5);
    
    /// <summary>
    /// Get integration dependency graph
    /// </summary>
    Task<Dictionary<Guid, List<Guid>>> GetDependencyGraphAsync();
}