using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Message Queue Aggregate - Manages asynchronous message processing and queuing
/// Supports standard, priority, delayed, dead letter, and topic-based queues
/// Industry Standard: Enterprise Message Broker patterns (RabbitMQ, Apache Kafka)
/// </summary>
public class MessageQueue : AggregateRoot
{
    // Core Properties
    public string QueueName { get; private set; }
    public string Description { get; private set; }
    public QueueType Type { get; private set; }
    public QueueStatus Status { get; private set; }
    
    // Configuration
    public int MaxMessageSize { get; private set; }
    public int MaxQueueSize { get; private set; }
    public TimeSpan MessageTTL { get; private set; }
    public bool IsDurable { get; private set; }
    public bool IsExclusive { get; private set; }
    public bool AutoDelete { get; private set; }
    public bool AutoAcknowledge { get; private set; }
    
    // Routing & Exchange
    public string ExchangeName { get; private set; }
    public string RoutingKey { get; private set; }
    public ExchangeType ExchangeType { get; private set; }
    public Dictionary<string, object> Arguments { get; private set; }
    
    // Statistics
    public int MessageCount { get; private set; }
    public int ConsumerCount { get; private set; }
    public DateTime? LastMessageAt { get; private set; }
    public long TotalMessagesEnqueued { get; private set; }
    public long TotalMessagesDequeued { get; private set; }
    public long TotalMessagesProcessed { get; private set; }
    public long TotalMessagesFailed { get; private set; }
    public long TotalMessagesExpired { get; private set; }
    
    // Dead Letter Queue
    public string DeadLetterQueue { get; private set; }
    public string DeadLetterExchange { get; private set; }
    public int MaxDeliveryAttempts { get; private set; }
    public TimeSpan DeadLetterTTL { get; private set; }
    
    // Priority Queue Settings
    public int MaxPriority { get; private set; }
    public PriorityStrategy PriorityStrategy { get; private set; }
    
    // Performance Metrics
    public TimeSpan AverageProcessingTime { get; private set; }
    public int MessagesPerSecond { get; private set; }
    public DateTime LastPerformanceUpdate { get; private set; }
    
    // Consumer Management
    public List<QueueConsumer> Consumers { get; private set; }
    public int MaxConsumers { get; private set; }
    public ConsumerStrategy ConsumerStrategy { get; private set; }
    
    // Message Storage
    public List<QueueMessage> Messages { get; private set; }
    public Dictionary<MessagePriority, Queue<QueueMessage>> PriorityQueues { get; private set; }
    
    // Metadata
    public Dictionary<string, object> Metadata { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string LastModifiedBy { get; private set; }

    // Private constructor for EF Core
    private MessageQueue() 
    {
        Arguments = new Dictionary<string, object>();
        Consumers = new List<QueueConsumer>();
        Messages = new List<QueueMessage>();
        PriorityQueues = new Dictionary<MessagePriority, Queue<QueueMessage>>();
        Metadata = new Dictionary<string, object>();
        
        // Initialize priority queues
        foreach (MessagePriority priority in Enum.GetValues<MessagePriority>())
        {
            PriorityQueues[priority] = new Queue<QueueMessage>();
        }
    }

    // Factory method for creating new message queue
    public static MessageQueue Create(
        string queueName,
        QueueType type,
        string createdBy,
        string description = null,
        QueueConfiguration configuration = null)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(queueName))
            throw new ArgumentException("Queue name cannot be empty", nameof(queueName));
        
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("Created by cannot be empty", nameof(createdBy));

        // Validate queue name format (alphanumeric, dots, hyphens, underscores)
        if (!System.Text.RegularExpressions.Regex.IsMatch(queueName, @"^[a-zA-Z0-9._-]+$"))
            throw new ArgumentException("Queue name contains invalid characters", nameof(queueName));

        var config = configuration ?? new QueueConfiguration();

