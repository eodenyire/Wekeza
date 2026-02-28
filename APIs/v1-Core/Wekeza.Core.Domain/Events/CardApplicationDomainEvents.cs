using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Events;

public record CardApplicationSubmittedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    Guid AccountId,
    CardType RequestedCardType) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CardApplicationReviewStartedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    string ReviewedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CardApplicationDocumentsRequestedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    string DocumentRemarks) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CardApplicationDocumentsSubmittedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CardApplicationRiskAssessedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    string RiskCategory,
    bool RequiresManualReview) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CardApplicationSentToWorkflowDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    Guid WorkflowInstanceId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CardApplicationApprovedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    Guid AccountId,
    CardType CardType) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CardApplicationRejectedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    string RejectionReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CardApplicationCardIssuedDomainEvent(
    Guid ApplicationId,
    Guid CustomerId,
    Guid CardId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}