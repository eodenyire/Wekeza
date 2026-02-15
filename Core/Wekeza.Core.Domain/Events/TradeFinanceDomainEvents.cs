using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Events;

// Letter of Credit Events
public record LCIssuedDomainEvent(Guid LCId, string LCNumber, Guid ApplicantId, Money Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record LCAmendedDomainEvent(Guid LCId, string LCNumber, int AmendmentNumber, string AmendmentDetails) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record LCConfirmedDomainEvent(Guid LCId, string LCNumber, Guid ConfirmingBankId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record DocumentsPresentedDomainEvent(Guid LCId, string LCNumber, int DocumentCount, Guid PresentingBankId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record DocumentsAcceptedDomainEvent(Guid LCId, string LCNumber, string AcceptanceNotes) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record DocumentsRejectedDomainEvent(Guid LCId, string LCNumber, string RejectionReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record LCNegotiatedDomainEvent(Guid LCId, string LCNumber, Money NegotiatedAmount, Guid NegotiatingBankId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record LCSettledDomainEvent(Guid LCId, string LCNumber, Money SettledAmount, string SettlementReference) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record LCCancelledDomainEvent(Guid LCId, string LCNumber, string CancellationReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record LCExpiredDomainEvent(Guid LCId, string LCNumber, DateTime ExpiryDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Bank Guarantee Events
public record BGIssuedDomainEvent(Guid BGId, string BGNumber, Guid PrincipalId, Money Amount, GuaranteeType Type) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BGAmendedDomainEvent(Guid BGId, string BGNumber, int AmendmentNumber, string AmendmentDetails) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BGClaimSubmittedDomainEvent(Guid BGId, string BGNumber, Money ClaimAmount, string ClaimReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BGInvokedDomainEvent(Guid BGId, string BGNumber, Money InvokedAmount, string InvocationReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BGClaimRejectedDomainEvent(Guid BGId, string BGNumber, Guid ClaimId, string RejectionReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BGCancelledDomainEvent(Guid BGId, string BGNumber, string CancellationReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BGExpiredDomainEvent(Guid BGId, string BGNumber, DateTime ExpiryDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BGExtendedDomainEvent(Guid BGId, string BGNumber, DateTime NewExpiryDate, string ExtensionReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Documentary Collection Events
public record CollectionCreatedDomainEvent(Guid CollectionId, string CollectionNumber, Guid DrawerId, Money Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CollectionSentDomainEvent(Guid CollectionId, string CollectionNumber, Guid CollectingBankId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CollectionPresentedDomainEvent(Guid CollectionId, string CollectionNumber, Guid DraweeId, Guid PresentingBankId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CollectionAcceptedDomainEvent(Guid CollectionId, string CollectionNumber, Guid DraweeId, string AcceptanceNotes) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CollectionPaidDomainEvent(Guid CollectionId, string CollectionNumber, Money PaymentAmount, string PaymentReference) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CollectionRejectedDomainEvent(Guid CollectionId, string CollectionNumber, Guid DraweeId, string RejectionReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CollectionProtestedDomainEvent(Guid CollectionId, string CollectionNumber, string ProtestReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CollectionReturnedDomainEvent(Guid CollectionId, string CollectionNumber, Guid RemittingBankId, string ReturnReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CollectionSettledDomainEvent(Guid CollectionId, string CollectionNumber, Money SettledAmount, string SettlementReference) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CollectionCancelledDomainEvent(Guid CollectionId, string CollectionNumber, string CancellationReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}