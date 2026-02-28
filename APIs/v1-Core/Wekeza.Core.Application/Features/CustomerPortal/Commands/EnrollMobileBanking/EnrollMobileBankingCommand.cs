using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.EnrollMobileBanking;

public record EnrollMobileBankingCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public string PIN { get; init; } = string.Empty;
}

public class EnrollMobileBankingHandler : IRequestHandler<EnrollMobileBankingCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public EnrollMobileBankingHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(EnrollMobileBankingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollmentId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(enrollmentId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to enroll mobile banking: {ex.Message}");
        }
    }
}
