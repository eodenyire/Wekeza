using Wekeza.Core.Infrastructure.Notifications;

namespace Wekeza.Core.Infrastructure.Services;

public class NotificationService : INotificationService
{
    public Task SendNotificationAsync(string userId, string message, string? title = null)
    {
        // Implementation for sending notifications
        return Task.CompletedTask;
    }

    public Task SendBroadcastNotificationAsync(string message, string? title = null)
    {
        // Implementation for broadcasting notifications
        return Task.CompletedTask;
    }

    public Task<bool> IsConnectedAsync(string userId)
    {
        return Task.FromResult(false);
    }
}