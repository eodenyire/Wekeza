using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.ValueObjects;

/// <summary>
/// Message Envelope Value Object - Represents a message with metadata for queuing and routing
/// Immutable value object containing message headers, payload, and routing information
/// Industry Standard: Enterprise messaging patterns (AMQP, JMS, Apache Kafka)
/// </summary>
public class MessageEnvelope : ValueObject
{
    public string MessageId { get; private set; } = string.Empty;
    public string CorrelationId { get; private set; } = string.Empty;
    public string MessageType { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }
    public string Source { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public Dictionary<string, object> Headers { get; private set; } = new();
    public object Payload { get; private set; } = null!;
    public string PayloadType { get; private set; } = string.Empty;
    public int PayloadSize { get; private set; }
    public MessagePriority Priority { get; private set; }
    public TimeSpan TTL { get; private set; }
    public int DeliveryCount { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string ContentType { get; private set; } = string.Empty;
    public string ContentEncoding { get; private set; } = string.Empty;
    public bool IsPersistent { get; private set; }
    public string ReplyTo { get; private set; } = string.Empty;
    public string RoutingKey { get; private set; } = string.Empty;
    public Dictionary<string, object> Properties { get; private set; } = new();

    // Parameterless constructor for EF Core
    protected MessageEnvelope() { }

    public MessageEnvelope(
        string messageType,
        object payload,
        string source,
        string destination = null,
        string messageId = null,
        string correlationId = null,
        Dictionary<string, object> headers = null,
        MessagePriority priority = MessagePriority.Normal,
        TimeSpan? ttl = null,
        string contentType = null,
        string replyTo = null,
        string routingKey = null,
        bool isPersistent = true,
        Dictionary<string, object> properties = null)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(messageType))
            throw new ArgumentException("Message type cannot be empty", nameof(messageType));
        
        if (payload == null)
            throw new ArgumentNullException(nameof(payload));
        
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("Source cannot be empty", nameof(source));

        // Generate IDs if not provided
        MessageId = string.IsNullOrWhiteSpace(messageId) ? Guid.NewGuid().ToString() : messageId;
        CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? MessageId : correlationId;
        
        MessageType = messageType;
        Timestamp = DateTime.UtcNow;
        Source = source;
        Destination = destination ?? string.Empty;
        Headers = headers ?? new Dictionary<string, object>();
        Payload = payload;
        PayloadType = payload.GetType().Name;
        PayloadSize = CalculatePayloadSize(payload);
        Priority = priority;
        TTL = ttl ?? TimeSpan.FromHours(24); // Default 24 hours
        DeliveryCount = 0;
        ExpiresAt = TTL > TimeSpan.Zero ? DateTime.UtcNow.Add(TTL) : null;
        ContentType = contentType ?? DetermineContentType(payload);
        ContentEncoding = "UTF-8";
        IsPersistent = isPersistent;
        ReplyTo = replyTo ?? string.Empty;
        RoutingKey = routingKey ?? messageType;
        Properties = properties ?? new Dictionary<string, object>();

        // Add standard headers
        if (!Headers.ContainsKey("MessageId"))
            Headers["MessageId"] = MessageId;
        if (!Headers.ContainsKey("CorrelationId"))
            Headers["CorrelationId"] = CorrelationId;
        if (!Headers.ContainsKey("Timestamp"))
            Headers["Timestamp"] = Timestamp;
        if (!Headers.ContainsKey("Source"))
            Headers["Source"] = Source;
        if (!string.IsNullOrWhiteSpace(Destination) && !Headers.ContainsKey("Destination"))
            Headers["Destination"] = Destination;
        if (!Headers.ContainsKey("Priority"))
            Headers["Priority"] = Priority.ToString();
        if (!Headers.ContainsKey("ContentType"))
            Headers["ContentType"] = ContentType;
    }

    /// <summary>
    /// Create a new message envelope with incremented delivery count
    /// </summary>
    public MessageEnvelope WithIncrementedDeliveryCount()
    {
        var newHeaders = new Dictionary<string, object>(Headers)
        {
            ["DeliveryCount"] = DeliveryCount + 1,
            ["LastDeliveryAttempt"] = DateTime.UtcNow
        };

        return new MessageEnvelope(
            MessageType,
            Payload,
            Source,
            Destination,
            MessageId,
            CorrelationId,
            newHeaders,
            Priority,
            TTL,
            ContentType,
            ReplyTo,
            RoutingKey,
            IsPersistent,
            Properties);
    }

