using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.RequestCard;

public record RequestCardCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public string CardType { get; init; } = string.Empty;
    public string DeliveryAddress { get; init; } = string.Empty;
}

public class RequestCardHandler : IRequestHandler<RequestCardCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RequestCardHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RequestCardCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var requestId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(requestId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to request card: {ex.Message}");
        }
    }
}
