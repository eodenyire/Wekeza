using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.BuyAirtime;

public record BuyAirtimeCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Provider { get; init; } = string.Empty;
}

public class BuyAirtimeHandler : IRequestHandler<BuyAirtimeCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public BuyAirtimeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(BuyAirtimeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transactionId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(transactionId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to buy airtime: {ex.Message}");
        }
    }
}
