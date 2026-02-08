using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Administration.Commands.UpdateSystemParameter;

public class UpdateSystemParameterHandler : IRequestHandler<UpdateSystemParameterCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSystemParameterHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateSystemParameterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to update system parameter: {ex.Message}");
        }
    }
}
