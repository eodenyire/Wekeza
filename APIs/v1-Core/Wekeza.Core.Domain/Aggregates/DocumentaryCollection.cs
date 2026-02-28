using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

public class DocumentaryCollection : AggregateRoot
{
    public string CollectionNumber { get; private set; }
    public Guid DrawerId { get; private set; }
    public Guid DraweeId { get; private set; }
    public Guid RemittingBankId { get; private set; }
    public Guid CollectingBankId { get; private set; }
    public Guid? PresentingBankId { get; private set; }
    public Money Amount { get; private set; }
    public CollectionType Type { get; private set; }
    public CollectionStatus Status { get; private set; }
    public DateTime CollectionDate { get; private set; }
    public DateTime? PresentationDate { get; private set; }
    public DateTime? AcceptanceDate { get; private set; }
    public DateTime? PaymentDate { get; private set; }
    public DateTime? MaturityDate { get; private set; }
    public string Terms { get; private set; }
    public string Instructions { get; private set; }
    public bool ProtestRequired { get; private set; }
    public List<TradeDocument> Documents { get; private set; }
    public List<CollectionEvent> Events { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private DocumentaryCollection() : base(Guid.NewGuid()) {
        Documents = new List<TradeDocument>();
        Events = new List<CollectionEvent>();
    }

    public static DocumentaryCollection Create(
        string collectionNumber,
        Guid drawerId,
        Guid draweeId,
        Guid remittingBankId,
        Guid collectingBankId,
        Money amount,
        CollectionType type,
        string terms,
        string instructions,
        DateTime? maturityDate = null,
        bool protestRequired = false)
    {
        if (string.IsNullOrWhiteSpace(collectionNumber))
            throw new ArgumentException("Collection number cannot be empty", nameof(collectionNumber));

        if (drawerId == Guid.Empty)
            throw new ArgumentException("Drawer ID cannot be empty", nameof(drawerId));

        if (draweeId == Guid.Empty)
            throw new ArgumentException("Drawee ID cannot be empty", nameof(draweeId));

        if (amount.Amount <= 0)
            throw new ArgumentException("Collection amount must be positive", nameof(amount));

        var collection = new DocumentaryCollection
        {
            Id = Guid.NewGuid(),
            CollectionNumber = collectionNumber,
            DrawerId = drawerId,
            DraweeId = draweeId,
            RemittingBankId = remittingBankId,
            CollectingBankId = collectingBankId,
            Amount = amount,
            Type = type,
            Status = CollectionStatus.Created,
            CollectionDate = DateTime.UtcNow,
            MaturityDate = maturityDate,
            Terms = terms ?? string.Empty,
            Instructions = instructions ?? string.Empty,
            ProtestRequired = protestRequired,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        collection.AddEvent("Collection Created", $"Documentary collection {collectionNumber} created");
        collection.AddDomainEvent(new CollectionCreatedDomainEvent(collection.Id, collection.CollectionNumber, collection.DrawerId, collection.Amount));

        return collection;
    }

    public void AddDocuments(List<TradeDocument> documents)
    {
        if (Status != CollectionStatus.Created)
            throw new InvalidOperationException($"Cannot add documents to collection in {Status} status");

        if (documents == null || !documents.Any())
            throw new ArgumentException("At least one document is required", nameof(documents));

        foreach (var document in documents)
        {
            Documents.Add(document);
        }

        UpdatedAt = DateTime.UtcNow;
        AddEvent("Documents Added", $"{documents.Count} documents added to collection");
    }

    public void SendToCollectingBank()
    {
        if (Status != CollectionStatus.Created)
            throw new InvalidOperationException($"Cannot send collection in {Status} status");

        if (!Documents.Any())
            throw new InvalidOperationException("Cannot send collection without documents");

        Status = CollectionStatus.SentToCollectingBank;
        UpdatedAt = DateTime.UtcNow;

        AddEvent("Sent to Collecting Bank", "Collection sent to collecting bank");
        AddDomainEvent(new CollectionSentDomainEvent(Id, CollectionNumber, CollectingBankId));
    }

    public void PresentToDrawee(Guid presentingBankId)
    {
        if (Status != CollectionStatus.SentToCollectingBank)
            throw new InvalidOperationException($"Cannot present collection in {Status} status");

        PresentingBankId = presentingBankId;
        PresentationDate = DateTime.UtcNow;
        Status = CollectionStatus.PresentedToDrawee;
        UpdatedAt = DateTime.UtcNow;

        AddEvent("Presented to Drawee", "Documents presented to drawee");
        AddDomainEvent(new CollectionPresentedDomainEvent(Id, CollectionNumber, DraweeId, presentingBankId));
    }

    public void Accept(string acceptanceNotes = "")
    {
        if (Status != CollectionStatus.PresentedToDrawee)
            throw new InvalidOperationException($"Cannot accept collection in {Status} status");

        if (Type == CollectionType.DocumentsAgainstPayment)
            throw new InvalidOperationException("D/P collections cannot be accepted, only paid");

        AcceptanceDate = DateTime.UtcNow;
        Status = CollectionStatus.Accepted;
        UpdatedAt = DateTime.UtcNow;

        AddEvent("Accepted", $"Collection accepted by drawee. {acceptanceNotes}");
        AddDomainEvent(new CollectionAcceptedDomainEvent(Id, CollectionNumber, DraweeId, acceptanceNotes));
    }

    public void Pay(Money paymentAmount, string paymentReference)
    {
        if (Type == CollectionType.DocumentsAgainstAcceptance && Status != CollectionStatus.Accepted)
            throw new InvalidOperationException("D/A collection must be accepted before payment");

        if (Type == CollectionType.DocumentsAgainstPayment && Status != CollectionStatus.PresentedToDrawee)
            throw new InvalidOperationException("D/P collection must be presented before payment");

        if (paymentAmount.Amount != Amount.Amount)
            throw new ArgumentException("Payment amount must match collection amount", nameof(paymentAmount));

        PaymentDate = DateTime.UtcNow;
        Status = CollectionStatus.Paid;
        UpdatedAt = DateTime.UtcNow;

        AddEvent("Paid", $"Collection paid. Reference: {paymentReference}");
        AddDomainEvent(new CollectionPaidDomainEvent(Id, CollectionNumber, paymentAmount, paymentReference));
    }

    public void Reject(string rejectionReason)
    {
        if (Status != CollectionStatus.PresentedToDrawee)
            throw new InvalidOperationException($"Cannot reject collection in {Status} status");

        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new ArgumentException("Rejection reason is required", nameof(rejectionReason));

        Status = CollectionStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;

        AddEvent("Rejected", $"Collection rejected by drawee. Reason: {rejectionReason}");
        AddDomainEvent(new CollectionRejectedDomainEvent(Id, CollectionNumber, DraweeId, rejectionReason));
    }

    public void Protest(string protestReason)
    {
        if (Status != CollectionStatus.Rejected && Status != CollectionStatus.PresentedToDrawee)
            throw new InvalidOperationException($"Cannot protest collection in {Status} status");

        if (!ProtestRequired)
            throw new InvalidOperationException("Protest not required for this collection");

        Status = CollectionStatus.Protested;
        UpdatedAt = DateTime.UtcNow;

        AddEvent("Protested", $"Collection protested. Reason: {protestReason}");
        AddDomainEvent(new CollectionProtestedDomainEvent(Id, CollectionNumber, protestReason));
    }

    public void Return(string returnReason)
    {
        if (Status == CollectionStatus.Paid || Status == CollectionStatus.Settled)
            throw new InvalidOperationException($"Cannot return collection in {Status} status");

        Status = CollectionStatus.Returned;
        UpdatedAt = DateTime.UtcNow;

        AddEvent("Returned", $"Collection returned to remitting bank. Reason: {returnReason}");
        AddDomainEvent(new CollectionReturnedDomainEvent(Id, CollectionNumber, RemittingBankId, returnReason));
    }

    public void Settle(string settlementReference)
    {
        if (Status != CollectionStatus.Paid)
            throw new InvalidOperationException($"Cannot settle collection in {Status} status");

        Status = CollectionStatus.Settled;
        UpdatedAt = DateTime.UtcNow;

        AddEvent("Settled", $"Collection settled. Reference: {settlementReference}");
        AddDomainEvent(new CollectionSettledDomainEvent(Id, CollectionNumber, Amount, settlementReference));
    }

    public void Cancel(string cancellationReason)
    {
        if (Status == CollectionStatus.Paid || Status == CollectionStatus.Settled)
            throw new InvalidOperationException($"Cannot cancel collection in {Status} status");

        Status = CollectionStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        AddEvent("Cancelled", $"Collection cancelled. Reason: {cancellationReason}");
        AddDomainEvent(new CollectionCancelledDomainEvent(Id, CollectionNumber, cancellationReason));
    }

    private void AddEvent(string eventType, string description)
    {
        Events.Add(new CollectionEvent
        {
            Id = Guid.NewGuid(),
            CollectionId = Id,
            EventType = eventType,
            Description = description,
            EventDate = DateTime.UtcNow
        });
    }

    public bool IsMatured => MaturityDate.HasValue && DateTime.UtcNow >= MaturityDate.Value;
    public bool IsActive => Status != CollectionStatus.Settled && Status != CollectionStatus.Cancelled && Status != CollectionStatus.Returned;
    public int DaysOutstanding => (DateTime.UtcNow - CollectionDate).Days;
}

public class CollectionEvent
{
    public Guid Id { get; set; }
    public Guid CollectionId { get; set; }
    public string EventType { get; set; }
    public string Description { get; set; }
    public DateTime EventDate { get; set; }
}

public enum CollectionType
{
    DocumentsAgainstPayment, // D/P
    DocumentsAgainstAcceptance // D/A
}

public enum CollectionStatus
{
    Created,
    SentToCollectingBank,
    PresentedToDrawee,
    Accepted,
    Paid,
    Rejected,
    Protested,
    Returned,
    Settled,
    Cancelled
}

