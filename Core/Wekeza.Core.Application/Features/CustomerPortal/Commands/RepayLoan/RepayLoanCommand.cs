using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.RepayLoan;

public record RepayLoanCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid LoanAccountId { get; init; }
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
}

public class RepayLoanHandler : IRequestHandler<RepayLoanCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RepayLoanHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RepayLoanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transactionId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(transactionId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to repay loan: {ex.Message}");
        }
    }
}
