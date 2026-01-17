using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Dashboard Repository Interface - Data access for Dashboard aggregate
/// Supports executive, operational, risk, and custom dashboards
/// Industry Standard: Repository pattern for business intelligence systems
/// </summary>
public interface IDashboardRepository : IRepository<Dashboard>
{
    // ============================================================================
    // BASIC CRUD OPERATIONS
    // ============================================================================
    
    /// <summary>
    /// Get dashboard by unique code
    /// </summary>
    Task<Dashboard> GetByCodeAsync(string dashboardCode);
    
    /// <summary>
    /// Check if dashboard code exists
    /// </summary>
    Task<bool> ExistsByCodeAsync(string dashboardCode);

    // ============================================================================
    // QUERY BY TYPE AND STATUS
    // ============================================================================
    
    /// <summary>
    /// Get dashboards by type
    /// </summary>
    Task<List<Dashboard>> GetByTypeAsync(DashboardType type, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get dashboards by status
    /// </summary>
    Task<List<Dashboard>> GetByStatusAsync(DashboardStatus status, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get active dashboards
    /// </summary>
    Task<List<Dashboard>> GetActiveDashboardsAsync(int pageNumber = 1, int pageSize = 20);

    // ============================================================================
    // QUERY BY USER AND ACCESS
    // ============================================================================
    
    /// <summary>
    /// Get dashboards created by user
    /// </summary>
    Task<List<Dashboard>> GetByCreatedByAsync(string userId, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get dashboards accessible by user (owned, shared, or public)
    /// </summary>
    Task<List<Dashboard>> GetAccessibleByUserAsync(string userId, List<string> userRoles, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get dashboards shared with user
    /// </summary>
    Task<List<Dashboard>> GetSharedWithUserAsync(string userId, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get dashboards accessible by role
    /// </summary>
    Task<List<Dashboard>> GetByRoleAsync(string role, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get public dashboards
    /// </summary>
    Task<List<Dashboard>> GetPublicDashboardsAsync(int pageNumber = 1, int pageSize = 20);

    // ============================================================================
    // QUERY BY VISIBILITY AND SHARING
    // ============================================================================
    
    /// <summary>
    /// Get dashboards by visibility
    /// </summary>
    Task<List<Dashboard>> GetByVisibilityAsync(DashboardVisibility visibility, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get dashboards shared with specific roles
    /// </summary>
    Task<List<Dashboard>> GetSharedWithRolesAsync(List<string> roles, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get most shared dashboards
    /// </summary>
    Task<List<Dashboard>> GetMostSharedDashboardsAsync(int count = 10);

    // ============================================================================
    // QUERY BY USAGE AND POPULARITY
    // ============================================================================
    
    /// <summary>
    /// Get most viewed dashboards
    /// </summary>
    Task<List<Dashboard>> GetMostViewedDashboardsAsync(int count = 10);
    
    /// <summary>
    /// Get recently viewed dashboards by user
    /// </summary>
    Task<List<Dashboard>> GetRecentlyViewedByUserAsync(string userId, int count = 10);
    
    /// <summary>
    /// Get dashboards viewed within date range
    /// </summary>
    Task<List<Dashboard>> GetViewedWithinDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get dashboards by view count range
    /// </summary>
    Task<List<Dashboard>> GetByViewCountRangeAsync(int minViews, int maxViews, int pageNumber = 1, int pageSize = 20);

    // ============================================================================
    // QUERY BY TIME AND REFRESH
    // ============================================================================
    
    /// <summary>
    /// Get dashboards created within date range
    /// </summary>
    Task<List<Dashboard>> GetCreatedWithinDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get dashboards requiring refresh
    /// </summary>
    Task<List<Dashboard>> GetRequiringRefreshAsync(DateTime currentTime);
    
    /// <summary>
    /// Get dashboards with auto refresh enabled
    /// </summary>
    Task<List<Dashboard>> GetWithAutoRefreshAsync(int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get dashboards last refreshed before date
    /// </summary>
    Task<List<Dashboard>> GetLastRefreshedBeforeAsync(DateTime cutoffDate, int pageNumber = 1, int pageSize = 20);

    // ============================================================================
    // SEARCH AND FILTER
    // ============================================================================
    
    /// <summary>
    /// Search dashboards by name, code, or description
    /// </summary>
    Task<List<Dashboard>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get dashboards with advanced filters
    /// </summary>
    Task<List<Dashboard>> GetWithFiltersAsync(
        DashboardType? type = null,
        DashboardStatus? status = null,
        DashboardVisibility? visibility = null,
        string createdBy = null,
        DateTime? createdStart = null,
        DateTime? createdEnd = null,
        bool? isPublic = null,
        bool? autoRefresh = null,
        int? minViews = null,
        int? maxViews = null,
        int pageNumber = 1,
        int pageSize = 20);

    // ============================================================================
    // WIDGET OPERATIONS
    // ============================================================================
    
    /// <summary>
    /// Get dashboards containing specific widget type
    /// </summary>
    Task<List<Dashboard>> GetWithWidgetTypeAsync(WidgetType widgetType, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get dashboards with widget count range
    /// </summary>
    Task<List<Dashboard>> GetByWidgetCountRangeAsync(int minWidgets, int maxWidgets, int pageNumber = 1, int pageSize = 20);
    
    /// <summary>
    /// Get dashboards with no widgets
    /// </summary>
    Task<List<Dashboard>> GetWithNoWidgetsAsync(int pageNumber = 1, int pageSize = 20);

    // ============================================================================
    // STATISTICS AND ANALYTICS
    // ============================================================================
    
    /// <summary>
    /// Get dashboard count by type
    /// </summary>
    Task<Dictionary<DashboardType, int>> GetCountByTypeAsync();
    
    /// <summary>
    /// Get dashboard count by status
    /// </summary>
    Task<Dictionary<DashboardStatus, int>> GetCountByStatusAsync();
    
    /// <summary>
    /// Get dashboard count by visibility
    /// </summary>
    Task<Dictionary<DashboardVisibility, int>> GetCountByVisibilityAsync();
    
    /// <summary>
    /// Get dashboard usage statistics
    /// </summary>
    Task<Dictionary<string, object>> GetUsageStatisticsAsync(DateTime periodStart, DateTime periodEnd);
    
    /// <summary>
    /// Get widget usage statistics
    /// </summary>
    Task<Dictionary<WidgetType, int>> GetWidgetUsageStatisticsAsync();
    
    /// <summary>
    /// Get dashboard creation trends
    /// </summary>
    Task<Dictionary<string, int>> GetCreationTrendsAsync(DateTime periodStart, DateTime periodEnd);

    // ============================================================================
    // PERFORMANCE AND MONITORING
    // ============================================================================
    
    /// <summary>
    /// Get dashboards with slow refresh times
    /// </summary>
    Task<List<Dashboard>> GetSlowRefreshDashboardsAsync(int thresholdMinutes = 5);
    
    /// <summary>
    /// Get dashboard performance metrics
    /// </summary>
    Task<Dictionary<string, object>> GetPerformanceMetricsAsync(Guid dashboardId, DateTime periodStart, DateTime periodEnd);
    
    /// <summary>
    /// Get system-wide dashboard metrics
    /// </summary>
    Task<Dictionary<string, object>> GetSystemMetricsAsync();

    // ============================================================================
    // MAINTENANCE AND CLEANUP
    // ============================================================================
    
    /// <summary>
    /// Get inactive dashboards (not viewed for specified days)
    /// </summary>
    Task<List<Dashboard>> GetInactiveDashboardsAsync(int inactiveDays = 90);
    
    /// <summary>
    /// Get dashboards eligible for archival
    /// </summary>
    Task<List<Dashboard>> GetEligibleForArchivalAsync(DateTime cutoffDate);
    
    /// <summary>
    /// Get orphaned dashboards (creator no longer exists)
    /// </summary>
    Task<List<Dashboard>> GetOrphanedDashboardsAsync();

    // ============================================================================
    // BULK OPERATIONS
    // ============================================================================
    
    /// <summary>
    /// Bulk update dashboard status
    /// </summary>
    Task BulkUpdateStatusAsync(List<Guid> dashboardIds, DashboardStatus newStatus);
    
    /// <summary>
    /// Bulk update auto refresh settings
    /// </summary>
    Task BulkUpdateAutoRefreshAsync(List<Guid> dashboardIds, bool autoRefresh, int intervalMinutes);
    
    /// <summary>
    /// Bulk share dashboards with role
    /// </summary>
    Task BulkShareWithRoleAsync(List<Guid> dashboardIds, string role);
    
    /// <summary>
    /// Bulk remove user access
    /// </summary>
    Task BulkRemoveUserAccessAsync(List<Guid> dashboardIds, string userId);

    // ============================================================================
    // ACCESS CONTROL
    // ============================================================================
    
    /// <summary>
    /// Check if user has access to dashboard
    /// </summary>
    Task<bool> HasUserAccessAsync(Guid dashboardId, string userId, List<string> userRoles);
    
    /// <summary>
    /// Get users with access to dashboard
    /// </summary>
    Task<List<string>> GetUsersWithAccessAsync(Guid dashboardId);
    
    /// <summary>
    /// Get roles with access to dashboard
    /// </summary>
    Task<List<string>> GetRolesWithAccessAsync(Guid dashboardId);

    // ============================================================================
    // VALIDATION AND INTEGRITY
    // ============================================================================
    
    /// <summary>
    /// Validate dashboard configuration
    /// </summary>
    Task<List<string>> ValidateConfigurationAsync(Guid dashboardId);
    
    /// <summary>
    /// Check for broken widget references
    /// </summary>
    Task<List<string>> CheckBrokenWidgetReferencesAsync(Guid dashboardId);
    
    /// <summary>
    /// Get dashboards with configuration issues
    /// </summary>
    Task<List<Dashboard>> GetWithConfigurationIssuesAsync();

    // ============================================================================
    // CUSTOM QUERIES
    // ============================================================================
    
    /// <summary>
    /// Execute custom query with parameters
    /// </summary>
    Task<List<Dashboard>> ExecuteCustomQueryAsync(string query, Dictionary<string, object> parameters);
    
    /// <summary>
    /// Get dashboard summary data
    /// </summary>
    Task<Dictionary<string, object>> GetDashboardSummaryAsync(Guid dashboardId);
    
    /// <summary>
    /// Get recommended dashboards for user
    /// </summary>
    Task<List<Dashboard>> GetRecommendedForUserAsync(string userId, List<string> userRoles, int count = 5);
}