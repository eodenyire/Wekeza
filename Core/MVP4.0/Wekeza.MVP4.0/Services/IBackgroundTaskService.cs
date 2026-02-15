using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services;

public interface IBackgroundTaskService
{
    // Workflow Processing
    Task ProcessWorkflowDeadlinesAsync();
    Task ProcessExpiredWorkflowsAsync();
    Task ProcessPendingNotificationsAsync();
    
    // Scheduled Tasks
    Task ExecuteScheduledTasksAsync();
    Task<bool> IsTaskRunningAsync(string taskName);
    
    // Task Management
    Task StartBackgroundProcessingAsync(CancellationToken cancellationToken);
    Task StopBackgroundProcessingAsync();
}

public class BackgroundTaskResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ProcessedCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}