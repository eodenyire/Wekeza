using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

/// <summary>
/// 2. AccountFrozenEvent.cs (The Security Lockdown)
/// 📂 Wekeza.Core.Domain/Events
/// When the Risk Manager (or an automated model) freezes an account, this signal ensures the Card Management System blocks the physical card and the Mobile App revokes the user's session.
/// Signal raised when an account is locked due to risk or regulatory orders.
/// Triggers immediate blocking of digital channels and physical cards.
/// </summary>
public record AccountFrozenEvent(Guid AccountId, string Reason) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
