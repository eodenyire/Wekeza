using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Card Application Aggregate - Manages card application and approval process
/// Supports workflow integration for card issuance approval
/// </summary>
public class CardApplication : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Guid AccountId { get; private set; }
    public string ApplicationNumber { get; private set; }
    public CardType RequestedCardType { get; private set; }
    public CardApplicationStatus Status { get; private set; }
    public string NameOnCard { get; private set; }
    public string DeliveryAddress { get; private set; }
    public string? AlternateDeliveryAddress { get; private set; }
    public string ContactNumber { get; private set; }
    public string EmailAddress { get; private set; }
    
    // Requested Limits
    public Money RequestedDailyWithdrawalLimit { get; private set; }
    public Money RequestedDailyPurchaseLimit { get; private set; }
    public Money RequestedMonthlyLimit { get; private set; }
    public int RequestedMaxMonthlyTransactions { get; private set; }
    
    // Channel Preferences
    public bool RequestATMEnabled { get; private set; }
    public bool RequestPOSEnabled { get; private set; }
    public bool RequestOnlineEnabled { get; private set; }
    public bool RequestInternationalEnabled { get; private set; }
    public bool RequestContactlessEnabled { get; private set; }
    
    // Application Processing
    public DateTime ApplicationDate { get; private set; }
    public string? ProcessedBy { get; private set; }
    public DateTime? ProcessedDate { get; private set; }
    public string? ApprovalComments { get; private set; }
    public string? RejectionReason { get; private set; }
    public Guid? WorkflowInstanceId { get; private set; }
    
    // Card Issuance
    public Guid? IssuedCardId { get; private set; }
    public DateTime? CardIssuedDate { get; private set; }
    public string? IssuedBy { get; private set; }
    
    // Document Requirements
    public bool IdentityDocumentProvided { get; private set; }
    public bool IncomeProofProvided { get; private set; }
    public bool AddressProofProvided { get; private set; }
    public string? DocumentRemarks { get; private set; }
    
    // Risk Assessment
    public string? CreditScore { get; private set; }
    public string? RiskCategory { get; private set; }
    public bool RequiresManualReview { get; private set; }
    public string? RiskAssessmentComments { get; private set; }

    private CardApplication() : base(Guid.NewGuid()) { }

    public static CardApplication Create(
        Guid customerId,
        Guid accountId,
        CardType requestedCardType,
        string nameOnCard,
        string deliveryAddress,
        string contactNumber,
        string emailAddress,
        Money requestedDailyWithdrawalLimit,
        Money requestedDailyPurchaseLimit,
        Money requestedMonthlyLimit,
        int requestedMaxMonthlyTransactions = 100)
    {
        var application = new CardApplication
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            AccountId = accountId,
            ApplicationNumber = GenerateApplicationNumber(),
            RequestedCardType = requestedCardType,
            Status = CardApplicationStatus.Submitted,
            NameOnCard = nameOnCard,
            DeliveryAddress = deliveryAddress,
            ContactNumber = contactNumber,
            EmailAddress = emailAddress,
            RequestedDailyWithdrawalLimit = requestedDailyWithdrawalLimit,
            RequestedDailyPurchaseLimit = requestedDailyPurchaseLimit,
            RequestedMonthlyLimit = requestedMonthlyLimit,
            RequestedMaxMonthlyTransactions = requestedMaxMonthlyTransactions,
            ApplicationDate = DateTime.UtcNow,
            RequestATMEnabled = true,
            RequestPOSEnabled = true,
            RequestOnlineEnabled = true,
            RequestInternationalEnabled = false,
            RequestContactlessEnabled = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "CUSTOMER"
        };

        application.AddDomainEvent(new CardApplicationSubmittedDomainEvent(
            application.Id, customerId, accountId, requestedCardType));

        return application;
    }

    public void StartReview(string reviewedBy)
    {
        if (Status != CardApplicationStatus.Submitted)
            throw new DomainException($"Cannot start review for application in {Status} status");

        Status = CardApplicationStatus.UnderReview;
        ProcessedBy = reviewedBy;
        ProcessedDate = DateTime.UtcNow;

        AddDomainEvent(new CardApplicationReviewStartedDomainEvent(Id, CustomerId, reviewedBy));
    }

    public void RequestDocuments(string documentRemarks)
    {
        if (Status != CardApplicationStatus.UnderReview)
            throw new DomainException($"Cannot request documents for application in {Status} status");

        Status = CardApplicationStatus.DocumentsRequired;
        DocumentRemarks = documentRemarks;

        AddDomainEvent(new CardApplicationDocumentsRequestedDomainEvent(Id, CustomerId, documentRemarks));
    }

    public void SubmitDocuments(
        bool identityDocumentProvided,
        bool incomeProofProvided,
        bool addressProofProvided)
    {
        if (Status != CardApplicationStatus.DocumentsRequired)
            throw new DomainException($"Cannot submit documents for application in {Status} status");

        IdentityDocumentProvided = identityDocumentProvided;
        IncomeProofProvided = incomeProofProvided;
        AddressProofProvided = addressProofProvided;

        // Check if all required documents are provided
        if (identityDocumentProvided && incomeProofProvided && addressProofProvided)
        {
            Status = CardApplicationStatus.UnderReview;
            AddDomainEvent(new CardApplicationDocumentsSubmittedDomainEvent(Id, CustomerId));
        }
    }

    public void PerformRiskAssessment(
        string creditScore,
        string riskCategory,
        bool requiresManualReview,
        string? riskAssessmentComments = null)
    {
        CreditScore = creditScore;
        RiskCategory = riskCategory;
        RequiresManualReview = requiresManualReview;
        RiskAssessmentComments = riskAssessmentComments;

        AddDomainEvent(new CardApplicationRiskAssessedDomainEvent(
            Id, CustomerId, riskCategory, requiresManualReview));
    }

    public void SendToWorkflow(Guid workflowInstanceId)
    {
        if (Status != CardApplicationStatus.UnderReview)
            throw new DomainException($"Cannot send to workflow for application in {Status} status");

        Status = CardApplicationStatus.PendingApproval;
        WorkflowInstanceId = workflowInstanceId;

        AddDomainEvent(new CardApplicationSentToWorkflowDomainEvent(Id, CustomerId, workflowInstanceId));
    }

    public void Approve(string approvedBy, string? approvalComments = null)
    {
        if (Status != CardApplicationStatus.PendingApproval && Status != CardApplicationStatus.UnderReview)
            throw new DomainException($"Cannot approve application in {Status} status");

        Status = CardApplicationStatus.Approved;
        ProcessedBy = approvedBy;
        ProcessedDate = DateTime.UtcNow;
        ApprovalComments = approvalComments;

        AddDomainEvent(new CardApplicationApprovedDomainEvent(Id, CustomerId, AccountId, RequestedCardType));
    }

    public void Reject(string rejectedBy, string rejectionReason)
    {
        if (Status == CardApplicationStatus.Approved || Status == CardApplicationStatus.CardIssued)
            throw new DomainException($"Cannot reject application in {Status} status");

        Status = CardApplicationStatus.Rejected;
        ProcessedBy = rejectedBy;
        ProcessedDate = DateTime.UtcNow;
        RejectionReason = rejectionReason;

        AddDomainEvent(new CardApplicationRejectedDomainEvent(Id, CustomerId, rejectionReason));
    }

    public void MarkCardIssued(Guid cardId, string issuedBy)
    {
        if (Status != CardApplicationStatus.Approved)
            throw new DomainException($"Cannot issue card for application in {Status} status");

        Status = CardApplicationStatus.CardIssued;
        IssuedCardId = cardId;
        CardIssuedDate = DateTime.UtcNow;
        IssuedBy = issuedBy;

        AddDomainEvent(new CardApplicationCardIssuedDomainEvent(Id, CustomerId, cardId));
    }

    public void UpdateChannelPreferences(
        bool atmEnabled,
        bool posEnabled,
        bool onlineEnabled,
        bool internationalEnabled,
        bool contactlessEnabled)
    {
        RequestATMEnabled = atmEnabled;
        RequestPOSEnabled = posEnabled;
        RequestOnlineEnabled = onlineEnabled;
        RequestInternationalEnabled = internationalEnabled;
        RequestContactlessEnabled = contactlessEnabled;
    }

    public void UpdateLimits(
        Money dailyWithdrawalLimit,
        Money dailyPurchaseLimit,
        Money monthlyLimit,
        int maxMonthlyTransactions)
    {
        RequestedDailyWithdrawalLimit = dailyWithdrawalLimit;
        RequestedDailyPurchaseLimit = dailyPurchaseLimit;
        RequestedMonthlyLimit = monthlyLimit;
        RequestedMaxMonthlyTransactions = maxMonthlyTransactions;
    }

    private static string GenerateApplicationNumber()
    {
        return $"CA{DateTime.UtcNow:yyyyMMdd}{new Random().Next(100000, 999999)}";
    }
}

// Enums for Card Application Management
public enum CardApplicationStatus
{
    Submitted = 1,
    UnderReview = 2,
    DocumentsRequired = 3,
    PendingApproval = 4,
    Approved = 5,
    Rejected = 6,
    CardIssued = 7,
    Cancelled = 8
}