namespace Wekeza.Core.Application.Common.Interfaces;

/// <summary>
/// 📂 Wekeza.Core.Application/Common/Interfaces/
/// 1. IDateTime.cs (The Global Chronometer)
/// In banking, time is everything—interest calculations, transaction timestamps, and audit logs depend on it. By using an interface, we can "freeze" time in our unit tests to verify interest accrual logic without waiting for a real clock.
/// 3. IEmailService.cs (The Communication Port)
/// Whether we are sending an OTP or a monthly statement, the Application layer just "requests" an email. The implementation (SendGrid, AWS SES, or SMTP) is hidden away in the Infrastructure layer.
/// Contract for sending automated communications from Wekeza Bank.
/// </summary>
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
    
    // Future-proofing: Template-based emails for 2026 marketing/regulatory alerts
    Task SendTemplateAsync(string to, string templateId, object templateData, CancellationToken ct = default);
}
