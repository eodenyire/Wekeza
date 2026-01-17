using MediatR;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Application.Common.Exceptions;

namespace Wekeza.Core.Application.Features.TradeFinance.Queries.GetLCDetails;

public class GetLCDetailsHandler : IRequestHandler<GetLCDetailsQuery, LCDetailsDto>
{
    private readonly ILetterOfCreditRepository _lcRepository;
    private readonly IPartyRepository _partyRepository;

    public GetLCDetailsHandler(
        ILetterOfCreditRepository lcRepository,
        IPartyRepository partyRepository)
    {
        _lcRepository = lcRepository;
        _partyRepository = partyRepository;
    }

    public async Task<LCDetailsDto> Handle(GetLCDetailsQuery request, CancellationToken cancellationToken)
    {
        // Get LC by ID or LC Number
        var letterOfCredit = request.LCId != Guid.Empty
            ? await _lcRepository.GetByIdAsync(request.LCId, cancellationToken)
            : await _lcRepository.GetByLCNumberAsync(request.LCNumber!, cancellationToken);

        if (letterOfCredit == null)
        {
            throw new NotFoundException("Letter of Credit", request.LCId != Guid.Empty ? request.LCId : request.LCNumber!);
        }

        // Get party details
        var applicant = await _partyRepository.GetByIdAsync(letterOfCredit.ApplicantId, cancellationToken);
        var beneficiary = await _partyRepository.GetByIdAsync(letterOfCredit.BeneficiaryId, cancellationToken);
        var issuingBank = await _partyRepository.GetByIdAsync(letterOfCredit.IssuingBankId, cancellationToken);
        var advisingBank = letterOfCredit.AdvisingBankId.HasValue
            ? await _partyRepository.GetByIdAsync(letterOfCredit.AdvisingBankId.Value, cancellationToken)
            : null;

        return new LCDetailsDto
        {
            Id = letterOfCredit.Id,
            LCNumber = letterOfCredit.LCNumber,
            ApplicantName = applicant?.Name ?? "Unknown",
            BeneficiaryName = beneficiary?.Name ?? "Unknown",
            IssuingBankName = issuingBank?.Name ?? "Unknown",
            AdvisingBankName = advisingBank?.Name,
            Amount = letterOfCredit.Amount.Amount,
            Currency = letterOfCredit.Amount.Currency,
            IssueDate = letterOfCredit.IssueDate,
            ExpiryDate = letterOfCredit.ExpiryDate,
            LastShipmentDate = letterOfCredit.LastShipmentDate,
            Status = letterOfCredit.Status.ToString(),
            Type = letterOfCredit.Type.ToString(),
            Terms = letterOfCredit.Terms,
            GoodsDescription = letterOfCredit.GoodsDescription,
            IsTransferable = letterOfCredit.IsTransferable,
            IsConfirmed = letterOfCredit.IsConfirmed,
            IsExpired = letterOfCredit.IsExpired,
            DaysToExpiry = letterOfCredit.DaysToExpiry,
            Amendments = letterOfCredit.Amendments.Select(a => new LCAmendmentDto
            {
                Id = a.Id,
                AmendmentNumber = a.AmendmentNumber,
                AmendmentDetails = a.AmendmentDetails,
                PreviousAmount = a.PreviousAmount.Amount,
                NewAmount = a.NewAmount.Amount,
                PreviousExpiryDate = a.PreviousExpiryDate,
                NewExpiryDate = a.NewExpiryDate,
                AmendmentDate = a.AmendmentDate,
                Status = a.Status.ToString()
            }).ToList(),
            Documents = letterOfCredit.Documents.Select(d => new TradeDocumentDto
            {
                Id = d.Id,
                DocumentType = d.DocumentType,
                DocumentNumber = d.DocumentNumber,
                Status = d.Status.ToString(),
                UploadedAt = d.UploadedAt,
                UploadedBy = d.UploadedBy,
                Comments = d.Comments
            }).ToList()
        };
    }
}