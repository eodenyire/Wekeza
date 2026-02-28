using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;
/// <summary>
/// 3. AccountOpenedDomainEvent.cs (The Onboarding Signal)
/// 📂 Wekeza.Core.Domain/Events
/// We will now implement the signals that keep Wekeza Bank reactive and secure. Every event inherits from our IDomainEvent interface to ensure consistency.
/// Referencing our earlier Aggregate logic: This triggers the creation of the welcome kit and the initial KYC verification in the CRM.
///</summary>
public record AccountOpenedDomainEvent(Guid AccountId, Guid CustomerId) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
