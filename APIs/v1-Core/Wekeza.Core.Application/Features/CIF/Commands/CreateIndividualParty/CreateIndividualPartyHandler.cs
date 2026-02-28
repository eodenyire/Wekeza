using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Exceptions;

namespace Wekeza.Core.Application.Features.CIF.Commands.CreateIndividualParty;

public class CreateIndividualPartyHandler : IRequestHandler<CreateIndividualPartyCommand, string>
{
    private readonly IPartyRepository _partyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateIndividualPartyHandler(
        IPartyRepository partyRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _partyRepository = partyRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<string> Handle(CreateIndividualPartyCommand request, CancellationToken ct)
    {
        // 1. Validate uniqueness
        if (await _partyRepository.ExistsByEmailAsync(request.PrimaryEmail, ct))
            throw new GenericDomainException("A party with this email already exists.");

        if (await _partyRepository.ExistsByIdentificationAsync(request.PrimaryIdentification.DocumentNumber, ct))
            throw new GenericDomainException("A party with this identification document already exists.");

        // 2. Generate Party Number (CIF Number)
        var partyNumber = await GeneratePartyNumberAsync(ct);

        // 3. Create Party aggregate
        var party = Party.CreateIndividual(
            partyNumber: partyNumber,
            firstName: request.FirstName,
            lastName: request.LastName,
            dateOfBirth: request.DateOfBirth,
            nationality: request.Nationality,
            createdBy: _currentUser.Username ?? "System"
        );

        // 4. Add contact information
        party.UpdateContactInfo(request.PrimaryEmail, request.PrimaryPhone);

        // 5. Add address
        var address = new Address(
            AddressType: request.PrimaryAddress.AddressType,
            AddressLine1: request.PrimaryAddress.AddressLine1,
            AddressLine2: request.PrimaryAddress.AddressLine2,
            City: request.PrimaryAddress.City,
            State: request.PrimaryAddress.State,
            Country: request.PrimaryAddress.Country,
            PostalCode: request.PrimaryAddress.PostalCode,
            IsPrimary: request.PrimaryAddress.IsPrimary
        );
        party.AddAddress(address);

        // 6. Add identification
        var identification = new IdentificationDocument(
            DocumentType: request.PrimaryIdentification.DocumentType,
            DocumentNumber: request.PrimaryIdentification.DocumentNumber,
            IssuingCountry: request.PrimaryIdentification.IssuingCountry,
            IssueDate: request.PrimaryIdentification.IssueDate,
            ExpiryDate: request.PrimaryIdentification.ExpiryDate,
            IsVerified: false // Will be verified in KYC process
        );
        party.AddIdentification(identification);

        // 7. Persist
        await _partyRepository.AddAsync(party, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // 8. Return Party Number (CIF)
        return partyNumber;
    }

    private async Task<string> GeneratePartyNumberAsync(CancellationToken ct)
    {
        // Generate unique party number (CIF)
        // Format: CIF + YYYYMMDD + Sequential Number
        // Example: CIF20260117001
        
        var prefix = "CIF";
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var sequential = 1;

        string partyNumber;
        do
        {
            partyNumber = $"{prefix}{date}{sequential:D3}";
            sequential++;
        } while (await _partyRepository.ExistsByPartyNumberAsync(partyNumber, ct));

        return partyNumber;
    }
}
