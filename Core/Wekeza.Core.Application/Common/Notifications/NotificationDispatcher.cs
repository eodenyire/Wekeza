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
    private readonly ICurrentUserService _currentUser;

    public NotificationDispatcher(INotificationService notificationService, ICurrentUserService currentUser)
    {
        _notificationService = notificationService;
        _currentUser = currentUser;
    }

    // 1. Handle Account Freezing (High Security)
    public async Task Handle(AccountFrozenEvent @event, CancellationToken ct)
    {
        var message = $"URGENT: Your Wekeza account {@event.AccountNumber} has been frozen for your security. Contact support immediately.";
        
        await _notificationService.SendSmsAsync(@event.PhoneNumber, message, ct);
        await _notificationService.SendEmailAsync(@event.Email, "Account Security Alert", message, ct);
    }

    // 2. Handle Transfers (Customer Visibility)
    public async Task Handle(TransferCompletedEvent @event, CancellationToken ct)
    {
        var message = $"Debit Alert: {@event.Currency} {@event.Amount:N2} moved from account {@event.FromAccount} to {@event.ToAccount}. Ref: {@event.CorrelationId}";
        
        await _notificationService.SendPushNotificationAsync(@event.DeviceToken, "Transaction Alert", message, ct);
    }

    // 3. Handle Loan Approvals (The Good News)
    public async Task Handle(LoanApprovedEvent @event, CancellationToken ct)
    {
        var message = $"Congratulations! Your loan of {@event.Currency} {@event.Amount:N2} has been approved and disbursed. Check your balance.";
        
        await _notificationService.SendSmsAsync(@event.PhoneNumber, message, ct);
    }
}
