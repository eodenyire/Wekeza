using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

public class BankGuarantee : AggregateRoot
{
    public string BGNumber { get; private set; }
    public Guid PrincipalId { get; private set; }
    public Guid BeneficiaryId { get; private set; }
    public Guid IssuingBankId { get; private set; }
    public Guid? CounterGuaranteeId { get; private set; }
    public Money Amount { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public BGStatus Status { get; private set; }
    public GuaranteeType Type { get; private set; }
    public string Terms { get; private set; }
    public string Purpose { get; private set; }
    public bool IsRevocable { get; private set; }
    public Money? ClaimedAmount { get; private set; }
    public List<BGClaim> Claims { get; private set; }
    public List<BGAmendment> Amendments { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private BankGuarantee() : base(Guid.NewGuid()) {
        Claims = new List<BGClaim>();
        Amendments = new List<BGAmendment>();
    }

    public static BankGuarantee Issue(
        string bgNumber,
        Guid principalId,
        Guid beneficiaryId,
        Guid issuingBankId,
        Money amount,
        DateTime expiryDate,
        GuaranteeType type,
        string terms,
        string purpose,
        bool isRevocable = false,
        Guid? counterGuaranteeId = null)
    {
        if (string.IsNullOrWhiteSpace(bgNumber))
            throw new ArgumentException("BG number cannot be empty", nameof(bgNumber));

        if (principalId == Guid.Empty)
            throw new ArgumentException("Principal ID cannot be empty", nameof(principalId));

        if (beneficiaryId == Guid.Empty)
            throw new ArgumentException("Beneficiary ID cannot be empty", nameof(beneficiaryId));

        if (expiryDate <= DateTime.UtcNow)
            throw new ArgumentException("Expiry date must be in the future", nameof(expiryDate));

        if (amount.Amount <= 0)
            throw new ArgumentException("BG amount must be positive", nameof(amount));

        var bg = new BankGuarantee
        {
            Id = Guid.NewGuid(),
            BGNumber = bgNumber,
            PrincipalId = principalId,
            BeneficiaryId = beneficiaryId,
            IssuingBankId = issuingBankId,
            CounterGuaranteeId = counterGuaranteeId,
            Amount = amount,
            IssueDate = DateTime.UtcNow,
            ExpiryDate = expiryDate,
            Status = BGStatus.Issued,
            Type = type,
            Terms = terms ?? string.Empty,
            Purpose = purpose ?? string.Empty,
            IsRevocable = isRevocable,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        bg.AddDomainEvent(new BGIssuedDomainEvent(bg.Id, bg.BGNumber, bg.PrincipalId, bg.Amount, bg.Type));

        return bg;
    }

    public void Amend(string amendmentDetails, Money? newAmount = null, DateTime? newExpiryDate = null)
    {
        if (Status == BGStatus.Expired || Status == BGStatus.Cancelled || Status == BGStatus.Invoked)
            throw new InvalidOperationException($"Cannot amend BG in {Status} status");

        var amendment = new BGAmendment
        {
            Id = Guid.NewGuid(),
            BankGuaranteeId = Id,
            AmendmentNumber = Amendments.Count + 1,
            AmendmentDetails = amendmentDetails,
            PreviousAmount = Amount,
            NewAmount = newAmount ?? Amount,
            PreviousExpiryDate = ExpiryDate,
            NewExpiryDate = newExpiryDate ?? ExpiryDate,
            AmendmentDate = DateTime.UtcNow,
            Status = AmendmentStatus.Pending
        };

        // TODO: Fix Money assignment issue
        // if (newAmount.HasValue)
        // {
        //     Amount = newAmount.Value;
        // }

        if (newExpiryDate.HasValue)
            ExpiryDate = newExpiryDate.Value;

        Amendments.Add(amendment);
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new BGAmendedDomainEvent(Id, BGNumber, amendment.AmendmentNumber, amendmentDetails));
    }

    public void Invoke(Money claimAmount, string claimReason, List<TradeDocument> supportingDocuments)
    {
        if (Status != BGStatus.Issued)
            throw new InvalidOperationException($"Cannot invoke BG in {Status} status");

        if (DateTime.UtcNow > ExpiryDate)
        {
            Status = BGStatus.Expired;
            throw new InvalidOperationException("BG has expired");
        }

        if (claimAmount.Amount <= 0)
            throw new ArgumentException("Claim amount must be positive", nameof(claimAmount));

        if (claimAmount.Amount > Amount.Amount)
            throw new ArgumentException("Claim amount cannot exceed BG amount", nameof(claimAmount));

        var claim = new BGClaim
        {
            Id = Guid.NewGuid(),
            BankGuaranteeId = Id,
            ClaimAmount = claimAmount,
            ClaimReason = claimReason,
            ClaimDate = DateTime.UtcNow,
            Status = ClaimStatus.Submitted,
            SupportingDocuments = supportingDocuments ?? new List<TradeDocument>()
        };

        Claims.Add(claim);
        Status = BGStatus.ClaimSubmitted;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new BGClaimSubmittedDomainEvent(Id, BGNumber, claimAmount, claimReason));
    }

    public void ProcessClaim(Guid claimId, bool isApproved, string processingNotes = "")
    {
        var claim = Claims.FirstOrDefault(c => c.Id == claimId);
        if (claim == null)
            throw new ArgumentException("Claim not found", nameof(claimId));

        if (claim.Status != ClaimStatus.Submitted)
            throw new InvalidOperationException($"Cannot process claim in {claim.Status} status");

        if (isApproved)
        {
            claim.Status = ClaimStatus.Approved;
            ClaimedAmount = claim.ClaimAmount;
            Status = BGStatus.Invoked;

            AddDomainEvent(new BGInvokedDomainEvent(Id, BGNumber, claim.ClaimAmount, claim.ClaimReason));
        }
        else
        {
            claim.Status = ClaimStatus.Rejected;
            Status = BGStatus.Issued; // Return to issued status

            AddDomainEvent(new BGClaimRejectedDomainEvent(Id, BGNumber, claimId, processingNotes));
        }

        claim.ProcessingNotes = processingNotes;
        claim.ProcessedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string cancellationReason)
    {
        if (Status == BGStatus.Invoked || Status == BGStatus.Expired)
            throw new InvalidOperationException($"Cannot cancel BG in {Status} status");

        if (!IsRevocable && Status != BGStatus.Draft)
            throw new InvalidOperationException("Cannot cancel irrevocable BG after issuance");

        Status = BGStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new BGCancelledDomainEvent(Id, BGNumber, cancellationReason));
    }

