using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Wekeza.Core.Infrastructure.Notifications;

namespace Wekeza.Core.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;
    private readonly ConcurrentDictionary<string, List<string>> _userGroups = new();
    private readonly ConcurrentDictionary<string, bool> _connectedUsers = new();
    private readonly ConcurrentDictionary<string, NotificationTemplate> _templates = new();
    private readonly ConcurrentDictionary<string, List<NotificationHistory>> _notificationHistory = new();
    private readonly ConcurrentDictionary<string, NotificationPreferences> _userPreferences = new();
    private readonly ConcurrentDictionary<Guid, ScheduledNotification> _scheduledNotifications = new();

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Real-time Notifications (SignalR)
    public async Task SendToUserAsync(string userId, string message, NotificationType type = NotificationType.Info, object? data = null)
    {
        try
        {
            _logger.LogInformation("Sending notification to user {UserId} with type {Type}", userId, type);
            
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
            {
                Message = message,
                Type = type.ToString(),
                Data = data,
                Timestamp = DateTime.UtcNow
            });

            await AddToHistoryAsync(userId, message, type, NotificationChannel.RealTime, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
            throw;
        }
    }

    public async Task SendToUsersAsync(IEnumerable<string> userIds, string message, NotificationType type = NotificationType.Info, object? data = null)
    {
        try
        {
            _logger.LogInformation("Sending notification to {Count} users with type {Type}", userIds.Count(), type);
            
            foreach (var userId in userIds)
            {
                await SendToUserAsync(userId, message, type, data);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to multiple users");
            throw;
        }
    }

    public async Task SendToGroupAsync(string groupName, string message, NotificationType type = NotificationType.Info, object? data = null)
    {
        try
        {
            _logger.LogInformation("Sending notification to group {GroupName} with type {Type}", groupName, type);
            
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", new
            {
                Message = message,
                Type = type.ToString(),
                Data = data,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to group {GroupName}", groupName);
            throw;
        }
    }

    public async Task SendToAllAsync(string message, NotificationType type = NotificationType.Info, object? data = null)
    {
        try
        {
            _logger.LogInformation("Broadcasting notification to all users with type {Type}", type);
            
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
            {
                Message = message,
                Type = type.ToString(),
                Data = data,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error broadcasting notification to all users");
            throw;
        }
    }

    // Connection Management
    public async Task AddUserToGroupAsync(string userId, string groupName)
    {
        try
        {
            _logger.LogInformation("Adding user {UserId} to group {GroupName}", userId, groupName);
            
            var groups = _userGroups.GetOrAdd(userId, _ => new List<string>());
            if (!groups.Contains(groupName))
            {
                groups.Add(groupName);
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user {UserId} to group {GroupName}", userId, groupName);
            throw;
        }
    }

    public async Task RemoveUserFromGroupAsync(string userId, string groupName)
    {
        try
        {
            _logger.LogInformation("Removing user {UserId} from group {GroupName}", userId, groupName);
            
            if (_userGroups.TryGetValue(userId, out var groups))
            {
                groups.Remove(groupName);
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user {UserId} from group {GroupName}", userId, groupName);
            throw;
        }
    }

    public async Task<List<string>> GetUserGroupsAsync(string userId)
    {
        try
        {
            return await Task.FromResult(_userGroups.GetValueOrDefault(userId) ?? new List<string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting groups for user {UserId}", userId);
            throw;
        }
    }

    public async Task<List<string>> GetConnectedUsersAsync()
    {
        try
        {
            return await Task.FromResult(_connectedUsers.Keys.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting connected users");
            throw;
        }
    }

    public async Task<bool> IsUserConnectedAsync(string userId)
    {
        try
        {
            return await Task.FromResult(_connectedUsers.ContainsKey(userId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} is connected", userId);
            throw;
        }
    }

    // Banking-Specific Notifications
    public async Task SendTransactionNotificationAsync(string userId, TransactionNotification notification)
    {
        try
        {
            _logger.LogInformation("Sending transaction notification to user {UserId} for transaction {TransactionId}", 
                userId, notification.TransactionId);

            var message = $"Transaction {notification.TransactionType}: {notification.Currency} {notification.Amount:N2}";
            await SendToUserAsync(userId, message, NotificationType.Transaction, notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending transaction notification to user {UserId}", userId);
            throw;
        }
    }

    public async Task SendAccountNotificationAsync(string userId, AccountNotification notification)
    {
        try
        {
            _logger.LogInformation("Sending account notification to user {UserId} for account {AccountNumber}", 
                userId, notification.AccountNumber);

            await SendToUserAsync(userId, notification.Message, NotificationType.Account, notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending account notification to user {UserId}", userId);
            throw;
        }
    }

    public async Task SendLoanNotificationAsync(string userId, LoanNotification notification)
    {
        try
        {
            _logger.LogInformation("Sending loan notification to user {UserId} for loan {LoanId}", 
                userId, notification.LoanId);

            await SendToUserAsync(userId, notification.Message, NotificationType.Loan, notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending loan notification to user {UserId}", userId);
            throw;
        }
    }

    public async Task SendSecurityAlertAsync(string userId, SecurityAlert alert)
    {
        try
        {
            _logger.LogWarning("Sending security alert to user {UserId}: {AlertType} - {Severity}", 
                userId, alert.AlertType, alert.Severity);

            await SendToUserAsync(userId, alert.Message, NotificationType.Security, alert);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending security alert to user {UserId}", userId);
            throw;
        }
    }

    public async Task SendSystemMaintenanceNotificationAsync(SystemMaintenanceNotification notification)
    {
        try
        {
            _logger.LogInformation("Sending system maintenance notification: {Title}", notification.Title);

            var message = $"{notification.Title}: {notification.Description}";
            
            if (notification.TargetGroups.Any())
            {
                foreach (var group in notification.TargetGroups)
                {
                    await SendToGroupAsync(group, message, NotificationType.System, notification);
                }
            }
            else
            {
                await SendToAllAsync(message, NotificationType.System, notification);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending system maintenance notification");
            throw;
        }
    }

    // Notification Templates
    public async Task<NotificationTemplate> GetTemplateAsync(string templateName)
    {
        try
        {
            if (_templates.TryGetValue(templateName, out var template))
            {
                return await Task.FromResult(template);
            }

            // Return a default template if not found
            return await Task.FromResult(new NotificationTemplate
            {
                Name = templateName,
                Subject = "Notification",
                Body = "Default notification message",
                Type = NotificationType.Info,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template {TemplateName}", templateName);
            throw;
        }
    }

    public async Task<string> RenderTemplateAsync(string templateName, object data)
    {
        try
        {
            var template = await GetTemplateAsync(templateName);
            
            // Simple template rendering - replace placeholders
            var rendered = template.Body;
            if (data != null)
            {
                var properties = data.GetType().GetProperties();
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(data)?.ToString() ?? string.Empty;
                    rendered = rendered.Replace($"{{{prop.Name}}}", value);
                }
            }

            return rendered;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering template {TemplateName}", templateName);
            throw;
        }
    }

    public async Task SendTemplatedNotificationAsync(string userId, string templateName, object data)
    {
        try
        {
            _logger.LogInformation("Sending templated notification to user {UserId} using template {TemplateName}", 
                userId, templateName);

            var template = await GetTemplateAsync(templateName);
            var message = await RenderTemplateAsync(templateName, data);
            
            await SendToUserAsync(userId, message, template.Type, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending templated notification to user {UserId}", userId);
            throw;
        }
    }

    // Notification History
    public async Task<List<NotificationHistory>> GetUserNotificationHistoryAsync(string userId, int pageSize = 20, int pageNumber = 1)
    {
        try
        {
            if (!_notificationHistory.TryGetValue(userId, out var history))
            {
                return await Task.FromResult(new List<NotificationHistory>());
            }

            var skip = (pageNumber - 1) * pageSize;
            return await Task.FromResult(history
                .OrderByDescending(h => h.SentAt)
                .Skip(skip)
                .Take(pageSize)
                .ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification history for user {UserId}", userId);
            throw;
        }
    }

    public async Task MarkNotificationAsReadAsync(Guid notificationId, string userId)
    {
        try
        {
            if (_notificationHistory.TryGetValue(userId, out var history))
            {
                var notification = history.FirstOrDefault(h => h.Id == notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                    _logger.LogInformation("Marked notification {NotificationId} as read for user {UserId}", 
                        notificationId, userId);
                }
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read for user {UserId}", 
                notificationId, userId);
            throw;
        }
    }

    public async Task MarkAllNotificationsAsReadAsync(string userId)
    {
        try
        {
            if (_notificationHistory.TryGetValue(userId, out var history))
            {
                foreach (var notification in history.Where(h => !h.IsRead))
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                }
                _logger.LogInformation("Marked all notifications as read for user {UserId}", userId);
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> GetUnreadNotificationCountAsync(string userId)
    {
        try
        {
            if (!_notificationHistory.TryGetValue(userId, out var history))
            {
                return await Task.FromResult(0);
            }

            return await Task.FromResult(history.Count(h => !h.IsRead));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread notification count for user {UserId}", userId);
            throw;
        }
    }

    // Notification Preferences
    public async Task<NotificationPreferences> GetUserPreferencesAsync(string userId)
    {
        try
        {
            if (_userPreferences.TryGetValue(userId, out var preferences))
            {
                return await Task.FromResult(preferences);
            }

            // Return default preferences
            var defaultPreferences = new NotificationPreferences
            {
                UserId = userId,
                ChannelEnabled = new Dictionary<NotificationChannel, bool>
                {
                    { NotificationChannel.RealTime, true },
                    { NotificationChannel.Email, true },
                    { NotificationChannel.SMS, true },
                    { NotificationChannel.Push, true },
                    { NotificationChannel.InApp, true }
                },
                EnableQuietHours = false,
                TimeZone = "UTC",
                Language = "en-US"
            };

            _userPreferences.TryAdd(userId, defaultPreferences);
            return await Task.FromResult(defaultPreferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting preferences for user {UserId}", userId);
            throw;
        }
    }

    public async Task UpdateUserPreferencesAsync(string userId, NotificationPreferences preferences)
    {
        try
        {
            _logger.LogInformation("Updating notification preferences for user {UserId}", userId);
            
            preferences.UserId = userId;
            _userPreferences.AddOrUpdate(userId, preferences, (key, old) => preferences);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating preferences for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ShouldSendNotificationAsync(string userId, NotificationType type, string channel)
    {
        try
        {
            var preferences = await GetUserPreferencesAsync(userId);
            
            if (!Enum.TryParse<NotificationChannel>(channel, out var notificationChannel))
            {
                return false;
            }

            // Check if channel is enabled
            if (preferences.ChannelEnabled.TryGetValue(notificationChannel, out var channelEnabled) && !channelEnabled)
            {
                return false;
            }

            // Check quiet hours
            if (preferences.EnableQuietHours)
            {
                var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, 
                    TimeZoneInfo.FindSystemTimeZoneById(preferences.TimeZone));
                var currentTime = now.TimeOfDay;

                if (preferences.QuietDays.Contains(now.DayOfWeek))
                {
                    return false;
                }

                if (preferences.QuietHoursStart < preferences.QuietHoursEnd)
                {
                    if (currentTime >= preferences.QuietHoursStart && currentTime <= preferences.QuietHoursEnd)
                    {
                        return false;
                    }
                }
                else
                {
                    if (currentTime >= preferences.QuietHoursStart || currentTime <= preferences.QuietHoursEnd)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if should send notification to user {UserId}", userId);
            return true; // Default to sending if error
        }
    }

    // Multi-Channel Notifications
    public async Task SendEmailNotificationAsync(string email, string subject, string body, bool isHtml = true)
    {
        try
        {
            _logger.LogInformation("Sending email notification to {Email} with subject: {Subject}", email, subject);
            
            // Stub implementation - would integrate with email service
            await Task.Delay(10); // Simulate sending
            _logger.LogInformation("Email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email notification to {Email}", email);
            throw;
        }
    }

    public async Task SendSmsNotificationAsync(string phoneNumber, string message)
    {
        try
        {
            _logger.LogInformation("Sending SMS notification to {PhoneNumber}", phoneNumber);
            
            // Stub implementation - would integrate with SMS service
            await Task.Delay(10); // Simulate sending
            _logger.LogInformation("SMS sent successfully to {PhoneNumber}", phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS notification to {PhoneNumber}", phoneNumber);
            throw;
        }
    }

    public async Task SendPushNotificationAsync(string userId, PushNotification notification)
    {
        try
        {
            _logger.LogInformation("Sending push notification to user {UserId}: {Title}", userId, notification.Title);
            
            // Stub implementation - would integrate with push notification service
            await Task.Delay(10); // Simulate sending
            _logger.LogInformation("Push notification sent successfully to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification to user {UserId}", userId);
            throw;
        }
    }

    // Bulk Notifications
    public async Task SendBulkNotificationsAsync(List<BulkNotificationRequest> requests)
    {
        try
        {
            _logger.LogInformation("Sending {Count} bulk notification requests", requests.Count);

            foreach (var request in requests)
            {
                foreach (var userId in request.UserIds)
                {
                    if (!string.IsNullOrEmpty(request.TemplateName))
                    {
                        await SendTemplatedNotificationAsync(userId, request.TemplateName, request.TemplateData);
                    }
                    else
                    {
                        await SendToUserAsync(userId, request.Message, request.Type, request.Data);
                    }
                }
            }

            _logger.LogInformation("Bulk notifications sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending bulk notifications");
            throw;
        }
    }

    public async Task<Guid> ScheduleNotificationAsync(ScheduledNotification notification)
    {
        try
        {
            notification.Id = Guid.NewGuid();
            notification.CreatedAt = DateTime.UtcNow;
            notification.Status = "Scheduled";

            _scheduledNotifications.TryAdd(notification.Id, notification);
            
            _logger.LogInformation("Scheduled notification {NotificationId} for {ScheduledTime}", 
                notification.Id, notification.ScheduledTime);

            return await Task.FromResult(notification.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling notification");
            throw;
        }
    }

    public async Task CancelScheduledNotificationAsync(Guid notificationId)
    {
        try
        {
            if (_scheduledNotifications.TryRemove(notificationId, out var notification))
            {
                notification.Status = "Cancelled";
                _logger.LogInformation("Cancelled scheduled notification {NotificationId}", notificationId);
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling scheduled notification {NotificationId}", notificationId);
            throw;
        }
    }

    // Analytics and Reporting
    public async Task<NotificationAnalytics> GetNotificationAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation("Generating notification analytics from {StartDate} to {EndDate}", 
                startDate, endDate);

            var analytics = new NotificationAnalytics
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalNotificationsSent = 0,
                TotalNotificationsDelivered = 0,
                TotalNotificationsFailed = 0,
                DeliveryRate = 100.0
            };

            // Calculate analytics from history
            foreach (var userHistory in _notificationHistory.Values)
            {
                var periodNotifications = userHistory
                    .Where(h => h.SentAt >= startDate && h.SentAt <= endDate)
                    .ToList();

                analytics.TotalNotificationsSent += periodNotifications.Count;

                foreach (var notification in periodNotifications)
                {
                    if (notification.Status == "Delivered")
                    {
                        analytics.TotalNotificationsDelivered++;
                    }
                    else if (notification.Status == "Failed")
                    {
                        analytics.TotalNotificationsFailed++;
                    }

                    // Count by type
                    if (analytics.NotificationsByType.ContainsKey(notification.Type))
                    {
                        analytics.NotificationsByType[notification.Type]++;
                    }
                    else
                    {
                        analytics.NotificationsByType[notification.Type] = 1;
                    }

                    // Count by channel
                    if (analytics.NotificationsByChannel.ContainsKey(notification.Channel))
                    {
                        analytics.NotificationsByChannel[notification.Channel]++;
                    }
                    else
                    {
                        analytics.NotificationsByChannel[notification.Channel] = 1;
                    }
                }
            }

            if (analytics.TotalNotificationsSent > 0)
            {
                analytics.DeliveryRate = (double)analytics.TotalNotificationsDelivered / analytics.TotalNotificationsSent * 100;
            }

            return await Task.FromResult(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating notification analytics");
            throw;
        }
    }

    public async Task<Dictionary<NotificationType, int>> GetNotificationCountsByTypeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var counts = new Dictionary<NotificationType, int>();

            foreach (var userHistory in _notificationHistory.Values)
            {
                var periodNotifications = userHistory
                    .Where(h => h.SentAt >= startDate && h.SentAt <= endDate);

                foreach (var notification in periodNotifications)
                {
                    if (counts.ContainsKey(notification.Type))
                    {
                        counts[notification.Type]++;
                    }
                    else
                    {
                        counts[notification.Type] = 1;
                    }
                }
            }

            return await Task.FromResult(counts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification counts by type");
            throw;
        }
    }

    public async Task<List<NotificationDeliveryReport>> GetDeliveryReportsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var reports = new List<NotificationDeliveryReport>();

            foreach (var kvp in _notificationHistory)
            {
                var userId = kvp.Key;
                var userHistory = kvp.Value;

                var periodNotifications = userHistory
                    .Where(h => h.SentAt >= startDate && h.SentAt <= endDate);

                foreach (var notification in periodNotifications)
                {
                    reports.Add(new NotificationDeliveryReport
                    {
                        NotificationId = notification.Id,
                        UserId = userId,
                        Type = notification.Type,
                        Channel = notification.Channel,
                        Status = notification.Status,
                        SentAt = notification.SentAt,
                        DeliveredAt = notification.ReadAt,
                        ReadAt = notification.ReadAt,
                        RetryCount = 0,
                        DeliveryTime = notification.ReadAt.HasValue 
                            ? notification.ReadAt.Value - notification.SentAt 
                            : null
                    });
                }
            }

            return await Task.FromResult(reports.OrderByDescending(r => r.SentAt).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting delivery reports");
            throw;
        }
    }

    // Helper method to add notifications to history
    private Task AddToHistoryAsync(string userId, string message, NotificationType type, NotificationChannel channel, object? data)
    {
        var history = _notificationHistory.GetOrAdd(userId, _ => new List<NotificationHistory>());
        
        history.Add(new NotificationHistory
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = type.ToString(),
            Message = message,
            Type = type,
            Channel = channel,
            IsRead = false,
            SentAt = DateTime.UtcNow,
            Status = "Delivered",
            Data = data != null ? new Dictionary<string, object> { { "data", data } } : new Dictionary<string, object>()
        });

        return Task.CompletedTask;
    }
}

// NotificationHub for SignalR
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} joined group {GroupName}", Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} left group {GroupName}", Context.ConnectionId, groupName);
    }
}