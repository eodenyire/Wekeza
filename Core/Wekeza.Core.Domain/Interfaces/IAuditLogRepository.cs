using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for AuditLog aggregate
/// Provides data access methods for audit trail operations
/// </summary>
public interface IAuditLogRepository
{
    // Basic CRUD Operations
    Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuditLog> AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<AuditLog> UpdateAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Query Operations by Event Type
    Task<List<AuditLog>> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByEventCategoryAsync(string eventCategory, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByLevelAsync(AuditLevel level, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByResultAsync(AuditResult result, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByRiskLevelAsync(RiskLevel riskLevel, CancellationToken cancellationToken = default);

    // Query Operations by User
    Task<List<AuditLog>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default);

    // Query Operations by Resource
    Task<List<AuditLog>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByResourceIdAsync(string resourceId, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByActionAsync(string action, CancellationToken cancellationToken = default);

    // Time-based Queries
    Task<List<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByTimestampAsync(DateTime timestamp, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetRecentAsync(int count = 100, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetTodayAsync(CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetThisWeekAsync(CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetThisMonthAsync(CancellationToken cancellationToken = default);

    // Security & Compliance Queries
    Task<List<AuditLog>> GetSecurityEventsAsync(CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetHighRiskEventsAsync(CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetFailedOperationsAsync(CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetUnauthorizedAttemptsAsync(CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetRequiringReviewAsync(CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetWithComplianceFlagAsync(string flag, CancellationToken cancellationToken = default);

    // Application & Module Queries
    Task<List<AuditLog>> GetByApplicationAsync(string applicationName, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByModuleAsync(string moduleName, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default);

    // Search Operations
    Task<List<AuditLog>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> SearchByMessageAsync(string message, CancellationToken cancellationToken = default);

    // Pagination Support
    Task<(List<AuditLog> AuditLogs, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? eventType = null,
        string? userId = null,
        AuditLevel? level = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    // Complex Filtering
    Task<List<AuditLog>> GetFilteredAsync(
        string? eventType = null,
        string? eventCategory = null,
        string? userId = null,
        string? resource = null,
        string? action = null,
        AuditLevel? level = null,
        AuditResult? result = null,
        RiskLevel? riskLevel = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        bool? requiresReview = null,
        CancellationToken cancellationToken = default);

    // Archival Operations
    Task<List<AuditLog>> GetExpiredAsync(CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetArchivedAsync(CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetForArchivalAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);
    Task BulkArchiveAsync(List<Guid> auditLogIds, string archiveLocation, CancellationToken cancellationToken = default);

    // Reporting & Analytics
    Task<Dictionary<string, int>> GetEventTypeCountsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<Dictionary<AuditLevel, int>> GetLevelCountsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<Dictionary<AuditResult, int>> GetResultCountsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetUserActivityCountsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetResourceAccessCountsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetIpAddressCountsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

    // Performance & Statistics
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<int> GetCountByUserAsync(string userId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<int> GetSecurityEventCountAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<int> GetFailedOperationCountAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

    // Bulk Operations
    Task<List<AuditLog>> AddRangeAsync(List<AuditLog> auditLogs, CancellationToken cancellationToken = default);
    Task BulkUpdateReviewStatusAsync(List<Guid> auditLogIds, bool requiresReview, string reason, CancellationToken cancellationToken = default);
    Task BulkAddComplianceFlagAsync(List<Guid> auditLogIds, string flag, CancellationToken cancellationToken = default);
    Task BulkExtendRetentionAsync(List<Guid> auditLogIds, TimeSpan extension, string reason, CancellationToken cancellationToken = default);

    // Cleanup Operations
    Task<int> DeleteExpiredAsync(CancellationToken cancellationToken = default);
    Task<int> DeleteOlderThanAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);
    Task<int> DeleteByRetentionPolicyAsync(CancellationToken cancellationToken = default);

    // Existence Checks
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasUserActivityAsync(string userId, DateTime? since = null, CancellationToken cancellationToken = default);
    Task<bool> HasSecurityEventsAsync(DateTime? since = null, CancellationToken cancellationToken = default);

    // Transaction & Financial Audit
    Task<List<AuditLog>> GetTransactionAuditAsync(string? accountId = null, decimal? minAmount = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetLargeTransactionAuditAsync(decimal threshold, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetFailedTransactionAuditAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

    // System & Administrative Audit
    Task<List<AuditLog>> GetSystemEventsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetConfigurationChangesAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetUserManagementEventsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetRoleManagementEventsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
}