        var queue = new MessageQueue
        {
            Id = Guid.NewGuid(),
            QueueName = queueName,
            Description = description,
            Type = type,
            Status = QueueStatus.Active,
            MaxMessageSize = config.MaxMessageSize,
            MaxQueueSize = config.MaxQueueSize,
            MessageTTL = config.MessageTTL,
            IsDurable = config.IsDurable,
            IsExclusive = config.IsExclusive,
            AutoDelete = config.AutoDelete,
            AutoAcknowledge = config.AutoAcknowledge,
            ExchangeName = config.ExchangeName ?? $"{queueName}.exchange",
            RoutingKey = config.RoutingKey ?? queueName,
            ExchangeType = config.ExchangeType,
            Arguments = config.Arguments ?? new Dictionary<string, object>(),
            MessageCount = 0,
            ConsumerCount = 0,
            TotalMessagesEnqueued = 0,
            TotalMessagesDequeued = 0,
            TotalMessagesProcessed = 0,
            TotalMessagesFailed = 0,
            TotalMessagesExpired = 0,
            DeadLetterQueue = config.DeadLetterQueue ?? $"{queueName}.dlq",
            DeadLetterExchange = config.DeadLetterExchange ?? $"{queueName}.dlx",
            MaxDeliveryAttempts = config.MaxDeliveryAttempts,
            DeadLetterTTL = config.DeadLetterTTL,
            MaxPriority = config.MaxPriority,
            PriorityStrategy = config.PriorityStrategy,
            AverageProcessingTime = TimeSpan.Zero,
            MessagesPerSecond = 0,
            LastPerformanceUpdate = DateTime.UtcNow,
            Consumers = new List<QueueConsumer>(),
            MaxConsumers = config.MaxConsumers,
            ConsumerStrategy = config.ConsumerStrategy,
            Messages = new List<QueueMessage>(),
            PriorityQueues = new Dictionary<MessagePriority, Queue<QueueMessage>>(),
            Metadata = new Dictionary<string, object>(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        // Initialize priority queues
        foreach (MessagePriority priority in Enum.GetValues<MessagePriority>())
        {
            queue.PriorityQueues[priority] = new Queue<QueueMessage>();
        }

        // Add creation event
        queue.AddDomainEvent(new MessageQueueCreatedDomainEvent(
            queue.Id,
            queue.QueueName,
            queue.Type,
            queue.CreatedBy));

        return queue;
    }

    // Enqueue message
    public void EnqueueMessage(QueueMessage message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        if (Status != QueueStatus.Active)
            throw new InvalidOperationException($"Cannot enqueue message to queue in status: {Status}");

        // Check queue size limit
        if (MaxQueueSize > 0 && MessageCount >= MaxQueueSize)
            throw new InvalidOperationException($"Queue has reached maximum size: {MaxQueueSize}");

        // Check message size limit
        if (MaxMessageSize > 0 && message.PayloadSize > MaxMessageSize)
            throw new InvalidOperationException($"Message size {message.PayloadSize} exceeds maximum: {MaxMessageSize}");

        // Set message expiration if TTL is configured
        if (MessageTTL > TimeSpan.Zero && !message.ExpiresAt.HasValue)
        {
            message.SetExpiration(DateTime.UtcNow.Add(MessageTTL));
        }

        // Add to appropriate queue based on type and priority
        if (Type == QueueType.Priority)
        {
            PriorityQueues[message.Priority].Enqueue(message);
        }
        else
        {
            Messages.Add(message);
        }

        // Update statistics
        MessageCount++;
        TotalMessagesEnqueued++;
        LastMessageAt = DateTime.UtcNow;

        // Update metadata
        Metadata["LastEnqueuedAt"] = DateTime.UtcNow;
        Metadata["LastEnqueuedMessageId"] = message.MessageId;

        AddDomainEvent(new MessageEnqueuedDomainEvent(
            Id,
            QueueName,
            message.MessageId,
            message.MessageType,
            message.Priority,
            DateTime.UtcNow));
    }

    // Dequeue message
    public QueueMessage DequeueMessage()
    {
        if (Status != QueueStatus.Active)
            throw new InvalidOperationException($"Cannot dequeue message from queue in status: {Status}");

        if (MessageCount == 0)
            return null;

        QueueMessage message = null;

        // Dequeue based on queue type
        if (Type == QueueType.Priority)
        {
            message = DequeueByPriority();
        }
        else
        {
            // Remove expired messages first
            RemoveExpiredMessages();
            
            if (Messages.Any())
            {
                message = Messages.First();
                Messages.RemoveAt(0);
            }
        }

        if (message != null)
        {
            // Check if message has expired
            if (message.IsExpired())
            {
                HandleExpiredMessage(message);
                return DequeueMessage(); // Try to get next message
            }

            // Update statistics
            MessageCount--;
            TotalMessagesDequeued++;
            message.IncrementDeliveryCount();

            // Update metadata
            Metadata["LastDequeuedAt"] = DateTime.UtcNow;
            Metadata["LastDequeuedMessageId"] = message.MessageId;

            AddDomainEvent(new MessageDequeuedDomainEvent(
                Id,
                QueueName,
                message.MessageId,
                message.MessageType,
                message.DeliveryCount,
                DateTime.UtcNow));
        }

        return message;
    }

    // Dequeue message by priority
    private QueueMessage DequeueByPriority()
    {
        // Dequeue from highest priority first
        var priorities = Enum.GetValues<MessagePriority>()
            .OrderByDescending(p => (int)p);

        foreach (var priority in priorities)
        {
            var queue = PriorityQueues[priority];
            
            // Remove expired messages from this priority queue
            RemoveExpiredMessagesFromPriorityQueue(priority);
            
            if (queue.Count > 0)
            {
                return queue.Dequeue();
            }
        }

        return null;
    }

    // Process message (mark as processed)
    public void ProcessMessage(string messageId, bool success, TimeSpan processingTime, string result = null)
    {
        if (string.IsNullOrWhiteSpace(messageId))
            throw new ArgumentException("Message ID cannot be empty", nameof(messageId));

        if (success)
        {
            TotalMessagesProcessed++;
            
            // Update average processing time
            if (TotalMessagesProcessed > 1)
            {
                var totalTime = AverageProcessingTime.TotalMilliseconds * (TotalMessagesProcessed - 1) + processingTime.TotalMilliseconds;
                AverageProcessingTime = TimeSpan.FromMilliseconds(totalTime / TotalMessagesProcessed);
            }
            else
            {
                AverageProcessingTime = processingTime;
            }

            AddDomainEvent(new MessageProcessedDomainEvent(
                Id,
                QueueName,
                messageId,
                processingTime,
                DateTime.UtcNow));
        }
        else
        {
            TotalMessagesFailed++;

            AddDomainEvent(new MessageProcessingFailedDomainEvent(
                Id,
                QueueName,
                messageId,
                result ?? "Processing failed",
                DateTime.UtcNow));
        }

        // Update performance metrics
        UpdatePerformanceMetrics();
    }

    // Move message to dead letter queue
    public void MoveToDeadLetter(QueueMessage message, string reason = null)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        // Remove from current queue
        if (Type == QueueType.Priority)
        {
            // Note: In a real implementation, we'd need to track which priority queue the message is in
            // For now, we'll just update the count
            MessageCount--;
        }
        else
        {
            Messages.Remove(message);
            MessageCount--;
        }

        // Update metadata
        Metadata["LastDeadLetterAt"] = DateTime.UtcNow;
        Metadata["LastDeadLetterMessageId"] = message.MessageId;
        Metadata["LastDeadLetterReason"] = reason ?? "Max delivery attempts exceeded";

        AddDomainEvent(new MessageMovedToDeadLetterDomainEvent(
            Id,
            QueueName,
            DeadLetterQueue,
            message.MessageId,
            reason ?? "Max delivery attempts exceeded",
            DateTime.UtcNow));
    }

