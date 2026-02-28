using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Analytics Repository Interface - Data access for Analytics aggregate
/// Supports descriptive, diagnostic, predictive, and prescriptive analytics
/// Industry Standard: Repository pattern for business intelligence and analytics systems
/// </summary>
public interface IAnalyticsRepository : IRepository<Analytics>
{
    // ============================================================================
    // BASIC CRUD OPERATIONS
    // ============================================================================
    
    /// <summary>
    /// Get analytics by unique code
    /// </summary>
    Task<Analytics> GetByCodeAsync(string analyticsCode);
    
    /// <summary>
    /// Check if analytics code exists
    /// </summary>
    Task<bool> ExistsByCodeAsync(string analyticsCode);

    // ============================================================================
    // QUERY BY TYPE AND CATEGORY
    // ============================================================================
    
    /// <summary>
    /// Get analytics by type
    /// </summary>
    Task<List<Analytics>> GetByTypeAsync(AnalyticsType type, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics by category
    /// </summary>
    Task<List<Analytics>> GetByCategoryAsync(AnalyticsCategory category, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics by type and category
    /// </summary>
    Task<List<Analytics>> GetByTypeAndCategoryAsync(AnalyticsType type, AnalyticsCategory category, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics by status
    /// </summary>
    Task<List<Analytics>> GetByStatusAsync(AnalyticsStatus status, int pageNumber = 1, int pageSize = 20);

    // ============================================================================
    // QUERY BY TIME PERIOD
    // ============================================================================
    
    /// <summary>
    /// Get analytics by analysis period
    /// </summary>
    Task<List<Analytics>> GetByAnalysisPeriodAsync(DateTime periodStart, DateTime periodEnd, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics computed within date range
    /// </summary>
    Task<List<Analytics>> GetByComputationDateAsync(DateTime computedStart, DateTime computedEnd, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics for specific date
    /// </summary>
    Task<List<Analytics>> GetForDateAsync(DateTime date, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get latest analytics by category
    /// </summary>
    Task<Analytics> GetLatestByCategoryAsync(AnalyticsCategory category);

    // ============================================================================
    // QUERY BY USER AND DATA SOURCE
    // ============================================================================
    
    /// <summary>
    /// Get analytics computed by user
    /// </summary>
    Task<List<Analytics>> GetByComputedByAsync(string userId, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics by data source
    /// </summary>
    Task<List<Analytics>> GetByDataSourceAsync(string dataSource, int pageNumber = 1, int pageSize = 20);

    // ============================================================================
    // QUERY BY FRESHNESS AND EXPIRATION
    // ============================================================================
    
    /// <summary>
    /// Get fresh analytics (not expired)
    /// </summary>
    Task<List<Analytics>> GetFreshAnalyticsAsync(DateTime currentTime, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get stale analytics
    /// </summary>
    Task<List<Analytics>> GetStaleAnalyticsAsync(int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get expired analytics
    /// </summary>
    Task<List<Analytics>> GetExpiredAnalyticsAsync(DateTime currentTime, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics expiring within hours
    /// </summary>
    Task<List<Analytics>> GetExpiringWithinAsync(int hours, DateTime currentTime);

    // ============================================================================
    // SEARCH AND FILTER
    // ============================================================================
    
    /// <summary>
    /// Search analytics by name, code, or description
    /// </summary>
    Task<List<Analytics>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics with advanced filters
    /// </summary>
    Task<List<Analytics>> GetWithFiltersAsync(
        AnalyticsType? type = null,
        AnalyticsCategory? category = null,
        AnalyticsStatus? status = null,
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        DateTime? computedStart = null,
        DateTime? computedEnd = null,
        string computedBy = null,
        string dataSource = null,
        bool? isStale = null,
        bool? hasInsights = null,
        bool? hasForecasts = null,
        int pageNumber = 1,
        int pageSize = 20);

    // ============================================================================
    // INSIGHTS AND KPI QUERIES
    // ============================================================================
    
    /// <summary>
    /// Get analytics with insights
    /// </summary>
    Task<List<Analytics>> GetWithInsightsAsync(int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics by insight type
    /// </summary>
    Task<List<Analytics>> GetByInsightTypeAsync(InsightType insightType, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics by insight severity
    /// </summary>
    Task<List<Analytics>> GetByInsightSeverityAsync(InsightSeverity severity, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics with KPIs
    /// </summary>
    Task<List<Analytics>> GetWithKPIsAsync(int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics with critical insights
    /// </summary>
    Task<List<Analytics>> GetWithCriticalInsightsAsync(int pageNumber = 1, int pageSize = 20);

    // ============================================================================
    // FORECASTING AND TRENDS
    // ============================================================================
    
    /// <summary>
    /// Get analytics with forecasts
    /// </summary>
    Task<List<Analytics>> GetWithForecastsAsync(int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics with trend data
    /// </summary>
    Task<List<Analytics>> GetWithTrendDataAsync(int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get predictive analytics
    /// </summary>
    Task<List<Analytics>> GetPredictiveAnalyticsAsync(int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics by confidence level range
    /// </summary>
    Task<List<Analytics>> GetByConfidenceLevelRangeAsync(decimal minConfidence, decimal maxConfidence, int pageNumber = 1, int pageSize = 20);

    // ============================================================================
    // METRICS AND PERFORMANCE
    // ============================================================================
    
    /// <summary>
    /// Get analytics by metric count range
    /// </summary>
    Task<List<Analytics>> GetByMetricCountRangeAsync(int minMetrics, int maxMetrics, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics by computation duration range
    /// </summary>
    Task<List<Analytics>> GetByComputationDurationRangeAsync(TimeSpan minDuration, TimeSpan maxDuration, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get slowest analytics computations
    /// </summary>
    Task<List<Analytics>> GetSlowestComputationsAsync(int count = 10);
    
    /// <summary>
    /// Get fastest analytics computations
    /// </summary>
    Task<List<Analytics>> GetFastestComputationsAsync(int count = 10);

    // ============================================================================
    // STATISTICS AND ANALYTICS
    // ============================================================================
    
    /// <summary>
    /// Get analytics count by type
    /// </summary>
    Task<Dictionary<AnalyticsType, int>> GetCountByTypeAsync();
    
    /// <summary>
    /// Get analytics count by category
    /// </summary>
    Task<Dictionary<AnalyticsCategory, int>> GetCountByCategoryAsync();
    
    /// <summary>
    /// Get analytics count by status
    /// </summary>
    Task<Dictionary<AnalyticsStatus, int>> GetCountByStatusAsync();
    
    /// <summary>
    /// Get computation statistics for period
    /// </summary>
    Task<Dictionary<string, object>> GetComputationStatisticsAsync(DateTime periodStart, DateTime periodEnd);
    
    /// <summary>
    /// Get insight statistics
    /// </summary>
    Task<Dictionary<string, object>> GetInsightStatisticsAsync();
    
    /// <summary>
    /// Get KPI statistics
    /// </summary>
    Task<Dictionary<string, object>> GetKPIStatisticsAsync();
    
    /// <summary>
    /// Get data source usage statistics
    /// </summary>
    Task<Dictionary<string, int>> GetDataSourceUsageStatisticsAsync();

    // ============================================================================
    // COMPARISON AND BENCHMARKING
    // ============================================================================
    
    /// <summary>
    /// Get analytics for comparison (same category, different periods)
    /// </summary>
    Task<List<Analytics>> GetForComparisonAsync(AnalyticsCategory category, List<DateTime> periods);
    
    /// <summary>
    /// Get benchmark analytics
    /// </summary>
    Task<List<Analytics>> GetBenchmarkAnalyticsAsync(AnalyticsCategory category, int count = 5);
    
    /// <summary>
    /// Get analytics with variance data
    /// </summary>
    Task<List<Analytics>> GetWithVarianceDataAsync(int pageNumber = 1, int pageSize = 20);

    // ============================================================================
    // MAINTENANCE AND CLEANUP
    // ============================================================================
    
    /// <summary>
    /// Get analytics eligible for cleanup
    /// </summary>
    Task<List<Analytics>> GetEligibleForCleanupAsync(DateTime cutoffDate);
    
    /// <summary>
    /// Get failed analytics computations
    /// </summary>
    Task<List<Analytics>> GetFailedComputationsAsync(int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get analytics requiring refresh
    /// </summary>
    Task<List<Analytics>> GetRequiringRefreshAsync(DateTime currentTime, int maxAgeHours = 24);

    // ============================================================================
    // BULK OPERATIONS
    // ============================================================================
    
    /// <summary>
    /// Bulk update analytics status
    /// </summary>
    Task BulkUpdateStatusAsync(List<Guid> analyticsIds, AnalyticsStatus newStatus);
    
    /// <summary>
    /// Bulk mark as stale
    /// </summary>
    Task BulkMarkAsStaleAsync(List<Guid> analyticsIds);
    
    /// <summary>
    /// Bulk refresh analytics
    /// </summary>
    Task BulkRefreshAsync(List<Guid> analyticsIds, string refreshedBy);
    
    /// <summary>
    /// Delete old analytics (hard delete for cleanup)
    /// </summary>
    Task DeleteOldAnalyticsAsync(DateTime cutoffDate);

    // ============================================================================
    // VALIDATION AND INTEGRITY
    // ============================================================================
    
    /// <summary>
    /// Validate analytics data integrity
    /// </summary>
    Task<List<string>> ValidateDataIntegrityAsync(Guid analyticsId);
    
    /// <summary>
    /// Check for inconsistent metrics
    /// </summary>
    Task<List<Analytics>> GetWithInconsistentMetricsAsync();
    
    /// <summary>
    /// Get analytics with missing required data
    /// </summary>
    Task<List<Analytics>> GetWithMissingDataAsync();

    // ============================================================================
    // ADVANCED ANALYTICS
    // ============================================================================
    
    /// <summary>
    /// Get analytics correlation matrix
    /// </summary>
    Task<Dictionary<string, Dictionary<string, decimal>>> GetCorrelationMatrixAsync(AnalyticsCategory category, DateTime periodStart, DateTime periodEnd);
    
    /// <summary>
    /// Get trend analysis for metric
    /// </summary>
    Task<List<Dictionary<string, object>>> GetTrendAnalysisAsync(string metricCode, AnalyticsCategory category, DateTime periodStart, DateTime periodEnd);
    
    /// <summary>
    /// Get anomaly detection results
    /// </summary>
    Task<List<Dictionary<string, object>>> GetAnomalyDetectionAsync(AnalyticsCategory category, DateTime periodStart, DateTime periodEnd);

    // ============================================================================
    // CUSTOM QUERIES
    // ============================================================================
    
    /// <summary>
    /// Execute custom query with parameters
    /// </summary>
    Task<List<Analytics>> ExecuteCustomQueryAsync(string query, Dictionary<string, object> parameters);
    
    /// <summary>
    /// Get analytics summary data
    /// </summary>
    Task<Dictionary<string, object>> GetAnalyticsSummaryAsync(Guid analyticsId);
    
    /// <summary>
    /// Get recommended analytics for user
    /// </summary>
    Task<List<Analytics>> GetRecommendedForUserAsync(string userId, List<AnalyticsCategory> categories, int count = 5);
    
    /// <summary>
    /// Get analytics performance benchmarks
    /// </summary>
    Task<Dictionary<string, object>> GetPerformanceBenchmarksAsync();
}