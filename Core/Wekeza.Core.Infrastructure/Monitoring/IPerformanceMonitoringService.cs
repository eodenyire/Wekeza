namespace Wekeza.Core.Infrastructure.Monitoring;

/// <summary>
/// Performance monitoring service interface
/// Provides comprehensive application performance monitoring and metrics collection
/// </summary>
public interface IPerformanceMonitoringService
{
    // Request/Response Monitoring
    Task<IDisposable> StartRequestMonitoringAsync(string operationName, Dictionary<string, object>? properties = null);
    Task RecordRequestAsync(string operationName, TimeSpan duration, bool success, Dictionary<string, object>? properties = null);
    Task RecordExceptionAsync(Exception exception, string operationName, Dictionary<string, object>? properties = null);

    // Database Performance Monitoring
    Task RecordDatabaseQueryAsync(string queryType, string tableName, TimeSpan duration, bool success);
    Task RecordDatabaseConnectionAsync(TimeSpan connectionTime, bool success);
    Task<DatabasePerformanceMetrics> GetDatabaseMetricsAsync(TimeSpan? period = null);

    // Cache Performance Monitoring
    Task RecordCacheOperationAsync(string operation, string key, TimeSpan duration, bool hit);
    Task<CachePerformanceMetrics> GetCacheMetricsAsync(TimeSpan? period = null);

    // Custom Metrics
    Task RecordMetricAsync(string metricName, double value, Dictionary<string, object>? properties = null);
    Task IncrementCounterAsync(string counterName, Dictionary<string, object>? properties = null);
    Task RecordGaugeAsync(string gaugeName, double value, Dictionary<string, object>? properties = null);
    Task RecordHistogramAsync(string histogramName, double value, Dictionary<string, object>? properties = null);

    // System Performance Monitoring
    Task<SystemPerformanceMetrics> GetSystemMetricsAsync();
    Task RecordMemoryUsageAsync();
    Task RecordCpuUsageAsync();
    Task RecordDiskUsageAsync();

    // Business Metrics
    Task RecordTransactionAsync(string transactionType, decimal amount, string currency, bool success);
    Task RecordUserActionAsync(string userId, string action, Dictionary<string, object>? properties = null);
    Task<BusinessMetrics> GetBusinessMetricsAsync(TimeSpan? period = null);

    // Performance Alerts
    Task CheckPerformanceThresholdsAsync();
    Task<List<PerformanceAlert>> GetActiveAlertsAsync();
    Task ResolveAlertAsync(Guid alertId, string resolvedBy);

    // Health Checks
    Task<HealthCheckResult> PerformHealthCheckAsync();
    Task<Dictionary<string, HealthCheckResult>> PerformDetailedHealthCheckAsync();

    // Reporting
    Task<PerformanceReport> GeneratePerformanceReportAsync(DateTime startDate, DateTime endDate);
    Task<List<PerformanceMetric>> GetMetricsAsync(string metricName, DateTime startDate, DateTime endDate);
    Task<Dictionary<string, object>> GetPerformanceSummaryAsync(TimeSpan? period = null);
}

/// <summary>
/// Database performance metrics
/// </summary>
public class DatabasePerformanceMetrics
{
    public double AverageQueryTime { get; set; }
    public double AverageConnectionTime { get; set; }
    public long TotalQueries { get; set; }
    public long SuccessfulQueries { get; set; }
    public long FailedQueries { get; set; }
    public double SuccessRate { get; set; }
    public Dictionary<string, QueryMetrics> QueryTypeMetrics { get; set; } = new();
    public List<SlowQuery> SlowQueries { get; set; } = new();
}

/// <summary>
/// Cache performance metrics
/// </summary>
public class CachePerformanceMetrics
{
    public double HitRate { get; set; }
    public double MissRate { get; set; }
    public double AverageResponseTime { get; set; }
    public long TotalOperations { get; set; }
    public long CacheHits { get; set; }
    public long CacheMisses { get; set; }
    public Dictionary<string, double> OperationMetrics { get; set; } = new();
}

/// <summary>
/// System performance metrics
/// </summary>
public class SystemPerformanceMetrics
{
    public double CpuUsagePercentage { get; set; }
    public long MemoryUsageBytes { get; set; }
    public double MemoryUsagePercentage { get; set; }
    public long DiskUsageBytes { get; set; }
    public double DiskUsagePercentage { get; set; }
    public int ActiveThreads { get; set; }
    public long GarbageCollections { get; set; }
    public TimeSpan Uptime { get; set; }
    public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
}

/// <summary>
/// Business metrics
/// </summary>
public class BusinessMetrics
{
    public long TotalTransactions { get; set; }
    public decimal TotalTransactionAmount { get; set; }
    public long SuccessfulTransactions { get; set; }
    public long FailedTransactions { get; set; }
    public double TransactionSuccessRate { get; set; }
    public Dictionary<string, TransactionTypeMetrics> TransactionTypeMetrics { get; set; } = new();
    public Dictionary<string, long> UserActionCounts { get; set; } = new();
    public long ActiveUsers { get; set; }
}

/// <summary>
/// Performance alert
/// </summary>
public class PerformanceAlert
{
    public Guid Id { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public double CurrentValue { get; set; }
    public double ThresholdValue { get; set; }
    public AlertSeverity Severity { get; set; }
    public DateTime TriggeredAt { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Health check result
/// </summary>
public class HealthCheckResult
{
    public string Name { get; set; } = string.Empty;
    public HealthStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public Exception? Exception { get; set; }
}

/// <summary>
/// Performance report
/// </summary>
public class PerformanceReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan Period { get; set; }
    public SystemPerformanceMetrics SystemMetrics { get; set; } = new();
    public DatabasePerformanceMetrics DatabaseMetrics { get; set; } = new();
    public CachePerformanceMetrics CacheMetrics { get; set; } = new();
    public BusinessMetrics BusinessMetrics { get; set; } = new();
    public List<PerformanceAlert> Alerts { get; set; } = new();
    public Dictionary<string, List<PerformanceMetric>> CustomMetrics { get; set; } = new();
}

/// <summary>
/// Performance metric data point
/// </summary>
public class PerformanceMetric
{
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Query metrics for specific query types
/// </summary>
public class QueryMetrics
{
    public string QueryType { get; set; } = string.Empty;
    public double AverageExecutionTime { get; set; }
    public long TotalExecutions { get; set; }
    public long SuccessfulExecutions { get; set; }
    public long FailedExecutions { get; set; }
    public double SuccessRate { get; set; }
}

/// <summary>
/// Slow query information
/// </summary>
public class SlowQuery
{
    public string Query { get; set; } = string.Empty;
    public TimeSpan ExecutionTime { get; set; }
    public DateTime Timestamp { get; set; }
    public string TableName { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// Transaction type metrics
/// </summary>
public class TransactionTypeMetrics
{
    public string TransactionType { get; set; } = string.Empty;
    public long Count { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageAmount { get; set; }
    public long SuccessfulCount { get; set; }
    public long FailedCount { get; set; }
    public double SuccessRate { get; set; }
}

/// <summary>
/// Alert severity levels
/// </summary>
public enum AlertSeverity
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Health status enumeration
/// </summary>
public enum HealthStatus
{
    Healthy,
    Degraded,
    Unhealthy,
    Unknown
}