    // Purge queue (remove all messages)
    public void PurgeQueue(string purgedBy = null)
    {
        var messageCount = MessageCount;
        
        Messages.Clear();
        foreach (var priorityQueue in PriorityQueues.Values)
        {
            priorityQueue.Clear();
        }
        
        MessageCount = 0;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = purgedBy;

        // Update metadata
        Metadata["PurgedAt"] = DateTime.UtcNow;
        Metadata["PurgedMessageCount"] = messageCount;
        if (!string.IsNullOrWhiteSpace(purgedBy))
            Metadata["PurgedBy"] = purgedBy;

        AddDomainEvent(new MessageQueuePurgedDomainEvent(
            Id,
            QueueName,
            messageCount,
            purgedBy,
            DateTime.UtcNow));
    }

    // Update queue configuration
    public void UpdateConfiguration(QueueConfiguration configuration, string modifiedBy)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new ArgumentException("Modified by cannot be empty", nameof(modifiedBy));

        MaxMessageSize = configuration.MaxMessageSize;
        MaxQueueSize = configuration.MaxQueueSize;
        MessageTTL = configuration.MessageTTL;
        MaxDeliveryAttempts = configuration.MaxDeliveryAttempts;
        MaxConsumers = configuration.MaxConsumers;
        
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Update metadata
        Metadata["ConfigurationUpdatedAt"] = DateTime.UtcNow;
        Metadata["ConfigurationUpdatedBy"] = modifiedBy;

