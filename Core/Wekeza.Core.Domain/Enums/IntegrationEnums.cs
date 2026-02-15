namespace Wekeza.Core.Domain.Enums;

/// <summary>
/// Endpoint Type - Types of integration endpoints
/// </summary>
public enum EndpointType
{
    REST,
    SOAP,
    GraphQL,
    gRPC,
    WebSocket,
    FTP,
    SFTP,
    Database,
    MessageQueue,
    File,
    Email,
    SMS,
    Custom
}

/// <summary>
/// Endpoint Protocol - Communication protocols
/// </summary>
public enum EndpointProtocol
{
    HTTP,
    HTTPS,
    TCP,
    UDP,
    AMQP,
    MQTT,
    WebSocket,
    FTP,
    SFTP,
    SMTP,
    POP3,
    IMAP,
    Custom
}

/// <summary>
/// Endpoint Status - Operational status of endpoints
/// </summary>
public enum EndpointStatus
{
    Active,
    Inactive,
    Maintenance,
    Error,
    Suspended,
    Testing
}

/// <summary>
/// Health Status - Health status of endpoints and services
/// </summary>
public enum HealthStatus
{
    Healthy,
    Degraded,
    Unhealthy,
    Unknown,
    Maintenance
}

/// <summary>
/// Authentication Method - Authentication methods for endpoints
/// </summary>
public enum AuthenticationMethod
{
    None,
    BasicAuth,
    BearerToken,
    ApiKey,
    OAuth2,
    JWT,
    Certificate,
    HMAC,
    SAML,
    Kerberos,
    Custom
}

/// <summary>
/// Circuit Breaker State - States of circuit breaker pattern
/// </summary>
public enum CircuitBreakerState
{
    Closed,
    Open,
    HalfOpen
}

/// <summary>
/// Load Balancing Strategy - Load balancing algorithms
/// </summary>
/// <summary>
/// Queue Type - Types of message queues
/// </summary>
public enum QueueType
{
    Standard,
    Priority,
    Delayed,
    DeadLetter,
    Broadcast,
    Topic,
    Direct,
    Fanout,
    Headers,
    RPC
}

/// <summary>
/// Queue Status - Operational status of queues
/// </summary>
public enum QueueStatus
{
    Active,
    Inactive,
    Paused,
    Draining,
    Error,
    Full
}

/// <summary>
/// Queue Health Status - Health status of queues
/// </summary>
public enum QueueHealthStatus
{
    Healthy,
    Degraded,
    Overloaded,
    Inactive,
    Error
}

/// <summary>
/// Exchange Type - Message exchange types
/// </summary>
public enum ExchangeType
{
    Direct,
    Topic,
    Fanout,
    Headers,
    Delayed,
    Custom
}

/// <summary>
/// Message Priority - Priority levels for messages
/// </summary>
public enum MessagePriority
{
    Low = 1,
    Normal = 5,
    High = 8,
    Critical = 10
}

/// <summary>
/// Priority Strategy - Strategies for handling priority queues
/// </summary>
public enum PriorityStrategy
{
    HighestFirst,
    WeightedRoundRobin,
    StrictPriority,
    Custom
}

/// <summary>
/// Consumer Strategy - Strategies for message consumption
/// </summary>
public enum ConsumerStrategy
{
    RoundRobin,
    Random,
    Sticky,
    LoadBalanced,
    Custom
}

/// <summary>
/// Webhook Status - Status of webhook subscriptions
/// </summary>
public enum WebhookStatus
{
    Active,
    Inactive,
    Suspended,
    Testing,
    Error
}

/// <summary>
/// Webhook Health Status - Health status of webhook subscriptions
/// </summary>
public enum WebhookHealthStatus
{
    Healthy,
    Degraded,
    Failing,
    Suspended,
    Error
}

/// <summary>
/// Webhook Format - Format for webhook payloads
/// </summary>
public enum WebhookFormat
{
    JSON,
    XML,
    FormData,
    Custom
}

/// <summary>
/// Signature Method - Methods for webhook signature verification
/// </summary>
public enum SignatureMethod
{
    None,
    HMACSHA256,
    HMACSHA512,
    RSA,
    ECDSA,
    Custom
}

/// <summary>
/// Retry Strategy - Strategies for retry mechanisms
/// </summary>
public enum RetryStrategy
{
    FixedDelay,
    ExponentialBackoff,
    LinearBackoff,
    Custom
}

/// <summary>
/// Data Transformation Type - Types of data transformations
/// </summary>
public enum TransformationType
{
    JSONToXML,
    XMLToJSON,
    CSVToJSON,
    JSONToCSV,
    FixedWidthToJSON,
    JSONToFixedWidth,
    XMLToXML,
    JSONToJSON,
    Custom,
    Passthrough
}

