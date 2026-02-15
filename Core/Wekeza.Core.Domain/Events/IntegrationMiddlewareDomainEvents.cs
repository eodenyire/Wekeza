using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Events;

// ============================================================================
// INTEGRATION ENDPOINT DOMAIN EVENTS
// ============================================================================

/// <summary>
/// Integration Endpoint Created Domain Event - Triggered when a new endpoint is created
/// </summary>
public record IntegrationEndpointCreatedDomainEvent(
    Guid EndpointId,
    string EndpointCode,
    string EndpointName,
    EndpointType Type,
    EndpointProtocol Protocol,
    string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Endpoint Configuration Updated Domain Event - Triggered when endpoint config changes
/// </summary>
public record IntegrationEndpointConfigurationUpdatedDomainEvent(
    Guid EndpointId,
    string EndpointCode,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Endpoint Credentials Updated Domain Event - Triggered when credentials are updated
/// </summary>
public record IntegrationEndpointCredentialsUpdatedDomainEvent(
    Guid EndpointId,
    string EndpointCode,
    DateTime? ExpiryDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Endpoint Call Succeeded Domain Event - Triggered when API call succeeds
/// </summary>
public record IntegrationEndpointCallSucceededDomainEvent(
    Guid EndpointId,
    string EndpointCode,
    TimeSpan ResponseTime,
    DateTime CalledAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Endpoint Call Failed Domain Event - Triggered when API call fails
/// </summary>
public record IntegrationEndpointCallFailedDomainEvent(
    Guid EndpointId,
    string EndpointCode,
    string ErrorMessage,
    DateTime FailedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Endpoint Health Status Changed Domain Event - Triggered when health status changes
/// </summary>
public record IntegrationEndpointHealthStatusChangedDomainEvent(
    Guid EndpointId,
    string EndpointCode,
    HealthStatus PreviousStatus,
    HealthStatus NewStatus,
    string Result) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Endpoint Enabled Domain Event - Triggered when endpoint is enabled
/// </summary>
public record IntegrationEndpointEnabledDomainEvent(
    Guid EndpointId,
    string EndpointCode,
    string EnabledBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Endpoint Disabled Domain Event - Triggered when endpoint is disabled
/// </summary>
public record IntegrationEndpointDisabledDomainEvent(
    Guid EndpointId,
    string EndpointCode,
    string DisabledBy,
    string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Endpoint Circuit Breaker Triggered Domain Event - Triggered when circuit breaker opens
/// </summary>
public record IntegrationEndpointCircuitBreakerTriggeredDomainEvent(
    Guid EndpointId,
    string EndpointCode,
    int FailureThreshold,
    DateTime TriggeredAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Endpoint Circuit Breaker Reset Domain Event - Triggered when circuit breaker closes
/// </summary>
public record IntegrationEndpointCircuitBreakerResetDomainEvent(
    Guid EndpointId,
    string EndpointCode,
    DateTime ResetAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Endpoint Rate Limit Exceeded Domain Event - Triggered when rate limit is exceeded
/// </summary>
public record IntegrationEndpointRateLimitExceededDomainEvent(
    Guid EndpointId,
    string EndpointCode,
    int CurrentRate,
    int RateLimit,
    DateTime ExceededAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ============================================================================
// MESSAGE QUEUE DOMAIN EVENTS
// ============================================================================

/// <summary>
/// Message Queue Created Domain Event - Triggered when a new queue is created
/// </summary>
public record MessageQueueCreatedDomainEvent(
    Guid QueueId,
    string QueueName,
    QueueType Type,
    string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Enqueued Domain Event - Triggered when a message is added to queue
/// </summary>
public record MessageEnqueuedDomainEvent(
    Guid QueueId,
    string QueueName,
    string MessageId,
    string MessageType,
    MessagePriority Priority,
    DateTime EnqueuedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Dequeued Domain Event - Triggered when a message is removed from queue
/// </summary>
public record MessageDequeuedDomainEvent(
    Guid QueueId,
    string QueueName,
    string MessageId,
    string MessageType,
    int DeliveryCount,
    DateTime DequeuedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Processed Domain Event - Triggered when a message is successfully processed
/// </summary>
public record MessageProcessedDomainEvent(
    Guid QueueId,
    string QueueName,
    string MessageId,
    TimeSpan ProcessingTime,
    DateTime ProcessedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Processing Failed Domain Event - Triggered when message processing fails
/// </summary>
public record MessageProcessingFailedDomainEvent(
    Guid QueueId,
    string QueueName,
    string MessageId,
    string ErrorMessage,
    DateTime FailedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Moved To Dead Letter Domain Event - Triggered when message is moved to DLQ
/// </summary>
public record MessageMovedToDeadLetterDomainEvent(
    Guid QueueId,
    string QueueName,
    string DeadLetterQueue,
    string MessageId,
    string Reason,
    DateTime MovedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Expired Domain Event - Triggered when a message expires
/// </summary>
public record MessageExpiredDomainEvent(
    Guid QueueId,
    string QueueName,
    string MessageId,
    string MessageType,
    DateTime ExpiredAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Queue Purged Domain Event - Triggered when queue is purged
/// </summary>
public record MessageQueuePurgedDomainEvent(
    Guid QueueId,
    string QueueName,
    int MessageCount,
    string PurgedBy,
    DateTime PurgedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Queue Configuration Updated Domain Event - Triggered when queue config changes
/// </summary>
public record MessageQueueConfigurationUpdatedDomainEvent(
    Guid QueueId,
    string QueueName,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Queue Consumer Added Domain Event - Triggered when consumer is added
/// </summary>
public record MessageQueueConsumerAddedDomainEvent(
    Guid QueueId,
    string QueueName,
    string ConsumerId,
    string ConsumerName,
    DateTime AddedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Queue Consumer Removed Domain Event - Triggered when consumer is removed
/// </summary>
public record MessageQueueConsumerRemovedDomainEvent(
    Guid QueueId,
    string QueueName,
    string ConsumerId,
    string ConsumerName,
    DateTime RemovedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Queue Activated Domain Event - Triggered when queue is activated
/// </summary>
public record MessageQueueActivatedDomainEvent(
    Guid QueueId,
    string QueueName,
    string ActivatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Queue Deactivated Domain Event - Triggered when queue is deactivated
/// </summary>
public record MessageQueueDeactivatedDomainEvent(
    Guid QueueId,
    string QueueName,
    string DeactivatedBy,
    string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Message Queue Overloaded Domain Event - Triggered when queue reaches capacity
/// </summary>
public record MessageQueueOverloadedDomainEvent(
    Guid QueueId,
    string QueueName,
    int CurrentSize,
    int MaxSize,
    DateTime OverloadedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ============================================================================
// WEBHOOK SUBSCRIPTION DOMAIN EVENTS
// ============================================================================

/// <summary>
/// Webhook Subscription Created Domain Event - Triggered when subscription is created
/// </summary>
public record WebhookSubscriptionCreatedDomainEvent(
    Guid SubscriptionId,
    string SubscriptionCode,
    string SubscriptionName,
    string CallbackUrl,
    List<string> EventTypes,
    string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Webhook Subscription Updated Domain Event - Triggered when subscription is updated
/// </summary>
public record WebhookSubscriptionUpdatedDomainEvent(
    Guid SubscriptionId,
    string SubscriptionCode,
    string PropertyName,
    string NewValue) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Webhook Delivery Succeeded Domain Event - Triggered when webhook delivery succeeds
/// </summary>
public record WebhookDeliverySucceededDomainEvent(
    Guid SubscriptionId,
    string SubscriptionCode,
    string CallbackUrl,
    int HttpStatusCode,
    TimeSpan DeliveryTime,
    DateTime DeliveredAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Webhook Delivery Failed Domain Event - Triggered when webhook delivery fails
/// </summary>
public record WebhookDeliveryFailedDomainEvent(
    Guid SubscriptionId,
    string SubscriptionCode,
    string CallbackUrl,
    string ErrorMessage,
    int? HttpStatusCode,
    int AttemptNumber,
    DateTime FailedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Webhook Subscription Enabled Domain Event - Triggered when subscription is enabled
/// </summary>
public record WebhookSubscriptionEnabledDomainEvent(
    Guid SubscriptionId,
    string SubscriptionCode,
    string EnabledBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Webhook Subscription Disabled Domain Event - Triggered when subscription is disabled
/// </summary>
public record WebhookSubscriptionDisabledDomainEvent(
    Guid SubscriptionId,
    string SubscriptionCode,
    string DisabledBy,
    string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Webhook Subscription Suspended Domain Event - Triggered when subscription is suspended
/// </summary>
public record WebhookSubscriptionSuspendedDomainEvent(
    Guid SubscriptionId,
    string SubscriptionCode,
    string SuspendedBy,
    string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Webhook Subscription Health Degraded Domain Event - Triggered when health degrades
/// </summary>
public record WebhookSubscriptionHealthDegradedDomainEvent(
    Guid SubscriptionId,
    string SubscriptionCode,
    int ConsecutiveFailures,
    WebhookHealthStatus HealthStatus,
    DateTime DegradedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Webhook Subscription Rate Limited Domain Event - Triggered when rate limit is hit
/// </summary>
public record WebhookSubscriptionRateLimitedDomainEvent(
    Guid SubscriptionId,
    string SubscriptionCode,
    int CurrentRate,
    int RateLimit,
    DateTime LimitedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ============================================================================
// API GATEWAY DOMAIN EVENTS
// ============================================================================

/// <summary>
/// API Route Created Domain Event - Triggered when API route is created
/// </summary>
public record ApiRouteCreatedDomainEvent(
    Guid RouteId,
    string Path,
    string Method,
    string UpstreamUrl,
    string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// API Route Updated Domain Event - Triggered when API route is updated
/// </summary>
public record ApiRouteUpdatedDomainEvent(
    Guid RouteId,
    string Path,
    string Method,
    string PropertyName,
    string ModifiedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// API Request Routed Domain Event - Triggered when request is routed
/// </summary>
public record ApiRequestRoutedDomainEvent(
    Guid RouteId,
    string Path,
    string Method,
    string UpstreamUrl,
    TimeSpan ResponseTime,
    int StatusCode,
    DateTime RoutedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// API Request Failed Domain Event - Triggered when request routing fails
/// </summary>
public record ApiRequestFailedDomainEvent(
    Guid RouteId,
    string Path,
    string Method,
    string ErrorMessage,
    int? StatusCode,
    DateTime FailedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// API Rate Limit Exceeded Domain Event - Triggered when rate limit is exceeded
/// </summary>
public record ApiRateLimitExceededDomainEvent(
    Guid RouteId,
    string Path,
    string Method,
    string ClientId,
    int CurrentRate,
    int RateLimit,
    DateTime ExceededAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// API Authentication Failed Domain Event - Triggered when authentication fails
/// </summary>
public record ApiAuthenticationFailedDomainEvent(
    Guid RouteId,
    string Path,
    string Method,
    string Reason,
    string ClientId,
    DateTime FailedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// API Authorization Failed Domain Event - Triggered when authorization fails
/// </summary>
public record ApiAuthorizationFailedDomainEvent(
    Guid RouteId,
    string Path,
    string Method,
    string UserId,
    List<string> RequiredRoles,
    List<string> UserRoles,
    DateTime FailedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ============================================================================
// DATA TRANSFORMATION DOMAIN EVENTS
// ============================================================================

/// <summary>
/// Data Transformation Created Domain Event - Triggered when transformation is created
/// </summary>
public record DataTransformationCreatedDomainEvent(
    Guid TransformationId,
    string TransformationCode,
    string TransformationName,
    TransformationType Type,
    string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Data Transformation Executed Domain Event - Triggered when transformation is executed
/// </summary>
public record DataTransformationExecutedDomainEvent(
    Guid TransformationId,
    string TransformationCode,
    TimeSpan ExecutionTime,
    bool Success,
    DateTime ExecutedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Data Transformation Failed Domain Event - Triggered when transformation fails
/// </summary>
public record DataTransformationFailedDomainEvent(
    Guid TransformationId,
    string TransformationCode,
    string ErrorMessage,
    DateTime FailedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ============================================================================
// SYSTEM INTEGRATION DOMAIN EVENTS
// ============================================================================

/// <summary>
/// Integration Health Check Completed Domain Event - Triggered when health check completes
/// </summary>
public record IntegrationHealthCheckCompletedDomainEvent(
    string ComponentName,
    HealthStatus Status,
    TimeSpan CheckDuration,
    string Details,
    DateTime CheckedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration System Connected Domain Event - Triggered when external system connects
/// </summary>
public record IntegrationSystemConnectedDomainEvent(
    string SystemName,
    string SystemType,
    string ConnectionId,
    DateTime ConnectedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration System Disconnected Domain Event - Triggered when external system disconnects
/// </summary>
public record IntegrationSystemDisconnectedDomainEvent(
    string SystemName,
    string SystemType,
    string ConnectionId,
    string Reason,
    DateTime DisconnectedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Batch Processing Started Domain Event - Triggered when batch processing starts
/// </summary>
public record IntegrationBatchProcessingStartedDomainEvent(
    string BatchId,
    string BatchType,
    int RecordCount,
    string ProcessedBy,
    DateTime StartedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Batch Processing Completed Domain Event - Triggered when batch processing completes
/// </summary>
public record IntegrationBatchProcessingCompletedDomainEvent(
    string BatchId,
    string BatchType,
    int ProcessedRecords,
    int SuccessfulRecords,
    int FailedRecords,
    TimeSpan ProcessingTime,
    DateTime CompletedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Error Threshold Exceeded Domain Event - Triggered when error rate is too high
/// </summary>
public record IntegrationErrorThresholdExceededDomainEvent(
    string ComponentName,
    int ErrorCount,
    int ErrorThreshold,
    TimeSpan TimeWindow,
    DateTime ExceededAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Performance Degraded Domain Event - Triggered when performance degrades
/// </summary>
public record IntegrationPerformanceDegradedDomainEvent(
    string ComponentName,
    TimeSpan CurrentResponseTime,
    TimeSpan ThresholdResponseTime,
    DateTime DegradedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Configuration Changed Domain Event - Triggered when integration config changes
/// </summary>
public record IntegrationConfigurationChangedDomainEvent(
    string ComponentName,
    string ConfigurationKey,
    string OldValue,
    string NewValue,
    string ChangedBy,
    DateTime ChangedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Security Event Domain Event - Triggered for security-related events
/// </summary>
public record IntegrationSecurityEventDomainEvent(
    string EventType,
    string ComponentName,
    string Details,
    string SourceIp,
    string UserId,
    DateTime OccurredAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}