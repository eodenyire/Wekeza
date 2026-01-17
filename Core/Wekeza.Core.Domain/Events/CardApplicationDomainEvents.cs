using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Events;

public record CardApplicationSubmittedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    Guid AccountId,
    CardType RequestedCardType) : IDomainEvent;

public record CardApplicationReviewStartedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    string ReviewedBy) : IDomainEvent;

public record CardApplicationDocumentsRequestedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    string DocumentRemarks) : IDomainEvent;

public record CardApplicationDocumentsSubmittedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId) : IDomainEvent;

public record CardApplicationRiskAssessedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    string RiskCategory,
    bool RequiresManualReview) : IDomainEvent;

public record CardApplicationSentToWorkflowDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    Guid WorkflowInstanceId) : IDomainEvent;

public record CardApplicationApprovedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    Guid AccountId,
    CardType CardType) : IDomainEvent;

public record CardApplicationRejectedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    string RejectionReason) : IDomainEvent;

public record CardApplicationCardIssuedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    Guid CardId) : IDomainEvent;