using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Commands.VerifyCustomer;

public record VerifyCustomerCommand : ICommand<Result<VerifyCustomerResponse>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string CustomerIdentifier { get; init; } = string.Empty;
    public string IdentificationType { get; init; } = string.Empty;
}

public record VerifyCustomerResponse
{
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string AccountNumber { get; init; } = string.Empty;
    public bool IsVerified { get; init; }
}

public class VerifyCustomerHandler : IRequestHandler<VerifyCustomerCommand, Result<VerifyCustomerResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public VerifyCustomerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VerifyCustomerResponse>> Handle(VerifyCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = new VerifyCustomerResponse
            {
                CustomerId = Guid.NewGuid(),
                CustomerName = "Customer Name",
                AccountNumber = "ACC123456",
                IsVerified = true
            };
            
            await Task.CompletedTask;
            return Result<VerifyCustomerResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<VerifyCustomerResponse>.Failure($"Failed to verify customer: {ex.Message}");
        }
    }
}
