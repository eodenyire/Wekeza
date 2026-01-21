/*
 * TEMPORARILY COMMENTED OUT - FIXING COMPILATION ERRORS
 * This file will be restored and fixed incrementally
 * Issue: INotificationService interface not found and domain events integration
 */

/*
using MediatR;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Application.Common.Notifications;

/// <summary>
/// Principal-Grade Event Handler: Listens for critical bank events 
/// and dispatches the correct notifications automatically.
///📂 Wekeza.Core.Application/Common/Notifications
///1. NotificationDispatcher.cs (The Intelligent Router)
///We don't want to hardcode SMS/Email calls inside our Handlers (like TransferFundsHandler). Instead, we create a dispatcher that listens for Domain Events and decides which notification to send.
///
/// </summary>
public class NotificationDispatcher : 
    INotificationHandler<AccountFrozenEvent>,
    INotificationHandler<TransferCompletedEvent>,
    INotificationHandler<LoanApprovedEvent>
{
    private readonly INotificationService _notificationService;

    public NotificationDispatcher(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// When an account gets frozen, immediately notify the customer
    /// </summary>
    public async Task Handle(AccountFrozenEvent notification, CancellationToken cancellationToken)
    {
        await _notificationService.SendSmsAsync(
            phoneNumber: notification.CustomerPhone,
            message: $"ALERT: Your account {notification.AccountNumber} has been temporarily frozen. Contact your branch immediately."
        );

        await _notificationService.SendEmailAsync(
            email: notification.CustomerEmail,
            subject: "Account Security Alert",
            body: $"Your account {notification.AccountNumber} has been frozen for security reasons. Please contact us immediately."
        );
    }

    public async Task Handle(TransferCompletedEvent notification, CancellationToken cancellationToken)
    {
        // Send confirmation SMS to sender
        await _notificationService.SendSmsAsync(
            phoneNumber: notification.SenderPhone,
            message: $"Transfer of {notification.Amount:C} to {notification.RecipientName} completed successfully. Ref: {notification.TransactionReference}"
        );
    }

    public async Task Handle(LoanApprovedEvent notification, CancellationToken cancellationToken)
    {
        // Send loan approval notification
        await _notificationService.SendSmsAsync(
            phoneNumber: notification.CustomerPhone,
            message: $"Congratulations! Your loan application for {notification.LoanAmount:C} has been approved. Visit your branch to complete disbursement."
        );
    }
}
*/

namespace Wekeza.Core.Application.Common.Notifications
{
    // This namespace is temporarily empty while we fix compilation issues
    // Original NotificationDispatcher functionality will be restored incrementally
}