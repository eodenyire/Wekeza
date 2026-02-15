using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Infrastructure.Services;

/// <summary>
/// Email service for sending notifications and alerts
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        // For now, we'll log the email instead of actually sending it
        // In production, integrate with SendGrid, AWS SES, or similar service
        
        _logger.LogInformation("📧 Email would be sent to {To} with subject: {Subject}", to, subject);
        _logger.LogDebug("Email body: {Body}", body);
        
        // Simulate async operation
        await Task.Delay(100, ct);
        
        // TODO: Implement actual email sending logic
        // Example with SendGrid:
        // var apiKey = _configuration["SendGrid:ApiKey"];
        // var client = new SendGridClient(apiKey);
        // var from = new EmailAddress(_configuration["SendGrid:FromEmail"], "Wekeza Bank");
        // var toEmail = new EmailAddress(to);
        // var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);
        // await client.SendEmailAsync(msg);
    }

    public async Task SendTemplateAsync(string to, string templateId, object templateData, CancellationToken ct = default)
    {
        _logger.LogInformation("📧 Template email would be sent to {To} using template: {TemplateId}", to, templateId);
        _logger.LogDebug("Template data: {@TemplateData}", templateData);
        
        // Simulate async operation
        await Task.Delay(100, ct);
        
        // TODO: Implement template-based email sending
    }

    public async Task SendTransactionAlertAsync(string customerEmail, string transactionDetails)
    {
        var subject = "Transaction Alert - Wekeza Bank";
        var body = $@"
Dear Valued Customer,

A transaction has been processed on your account:

{transactionDetails}

If you did not authorize this transaction, please contact us immediately at support@wekeza.com or call +254-700-WEKEZA.

Best regards,
Wekeza Bank Security Team
";

        await SendAsync(customerEmail, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string customerEmail, string customerName)
    {
        var subject = "Welcome to Wekeza Bank!";
        var body = $@"
Dear {customerName},

Welcome to Wekeza Bank! Your account has been successfully created.

You can now:
- Access your account through our mobile app
- Make deposits and withdrawals
- Transfer money to other accounts
- Apply for loans and credit facilities

Download our mobile app: https://wekeza.com/app

For support, contact us at support@wekeza.com

Best regards,
Wekeza Bank Team
";

        await SendAsync(customerEmail, subject, body);
    }
}