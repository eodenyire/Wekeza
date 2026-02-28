using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.ApplyForLoan;

public record ApplyForLoanCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; init; }
    public string LoanProductCode { get; init; } = string.Empty;
    public decimal RequestedAmount { get; init; }
    public int TermInMonths { get; init; }
    public string Purpose { get; init; } = string.Empty;
}

public class ApplyForLoanHandler : IRequestHandler<ApplyForLoanCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApplyForLoanHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ApplyForLoanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var applicationId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(applicationId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to apply for loan: {ex.Message}");
        }
    }
}
