using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.DigitalChannels.Commands.EnrollCustomer;

/// <summary>
/// Command to enroll customer in digital banking channels
/// Supports Internet Banking, Mobile Banking, and USSD enrollment
/// </summary>
[Authorize(UserRole.Teller, UserRole.CustomerService, UserRole.Customer)]
public record EnrollCustomerCommand : ICommand<Result<EnrollmentResult>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    
    public Guid CustomerId { get; init; }
    public List<ChannelType> Channels { get; init; } = new();
    public string? PreferredUsername { get; init; }
    public string? MobileNumber { get; init; }
    public string? Email { get; init; }
    public bool AcceptTermsAndConditions { get; init; }
    public string? DeviceId { get; init; }
    public string? DeviceType { get; init; }
    public Dictionary<string, object> ChannelPreferences { get; init; } = new();
}

public enum ChannelType
{
    InternetBanking,
    MobileBanking,
    USSD,
    SMS,
    WhatsApp
}

public record EnrollmentResult
{
    public Guid EnrollmentId { get; init; }
    public List<ChannelEnrollmentStatus> ChannelStatuses { get; init; } = new();
    public string? TemporaryPassword { get; init; }
    public string? USSDCode { get; init; }
    public string Message { get; init; } = string.Empty;
}

public record ChannelEnrollmentStatus
{
    public ChannelType Channel { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? AccessCode { get; init; }
    public string? Message { get; init; }
}