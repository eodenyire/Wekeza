using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Batch Job - Tracks system batch processing jobs and scheduler execution
/// Supports monitoring, restart, rollback, and error recovery
/// Industry standard: Finacle Batch, T24 Scheduler, Oracle FLEXCUBE Batch Framework
/// </summary>
public class BatchJob : AggregateRoot
{
    public string JobCode { get; private set; }
    public string JobName { get; private set; }
    public string Description { get; private set; }
    public string Category { get; private set; } // EOD, Housekeeping, Reporting, Integration, Settlement
    
    // Schedule
    public string ScheduleExpression { get; private set; } // Cron format
    public DateTime? NextScheduledRun { get; private set; }
    public DateTime? LastRunTime { get; private set; }
    public List<BatchJobExecution> ExecutionHistory { get; private set; }
    
    // Job Configuration
    public Dictionary<string, object> Parameters { get; private set; }
    public int TimeoutSeconds { get; private set; }
    public int RetryCount { get; private set; }
    public int MaxConcurrentRuns { get; private set; }
    public List<string> Dependencies { get; private set; }
    
    // Status & Monitoring
    public BatchJobStatus Status { get; private set; }
    public BatchJobExecutionStatus CurrentExecutionStatus { get; private set; }
    public bool IsEnabled { get; private set; }
    public bool IsRunning { get; private set; }
    public DateTime? CurrentRunStartedAt { get; private set; }
    
    // Performance Metrics
    public long TotalRecordsProcessed { get; private set; }
    public long TotalRecordsFailed { get; private set; }
    public List<BatchJobMetric> PerformanceMetrics { get; private set; }
    public TimeSpan? AverageExecutionTime { get; private set; }
    
    // Error Handling
    public List<BatchJobError> Errors { get; private set; }
    public string? LastErrorMessage { get; private set; }
    public DateTime? LastErrorTime { get; private set; }
    
    // Audit
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string? LastModifiedBy { get; private set; }

    private BatchJob() : base(Guid.NewGuid())
    {
        ExecutionHistory = new List<BatchJobExecution>();
        Parameters = new Dictionary<string, object>();
        Dependencies = new List<string>();
        PerformanceMetrics = new List<BatchJobMetric>();
        Errors = new List<BatchJobError>();
    }

    public BatchJob(
        string jobCode,
        string jobName,
        string description,
        string category,
        string scheduleExpression,
        string createdBy) : this()
    {
        if (string.IsNullOrWhiteSpace(jobCode))
            throw new ArgumentException("Job code cannot be empty", nameof(jobCode));

        Id = Guid.NewGuid();
        JobCode = jobCode;
        JobName = jobName;
        Description = description;
        Category = category;
        ScheduleExpression = scheduleExpression;
        CreatedBy = createdBy;
        Status = BatchJobStatus.Configured;
        CurrentExecutionStatus = BatchJobExecutionStatus.Ready;
        IsEnabled = true;
        IsRunning = false;
        TimeoutSeconds = 3600;
        RetryCount = 3;
        MaxConcurrentRuns = 1;
    }

    public void StartExecution(string executedBy, Dictionary<string, object>? runtimeParameters = null)
    {
        if (IsRunning || ExecutionHistory.Any(e => e.Status == BatchJobExecutionStatus.Running))
            throw new InvalidOperationException($"Job {JobCode} is already running");

        if (!IsEnabled)
            throw new InvalidOperationException($"Job {JobCode} is disabled");

        CurrentExecutionStatus = BatchJobExecutionStatus.Running;
        IsRunning = true;
        CurrentRunStartedAt = DateTime.UtcNow;

        var execution = new BatchJobExecution
        {
            StartedAt = DateTime.UtcNow,
            StartedBy = executedBy,
            RuntimeParameters = runtimeParameters ?? new Dictionary<string, object>(),
            Status = BatchJobExecutionStatus.Running
        };

        ExecutionHistory.Add(execution);
        Status = BatchJobStatus.Running;
    }