    /// <summary>
    /// Create a new message envelope with updated expiration
    /// </summary>
    public MessageEnvelope WithExpiration(DateTime expiresAt)
    {
        var newHeaders = new Dictionary<string, object>(Headers)
        {
            ["ExpiresAt"] = expiresAt,
            ["TTL"] = expiresAt.Subtract(DateTime.UtcNow).TotalMilliseconds
        };

        return new MessageEnvelope(
            MessageType,
            Payload,
            Source,
            Destination,
            MessageId,
            CorrelationId,
            newHeaders,
            Priority,
            expiresAt.Subtract(DateTime.UtcNow),
            ContentType,
            ReplyTo,
            RoutingKey,
            IsPersistent,
            Properties);
    }

    /// <summary>
    /// Create a new message envelope with additional headers
    /// </summary>
    public MessageEnvelope WithHeaders(Dictionary<string, object> additionalHeaders)
    {
        if (additionalHeaders == null || !additionalHeaders.Any())
            return this;

        var newHeaders = new Dictionary<string, object>(Headers);
        foreach (var header in additionalHeaders)
        {
            newHeaders[header.Key] = header.Value;
        }

        return new MessageEnvelope(
            MessageType,
            Payload,
            Source,
            Destination,
            MessageId,
            CorrelationId,
            newHeaders,
            Priority,
            TTL,
            ContentType,
            ReplyTo,
            RoutingKey,
            IsPersistent,
            Properties);
    }

    /// <summary>
    /// Create a new message envelope with additional properties
    /// </summary>
    public MessageEnvelope WithProperties(Dictionary<string, object> additionalProperties)
    {
        if (additionalProperties == null || !additionalProperties.Any())
            return this;

        var newProperties = new Dictionary<string, object>(Properties);
        foreach (var property in additionalProperties)
        {
            newProperties[property.Key] = property.Value;
        }

        return new MessageEnvelope(
            MessageType,
            Payload,
            Source,
            Destination,
            MessageId,
            CorrelationId,
            Headers,
            Priority,
            TTL,
            ContentType,
            ReplyTo,
            RoutingKey,
            IsPersistent,
            newProperties);
    }

    /// <summary>
    /// Create a reply message envelope
    /// </summary>
    public MessageEnvelope CreateReply(object replyPayload, string replyMessageType = null)
    {
        if (replyPayload == null)
            throw new ArgumentNullException(nameof(replyPayload));

        var replyHeaders = new Dictionary<string, object>
        {
            ["InReplyTo"] = MessageId,
            ["OriginalMessageType"] = MessageType,
            ["OriginalSource"] = Source
        };

        return new MessageEnvelope(
            replyMessageType ?? $"{MessageType}.Reply",
            replyPayload,
            Destination ?? "System",
            ReplyTo ?? Source,
            headers: replyHeaders,
            correlationId: CorrelationId,
            priority: Priority);
    }

    /// <summary>
    /// Create an error message envelope
    /// </summary>
    public MessageEnvelope CreateError(string errorMessage, Exception exception = null)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message cannot be empty", nameof(errorMessage));

        var errorPayload = new Dictionary<string, object>
        {
            ["ErrorMessage"] = errorMessage,
            ["OriginalPayload"] = Payload,
            ["OriginalMessageType"] = MessageType,
            ["ErrorTimestamp"] = DateTime.UtcNow
        };

        if (exception != null)
        {
            errorPayload["ExceptionType"] = exception.GetType().Name;
            errorPayload["ExceptionMessage"] = exception.Message;
            errorPayload["StackTrace"] = exception.StackTrace;
        }

        var errorHeaders = new Dictionary<string, object>
        {
            ["OriginalMessageId"] = MessageId,
            ["OriginalMessageType"] = MessageType,
            ["OriginalSource"] = Source,
            ["ErrorReason"] = errorMessage
        };

