using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.PayBill;

public record PayBillCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public string BillerCode { get; init; } = string.Empty;
    public string AccountReference { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}

public class PayBillHandler : IRequestHandler<PayBillCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public PayBillHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(PayBillCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transactionId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(transactionId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to pay bill: {ex.Message}");
        }
    }
}
