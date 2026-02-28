using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Commands.ProcessChequeDeposit;

public class ProcessChequeDepositHandler : IRequestHandler<ProcessChequeDepositCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProcessChequeDepositHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessChequeDepositCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transactionId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(transactionId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to process cheque deposit: {ex.Message}");
        }
    }
}
