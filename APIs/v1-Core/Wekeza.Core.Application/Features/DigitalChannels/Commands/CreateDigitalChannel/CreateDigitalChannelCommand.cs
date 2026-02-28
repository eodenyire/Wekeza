using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.DigitalChannels.Commands.CreateDigitalChannel;

/// <summary>
/// Command to create a new digital channel
/// </summary>
public record CreateDigitalChannelCommand(
    Guid ChannelId,
    string ChannelCode,
    string ChannelName,
    ChannelType ChannelType,
    string Description,
    string BaseUrl,
    string ApiVersion,
    bool IsSecure,
    bool RequiresAuthentication,
    bool SupportsMFA,
    int MaxConcurrentSessions,
    int SessionTimeoutMinutes,
    decimal DailyTransactionLimit,
    decimal SingleTransactionLimit,
    int MaxDailyTransactions,
    string Currency,
    string CreatedBy) : IRequest<Result<Guid>>;