using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.TransferFunds;

public record TransferFundsCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string FromAccount { get; init; } = string.Empty;
    public string ToAccount { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
    public string? Narration { get; init; }
}

public class TransferFundsHandler : IRequestHandler<TransferFundsCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransferFundsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transactionId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(transactionId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to transfer funds: {ex.Message}");
        }
    }
}
