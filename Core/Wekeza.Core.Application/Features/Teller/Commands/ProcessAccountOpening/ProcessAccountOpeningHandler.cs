using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Commands.ProcessAccountOpening;

public class ProcessAccountOpeningHandler : IRequestHandler<ProcessAccountOpeningCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProcessAccountOpeningHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessAccountOpeningCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var accountId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(accountId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to process account opening: {ex.Message}");
        }
    }
}
