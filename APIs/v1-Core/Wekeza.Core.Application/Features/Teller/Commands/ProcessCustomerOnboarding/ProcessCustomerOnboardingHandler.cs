using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Commands.ProcessCustomerOnboarding;

public class ProcessCustomerOnboardingHandler : IRequestHandler<ProcessCustomerOnboardingCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProcessCustomerOnboardingHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessCustomerOnboardingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customerId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(customerId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to process customer onboarding: {ex.Message}");
        }
    }
}
