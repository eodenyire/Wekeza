using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

public class LetterOfCredit : AggregateRoot
{
    public string LCNumber { get; private set; }
    public Guid ApplicantId { get; private set; }
    public Guid BeneficiaryId { get; private set; }
    public Guid IssuingBankId { get; private set; }
    public Guid? AdvisingBankId { get; private set; }
    public Money Amount { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public DateTime? LastShipmentDate { get; private set; }
    public LCStatus Status { get; private set; }
    public LCType Type { get; private set; }
    public string Terms { get; private set; }
    public string GoodsDescription { get; private set; }
    public bool IsTransferable { get; private set; }
    public bool IsConfirmed { get; private set; }
    public List<TradeDocument> Documents { get; private set; }
    public List<LCAmendment> Amendments { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private LetterOfCredit() 
    {
        Documents = new List<TradeDocument>();
        Amendments = new List<LCAmendment>();
    }

    public static LetterOfCredit Issue(
        string lcNumber,
        Guid applicantId,
        Guid beneficiaryId,
        Guid issuingBankId,
        Money amount,
        DateTime expiryDate,
        string terms,
        string goodsDescription,
        LCType type = LCType.Irrevocable,
        DateTime? lastShipmentDate = null,
        Guid? advisingBankId = null,
        bool isTransferable = false)
    {
        if (string.IsNullOrWhiteSpace(lcNumber))
            throw new ArgumentException("LC number cannot be empty", nameof(lcNumber));

        if (applicantId == Guid.Empty)
            throw new ArgumentException("Applicant ID cannot be empty", nameof(applicantId));

        if (beneficiaryId == Guid.Empty)
            throw new ArgumentException("Beneficiary ID cannot be empty", nameof(beneficiaryId));

        if (expiryDate <= DateTime.UtcNow)
            throw new ArgumentException("Expiry date must be in the future", nameof(expiryDate));

        if (amount.Amount <= 0)
            throw new ArgumentException("LC amount must be positive", nameof(amount));

        var lc = new LetterOfCredit
        {
            Id = Guid.NewGuid(),
            LCNumber = lcNumber,
            ApplicantId = applicantId,
            BeneficiaryId = beneficiaryId,
            IssuingBankId = issuingBankId,
            AdvisingBankId = advisingBankId,
            Amount = amount,
            IssueDate = DateTime.UtcNow,
            ExpiryDate = expiryDate,
            LastShipmentDate = lastShipmentDate,
            Status = LCStatus.Issued,
            Type = type,
            Terms = terms ?? string.Empty,
            GoodsDescription = goodsDescription ?? string.Empty,
            IsTransferable = isTransferable,
            IsConfirmed = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        lc.AddDomainEvent(new LCIssuedDomainEvent(lc.Id, lc.LCNumber, lc.ApplicantId, lc.Amount));

        return lc;
    }

    public void Amend(string amendmentDetails, Money? newAmount = null, DateTime? newExpiryDate = null)
    {
        if (Status == LCStatus.Expired || Status == LCStatus.Cancelled)
            throw new InvalidOperationException($"Cannot amend LC in {Status} status");

        var amendment = new LCAmendment
        {
            Id = Guid.NewGuid(),
            LetterOfCreditId = Id,
            AmendmentNumber = Amendments.Count + 1,
            AmendmentDetails = amendmentDetails,
            PreviousAmount = Amount,
            NewAmount = newAmount ?? Amount,
            PreviousExpiryDate = ExpiryDate,
            NewExpiryDate = newExpiryDate ?? ExpiryDate,
            AmendmentDate = DateTime.UtcNow,
            Status = AmendmentStatus.Pending
        };

        if (newAmount.HasValue)
            Amount = newAmount.Value;

        if (newExpiryDate.HasValue)
            ExpiryDate = newExpiryDate.Value;

        Amendments.Add(amendment);
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LCAmendedDomainEvent(Id, LCNumber, amendment.AmendmentNumber, amendmentDetails));
    }

    public void Confirm(Guid confirmingBankId)
    {
        if (Status != LCStatus.Issued)
            throw new InvalidOperationException($"Cannot confirm LC in {Status} status");

        IsConfirmed = true;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LCConfirmedDomainEvent(Id, LCNumber, confirmingBankId));
    }

    public void PresentDocuments(List<TradeDocument> documents, Guid presentingBankId)
    {
        if (Status != LCStatus.Issued)
            throw new InvalidOperationException($"Cannot present documents for LC in {Status} status");

        if (DateTime.UtcNow > ExpiryDate)
        {
            Status = LCStatus.Expired;
            throw new InvalidOperationException("LC has expired");
        }

        foreach (var document in documents)
        {
            Documents.Add(document);
        }

        Status = LCStatus.DocumentsPresented;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DocumentsPresentedDomainEvent(Id, LCNumber, documents.Count, presentingBankId));
    }