    public void CompleteExecution(long recordsProcessed, long recordsFailed, string executedBy)
    {
        if (!IsRunning)
            throw new InvalidOperationException("No active execution to complete");

        var currentExecution = ExecutionHistory.OrderByDescending(e => e.StartedAt).FirstOrDefault();
        if (currentExecution == null)
            return;

        currentExecution.Status = BatchJobExecutionStatus.Completed;
        currentExecution.CompletedAt = DateTime.UtcNow;
        currentExecution.CompletedBy = executedBy;
        currentExecution.RecordsProcessed = recordsProcessed;
        currentExecution.RecordsFailed = recordsFailed;
        currentExecution.ExecutionDuration = currentExecution.CompletedAt.Value - currentExecution.StartedAt;

        TotalRecordsProcessed += recordsProcessed;
        TotalRecordsFailed += recordsFailed;
        IsRunning = false;
        CurrentExecutionStatus = BatchJobExecutionStatus.Completed;
        Status = BatchJobStatus.Idle;
        LastRunTime = DateTime.UtcNow;

        // Update average execution time
        UpdateAverageExecutionTime();
    }

    public void FailExecution(string errorMessage, string executedBy)
    {
        if (!IsRunning)
            throw new InvalidOperationException("No active execution to fail");

        var currentExecution = ExecutionHistory.OrderByDescending(e => e.StartedAt).FirstOrDefault();
        if (currentExecution == null)
            return;

        currentExecution.Status = BatchJobExecutionStatus.Failed;
        currentExecution.CompletedAt = DateTime.UtcNow;
        currentExecution.CompletedBy = executedBy;
        currentExecution.ErrorMessage = errorMessage;

        IsRunning = false;
        CurrentExecutionStatus = BatchJobExecutionStatus.Failed;
        Status = BatchJobStatus.Error;
        LastErrorMessage = errorMessage;
        LastErrorTime = DateTime.UtcNow;

        Errors.Add(new BatchJobError
        {
            ErrorMessage = errorMessage,
            ErrorTime = DateTime.UtcNow,
            StackTrace = new Dictionary<string, object>()
        });
    }

    public void RestartJob(string restartedBy)
    {
        if (IsRunning)
            throw new InvalidOperationException($"Job {JobCode} is already running");

        Status = BatchJobStatus.Configured;
        CurrentExecutionStatus = BatchJobExecutionStatus.Ready;
        StartExecution(restartedBy);
    }

    public void DisableJob(string disabledBy, string reason)
    {
        IsEnabled = false;
        Status = BatchJobStatus.Disabled;
        LastModifiedBy = disabledBy;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void EnableJob(string enabledBy)
    {
        IsEnabled = true;
        Status = BatchJobStatus.Configured;
        LastModifiedBy = enabledBy;
        LastModifiedAt = DateTime.UtcNow;
    }

    private void UpdateAverageExecutionTime()
    {
        var completedExecutions = ExecutionHistory
            .Where(e => e.Status == BatchJobExecutionStatus.Completed && e.ExecutionDuration.HasValue)
            .ToList();

        if (completedExecutions.Any())
        {
            var avgTicks = (long)completedExecutions.Average(e => e.ExecutionDuration.Value.Ticks);
            AverageExecutionTime = new TimeSpan(avgTicks);
        }
    }
}

public class BatchJobExecution
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime StartedAt { get; set; }
    public string StartedBy { get; set; }
    public Dictionary<string, object> RuntimeParameters { get; set; }
    public BatchJobExecutionStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletedBy { get; set; }
    public TimeSpan? ExecutionDuration { get; set; }
    public long RecordsProcessed { get; set; }
    public long RecordsFailed { get; set; }
    public string? ErrorMessage { get; set; }
}

public class BatchJobMetric
{
    public DateTime RecordedAt { get; set; }
    public long RecordsProcessed { get; set; }
    public long RecordsFailed { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public decimal ThroughputPerSecond { get; set; }
}

public class BatchJobError
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ErrorMessage { get; set; }
    public DateTime ErrorTime { get; set; }
    public Dictionary<string, object> StackTrace { get; set; }
    public int RetryCount { get; set; }
}

public enum BatchJobStatus
{
    Configured = 1,
    Running = 2,
    Idle = 3,
    Error = 4,
    Disabled = 5,
    Suspended = 6
}

public enum BatchJobExecutionStatus
{
    Ready = 1,
    Running = 2,
    Completed = 3,
    Failed = 4,
    Timeout = 5,
    Cancelled = 6
}
