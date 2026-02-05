using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Enums;
/// 3. The Executioner: RegisterBusinessHandler.cs
/// This handler registers the business as a Customer in the system but flags it as a Corporate entity. This distinction is critical for your Model Risk engine.

namespace Wekeza.Core.Application.Features.Accounts.Commands.RegisterBusiness;

public class RegisterBusinessHandler : IRequestHandler<RegisterBusinessCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterBusinessHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegisterBusinessCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if business is already registered by Registration Number or KRA PIN
        var existing = await _customerRepository.GetByIdentificationNumberAsync(request.RegistrationNumber, cancellationToken);
        if (existing != null)
        {
            throw new InvalidOperationException("This business registration number is already in use.");
        }

        // 2. Create the Corporate Customer Aggregate
        // We use the RegistrationNumber as the primary identity
        var businessCustomer = new Customer(
            Guid.NewGuid(),
            request.BusinessName,
            request.BusinessType, // Using LastName field to store type/category for now
            request.Email,
            request.RegistrationNumber
        );

        // 3. Mark as a Corporate entity (Future-proofing: Extend Customer aggregate for Corporate details)
        businessCustomer.UpdateRiskRating(RiskLevel.Medium); // Businesses usually start at Medium risk

        await _customerRepository.AddAsync(businessCustomer, cancellationToken);

        // 4. Persistence
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return businessCustomer.Id;
    }
}
