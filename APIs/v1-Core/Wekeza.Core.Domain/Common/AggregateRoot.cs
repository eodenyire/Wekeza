namespace Wekeza.Core.Domain.Common;

/// <summary>
/// 3. AggregateRoot.cs
/// This is the "Entry Point." Only Aggregate Roots get their own Repositories. For example, an Account is an Aggregate Root, but a TransactionLine inside it is just an Entity.
/// The base for all "Entry Points" in Wekeza Bank. 
/// It maintains a list of Domain Events that occurred during a business transaction.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected AggregateRoot(Guid id) : base(id) { }

    // Audit properties
    public DateTime CreatedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public DateTime? UpdatedAt { get; protected set; }
    public string? UpdatedBy { get; protected set; }

    /// <summary>
    /// A read-only collection of events that happened to this Aggregate.
    /// These will be dispatched to other systems (like Fraud or Big Data) after saving.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void SetAuditInfo(string createdBy, DateTime? createdAt = null)
    {
        CreatedAt = createdAt ?? DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    protected void UpdateAuditInfo(string updatedBy, DateTime? updatedAt = null)
    {
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