        AddDomainEvent(new MessageQueueConfigurationUpdatedDomainEvent(
            Id,
            QueueName,
            modifiedBy));
    }

    // Add consumer
    public void AddConsumer(QueueConsumer consumer)
    {
        if (consumer == null)
            throw new ArgumentNullException(nameof(consumer));

        if (MaxConsumers > 0 && ConsumerCount >= MaxConsumers)
            throw new InvalidOperationException($"Queue has reached maximum consumers: {MaxConsumers}");

        if (Consumers.Any(c => c.ConsumerId == consumer.ConsumerId))
            throw new InvalidOperationException($"Consumer {consumer.ConsumerId} already exists");

        Consumers.Add(consumer);
        ConsumerCount++;

        // Update metadata
        Metadata["LastConsumerAddedAt"] = DateTime.UtcNow;
        Metadata["LastConsumerAddedId"] = consumer.ConsumerId;

        AddDomainEvent(new MessageQueueConsumerAddedDomainEvent(
            Id,
            QueueName,
            consumer.ConsumerId,
            consumer.ConsumerName,
            DateTime.UtcNow));
    }

    // Remove consumer
    public void RemoveConsumer(string consumerId)
    {
        if (string.IsNullOrWhiteSpace(consumerId))
            throw new ArgumentException("Consumer ID cannot be empty", nameof(consumerId));

        var consumer = Consumers.FirstOrDefault(c => c.ConsumerId == consumerId);
        if (consumer == null)
            throw new InvalidOperationException($"Consumer {consumerId} not found");

        Consumers.Remove(consumer);
        ConsumerCount--;

        // Update metadata
        Metadata["LastConsumerRemovedAt"] = DateTime.UtcNow;
        Metadata["LastConsumerRemovedId"] = consumerId;

        AddDomainEvent(new MessageQueueConsumerRemovedDomainEvent(
            Id,
            QueueName,
            consumerId,
            consumer.ConsumerName,
            DateTime.UtcNow));
    }

    // Activate queue
    public void Activate(string activatedBy = null)
    {
        if (Status == QueueStatus.Active)
            return;

        Status = QueueStatus.Active;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = activatedBy;

        // Update metadata
        Metadata["ActivatedAt"] = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(activatedBy))
            Metadata["ActivatedBy"] = activatedBy;

        AddDomainEvent(new MessageQueueActivatedDomainEvent(
            Id,
            QueueName,
            activatedBy));
    }

    // Deactivate queue
    public void Deactivate(string deactivatedBy = null, string reason = null)
    {
        if (Status == QueueStatus.Inactive)
            return;

        Status = QueueStatus.Inactive;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = deactivatedBy;

        // Update metadata
        Metadata["DeactivatedAt"] = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(deactivatedBy))
            Metadata["DeactivatedBy"] = deactivatedBy;
        if (!string.IsNullOrWhiteSpace(reason))
            Metadata["DeactivatedReason"] = reason;

        AddDomainEvent(new MessageQueueDeactivatedDomainEvent(
            Id,
            QueueName,
            deactivatedBy,
            reason));
    }

    // Remove expired messages
    private void RemoveExpiredMessages()
    {
        var now = DateTime.UtcNow;
        var expiredMessages = Messages.Where(m => m.IsExpired()).ToList();
        
        foreach (var expiredMessage in expiredMessages)
        {
            Messages.Remove(expiredMessage);
            MessageCount--;
            TotalMessagesExpired++;
            
            HandleExpiredMessage(expiredMessage);
        }
    }

    // Remove expired messages from priority queue
    private void RemoveExpiredMessagesFromPriorityQueue(MessagePriority priority)
    {
        var queue = PriorityQueues[priority];
        var tempMessages = new List<QueueMessage>();
        
        // Dequeue all messages and check expiration
        while (queue.Count > 0)
        {
            var message = queue.Dequeue();
            if (!message.IsExpired())
            {
                tempMessages.Add(message);
            }
            else
            {
                MessageCount--;
                TotalMessagesExpired++;
                HandleExpiredMessage(message);
            }
        }
        
        // Re-enqueue non-expired messages
        foreach (var message in tempMessages)
        {
            queue.Enqueue(message);
        }
    }

    // Handle expired message
    private void HandleExpiredMessage(QueueMessage message)
    {
        AddDomainEvent(new MessageExpiredDomainEvent(
            Id,
            QueueName,
            message.MessageId,
            message.MessageType,
            DateTime.UtcNow));
    }

    // Update performance metrics
    private void UpdatePerformanceMetrics()
    {
        var now = DateTime.UtcNow;
        var timeSinceLastUpdate = now.Subtract(LastPerformanceUpdate);
        
        if (timeSinceLastUpdate.TotalSeconds >= 1)
        {
            // Calculate messages per second
            var messagesProcessedSinceLastUpdate = TotalMessagesProcessed - (Metadata.ContainsKey("LastProcessedCount") 
                ? (long)Metadata["LastProcessedCount"] 
                : 0);
            
            MessagesPerSecond = (int)(messagesProcessedSinceLastUpdate / timeSinceLastUpdate.TotalSeconds);
            LastPerformanceUpdate = now;
            
            // Update metadata
            Metadata["LastProcessedCount"] = TotalMessagesProcessed;
            Metadata["PerformanceUpdatedAt"] = now;
        }
    }

    // Get queue health status
    public QueueHealthStatus GetHealthStatus()
    {
        if (Status != QueueStatus.Active)
            return QueueHealthStatus.Inactive;

        // Check if queue is overloaded
        if (MaxQueueSize > 0 && MessageCount >= MaxQueueSize * 0.9)
            return QueueHealthStatus.Overloaded;

        // Check if there are too many failed messages
        var totalMessages = TotalMessagesProcessed + TotalMessagesFailed;
        if (totalMessages > 100 && (double)TotalMessagesFailed / totalMessages > 0.1)
            return QueueHealthStatus.Degraded;

        return QueueHealthStatus.Healthy;
    }

    // Get queue statistics
    public Dictionary<string, object> GetStatistics()
    {
        return new Dictionary<string, object>
        {
            ["QueueName"] = QueueName,
            ["Type"] = Type.ToString(),
            ["Status"] = Status.ToString(),
            ["MessageCount"] = MessageCount,
            ["ConsumerCount"] = ConsumerCount,
            ["TotalEnqueued"] = TotalMessagesEnqueued,
            ["TotalDequeued"] = TotalMessagesDequeued,
            ["TotalProcessed"] = TotalMessagesProcessed,
            ["TotalFailed"] = TotalMessagesFailed,
            ["TotalExpired"] = TotalMessagesExpired,
            ["AverageProcessingTime"] = AverageProcessingTime.TotalMilliseconds,
            ["MessagesPerSecond"] = MessagesPerSecond,
            ["HealthStatus"] = GetHealthStatus().ToString(),
            ["LastMessageAt"] = LastMessageAt,
            ["CreatedAt"] = CreatedAt
        };
    }
}

