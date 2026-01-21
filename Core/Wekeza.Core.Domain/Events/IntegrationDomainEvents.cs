using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

// Integration Events
public record IntegrationCreatedDomainEvent(
    Guid IntegrationId,
    string IntegrationCode,
    string IntegrationName,
    string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record IntegrationEndpointUpdatedDomainEvent(
    Guid IntegrationId,
    string IntegrationCode,
    string EndpointUrl,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record IntegrationAuthenticationUpdatedDomainEvent(
    Guid IntegrationId,
    string IntegrationCode,
    string AuthenticationType,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record IntegrationCallSucceededDomainEvent(
    Guid IntegrationId,
    string IntegrationCode,
    string Endpoint,
    TimeSpan ResponseTime) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record IntegrationCallFailedDomainEvent(
    Guid IntegrationId,
    string IntegrationCode,
    string Endpoint,
    string ErrorMessage) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record IntegrationActivatedDomainEvent(
    Guid IntegrationId,
    string IntegrationCode,
    string ActivatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record IntegrationDeactivatedDomainEvent(
    Guid IntegrationId,
    string IntegrationCode,
    string DeactivatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record IntegrationConfigurationUpdatedDomainEvent(
    Guid IntegrationId,
    string IntegrationCode,
    string ConfigurationKey,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record IntegrationMaintenanceModeChangedDomainEvent(
    Guid IntegrationId,
    string IntegrationCode,
    bool IsMaintenanceMode,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record IntegrationCircuitBreakerOpenedDomainEvent(
    Guid IntegrationId,
    string IntegrationCode,
    string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record IntegrationCircuitBreakerClosedDomainEvent(
    Guid IntegrationId,
    string IntegrationCode) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}