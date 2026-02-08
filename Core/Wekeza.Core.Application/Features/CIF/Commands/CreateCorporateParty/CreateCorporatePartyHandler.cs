using MediatR;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CIF.Commands.CreateCorporateParty;

public class CreateCorporatePartyHandler : IRequestHandler<CreateCorporatePartyCommand, string>
{
    private readonly IPartyRepository _partyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;

    public CreateCorporatePartyHandler(
        IPartyRepository partyRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IDateTime dateTime)
    {
        _partyRepository = partyRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    public async Task<string> Handle(CreateCorporatePartyCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicates
        var existingByEmail = await _partyRepository.GetByEmailAsync(request.PrimaryEmail, cancellationToken);
        if (existingByEmail != null)
        {
            throw new InvalidOperationException($"A party with email {request.PrimaryEmail} already exists.");
        }

        var existingByPhone = await _partyRepository.GetByPhoneAsync(request.PrimaryPhone, cancellationToken);
        if (existingByPhone != null)
        {
            throw new InvalidOperationException($"A party with phone {request.PrimaryPhone} already exists.");
        }

        var existingByRegistration = await _partyRepository.GetByRegistrationNumberAsync(request.RegistrationNumber, cancellationToken);
        if (existingByRegistration != null)
        {
            throw new InvalidOperationException($"A company with registration number {request.RegistrationNumber} already exists.");
        }

        // Generate unique party number (CIF number)
        var partyNumber = await GeneratePartyNumberAsync(cancellationToken);

        // Create corporate party
        var party = Party.CreateCorporate(
            partyNumber,
            request.CompanyName,
            request.RegistrationNumber,
            request.IncorporationDate,
            request.Industry,
            (_currentUserService.UserId ?? Guid.Empty).ToString()
        );

        // Add contact information
        party.UpdateContactInfo(request.PrimaryEmail, request.PrimaryPhone);

        // Add registered address
        var address = new Address(
            request.RegisteredAddress.AddressType,
            request.RegisteredAddress.AddressLine1,
            request.RegisteredAddress.AddressLine2,
            request.RegisteredAddress.City,
            request.RegisteredAddress.State,
            request.RegisteredAddress.Country,
            request.RegisteredAddress.PostalCode,
            request.RegisteredAddress.IsPrimary
        );
        party.AddAddress(address);

        // Add directors as identification documents (for tracking)
        foreach (var director in request.Directors)
        {
            var directorDoc = new IdentificationDocument(
                $"Director-{director.Role}",
                director.IdentificationNumber,
                director.Nationality,
                _dateTime.Now,
                _dateTime.Now.AddYears(10),
                false
            );
            party.AddIdentification(directorDoc);
        }

        // Save party
        await _partyRepository.AddAsync(party, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return partyNumber;
    }

    private async Task<string> GeneratePartyNumberAsync(CancellationToken cancellationToken)
    {
        var today = _dateTime.Now;
        var prefix = $"CIF{today:yyyyMMdd}";
        
        var lastParty = await _partyRepository.GetLastPartyByPrefixAsync(prefix, cancellationToken);
        
        int sequence = 1;
        if (lastParty != null)
        {
            var lastSequence = lastParty.PartyNumber.Substring(prefix.Length);
            if (int.TryParse(lastSequence, out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{sequence:D3}";
    }
}
