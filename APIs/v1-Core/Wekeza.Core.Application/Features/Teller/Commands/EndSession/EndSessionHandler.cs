using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Commands.EndSession;

public class EndSessionHandler : IRequestHandler<EndSessionCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public EndSessionHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(EndSessionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to end session: {ex.Message}");
        }
    }
}
