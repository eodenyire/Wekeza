using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.EnrollUSSD;

public record EnrollUSSDCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public string PIN { get; init; } = string.Empty;
}

public class EnrollUSSDHandler : IRequestHandler<EnrollUSSDCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public EnrollUSSDHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(EnrollUSSDCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollmentId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(enrollmentId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to enroll USSD: {ex.Message}");
        }
    }
}
