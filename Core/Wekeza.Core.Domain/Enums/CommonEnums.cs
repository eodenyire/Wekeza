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
    Certificate = 5,
    HMAC = 6,
    BasicAuth = 7,
    BearerToken = 8,
    OAuth2 = 9,
    JWT = 10,
    Custom = 11,
    APIKey = 12
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
    Error = 4,
    Maintenance = 5
}

public enum AccessLevel
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 3,
    Delete = 4,
    Admin = 5,
    SuperAdmin = 6
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
    Administrative = 4,
    Functional = 5,
    Departmental = 6,
    Custom = 7,
    Temporary = 8
}

public enum RoleStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Deprecated = 4
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
    Error = 4,
    Active = 5,
    Inactive = 6
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
    Unknown = 4,
    Unhealthy = 5,
    Degraded = 6
}

public enum MonitorStatus
{
    Active = 1,
    Disabled = 2,
    Paused = 3,
    Error = 4,
    Maintenance = 5
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
    Product = 5,
    Party = 6
}

public enum AMLAlertType
{
    HighValue = 1,
    StructuredTransaction = 2,
    UnusualPattern = 3,
    SanctionsMatch = 4,
    PEPMatch = 5,
    StructuringActivity = 6,
    PEPActivity = 7,
    SuspiciousTransaction = 8,
    UnusualCashActivity = 9,
    RapidMovementOfFunds = 10,
    CrossBorderActivity = 11,
    ThresholdExceeded = 12,
    UnusualAccountActivity = 13,
    HighRiskCustomer = 14
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
    Failed = 4,
    Clear = 5,
    Blocked = 6,
    UnderReview = 7,
    Alert = 8,
    Escalated = 9,
    Whitelisted = 10
}

public enum ScreeningDecision
{
    Approve = 1,
    Reject = 2,
    Review = 3,
    Escalate = 4,
    FalsePositive = 5,
    TruePositive = 6,
    RequiresEscalation = 7,
    PendingInvestigation = 8
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

public enum UserStatus
{
    Active = 1,
    Inactive = 2,
    Locked = 3,
    Suspended = 4,
    PendingActivation = 5,
    Expired = 6,
    Disabled = 7
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

// Additional missing enums
public enum WorkflowStatus
{
    Draft = 1,
    Initiated = 2,
    InProgress = 3,
    Approved = 4,
    Rejected = 5,
    Cancelled = 6,
    Completed = 7,
    Expired = 8,
    Suspended = 9,
    Active = 10,
    Inactive = 11,
    Pending = 12,
    ValidationFailed = 13,
    Validated = 14,
    Archived = 15
}

public enum ApprovalStepStatus
{
    Pending = 1,
    Assigned = 2,
    Approved = 3,
    Rejected = 4,
    Skipped = 5,
    Expired = 6
}

public enum WorkflowCommentType
{
    General = 1,
    Approval = 2,
    Rejection = 3,
    Escalation = 4,
    Cancellation = 5,
    System = 6
}

public enum Priority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Urgent = 4,
    Critical = 5
}

public enum ReportStatus
{
    Draft = 1,
    Pending = 2,
    InProgress = 3,
    Completed = 4,
    Failed = 5,
    Cancelled = 6,
    Archived = 7,
    Generated = 8,
    Submitted = 9,
    Validated = 10,
    ValidationFailed = 11,
    Approved = 12,
    Rejected = 13,
    Acknowledged = 14
}

public enum InterestCalculationMethod
{
    Simple = 1,
    Compound = 2,
    ReducingBalance = 3,
    FlatRate = 4,
    AnnuityMethod = 5,
    SimpleInterest = 6,
    CompoundInterest = 7,
    CompoundQuarterly = 8,
    CompoundMonthly = 9,
    CompoundDaily = 10
}

public enum DepositStatus
{
    Active = 1,
    Inactive = 2,
    Matured = 3,
    Closed = 4,
    Suspended = 5,
    PendingClosure = 6,
    PrematurelyClosed = 7,
    Discontinued = 8
}

public enum KYCStatus
{
    Pending = 1,
    InProgress = 2,
    Verified = 3,
    Rejected = 4,
    Expired = 5,
    RequiresUpdate = 6,
    Completed = 7
}

public enum AccountStatus
{
    Active = 1,
    Inactive = 2,
    Frozen = 3,
    Closed = 4,
    Suspended = 5,
    PendingActivation = 6,
    PendingVerification = 7,
    Dormant = 8
}

public enum AMLRiskRating
{
    Low = 1,
    Medium = 2,
    High = 3,
    VeryHigh = 4,
    Prohibited = 5
}

/// <summary>
/// Channel types for digital banking
/// </summary>
public enum ChannelType
{
    InternetBanking = 1,
    MobileBanking = 2,
    ATM = 3,
    POS = 4,
    BranchTeller = 5,
    CallCenter = 6,
    SMS = 7,
    USSD = 8,
    API = 9,
    WebPortal = 10,
    MobileApp = 11,
    TabletApp = 12
}

/// <summary>
/// Loan status enumeration
/// </summary>
public enum LoanStatus
{
    Application = 1,
    Applied = 2,
    UnderReview = 3,
    Approved = 4,
    Rejected = 5,
    Disbursed = 6,
    Active = 7,
    Overdue = 8,
    WriteOff = 9,
    WrittenOff = 10,
    Closed = 11,
    Cancelled = 12,
    Suspended = 13,
    Restructured = 14,
    PaidInFull = 15
}

/// <summary>
/// FX Deal types
/// </summary>
public enum FXDealType
{
    Spot = 1,
    Forward = 2,
    Swap = 3,
    Option = 4,
    Future = 5
}

/// <summary>
/// Regulatory authorities
/// </summary>
public enum RegulatoryAuthority
{
    CentralBank = 1,
    BankingRegulator = 2,
    SecuritiesCommission = 3,
    InsuranceRegulator = 4,
    TaxAuthority = 5,
    AMLAuthority = 6,
    CBK = 7,    // Central Bank of Kenya
    KRA = 8,    // Kenya Revenue Authority
    CMA = 9,    // Capital Markets Authority
    IRA = 10,   // Insurance Regulatory Authority
    SASRA = 11, // Sacco Societies Regulatory Authority
    FRC = 12    // Financial Reporting Centre
}

/// <summary>
/// Report frequency
/// </summary>
public enum ReportFrequency
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Quarterly = 4,
    SemiAnnually = 5,
    Annually = 6,
    OnDemand = 7
}

/// <summary>
/// Workflow priority levels
/// </summary>
public enum WorkflowPriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Urgent = 4,
    Critical = 5
}

/// <summary>
/// Letter of Credit types
/// </summary>
public enum LCType
{
    Commercial = 1,
    Standby = 2,
    Revolving = 3,
    Transferable = 4,
    BackToBack = 5,
    Confirmed = 6,
    Unconfirmed = 7
}

/// <summary>
/// Loan sub-status enumeration
/// </summary>
public enum LoanSubStatus
{
    PendingDocuments = 1,
    CreditAssessed = 2,
    AwaitingDisbursement = 3,
    Current = 4,
    PastDue1to30 = 5,
    PastDue31to60 = 6,
    PastDue61to90 = 7,
    NonPerforming = 8,
    Restructured = 9,
    Closed = 10
}