/// <summary>
/// Transformation Status - Status of transformations
/// </summary>
public enum TransformationStatus
{
    Active,
    Inactive,
    Testing,
    Error,
    Deprecated
}

/// <summary>
/// Data Format - Data formats for transformation
/// </summary>
public enum DataFormat
{
    JSON,
    XML,
    CSV,
    FixedWidth,
    EDI,
    SWIFT,
    ISO20022,
    Custom,
    Binary
}

/// <summary>
/// Integration Pattern - Integration patterns
/// </summary>
public enum IntegrationPattern
{
    RequestResponse,
    PublishSubscribe,
    MessageQueue,
    EventDriven,
    Batch,
    Streaming,
    RPC,
    REST,
    GraphQL,
    Custom
}

/// <summary>
/// Message Status - Status of individual messages
/// </summary>
public enum MessageStatus
{
    Pending,
    Processing,
    Processed,
    Failed,
    Expired,
    DeadLetter,
    Acknowledged,
    Rejected
}

/// <summary>
/// Delivery Status - Status of message/webhook deliveries
/// </summary>
public enum DeliveryStatus
{
    Pending,
    InProgress,
    Delivered,
    Failed,
    Retrying,
    Expired,
    Cancelled
}

/// <summary>
/// API Gateway Feature - Features of API gateway
/// </summary>
public enum ApiGatewayFeature
{
    Routing,
    LoadBalancing,
    Authentication,
    Authorization,
    RateLimiting,
    Caching,
    Transformation,
    Monitoring,
    Logging,
    CircuitBreaker,
    Retry,
    Timeout
}

// Note: RateLimitType, CacheStrategy, MonitoringLevel, LogLevel, SecurityLevel, and CompressionType 
// are now defined in CommonEnums.cs to avoid duplicates

/// <summary>
/// Serialization Format - Serialization formats
/// </summary>
public enum SerializationFormat
{
    JSON,
    XML,
    MessagePack,
    ProtocolBuffers,
    Avro,
    Thrift,
    Custom
}

/// <summary>
/// Connection Pool Status - Status of connection pools
/// </summary>
public enum ConnectionPoolStatus
{
    Active,
    Inactive,
    Draining,
    Full,
    Error
}

/// <summary>
/// Failover Strategy - Strategies for failover
/// </summary>
public enum FailoverStrategy
{
    None,
    Automatic,
    Manual,
    Hybrid,
    Custom
}

/// <summary>
/// Backup Strategy - Strategies for backup
/// </summary>
public enum BackupStrategy
{
    None,
    Primary,
    Secondary,
    LoadBalanced,
    Custom
}

/// <summary>
/// Throttling Strategy - Strategies for throttling
/// </summary>
public enum ThrottlingStrategy
{
    None,
    FixedWindow,
    SlidingWindow,
    TokenBucket,
    LeakyBucket,
    Custom
}

/// <summary>
/// Validation Level - Levels of validation
/// </summary>
public enum ValidationLevel
{
    None,
    Basic,
    Schema,
    Business,
    Strict,
    Custom
}

/// <summary>
/// Error Handling Strategy - Strategies for error handling
/// </summary>
public enum ErrorHandlingStrategy
{
    Fail,
    Retry,
    Ignore,
    Fallback,
    Circuit,
    Custom
}

/// <summary>
/// Transaction Isolation Level - Isolation levels for transactions
/// </summary>
public enum TransactionIsolationLevel
{
    ReadUncommitted,
    ReadCommitted,
    RepeatableRead,
    Serializable,
    Snapshot
}

/// <summary>
/// Consistency Level - Consistency levels for distributed systems
/// </summary>
public enum ConsistencyLevel
{
    Eventual,
    Strong,
    Weak,
    Session,
    BoundedStaleness,
    Custom
}

/// <summary>
/// Partition Strategy - Strategies for data partitioning
/// </summary>
public enum PartitionStrategy
{
    None,
    Hash,
    Range,
    List,
    Round,
    Custom
}

/// <summary>
/// Replication Strategy - Strategies for data replication
/// </summary>
public enum ReplicationStrategy
{
    None,
    Master,
    MultiMaster,
    Peer,
    Chain,
    Custom
}

/// <summary>
/// Sync Mode - Synchronization modes
/// </summary>
public enum SyncMode
{
    Synchronous,
    Asynchronous,
    Hybrid,
    Custom
}

/// <summary>
/// Batch Mode - Batch processing modes
/// </summary>
public enum BatchMode
{
    None,
    Size,
    Time,
    SizeOrTime,
    Custom
}

/// <summary>
/// Stream Mode - Stream processing modes
/// </summary>
public enum StreamMode
{
    None,
    RealTime,
    NearRealTime,
    Batch,
    Micro,
    Custom
}