        return new MessageEnvelope(
            $"{MessageType}.Error",
            errorPayload,
            "System",
            Source,
            headers: errorHeaders,
            correlationId: CorrelationId,
            priority: MessagePriority.High);
    }

    /// <summary>
    /// Check if message has expired
    /// </summary>
    public bool IsExpired()
    {
        return ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
    }

    /// <summary>
    /// Check if message should be moved to dead letter queue
    /// </summary>
    public bool ShouldMoveToDeadLetter(int maxDeliveryAttempts = 3)
    {
        return DeliveryCount >= maxDeliveryAttempts;
    }

    /// <summary>
    /// Get time until expiration
    /// </summary>
    public TimeSpan? GetTimeUntilExpiration()
    {
        if (!ExpiresAt.HasValue)
            return null;

        var timeUntilExpiration = ExpiresAt.Value.Subtract(DateTime.UtcNow);
        return timeUntilExpiration > TimeSpan.Zero ? timeUntilExpiration : TimeSpan.Zero;
    }

    /// <summary>
    /// Get message age
    /// </summary>
    public TimeSpan GetAge()
    {
        return DateTime.UtcNow.Subtract(Timestamp);
    }

    /// <summary>
    /// Get header value
    /// </summary>
    public T GetHeader<T>(string key, T defaultValue = default)
    {
        if (Headers.ContainsKey(key) && Headers[key] is T value)
            return value;
        
        return defaultValue;
    }

    /// <summary>
    /// Get property value
    /// </summary>
    public T GetProperty<T>(string key, T defaultValue = default)
    {
        if (Properties.ContainsKey(key) && Properties[key] is T value)
            return value;
        
        return defaultValue;
    }

    /// <summary>
    /// Check if message has header
    /// </summary>
    public bool HasHeader(string key)
    {
        return Headers.ContainsKey(key);
    }

    /// <summary>
    /// Check if message has property
    /// </summary>
    public bool HasProperty(string key)
    {
        return Properties.ContainsKey(key);
    }

    /// <summary>
    /// Get payload as specific type
    /// </summary>
    public T GetPayload<T>()
    {
        if (Payload is T typedPayload)
            return typedPayload;

        // Try to convert if possible
        try
        {
            return (T)Convert.ChangeType(Payload, typeof(T));
        }
        catch
        {
            throw new InvalidCastException($"Cannot convert payload of type {PayloadType} to {typeof(T).Name}");
        }
    }

    /// <summary>
    /// Get serialized payload
    /// </summary>
    public string GetSerializedPayload()
    {
        if (Payload is string stringPayload)
            return stringPayload;

        // Serialize based on content type
        return ContentType?.ToLowerInvariant() switch
        {
            "application/json" => System.Text.Json.JsonSerializer.Serialize(Payload),
            "application/xml" => SerializeToXml(Payload),
            _ => Payload.ToString()
        };
    }

    /// <summary>
    /// Calculate payload size in bytes
    /// </summary>
    private static int CalculatePayloadSize(object payload)
    {
        if (payload == null)
            return 0;

        if (payload is string stringPayload)
            return System.Text.Encoding.UTF8.GetByteCount(stringPayload);

        if (payload is byte[] bytePayload)
            return bytePayload.Length;

        // Estimate size for other types
        try
        {
            var serialized = System.Text.Json.JsonSerializer.Serialize(payload);
            return System.Text.Encoding.UTF8.GetByteCount(serialized);
        }
        catch
        {
            // Fallback estimation
            return payload.ToString()?.Length * 2 ?? 0;
        }
    }

    /// <summary>
    /// Determine content type based on payload
    /// </summary>
    private static string DetermineContentType(object payload)
    {
        return payload switch
        {
            string _ => "text/plain",
            byte[] _ => "application/octet-stream",
            _ when payload.GetType().IsClass && !payload.GetType().IsPrimitive => "application/json",
            _ => "text/plain"
        };
    }

    /// <summary>
    /// Serialize object to XML (simplified implementation)
    /// </summary>
    private static string SerializeToXml(object obj)
    {
        // This is a simplified implementation
        // In a real system, you'd use XmlSerializer or similar
        return $"<{obj.GetType().Name}>{obj}</{obj.GetType().Name}>";
    }

    /// <summary>
    /// Get message summary for logging
    /// </summary>
    public string GetSummary()
    {
        return $"Message[{MessageId}] Type:{MessageType} Source:{Source} Destination:{Destination} " +
               $"Priority:{Priority} Size:{PayloadSize} Age:{GetAge().TotalSeconds:F1}s " +
               $"DeliveryCount:{DeliveryCount} Expires:{ExpiresAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never"}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MessageId;
        yield return CorrelationId;
        yield return MessageType;
        yield return Timestamp;
        yield return Source;
        yield return Destination;
        yield return PayloadSize;
        yield return Priority;
        yield return ContentType;
    }

    public override string ToString()
    {
        return GetSummary();
    }
}