    public void Expire()
    {
        if (DateTime.UtcNow > ExpiryDate && Status != BGStatus.Invoked)
        {
            Status = BGStatus.Expired;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new BGExpiredDomainEvent(Id, BGNumber, ExpiryDate));
        }
    }

    public void Extend(DateTime newExpiryDate, string extensionReason)
    {
        if (Status != BGStatus.Issued)
            throw new InvalidOperationException($"Cannot extend BG in {Status} status");

        if (newExpiryDate <= ExpiryDate)
            throw new ArgumentException("New expiry date must be later than current expiry date", nameof(newExpiryDate));

        ExpiryDate = newExpiryDate;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new BGExtendedDomainEvent(Id, BGNumber, newExpiryDate, extensionReason));
    }

    public bool IsExpired => DateTime.UtcNow > ExpiryDate;
    public bool IsActive => Status == BGStatus.Issued;
    public int DaysToExpiry => (ExpiryDate - DateTime.UtcNow).Days;
    public Money AvailableAmount => new Money(Amount.Amount - (ClaimedAmount?.Amount ?? 0), Amount.Currency);
}

public class BGClaim
{
    public Guid Id { get; set; }
    public Guid BankGuaranteeId { get; set; }
    public Money ClaimAmount { get; set; }
    public string ClaimReason { get; set; }
    public DateTime ClaimDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public ClaimStatus Status { get; set; }
    public string ProcessingNotes { get; set; }
    public List<TradeDocument> SupportingDocuments { get; set; }
}

public class BGAmendment
{
    public Guid Id { get; set; }
    public Guid BankGuaranteeId { get; set; }
    public int AmendmentNumber { get; set; }
    public string AmendmentDetails { get; set; }
    public Money PreviousAmount { get; set; }
    public Money NewAmount { get; set; }
    public DateTime PreviousExpiryDate { get; set; }
    public DateTime NewExpiryDate { get; set; }
    public DateTime AmendmentDate { get; set; }
    public AmendmentStatus Status { get; set; }
}

public enum BGStatus
{
    Draft,
    Issued,
    ClaimSubmitted,
    Invoked,
    Cancelled,
    Expired
}



public enum ClaimStatus
{
    Submitted,
    UnderReview,
    Approved,
    Rejected,
    Paid
}


