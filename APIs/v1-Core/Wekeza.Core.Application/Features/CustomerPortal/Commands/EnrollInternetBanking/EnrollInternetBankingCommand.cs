using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.EnrollInternetBanking;

public record EnrollInternetBankingCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

public class EnrollInternetBankingHandler : IRequestHandler<EnrollInternetBankingCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public EnrollInternetBankingHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(EnrollInternetBankingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollmentId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(enrollmentId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to enroll internet banking: {ex.Message}");
        }
    }
}
