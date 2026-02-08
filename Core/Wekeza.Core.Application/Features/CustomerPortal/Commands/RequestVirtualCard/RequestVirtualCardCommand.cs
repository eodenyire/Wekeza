using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.RequestVirtualCard;

public record RequestVirtualCardCommand : ICommand<Result<VirtualCardResponse>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; init; }
    public string CardType { get; init; } = string.Empty;
    public string LinkedAccountNumber { get; init; } = string.Empty;
}

public record VirtualCardResponse
{
    public Guid CardId { get; init; }
    public string MaskedCardNumber { get; init; } = string.Empty;
    public string ExpiryDate { get; init; } = string.Empty;
    public string CVV { get; init; } = string.Empty;
}

public class RequestVirtualCardHandler : IRequestHandler<RequestVirtualCardCommand, Result<VirtualCardResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RequestVirtualCardHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VirtualCardResponse>> Handle(RequestVirtualCardCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = new VirtualCardResponse
            {
                CardId = Guid.NewGuid(),
                MaskedCardNumber = "**** **** **** 1234",
                ExpiryDate = "12/25",
                CVV = "***"
            };
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<VirtualCardResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<VirtualCardResponse>.Failure($"Failed to request virtual card: {ex.Message}");
        }
    }
}
