using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Instruments.Cards.Commands.ActivateCard;

public class ActivateCardHandler : IRequestHandler<ActivateCardCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivateCardHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ActivateCardCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to activate card: {ex.Message}");
        }
    }
}
