using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Common.Exceptions;

namespace Wekeza.Core.Application.Features.TradeFinance.Commands.IssueBGCommand;

public class IssueBGHandler : IRequestHandler<IssueBGCommand, IssueBGResponse>
{
    private readonly IBankGuaranteeRepository _bgRepository;
    private readonly IPartyRepository _partyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IssueBGHandler(
        IBankGuaranteeRepository bgRepository,
        IPartyRepository partyRepository,
        IUnitOfWork unitOfWork)
    {
        _bgRepository = bgRepository;
        _partyRepository = partyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IssueBGResponse> Handle(IssueBGCommand request, CancellationToken cancellationToken)
    {
        // Validate BG number uniqueness
        if (await _bgRepository.ExistsAsync(request.BGNumber, cancellationToken))
        {
            throw new ValidationException($"Bank Guarantee with number {request.BGNumber} already exists");
        }

        // Validate principal exists
        var principal = await _partyRepository.GetByIdAsync(request.PrincipalId, cancellationToken);
        if (principal == null)
        {
            throw new NotFoundException("Principal", request.PrincipalId);
        }

        // Validate beneficiary exists
        var beneficiary = await _partyRepository.GetByIdAsync(request.BeneficiaryId, cancellationToken);
        if (beneficiary == null)
        {
            throw new NotFoundException("Beneficiary", request.BeneficiaryId);
        }

        // Create money value object
        var amount = new Money(request.Amount, request.Currency);

        // Issue the Bank Guarantee
        var bankGuarantee = BankGuarantee.Issue(
            request.BGNumber,
            request.PrincipalId,
            request.BeneficiaryId,
            request.IssuingBankId,
            amount,
            request.ExpiryDate,
            request.Type,
            request.Terms,
            request.Purpose,
            request.IsRevocable,
            request.CounterGuaranteeId);

        // Add to repository
        await _bgRepository.AddAsync(bankGuarantee, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate SWIFT MT760 message (simplified)
        var swiftMessage = GenerateSwiftMT760(bankGuarantee, principal, beneficiary);

        return new IssueBGResponse
        {
            BGId = bankGuarantee.Id,
            BGNumber = bankGuarantee.BGNumber,
            Status = bankGuarantee.Status.ToString(),
            IssueDate = bankGuarantee.IssueDate,
            SwiftMessage = swiftMessage
        };
    }

    private string GenerateSwiftMT760(BankGuarantee bg, Party principal, Party beneficiary)
    {
        // Simplified SWIFT MT760 message generation
        // In a real implementation, this would use a proper SWIFT message library
        return $@"{{1:F01BANKKENAXXX0000000000}}
{{2:I760BANKUSAXXX}}
{{3:{{108:BG{bg.BGNumber}}}}}
{{4:
:20:{bg.BGNumber}
:31C:{bg.IssueDate:yyMMdd}
:31D:{bg.ExpiryDate:yyMMdd}
:32B:{bg.Amount.Currency}{bg.Amount.Amount:F2}
:50:{principal.Name}
:59:{beneficiary.Name}
:77C:{bg.Purpose}
:77D:{bg.Terms}
-}}";
    }
}