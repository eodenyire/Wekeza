using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for SystemParameter aggregate
/// Provides data access methods for system parameter management operations
/// </summary>
public interface ISystemParameterRepository
{
    // Basic CRUD Operations
    Task<SystemParameter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SystemParameter?> GetByCodeAsync(string parameterCode, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SystemParameter> AddAsync(SystemParameter parameter, CancellationToken cancellationToken = default);
    Task<SystemParameter> UpdateAsync(SystemParameter parameter, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Query Operations by Type and Category
    Task<List<SystemParameter>> GetByTypeAsync(ParameterType type, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetByCategoryAsync(ParameterCategory category, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetByDataTypeAsync(ParameterDataType dataType, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetInactiveAsync(CancellationToken cancellationToken = default);

    // Environment-based Queries
    Task<List<SystemParameter>> GetByEnvironmentAsync(string environment, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetEffectiveAsync(DateTime? effectiveDate = null, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetEffectiveByEnvironmentAsync(string environment, DateTime? effectiveDate = null, CancellationToken cancellationToken = default);

    // Security and Access Control
    Task<List<SystemParameter>> GetBySecurityLevelAsync(SecurityLevel securityLevel, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetEncryptedAsync(CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetRequiringApprovalAsync(CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetAccessibleByRoleAsync(string role, CancellationToken cancellationToken = default);

    // Value Operations
    Task<string?> GetValueAsync(string parameterCode, CancellationToken cancellationToken = default);
    Task<T?> GetTypedValueAsync<T>(string parameterCode, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> GetValuesAsync(List<string> parameterCodes, CancellationToken cancellationToken = default);
    Task<Dictionary<string, object>> GetTypedValuesAsync(List<string> parameterCodes, CancellationToken cancellationToken = default);

    // Search Operations
    Task<List<SystemParameter>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> SearchByDescriptionAsync(string description, CancellationToken cancellationToken = default);

    // Change History and Audit
    Task<List<SystemParameter>> GetRecentlyChangedAsync(DateTime since, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetChangedByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetChangedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<ParameterChange>> GetChangeHistoryAsync(string parameterCode, CancellationToken cancellationToken = default);

    // Pagination Support
    Task<(List<SystemParameter> Parameters, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        ParameterType? type = null,
        ParameterCategory? category = null,
        string? environment = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    // Validation Support
    Task<bool> IsCodeAvailableAsync(string parameterCode, Guid? excludeParameterId = null, CancellationToken cancellationToken = default);
    Task<bool> CanDeleteParameterAsync(Guid parameterId, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetDependentParametersAsync(string parameterCode, CancellationToken cancellationToken = default);

    // Configuration Management
    Task<Dictionary<string, string>> GetConfigurationSetAsync(string configurationName, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetSystemConfigurationAsync(CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetBusinessConfigurationAsync(CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetSecurityConfigurationAsync(CancellationToken cancellationToken = default);

    // Reporting & Analytics
    Task<Dictionary<ParameterType, int>> GetCountByTypeAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<ParameterCategory, int>> GetCountByCategoryAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetCountByEnvironmentAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<SecurityLevel, int>> GetCountBySecurityLevelAsync(CancellationToken cancellationToken = default);
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetEncryptedCountAsync(CancellationToken cancellationToken = default);

    // Bulk Operations
    Task<List<SystemParameter>> AddRangeAsync(List<SystemParameter> parameters, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> UpdateRangeAsync(List<SystemParameter> parameters, CancellationToken cancellationToken = default);
    Task BulkUpdateValuesAsync(Dictionary<string, string> parameterValues, string updatedBy, CancellationToken cancellationToken = default);
    Task BulkActivateAsync(List<string> parameterCodes, string activatedBy, CancellationToken cancellationToken = default);
    Task BulkDeactivateAsync(List<string> parameterCodes, string deactivatedBy, CancellationToken cancellationToken = default);
    Task BulkUpdateSecurityLevelAsync(List<string> parameterCodes, SecurityLevel securityLevel, string updatedBy, CancellationToken cancellationToken = default);

    // Existence Checks
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string parameterCode, CancellationToken cancellationToken = default);
    Task<bool> IsActiveAsync(string parameterCode, CancellationToken cancellationToken = default);
    Task<bool> IsEffectiveAsync(string parameterCode, DateTime? checkDate = null, CancellationToken cancellationToken = default);

    // Default Value Operations
    Task<List<SystemParameter>> GetWithDefaultValuesAsync(CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetModifiedFromDefaultAsync(CancellationToken cancellationToken = default);
    Task ResetToDefaultAsync(string parameterCode, string resetBy, CancellationToken cancellationToken = default);
    Task BulkResetToDefaultAsync(List<string> parameterCodes, string resetBy, CancellationToken cancellationToken = default);

    // Validation and Constraints
    Task<List<SystemParameter>> GetWithValidationRulesAsync(CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetWithAllowedValuesAsync(CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetRequiredParametersAsync(CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetWithConstraintsAsync(CancellationToken cancellationToken = default);

    // Import/Export Operations
    Task<List<SystemParameter>> ExportConfigurationAsync(string environment, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> ExportByTypeAsync(ParameterType type, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> ExportByCategoryAsync(ParameterCategory category, CancellationToken cancellationToken = default);
    Task ImportConfigurationAsync(List<SystemParameter> parameters, string importedBy, bool overwriteExisting = false, CancellationToken cancellationToken = default);

    // Monitoring and Health
    Task<List<SystemParameter>> GetCriticalParametersAsync(CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetParametersNearingExpirationAsync(int daysAhead = 30, CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetUnusedParametersAsync(DateTime since, CancellationToken cancellationToken = default);
    Task<Dictionary<string, object>> GetSystemHealthParametersAsync(CancellationToken cancellationToken = default);

    // Cache Support
    Task<Dictionary<string, string>> GetCacheableParametersAsync(CancellationToken cancellationToken = default);
    Task<List<SystemParameter>> GetFrequentlyAccessedAsync(int topCount = 50, CancellationToken cancellationToken = default);
    Task InvalidateCacheAsync(string parameterCode, CancellationToken cancellationToken = default);
    Task InvalidateAllCacheAsync(CancellationToken cancellationToken = default);
}