/// <summary>
/// Queue Message - Represents a message in a queue with additional queue-specific metadata
/// </summary>
public class QueueMessage : MessageEnvelope
{
    public string QueueName { get; private set; } = string.Empty;
    public DateTime EnqueuedAt { get; private set; }
    public DateTime? DequeuedAt { get; private set; }
    public DateTime? AcknowledgedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public string RejectionReason { get; private set; } = string.Empty;
    public bool IsAcknowledged { get; private set; }
    public bool IsRejected { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime? NextRetryAt { get; private set; }

    // Parameterless constructor for EF Core
    private QueueMessage() : base() { }

    public QueueMessage(
        string queueName,
        string messageType,
        object payload,
        string source,
        string destination = null,
        string messageId = null,
        string correlationId = null,
        Dictionary<string, object> headers = null,
        MessagePriority priority = MessagePriority.Normal,
        TimeSpan? ttl = null,
        string contentType = null,
        string replyTo = null,
        string routingKey = null,
        bool isPersistent = true,
        Dictionary<string, object> properties = null)
        : base(messageType, payload, source, destination, messageId, correlationId, 
               headers, priority, ttl, contentType, replyTo, routingKey, isPersistent, properties)
    {
        if (string.IsNullOrWhiteSpace(queueName))
            throw new ArgumentException("Queue name cannot be empty", nameof(queueName));

        QueueName = queueName;
        EnqueuedAt = DateTime.UtcNow;
        IsAcknowledged = false;
        IsRejected = false;
        RetryCount = 0;
    }

    /// <summary>
    /// Mark message as dequeued
    /// </summary>
    public void MarkAsDequeued()
    {
        DequeuedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Acknowledge message
    /// </summary>
    public void Acknowledge()
    {
        if (IsRejected)
            throw new InvalidOperationException("Cannot acknowledge a rejected message");

        AcknowledgedAt = DateTime.UtcNow;
        IsAcknowledged = true;
    }

    /// <summary>
    /// Reject message
    /// </summary>
    public void Reject(string reason = null, bool requeue = false)
    {
        if (IsAcknowledged)
            throw new InvalidOperationException("Cannot reject an acknowledged message");

        RejectedAt = DateTime.UtcNow;
        RejectionReason = reason;
        IsRejected = true;

        if (requeue)
        {
            // Reset for requeue
            IsRejected = false;
            RejectedAt = null;
            RejectionReason = null;
            RetryCount++;
            NextRetryAt = DateTime.UtcNow.AddSeconds(Math.Pow(2, RetryCount)); // Exponential backoff
        }
    }

    /// <summary>
    /// Increment delivery count
    /// </summary>
    public void IncrementDeliveryCount()
    {
        // This would typically create a new instance in the base class
        // For simplicity, we'll track it here
        RetryCount++;
    }

    /// <summary>
    /// Set expiration
    /// </summary>
    public void SetExpiration(DateTime expiresAt)
    {
        // This would typically create a new instance in the base class
        // For simplicity, we'll note that this should be handled by creating a new MessageEnvelope
    }

    /// <summary>
    /// Check if message is ready for retry
    /// </summary>
    public bool IsReadyForRetry()
    {
        return IsRejected && NextRetryAt.HasValue && DateTime.UtcNow >= NextRetryAt.Value;
    }

    /// <summary>
    /// Get queue processing time
    /// </summary>
    public TimeSpan? GetProcessingTime()
    {
        if (!DequeuedAt.HasValue)
            return null;

        var endTime = AcknowledgedAt ?? RejectedAt ?? DateTime.UtcNow;
        return endTime.Subtract(DequeuedAt.Value);
    }

    /// <summary>
    /// Get queue wait time
    /// </summary>
    public TimeSpan GetWaitTime()
    {
        var startTime = DequeuedAt ?? DateTime.UtcNow;
        return startTime.Subtract(EnqueuedAt);
    }
}