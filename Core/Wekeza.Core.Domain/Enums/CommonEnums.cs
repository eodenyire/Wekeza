namespace Wekeza.Core.Domain.Enums;

public enum RiskLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum AlertSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum AlertStatus
{
    Open = 1,
    InProgress = 2,
    Resolved = 3,
    Closed = 4
}

public enum AuthenticationType
{
    None = 0,
    Basic = 1,
    Bearer = 2,
    ApiKey = 3,
    OAuth = 4,
    Certificate = 5
}

public enum IntegrationType
{
    REST = 1,
    SOAP = 2,
    GraphQL = 3,
    gRPC = 4,
    WebSocket = 5,
    MessageQueue = 6
}

public enum IntegrationStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Error = 4
}

public enum AccessLevel
{
    None = 0,
    Read = 1,
    Write = 2,
    Admin = 3,
    SuperAdmin = 4
}

public enum SecurityLevel
{
    Public = 1,
    Standard = 2,
    Internal = 3,
    Confidential = 4,
    Secret = 5,
    TopSecret = 6
}

public enum SecurityClearanceLevel
{
    None = 0,
    Basic = 1,
    Standard = 2,
    Enhanced = 3,
    Maximum = 4
}

public enum RoleType
{
    System = 1,
    Business = 2,
    Technical = 3,
    Administrative = 4
}

public enum MfaMethod
{
    None = 0,
    SMS = 1,
    Email = 2,
    TOTP = 3,
    Hardware = 4,
    Biometric = 5
}

public enum GatewayStatus
{
    Online = 1,
    Offline = 2,
    Maintenance = 3,
    Error = 4
}

public enum ParameterType
{
    String = 1,
    Integer = 2,
    Decimal = 3,
    Boolean = 4,
    Date = 5,
    Json = 6
}

public enum MonitorType
{
    System = 1,
    Application = 2,
    Database = 3,
    Network = 4,
    Security = 5
}

public enum MonitorHealth
{
    Healthy = 1,
    Warning = 2,
    Critical = 3,
    Unknown = 4
}

public enum PolicyType
{
    Security = 1,
    Compliance = 2,
    Operational = 3,
    Business = 4
}

public enum TimeWindow
{
    Minute = 1,
    Hour = 2,
    Day = 3,
    Week = 4,
    Month = 5,
    Year = 6
}

public enum EntityType
{
    Customer = 1,
    Account = 2,
    Transaction = 3,
    User = 4,
    Product = 5
}

public enum AMLAlertType
{
    HighValue = 1,
    StructuredTransaction = 2,
    UnusualPattern = 3,
    SanctionsMatch = 4,
    PEPMatch = 5
}

public enum AMLResolution
{
    FalsePositive = 1,
    Escalated = 2,
    SARFiled = 3,
    Closed = 4
}

public enum ScreeningResult
{
    Clear = 1,
    Match = 2,
    PotentialMatch = 3,
    Error = 4
}

public enum ScreeningStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Failed = 4
}

public enum ScreeningDecision
{
    Approve = 1,
    Reject = 2,
    Review = 3,
    Escalate = 4
}

public enum MonitoringDecision
{
    Allow = 1,
    Block = 2,
    Review = 3,
    Escalate = 4
}

public enum CreditRiskGrade
{
    AAA = 1,
    AA = 2,
    A = 3,
    BBB = 4,
    BB = 5,
    B = 6,
    CCC = 7,
    CC = 8,
    C = 9,
    D = 10
}

public enum GuaranteeType
{
    Performance = 1,
    Payment = 2,
    Advance = 3,
    Warranty = 4,
    Customs = 5
}

public enum MoneyMarketDealType
{
    Deposit = 1,
    Loan = 2,
    Repo = 3,
    ReverseRepo = 4
}

public enum TradeType
{
    Buy = 1,
    Sell = 2
}

// Additional enums for API Gateway functionality
public enum RateLimitType
{
    PerSecond = 1,
    PerMinute = 2,
    PerHour = 3,
    PerDay = 4
}

public enum CacheStrategy
{
    TimeToLive = 1,
    LeastRecentlyUsed = 2,
    LeastFrequentlyUsed = 3,
    FirstInFirstOut = 4
}

public enum LoadBalancingStrategy
{
    RoundRobin = 1,
    WeightedRoundRobin = 2,
    LeastConnections = 3,
    IpHash = 4,
    Random = 5
}

public enum MonitoringLevel
{
    None = 0,
    Basic = 1,
    Standard = 2,
    Detailed = 3,
    Debug = 4
}

public enum LogLevel
{
    Trace = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    Critical = 5,
    None = 6
}

public enum CompressionType
{
    None = 0,
    Gzip = 1,
    Deflate = 2,
    Brotli = 3
}