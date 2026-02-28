namespace Wekeza.Core.Infrastructure.Notifications;

/// <summary>
/// Real-time notification service interface
/// Provides enterprise-grade real-time notifications via SignalR and other channels
/// </summary>
public interface INotificationService
{
    // Real-time Notifications (SignalR)
    Task SendToUserAsync(string userId, string message, NotificationType type = NotificationType.Info, object? data = null);
    Task SendToUsersAsync(IEnumerable<string> userIds, string message, NotificationType type = NotificationType.Info, object? data = null);
    Task SendToGroupAsync(string groupName, string message, NotificationType type = NotificationType.Info, object? data = null);
    Task SendToAllAsync(string message, NotificationType type = NotificationType.Info, object? data = null);

    // Connection Management
    Task AddUserToGroupAsync(string userId, string groupName);
    Task RemoveUserFromGroupAsync(string userId, string groupName);
    Task<List<string>> GetUserGroupsAsync(string userId);
    Task<List<string>> GetConnectedUsersAsync();
    Task<bool> IsUserConnectedAsync(string userId);

    // Banking-Specific Notifications
    Task SendTransactionNotificationAsync(string userId, TransactionNotification notification);
    Task SendAccountNotificationAsync(string userId, AccountNotification notification);
    Task SendLoanNotificationAsync(string userId, LoanNotification notification);
    Task SendSecurityAlertAsync(string userId, SecurityAlert alert);
    Task SendSystemMaintenanceNotificationAsync(SystemMaintenanceNotification notification);

    // Notification Templates
    Task<NotificationTemplate> GetTemplateAsync(string templateName);
    Task<string> RenderTemplateAsync(string templateName, object data);
    Task SendTemplatedNotificationAsync(string userId, string templateName, object data);

    // Notification History
    Task<List<NotificationHistory>> GetUserNotificationHistoryAsync(string userId, int pageSize = 20, int pageNumber = 1);
    Task MarkNotificationAsReadAsync(Guid notificationId, string userId);
    Task MarkAllNotificationsAsReadAsync(string userId);
    Task<int> GetUnreadNotificationCountAsync(string userId);

    // Notification Preferences
    Task<NotificationPreferences> GetUserPreferencesAsync(string userId);
    Task UpdateUserPreferencesAsync(string userId, NotificationPreferences preferences);
    Task<bool> ShouldSendNotificationAsync(string userId, NotificationType type, string channel);

    // Multi-Channel Notifications
    Task SendEmailNotificationAsync(string email, string subject, string body, bool isHtml = true);
    Task SendSmsNotificationAsync(string phoneNumber, string message);
    Task SendPushNotificationAsync(string userId, PushNotification notification);

    // Bulk Notifications
    Task SendBulkNotificationsAsync(List<BulkNotificationRequest> requests);
    Task<Guid> ScheduleNotificationAsync(ScheduledNotification notification);
    Task CancelScheduledNotificationAsync(Guid notificationId);

    // Analytics and Reporting
    Task<NotificationAnalytics> GetNotificationAnalyticsAsync(DateTime startDate, DateTime endDate);
    Task<Dictionary<NotificationType, int>> GetNotificationCountsByTypeAsync(DateTime startDate, DateTime endDate);
    Task<List<NotificationDeliveryReport>> GetDeliveryReportsAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// Notification types
/// </summary>
public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error,
    Transaction,
    Account,
    Loan,
    Security,
    System,
    Marketing,
    Reminder
}

/// <summary>
/// Notification channels
/// </summary>
public enum NotificationChannel
{
    RealTime,
    Email,
    SMS,
    Push,
    InApp
}

/// <summary>
/// Transaction notification
/// </summary>
public class TransactionNotification
{
    public string TransactionId { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Account notification
/// </summary>
public class AccountNotification
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal? CurrentBalance { get; set; }
    public decimal? AvailableBalance { get; set; }
    public DateTime EventDate { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Loan notification
/// </summary>
public class LoanNotification
{
    public string LoanId { get; set; } = string.Empty;
    public string LoanType { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? OutstandingBalance { get; set; }
    public DateTime EventDate { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Security alert
/// </summary>
public class SecurityAlert
{
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// System maintenance notification
/// </summary>
public class SystemMaintenanceNotification
{
    public string MaintenanceType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }
    public List<string> AffectedServices { get; set; } = new();
    public string Impact { get; set; } = string.Empty;
    public List<string> TargetGroups { get; set; } = new();
}

/// <summary>
/// Notification template
/// </summary>
public class NotificationTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public List<NotificationChannel> Channels { get; set; } = new();
    public Dictionary<string, object> DefaultData { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Notification history
/// </summary>
public class NotificationHistory
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Notification preferences
/// </summary>
public class NotificationPreferences
{
    public string UserId { get; set; } = string.Empty;
    public Dictionary<NotificationType, List<NotificationChannel>> TypeChannelPreferences { get; set; } = new();
    public Dictionary<NotificationChannel, bool> ChannelEnabled { get; set; } = new();
    public TimeSpan QuietHoursStart { get; set; }
    public TimeSpan QuietHoursEnd { get; set; }
    public bool EnableQuietHours { get; set; }
    public List<DayOfWeek> QuietDays { get; set; } = new();
    public string TimeZone { get; set; } = "UTC";
    public string Language { get; set; } = "en-US";
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// Push notification
/// </summary>
public class PushNotification
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    public string Sound { get; set; } = "default";
    public Dictionary<string, object> Data { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public DateTime? ScheduledTime { get; set; }
    public TimeSpan? TimeToLive { get; set; }
}

/// <summary>
/// Bulk notification request
/// </summary>
public class BulkNotificationRequest
{
    public List<string> UserIds { get; set; } = new();
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public List<NotificationChannel> Channels { get; set; } = new();
    public object? Data { get; set; }
    public string? TemplateName { get; set; }
    public Dictionary<string, object> TemplateData { get; set; } = new();
}

/// <summary>
/// Scheduled notification
/// </summary>
public class ScheduledNotification
{
    public Guid Id { get; set; }
    public List<string> UserIds { get; set; } = new();
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public List<NotificationChannel> Channels { get; set; } = new();
    public DateTime ScheduledTime { get; set; }
    public object? Data { get; set; }
    public string? TemplateName { get; set; }
    public Dictionary<string, object> TemplateData { get; set; } = new();
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }
    public DateTime? RecurrenceEnd { get; set; }
    public string Status { get; set; } = "Scheduled";
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

/// <summary>
/// Notification analytics
/// </summary>
public class NotificationAnalytics
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public long TotalNotificationsSent { get; set; }
    public long TotalNotificationsDelivered { get; set; }
    public long TotalNotificationsFailed { get; set; }
    public double DeliveryRate { get; set; }
    public Dictionary<NotificationType, long> NotificationsByType { get; set; } = new();
    public Dictionary<NotificationChannel, long> NotificationsByChannel { get; set; } = new();
    public Dictionary<string, long> NotificationsByUser { get; set; } = new();
    public List<NotificationTrend> Trends { get; set; } = new();
    public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
}

/// <summary>
/// Notification delivery report
/// </summary>
public class NotificationDeliveryReport
{
    public Guid NotificationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public TimeSpan? DeliveryTime { get; set; }
}

/// <summary>
/// Notification trend data
/// </summary>
public class NotificationTrend
{
    public DateTime Date { get; set; }
    public long Count { get; set; }
    public NotificationType? Type { get; set; }
    public NotificationChannel? Channel { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}