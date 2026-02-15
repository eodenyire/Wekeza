using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services;

public class NotificationService : INotificationService
{
    private readonly MVP4DbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        MVP4DbContext context,
        ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Notification Management

    public async Task<bool> SendApprovalNotificationAsync(Guid workflowId, string approverRole, string message)
    {
        try
        {
            var workflow = await _context.WorkflowInstances.FindAsync(workflowId);
            if (workflow == null) return false;

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                RoleName = approverRole,
                NotificationType = NotificationType.ApprovalRequired.ToString(),
                Title = $"Approval Required: {workflow.WorkflowType}",
                Message = message,
                RelatedWorkflowId = workflowId,
                Priority = DeterminePriority(workflow.Priority),
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = workflow.ApprovalDeadline,
                ActionUrl = $"/approval/queue/{workflowId}",
                Metadata = new Dictionary<string, object>
                {
                    ["WorkflowType"] = workflow.WorkflowType,
                    ["Amount"] = workflow.Amount ?? 0,
                    ["InitiatedBy"] = workflow.InitiatedBy,
                    ["BusinessJustification"] = workflow.BusinessJustification ?? ""
                }
            };

            await SaveNotificationAsync(notification);

            _logger.LogInformation("Approval notification sent for workflow {WorkflowId} to role {ApproverRole}",
                workflowId, approverRole);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending approval notification for workflow {WorkflowId}", workflowId);
            return false;
        }
    }

    public async Task<bool> SendEscalationNotificationAsync(Guid workflowId, string escalatedToRole, string reason)
    {
        try
        {
            var workflow = await _context.WorkflowInstances.FindAsync(workflowId);
            if (workflow == null) return false;

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                RoleName = escalatedToRole,
                NotificationType = NotificationType.WorkflowEscalated.ToString(),
                Title = $"Escalated Workflow: {workflow.WorkflowType}",
                Message = $"Workflow has been escalated. Reason: {reason}",
                RelatedWorkflowId = workflowId,
                Priority = "High",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = workflow.ApprovalDeadline,
                ActionUrl = $"/approval/queue/{workflowId}",
                Metadata = new Dictionary<string, object>
                {
                    ["WorkflowType"] = workflow.WorkflowType,
                    ["Amount"] = workflow.Amount ?? 0,
                    ["EscalationReason"] = reason,
                    ["OriginalDeadline"] = workflow.ApprovalDeadline?.ToString() ?? ""
                }
            };

            await SaveNotificationAsync(notification);

            _logger.LogInformation("Escalation notification sent for workflow {WorkflowId} to role {EscalatedToRole}",
                workflowId, escalatedToRole);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending escalation notification for workflow {WorkflowId}", workflowId);
            return false;
        }
    }

    public async Task<bool> SendDeadlineReminderAsync(Guid workflowId, string approverRole, DateTime deadline)
    {
        try
        {
            var workflow = await _context.WorkflowInstances.FindAsync(workflowId);
            if (workflow == null) return false;

            var timeRemaining = deadline - DateTime.UtcNow;
            var urgencyMessage = timeRemaining.TotalHours < 4 ? "URGENT: " : "";
            
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                RoleName = approverRole,
                NotificationType = NotificationType.DeadlineReminder.ToString(),
                Title = $"{urgencyMessage}Approval Deadline Approaching",
                Message = $"Workflow {workflow.WorkflowType} requires approval by {deadline:yyyy-MM-dd HH:mm}. Time remaining: {FormatTimeRemaining(timeRemaining)}",
                RelatedWorkflowId = workflowId,
                Priority = timeRemaining.TotalHours < 4 ? "Critical" : "High",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = deadline,
                ActionUrl = $"/approval/queue/{workflowId}",
                Metadata = new Dictionary<string, object>
                {
                    ["WorkflowType"] = workflow.WorkflowType,
                    ["Deadline"] = deadline,
                    ["HoursRemaining"] = timeRemaining.TotalHours,
                    ["IsUrgent"] = timeRemaining.TotalHours < 4
                }
            };

            await SaveNotificationAsync(notification);

            _logger.LogInformation("Deadline reminder sent for workflow {WorkflowId} to role {ApproverRole}",
                workflowId, approverRole);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending deadline reminder for workflow {WorkflowId}", workflowId);
            return false;
        }
    }

    public async Task<bool> SendWorkflowCompletedNotificationAsync(Guid workflowId, Guid initiatedBy, string status)
    {
        try
        {
            var workflow = await _context.WorkflowInstances.FindAsync(workflowId);
            if (workflow == null) return false;

            var notificationType = status.ToLower() switch
            {
                "approved" => NotificationType.WorkflowCompleted,
                "rejected" => NotificationType.WorkflowRejected,
                "cancelled" => NotificationType.WorkflowCancelled,
                _ => NotificationType.WorkflowCompleted
            };

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = initiatedBy,
                NotificationType = notificationType.ToString(),
                Title = $"Workflow {status}: {workflow.WorkflowType}",
                Message = $"Your workflow request has been {status.ToLower()}.",
                RelatedWorkflowId = workflowId,
                Priority = "Normal",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ActionUrl = $"/workflow/history/{workflowId}",
                Metadata = new Dictionary<string, object>
                {
                    ["WorkflowType"] = workflow.WorkflowType,
                    ["FinalStatus"] = status,
                    ["CompletedAt"] = workflow.CompletedAt?.ToString() ?? ""
                }
            };

            await SaveNotificationAsync(notification);

            _logger.LogInformation("Workflow completion notification sent for workflow {WorkflowId} to user {InitiatedBy}",
                workflowId, initiatedBy);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending workflow completion notification for workflow {WorkflowId}", workflowId);
            return false;
        }
    }

    #endregion

    #region Notification Retrieval

    public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false)
    {
        var query = _context.Set<Notification>()
            .Where(n => n.UserId == userId);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(100) // Limit to recent notifications
            .ToListAsync();
    }

    public async Task<List<Notification>> GetRoleNotificationsAsync(string roleName, bool unreadOnly = false)
    {
        var query = _context.Set<Notification>()
            .Where(n => n.RoleName == roleName);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(100) // Limit to recent notifications
            .ToListAsync();
    }

    public async Task<bool> MarkNotificationAsReadAsync(Guid notificationId, Guid userId)
    {
        try
        {
            var notification = await _context.Set<Notification>()
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && 
                                         (n.UserId == userId || n.RoleName != null));

            if (notification == null) return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
            return false;
        }
    }

    public async Task<bool> MarkAllNotificationsAsReadAsync(Guid userId)
    {
        try
        {
            var notifications = await _context.Set<Notification>()
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
            return false;
        }
    }

    #endregion

    #region Notification Settings

    public async Task<NotificationSettings> GetUserNotificationSettingsAsync(Guid userId)
    {
        var settings = await _context.Set<NotificationSettings>()
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings == null)
        {
            // Return default settings
            settings = new NotificationSettings
            {
                UserId = userId,
                EmailNotifications = true,
                InAppNotifications = true,
                ApprovalReminders = true,
                EscalationAlerts = true,
                DeadlineWarnings = true,
                ReminderFrequencyHours = 24
            };
        }

        return settings;
    }

    public async Task<bool> UpdateUserNotificationSettingsAsync(Guid userId, NotificationSettings settings)
    {
        try
        {
            var existingSettings = await _context.Set<NotificationSettings>()
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (existingSettings == null)
            {
                settings.UserId = userId;
                _context.Set<NotificationSettings>().Add(settings);
            }
            else
            {
                existingSettings.EmailNotifications = settings.EmailNotifications;
                existingSettings.InAppNotifications = settings.InAppNotifications;
                existingSettings.ApprovalReminders = settings.ApprovalReminders;
                existingSettings.EscalationAlerts = settings.EscalationAlerts;
                existingSettings.DeadlineWarnings = settings.DeadlineWarnings;
                existingSettings.ReminderFrequencyHours = settings.ReminderFrequencyHours;
                existingSettings.NotificationTypeSettings = settings.NotificationTypeSettings;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification settings for user {UserId}", userId);
            return false;
        }
    }

    #endregion

    #region Background Processing

    public async Task ProcessPendingNotificationsAsync()
    {
        try
        {
            // Get expired notifications and clean them up
            var expiredNotifications = await _context.Set<Notification>()
                .Where(n => n.ExpiresAt.HasValue && n.ExpiresAt.Value < DateTime.UtcNow)
                .ToListAsync();

            if (expiredNotifications.Any())
            {
                _context.Set<Notification>().RemoveRange(expiredNotifications);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cleaned up {Count} expired notifications", expiredNotifications.Count);
            }

            // Process any pending notification delivery logic here
            // (e.g., email sending, push notifications, etc.)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing pending notifications");
        }
    }

    public async Task SendDeadlineRemindersAsync()
    {
        try
        {
            var now = DateTime.UtcNow;
            var reminderThreshold = now.AddHours(24); // Send reminders 24 hours before deadline

            var workflowsNearingDeadline = await _context.WorkflowInstances
                .Where(w => w.Status == WorkflowStatus.Pending.ToString() &&
                           w.ApprovalDeadline.HasValue &&
                           w.ApprovalDeadline.Value <= reminderThreshold &&
                           w.ApprovalDeadline.Value > now)
                .ToListAsync();

            foreach (var workflow in workflowsNearingDeadline)
            {
                // Check if reminder was already sent recently
                var recentReminder = await _context.Set<Notification>()
                    .Where(n => n.RelatedWorkflowId == workflow.WorkflowId &&
                               n.NotificationType == NotificationType.DeadlineReminder.ToString() &&
                               n.CreatedAt > now.AddHours(-12)) // Don't send more than twice per day
                    .AnyAsync();

                if (!recentReminder)
                {
                    // Get current approval step to determine approver role
                    var currentStep = await _context.ApprovalSteps
                        .Where(s => s.WorkflowId == workflow.WorkflowId && 
                                   s.Status == ApprovalStepStatus.Pending.ToString())
                        .OrderBy(s => s.StepOrder)
                        .FirstOrDefaultAsync();

                    if (currentStep != null)
                    {
                        await SendDeadlineReminderAsync(workflow.WorkflowId, currentStep.ApproverRole, workflow.ApprovalDeadline!.Value);
                    }
                }
            }

            _logger.LogInformation("Processed deadline reminders for {Count} workflows", workflowsNearingDeadline.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending deadline reminders");
        }
    }

    #endregion

    #region Private Helper Methods

    private async Task SaveNotificationAsync(Notification notification)
    {
        _context.Set<Notification>().Add(notification);
        await _context.SaveChangesAsync();
    }

    private string DeterminePriority(string? workflowPriority)
    {
        return workflowPriority?.ToLower() switch
        {
            "critical" => "Critical",
            "high" => "High",
            "low" => "Low",
            _ => "Normal"
        };
    }

    private string FormatTimeRemaining(TimeSpan timeRemaining)
    {
        if (timeRemaining.TotalDays >= 1)
        {
            return $"{(int)timeRemaining.TotalDays} day(s)";
        }
        else if (timeRemaining.TotalHours >= 1)
        {
            return $"{(int)timeRemaining.TotalHours} hour(s)";
        }
        else
        {
            return $"{(int)timeRemaining.TotalMinutes} minute(s)";
        }
    }

    #endregion
}