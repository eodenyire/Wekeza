using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Events;

// Letter of Credit Events
public record LCIssuedDomainEvent(Guid LCId, string LCNumber, Guid ApplicantId, Money Amount) : IDomainEvent;

public record LCAmendedDomainEvent(Guid LCId, string LCNumber, int AmendmentNumber, string AmendmentDetails) : IDomainEvent;

public record LCConfirmedDomainEvent(Guid LCId, string LCNumber, Guid ConfirmingBankId) : IDomainEvent;

public record DocumentsPresentedDomainEvent(Guid LCId, string LCNumber, int DocumentCount, Guid PresentingBankId) : IDomainEvent;

public record DocumentsAcceptedDomainEvent(Guid LCId, string LCNumber, string AcceptanceNotes) : IDomainEvent;

public record DocumentsRejectedDomainEvent(Guid LCId, string LCNumber, string RejectionReason) : IDomainEvent;

public record LCNegotiatedDomainEvent(Guid LCId, string LCNumber, Money NegotiatedAmount, Guid NegotiatingBankId) : IDomainEvent;

public record LCSettledDomainEvent(Guid LCId, string LCNumber, Money SettledAmount, string SettlementReference) : IDomainEvent;

public record LCCancelledDomainEvent(Guid LCId, string LCNumber, string CancellationReason) : IDomainEvent;

public record LCExpiredDomainEvent(Guid LCId, string LCNumber, DateTime ExpiryDate) : IDomainEvent;

// Bank Guarantee Events
public record BGIssuedDomainEvent(Guid BGId, string BGNumber, Guid PrincipalId, Money Amount, GuaranteeType Type) : IDomainEvent;

public record BGAmendedDomainEvent(Guid BGId, string BGNumber, int AmendmentNumber, string AmendmentDetails) : IDomainEvent;

public record BGClaimSubmittedDomainEvent(Guid BGId, string BGNumber, Money ClaimAmount, string ClaimReason) : IDomainEvent;

public record BGInvokedDomainEvent(Guid BGId, string BGNumber, Money InvokedAmount, string InvocationReason) : IDomainEvent;

public record BGClaimRejectedDomainEvent(Guid BGId, string BGNumber, Guid ClaimId, string RejectionReason) : IDomainEvent;

public record BGCancelledDomainEvent(Guid BGId, string BGNumber, string CancellationReason) : IDomainEvent;

public record BGExpiredDomainEvent(Guid BGId, string BGNumber, DateTime ExpiryDate) : IDomainEvent;

public record BGExtendedDomainEvent(Guid BGId, string BGNumber, DateTime NewExpiryDate, string ExtensionReason) : IDomainEvent;

// Documentary Collection Events
public record CollectionCreatedDomainEvent(Guid CollectionId, string CollectionNumber, Guid DrawerId, Money Amount) : IDomainEvent;

public record CollectionSentDomainEvent(Guid CollectionId, string CollectionNumber, Guid CollectingBankId) : IDomainEvent;

public record CollectionPresentedDomainEvent(Guid CollectionId, string CollectionNumber, Guid DraweeId, Guid PresentingBankId) : IDomainEvent;

public record CollectionAcceptedDomainEvent(Guid CollectionId, string CollectionNumber, Guid DraweeId, string AcceptanceNotes) : IDomainEvent;

public record CollectionPaidDomainEvent(Guid CollectionId, string CollectionNumber, Money PaymentAmount, string PaymentReference) : IDomainEvent;

public record CollectionRejectedDomainEvent(Guid CollectionId, string CollectionNumber, Guid DraweeId, string RejectionReason) : IDomainEvent;

public record CollectionProtestedDomainEvent(Guid CollectionId, string CollectionNumber, string ProtestReason) : IDomainEvent;

public record CollectionReturnedDomainEvent(Guid CollectionId, string CollectionNumber, Guid RemittingBankId, string ReturnReason) : IDomainEvent;

public record CollectionSettledDomainEvent(Guid CollectionId, string CollectionNumber, Money SettledAmount, string SettlementReference) : IDomainEvent;

public record CollectionCancelledDomainEvent(Guid CollectionId, string CollectionNumber, string CancellationReason) : IDomainEvent;