    public void AcceptDocuments(string acceptanceNotes = "")
    {
        if (Status != LCStatus.DocumentsPresented)
            throw new InvalidOperationException($"Cannot accept documents for LC in {Status} status");

        Status = LCStatus.DocumentsAccepted;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DocumentsAcceptedDomainEvent(Id, LCNumber, acceptanceNotes));
    }

    public void RejectDocuments(string rejectionReason)
    {
        if (Status != LCStatus.DocumentsPresented)
            throw new InvalidOperationException($"Cannot reject documents for LC in {Status} status");

        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new ArgumentException("Rejection reason is required", nameof(rejectionReason));

        Status = LCStatus.DocumentsRejected;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DocumentsRejectedDomainEvent(Id, LCNumber, rejectionReason));
    }

    public void Negotiate(Money negotiatedAmount, Guid negotiatingBankId)
    {
        if (Status != LCStatus.DocumentsAccepted)
            throw new InvalidOperationException($"Cannot negotiate LC in {Status} status");

        if (negotiatedAmount.Amount > Amount.Amount)
            throw new InvalidOperationException("Negotiated amount cannot exceed LC amount");

        Status = LCStatus.Negotiated;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LCNegotiatedDomainEvent(Id, LCNumber, negotiatedAmount, negotiatingBankId));
    }

    public void Settle(Money settledAmount, string settlementReference)
    {
        if (Status != LCStatus.Negotiated)
            throw new InvalidOperationException($"Cannot settle LC in {Status} status");

        Status = LCStatus.Settled;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LCSettledDomainEvent(Id, LCNumber, settledAmount, settlementReference));
    }

    public void Cancel(string cancellationReason)
    {
        if (Status == LCStatus.Settled || Status == LCStatus.Expired)
            throw new InvalidOperationException($"Cannot cancel LC in {Status} status");

        Status = LCStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LCCancelledDomainEvent(Id, LCNumber, cancellationReason));
    }

    public void Expire()
    {
        if (DateTime.UtcNow > ExpiryDate && Status != LCStatus.Settled)
        {
            Status = LCStatus.Expired;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new LCExpiredDomainEvent(Id, LCNumber, ExpiryDate));
        }
    }

    public bool IsExpired => DateTime.UtcNow > ExpiryDate;
    public bool IsActive => Status == LCStatus.Issued || Status == LCStatus.DocumentsPresented;
    public int DaysToExpiry => (ExpiryDate - DateTime.UtcNow).Days;
}

public class LCAmendment
{
    public Guid Id { get; set; }
    public Guid LetterOfCreditId { get; set; }
    public int AmendmentNumber { get; set; }
    public string AmendmentDetails { get; set; }
    public Money PreviousAmount { get; set; }
    public Money NewAmount { get; set; }
    public DateTime PreviousExpiryDate { get; set; }
    public DateTime NewExpiryDate { get; set; }
    public DateTime AmendmentDate { get; set; }
    public AmendmentStatus Status { get; set; }
}

public class TradeDocument
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; }
    public string DocumentNumber { get; set; }
    public Guid TradeTransactionId { get; set; }
    public string TradeTransactionType { get; set; }
    public DocumentStatus Status { get; set; }
    public DateTime UploadedAt { get; set; }
    public string FilePath { get; set; }
    public string UploadedBy { get; set; }
    public string Comments { get; set; }
}

public enum LCStatus
{
    Draft,
    Issued,
    Advised,
    DocumentsPresented,
    DocumentsAccepted,
    DocumentsRejected,
    Negotiated,
    Settled,
    Cancelled,
    Expired
}

public enum LCType
{
    Revocable,
    Irrevocable,
    Confirmed,
    Unconfirmed,
    Transferable,
    NonTransferable,
    Standby
}

public enum AmendmentStatus
{
    Pending,
    Accepted,
    Rejected
}

public enum DocumentStatus
{
    Uploaded,
    Verified,
    Rejected,
    Accepted
}