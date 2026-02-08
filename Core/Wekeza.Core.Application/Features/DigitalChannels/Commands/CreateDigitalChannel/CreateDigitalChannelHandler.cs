using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.DigitalChannels.Commands.CreateDigitalChannel;

/// <summary>
/// Handler for creating digital channels
/// </summary>
public class CreateDigitalChannelHandler : IRequestHandler<CreateDigitalChannelCommand, Result<Guid>>
{
    private readonly IDigitalChannelRepository _digitalChannelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDigitalChannelHandler(
        IDigitalChannelRepository digitalChannelRepository,
        IUnitOfWork unitOfWork)
    {
        _digitalChannelRepository = digitalChannelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateDigitalChannelCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if channel code already exists
            if (await _digitalChannelRepository.ExistsByCodeAsync(request.ChannelCode))
                return Result<Guid>.Failure("Channel code already exists");

            // Create digital channel
            var channel = new DigitalChannel(
                request.ChannelId,
                request.ChannelCode,
                request.ChannelName,
                (Wekeza.Core.Domain.Aggregates.ChannelType)request.ChannelType,
                request.Description,
                request.BaseUrl,
                request.ApiVersion,
                request.IsSecure,
                request.RequiresAuthentication,
                request.SupportsMFA,
                request.MaxConcurrentSessions,
                TimeSpan.FromMinutes(request.SessionTimeoutMinutes),
                new Money(request.DailyTransactionLimit, Currency.FromCode(request.Currency)),
                new Money(request.SingleTransactionLimit, Currency.FromCode(request.Currency)),
                request.MaxDailyTransactions,
                request.CreatedBy);

            // Save channel
            await _digitalChannelRepository.AddAsync(channel);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(channel.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create digital channel: {ex.Message}");
        }
    }
}