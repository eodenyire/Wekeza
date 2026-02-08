using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
/// 3. The Executioner: VerifyCustomerHandler.cs
/// This is where we update the Risk Profile and the status. Notice how we use the Customer Aggregate.

namespace Wekeza.Core.Application.Features.Accounts.Commands.VerifyCustomer;

public class VerifyCustomerHandler : IRequestHandler<VerifyCustomerCommand, bool>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyCustomerHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(VerifyCustomerCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch the Customer
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new NotFoundException("Customer", request.CustomerId);

        // 2. Perform Domain State Change
        // In the aggregate, this could trigger an event: CustomerVerifiedDomainEvent
        // For now, we update the Risk Rating to Low as they are now 'Know Your Customer' compliant.
        customer.UpdateRiskRating(Domain.Enums.RiskLevel.Low);

        // 3. Log the Verification (In a real bank, we'd have a separate Audit Table here)
        _customerRepository.Update(customer);

        // 4. Persistence
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
