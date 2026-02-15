using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services;

public class BackgroundTaskService : BackgroundService, IBackgroundTaskService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundTaskService> _logger;
    private readonly Dictionary<string, DateTime> _lastExecutionTimes = new();
    private readonly Dictionary<string, bool> _runningTasks = new();
    private readonly object _lockObject = new();

    public BackgroundTaskService(
        IServiceProvider serviceProvider,
        ILogger<BackgroundTaskService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Task Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteScheduledTasksAsync();
                
                // Wait for 5 minutes before next execution
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Background Task Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background task execution");
                
                // Wait longer on error to avoid rapid retries
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        _logger.LogInformation("Background Task Service stopped");
    }

    public async Task ExecuteScheduledTasksAsync()
    {
        var now = DateTime.UtcNow;

        // Process workflow deadlines every 15 minutes
        if (ShouldExecuteTask("ProcessWorkflowDeadlines", TimeSpan.FromMinutes(15)))
        {
            await ProcessWorkflowDeadlinesAsync();
        }

        // Process expired workflows every 30 minutes
        if (ShouldExecuteTask("ProcessExpiredWorkflows", TimeSpan.FromMinutes(30)))
        {
            await ProcessExpiredWorkflowsAsync();
        }

        // Process pending notifications every 10 minutes
        if (ShouldExecuteTask("ProcessPendingNotifications", TimeSpan.FromMinutes(10)))
        {
            await ProcessPendingNotificationsAsync();
        }
    }

    public async Task ProcessWorkflowDeadlinesAsync()
    {
        if (!await TryStartTaskAsync("ProcessWorkflowDeadlines"))
            return;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            _logger.LogInformation("Starting workflow deadline processing");

            await notificationService.SendDeadlineRemindersAsync();

            _logger.LogInformation("Completed workflow deadline processing");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing workflow deadlines");
        }
        finally
        {
            await CompleteTaskAsync("ProcessWorkflowDeadlines");
        }
    }

    public async Task ProcessExpiredWorkflowsAsync()
    {
        if (!await TryStartTaskAsync("ProcessExpiredWorkflows"))
            return;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var makerCheckerService = scope.ServiceProvider.GetRequiredService<IMakerCheckerService>();

            _logger.LogInformation("Starting expired workflow processing");

            var result = await makerCheckerService.AutoEscalateExpiredWorkflowsAsync();
            
            if (result.IsSuccess && result.EscalatedWorkflowIds.Any())
            {
                _logger.LogInformation("Auto-escalated {Count} expired workflows: {WorkflowIds}",
                    result.EscalatedWorkflowIds.Count,
                    string.Join(", ", result.EscalatedWorkflowIds));
            }

            _logger.LogInformation("Completed expired workflow processing");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing expired workflows");
        }
        finally
        {
            await CompleteTaskAsync("ProcessExpiredWorkflows");
        }
    }

    public async Task ProcessPendingNotificationsAsync()
    {
        if (!await TryStartTaskAsync("ProcessPendingNotifications"))
            return;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            _logger.LogInformation("Starting pending notification processing");

            await notificationService.ProcessPendingNotificationsAsync();

            _logger.LogInformation("Completed pending notification processing");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing pending notifications");
        }
        finally
        {
            await CompleteTaskAsync("ProcessPendingNotifications");
        }
    }

    public async Task<bool> IsTaskRunningAsync(string taskName)
    {
        await Task.CompletedTask; // Make async
        
        lock (_lockObject)
        {
            return _runningTasks.ContainsKey(taskName) && _runningTasks[taskName];
        }
    }

    public async Task StartBackgroundProcessingAsync(CancellationToken cancellationToken)
    {
        await ExecuteAsync(cancellationToken);
    }

    public async Task StopBackgroundProcessingAsync()
    {
        await Task.CompletedTask;
        _logger.LogInformation("Background processing stop requested");
    }

    private bool ShouldExecuteTask(string taskName, TimeSpan interval)
    {
        lock (_lockObject)
        {
            if (_runningTasks.ContainsKey(taskName) && _runningTasks[taskName])
            {
                return false; // Task is already running
            }

            if (!_lastExecutionTimes.ContainsKey(taskName))
            {
                return true; // First execution
            }

            var timeSinceLastExecution = DateTime.UtcNow - _lastExecutionTimes[taskName];
            return timeSinceLastExecution >= interval;
        }
    }

    private async Task<bool> TryStartTaskAsync(string taskName)
    {
        await Task.CompletedTask; // Make async
        
        lock (_lockObject)
        {
            if (_runningTasks.ContainsKey(taskName) && _runningTasks[taskName])
            {
                return false; // Task is already running
            }

            _runningTasks[taskName] = true;
            _lastExecutionTimes[taskName] = DateTime.UtcNow;
            return true;
        }
    }

    private async Task CompleteTaskAsync(string taskName)
    {
        await Task.CompletedTask; // Make async
        
        lock (_lockObject)
        {
            _runningTasks[taskName] = false;
        }
    }

    public override void Dispose()
    {
        _logger.LogInformation("Background Task Service disposing");
        base.Dispose();
    }
}