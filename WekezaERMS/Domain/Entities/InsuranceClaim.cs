using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Insurance Claim Entity - Represents insurance claims filed
/// Aligned with Riskonnect claims tracking capabilities
/// </summary>
public class InsuranceClaim
{
    /// <summary>
    /// Unique identifier for the claim
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Claim reference number
    /// </summary>
    public string ClaimNumber { get; private set; }

    /// <summary>
    /// Associated insurance policy ID
    /// </summary>
    public Guid PolicyId { get; private set; }

    /// <summary>
    /// Related incident ID if applicable
    /// </summary>
    public Guid? IncidentId { get; private set; }

    /// <summary>
    /// Claim status
    /// </summary>
    public InsuranceClaimStatus Status { get; private set; }

    /// <summary>
    /// Description of the claim
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Date of loss or incident
    /// </summary>
    public DateTime LossDate { get; private set; }

    /// <summary>
    /// Date claim was filed
    /// </summary>
    public DateTime FiledDate { get; private set; }

    /// <summary>
    /// Amount claimed
    /// </summary>
    public decimal ClaimAmount { get; private set; }

    /// <summary>
    /// Amount approved by insurer
    /// </summary>
    public decimal? ApprovedAmount { get; private set; }

    /// <summary>
    /// Amount paid out
    /// </summary>
    public decimal? PaidAmount { get; private set; }

    /// <summary>
    /// Date payment was received
    /// </summary>
    public DateTime? PaymentReceivedDate { get; private set; }

    /// <summary>
    /// Claim adjuster assigned
    /// </summary>
    public string? ClaimAdjuster { get; private set; }

    /// <summary>
    /// Internal claim handler
    /// </summary>
    public Guid HandlerId { get; private set; }

    /// <summary>
    /// Supporting documentation references
    /// </summary>
    public string? SupportingDocuments { get; private set; }

    /// <summary>
    /// Denial reason if claim was denied
    /// </summary>
    public string? DenialReason { get; private set; }

    /// <summary>
    /// Resolution notes
    /// </summary>
    public string? ResolutionNotes { get; private set; }

    /// <summary>
    /// Date claim was closed
    /// </summary>
    public DateTime? ClosedDate { get; private set; }

    /// <summary>
    /// Audit trail
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private InsuranceClaim() { }

    public static InsuranceClaim Create(
        string claimNumber,
        Guid policyId,
        string description,
        DateTime lossDate,
        decimal claimAmount,
        Guid handlerId,
        Guid? incidentId,
        Guid createdBy)
    {
        return new InsuranceClaim
        {
            Id = Guid.NewGuid(),
            ClaimNumber = claimNumber,
            PolicyId = policyId,
            Description = description,
            LossDate = lossDate,
            FiledDate = DateTime.UtcNow,
            ClaimAmount = claimAmount,
            HandlerId = handlerId,
            IncidentId = incidentId,
            Status = InsuranceClaimStatus.Submitted,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void UpdateStatus(InsuranceClaimStatus newStatus, Guid updatedBy)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void AssignAdjuster(string adjusterName, Guid assignedBy)
    {
        ClaimAdjuster = adjusterName;
        Status = InsuranceClaimStatus.UnderReview;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = assignedBy;
    }

    public void Approve(decimal approvedAmount, Guid approvedBy)
    {
        ApprovedAmount = approvedAmount;
        Status = InsuranceClaimStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = approvedBy;
    }

    public void Deny(string reason, Guid deniedBy)
    {
        Status = InsuranceClaimStatus.Denied;
        DenialReason = reason;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deniedBy;
    }

    public void RecordPayment(decimal paidAmount, Guid recordedBy)
    {
        PaidAmount = paidAmount;
        PaymentReceivedDate = DateTime.UtcNow;
        Status = InsuranceClaimStatus.Settled;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = recordedBy;
    }

    public void Close(string resolutionNotes, Guid closedBy)
    {
        ResolutionNotes = resolutionNotes;
        ClosedDate = DateTime.UtcNow;
        Status = InsuranceClaimStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = closedBy;
    }
}
