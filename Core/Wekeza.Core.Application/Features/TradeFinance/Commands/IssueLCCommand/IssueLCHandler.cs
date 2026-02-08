using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Common.Exceptions;

namespace Wekeza.Core.Application.Features.TradeFinance.Commands.IssueLCCommand;

public class IssueLCHandler : IRequestHandler<IssueLCCommand, IssueLCResponse>
{
    private readonly ILetterOfCreditRepository _lcRepository;
    private readonly IPartyRepository _partyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IssueLCHandler(
        ILetterOfCreditRepository lcRepository,
        IPartyRepository partyRepository,
        IUnitOfWork unitOfWork)
    {
        _lcRepository = lcRepository;
        _partyRepository = partyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IssueLCResponse> Handle(IssueLCCommand request, CancellationToken cancellationToken)
    {
        // Validate LC number uniqueness
        if (await _lcRepository.ExistsAsync(request.LCNumber, cancellationToken))
        {
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure> { new FluentValidation.Results.ValidationFailure("", $"LC with number {request.LCNumber} already exists") });
        }

        // Validate applicant exists
        var applicant = await _partyRepository.GetByIdAsync(request.ApplicantId, cancellationToken);
        if (applicant == null)
        {
            throw new NotFoundException("Applicant", request.ApplicantId);
        }

        // Validate beneficiary exists
        var beneficiary = await _partyRepository.GetByIdAsync(request.BeneficiaryId, cancellationToken);
        if (beneficiary == null)
        {
            throw new NotFoundException("Beneficiary", request.BeneficiaryId);
        }

        // Create money value object
        var amount = new Money(request.Amount, Currency.FromCode(request.Currency));

        // Issue the Letter of Credit
        var letterOfCredit = LetterOfCredit.Issue(
            request.LCNumber,
            request.ApplicantId,
            request.BeneficiaryId,
            request.IssuingBankId,
            amount,
            request.ExpiryDate,
            request.Terms,
            request.GoodsDescription,
            request.Type,
            request.LastShipmentDate,
            request.AdvisingBankId,
            request.IsTransferable);

        // Add to repository
        await _lcRepository.AddAsync(letterOfCredit, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate SWIFT MT700 message (simplified)
        var swiftMessage = GenerateSwiftMT700(letterOfCredit, applicant, beneficiary);

        return new IssueLCResponse
        {
            LCId = letterOfCredit.Id,
            LCNumber = letterOfCredit.LCNumber,
            Status = letterOfCredit.Status.ToString(),
            IssueDate = letterOfCredit.IssueDate,
            SwiftMessage = swiftMessage
        };
    }

    private string GenerateSwiftMT700(LetterOfCredit lc, Party applicant, Party beneficiary)
    {
        // Simplified SWIFT MT700 message generation
        // In a real implementation, this would use a proper SWIFT message library
        return $@"{{1:F01BANKKENAXXX0000000000}}
{{2:I700BANKUSAXXX}}
{{3:{{108:LC{lc.LCNumber}}}}}
{{4:
:20:{lc.LCNumber}
:31C:{lc.IssueDate:yyMMdd}
:31D:{lc.ExpiryDate:yyMMdd}
:32B:{lc.Amount.Currency}{lc.Amount.Amount:F2}
:50:{applicant.Name}
:59:{beneficiary.Name}
:46A:{lc.GoodsDescription}
:47A:{lc.Terms}
-}}";
    }
}