/// <summary>
/// Queue Configuration - Configuration settings for message queues
/// </summary>
public class QueueConfiguration
{
    public int MaxMessageSize { get; set; } = 1024 * 1024; // 1MB
    public int MaxQueueSize { get; set; } = 10000;
    public TimeSpan MessageTTL { get; set; } = TimeSpan.FromHours(24);
    public bool IsDurable { get; set; } = true;
    public bool IsExclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
    public bool AutoAcknowledge { get; set; } = false;
    public string ExchangeName { get; set; }
    public string RoutingKey { get; set; }
    public ExchangeType ExchangeType { get; set; } = ExchangeType.Direct;
    public Dictionary<string, object> Arguments { get; set; } = new();
    public string DeadLetterQueue { get; set; }
    public string DeadLetterExchange { get; set; }
    public int MaxDeliveryAttempts { get; set; } = 3;
    public TimeSpan DeadLetterTTL { get; set; } = TimeSpan.FromDays(7);
    public int MaxPriority { get; set; } = 10;
    public PriorityStrategy PriorityStrategy { get; set; } = PriorityStrategy.HighestFirst;
    public int MaxConsumers { get; set; } = 10;
    public ConsumerStrategy ConsumerStrategy { get; set; } = ConsumerStrategy.RoundRobin;
}

/// <summary>
/// Queue Consumer - Represents a consumer of messages from the queue
/// </summary>
public class QueueConsumer
{
    public string ConsumerId { get; set; }
    public string ConsumerName { get; set; }
    public string ConsumerType { get; set; }
    public DateTime ConnectedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int MessagesProcessed { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}