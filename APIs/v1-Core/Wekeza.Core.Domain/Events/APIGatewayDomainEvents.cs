using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

// API Gateway Events
public record APIGatewayCreatedDomainEvent(
    Guid GatewayId,
    string GatewayCode,
    string GatewayName,
    string BaseUrl,
    string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record APIRouteAddedDomainEvent(
    Guid GatewayId,
    string GatewayCode,
    string Path,
    string Method,
    string UpstreamUrl,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record APIRouteRemovedDomainEvent(
    Guid GatewayId,
    string GatewayCode,
    string Path,
    string Method,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record APIRateLimitUpdatedDomainEvent(
    Guid GatewayId,
    string GatewayCode,
    string Endpoint,
    int RequestsPerMinute,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record APIUpstreamServerAddedDomainEvent(
    Guid GatewayId,
    string GatewayCode,
    string ServerUrl,
    int Weight,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record APIServerHealthChangedDomainEvent(
    Guid GatewayId,
    string GatewayCode,
    string ServerUrl,
    bool IsHealthy,
    string? HealthCheckResponse) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record APIMetricsUpdatedDomainEvent(
    Guid GatewayId,
    string GatewayCode,
    long TotalRequests,
    double AverageResponseTime,
    double ErrorRate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record APIGatewayActivatedDomainEvent(
    Guid GatewayId,
    string GatewayCode,
    string ActivatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record APIGatewayDeactivatedDomainEvent(
    Guid GatewayId,
    string GatewayCode,
    string DeactivatedBy,
    string? Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record APISecurityConfigUpdatedDomainEvent(
    Guid GatewayId,
    string GatewayCode,
    bool RequireAuthentication,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record APICircuitBreakerOpenedDomainEvent(
    Guid GatewayId,
    string GatewayCode,
    string Endpoint,
    int FailureCount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record APICircuitBreakerClosedDomainEvent(
    Guid GatewayId,
    string GatewayCode,
    string Endpoint) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}