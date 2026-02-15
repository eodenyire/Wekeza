using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services;

public interface INotificationService
{
    // Notification Management
    Task<bool> SendApprovalNotificationAsync(Guid workflowId, string approverRole, string message);
    Task<bool> SendEscalationNotificationAsync(Guid workflowId, string escalatedToRole, string reason);
    Task<bool> SendDeadlineReminderAsync(Guid workflowId, string approverRole, DateTime deadline);
    Task<bool> SendWorkflowCompletedNotificationAsync(Guid workflowId, Guid initiatedBy, string status);

    // Notification Retrieval
    Task<List<Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false);
    Task<List<Notification>> GetRoleNotificationsAsync(string roleName, bool unreadOnly = false);
    Task<bool> MarkNotificationAsReadAsync(Guid notificationId, Guid userId);
    Task<bool> MarkAllNotificationsAsReadAsync(Guid userId);

    // Notification Settings
    Task<NotificationSettings> GetUserNotificationSettingsAsync(Guid userId);
    Task<bool> UpdateUserNotificationSettingsAsync(Guid userId, NotificationSettings settings);

    // Background Processing
    Task ProcessPendingNotificationsAsync();
    Task SendDeadlineRemindersAsync();
}

// DTOs and Models
public class Notification
{
    public Guid NotificationId { get; set; }
    public Guid? UserId { get; set; }
    public string? RoleName { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid? RelatedWorkflowId { get; set; }
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Critical
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? ActionUrl { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class NotificationSettings
{
    public Guid UserId { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public bool InAppNotifications { get; set; } = true;
    public bool ApprovalReminders { get; set; } = true;
    public bool EscalationAlerts { get; set; } = true;
    public bool DeadlineWarnings { get; set; } = true;
    public int ReminderFrequencyHours { get; set; } = 24;
    public Dictionary<string, bool> NotificationTypeSettings { get; set; } = new();
}

public enum NotificationType
{
    ApprovalRequired,
    WorkflowEscalated,
    DeadlineReminder,
    WorkflowCompleted,
    WorkflowRejected,
    WorkflowCancelled,
    SystemAlert
}

public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Critical
}