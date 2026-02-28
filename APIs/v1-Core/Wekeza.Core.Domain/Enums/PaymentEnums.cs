namespace Wekeza.Core.Domain.Enums;

/// <summary>
/// Payment type classification
/// </summary>
public enum PaymentType
{
    InternalTransfer,       // Account to account within bank
    ExternalTransfer,       // To external bank account
    BulkPayment,           // Batch payment processing
    StandingOrder,         // Recurring payment
    MobileMoneyTransfer,   // Mobile money integration
    CardPayment,           // Card-based payment
    OnlinePayment,         // Internet banking payment
    ChequePayment,         // Cheque-based payment
    CashPayment,           // Cash transaction
    WireTransfer           // International wire transfer
}

/// <summary>
/// Payment processing channel
/// </summary>
public enum PaymentChannel
{
    Internal,              // Internal bank system
    Eft,                   // Electronic Funds Transfer
    Rtgs,                  // Real Time Gross Settlement
    Swift,                 // SWIFT network
    Ach,                   // Automated Clearing House
    MobileMoney,           // Mobile money platforms
    Card,                  // Card networks
    Online,                // Internet banking
    Atm,                   // ATM network
    Branch,                // Branch teller
    Correspondent,         // Correspondent banking
    Api                    // API integration
}

/// <summary>
/// Payment processing status
/// </summary>
public enum PaymentStatus
{
    Pending,               // Awaiting processing
    Authorized,            // Approved for processing
    Processing,            // Currently being processed
    Completed,             // Successfully completed
    Failed,                // Processing failed
    Cancelled,             // Cancelled by user/system
    Rejected,              // Rejected by system/bank
    OnHold,                // Temporarily held
    Returned,              // Returned by beneficiary bank
    Settled                // Final settlement completed
}

/// <summary>
/// Payment priority levels
/// </summary>
public enum PaymentPriority
{
    Low,                   // Standard processing
    Normal,                // Regular priority
    High,                  // Expedited processing
    Urgent,                // Immediate processing
    Emergency              // Critical/emergency payment
}

/// <summary>
/// Fee bearer designation
/// </summary>
public enum FeeBearer
{
    Sender,                // Sender pays all fees
    Receiver,              // Receiver pays all fees
    Shared,                // Fees split between parties
    Each                   // Each party pays their own fees
}

/// <summary>
/// Payment method classification
/// </summary>
public enum PaymentMethod
{
    AccountTransfer,       // Direct account transfer
    CreditCard,            // Credit card payment
    DebitCard,             // Debit card payment
    MobileMoney,           // Mobile money transfer
    BankDraft,             // Bank draft
    Cheque,                // Cheque payment
    Cash,                  // Cash payment
    WireTransfer,          // Wire transfer
    OnlineBanking,         // Internet banking
    StandingOrder,         // Recurring payment
    DirectDebit            // Direct debit
}

/// <summary>
/// Standing order frequency
/// </summary>
public enum StandingOrderFrequency
{
    Daily,                 // Every day
    Weekly,                // Every week
    Fortnightly,           // Every two weeks
    Monthly,               // Every month
    Quarterly,             // Every quarter
    SemiAnnually,          // Twice a year
    Annually,              // Once a year
    Custom                 // Custom schedule
}

/// <summary>
/// Standing order status
/// </summary>
public enum StandingOrderStatus
{
    Active,                // Currently active
    Suspended,             // Temporarily suspended
    Expired,               // Past end date
    Cancelled,             // Cancelled by user
    Failed,                // Failed execution
    Completed              // All payments completed
}

/// <summary>
/// Bulk payment status
/// </summary>
public enum BulkPaymentStatus
{
    Uploaded,              // File uploaded
    Validating,            // Validation in progress
    Validated,             // Validation completed
    Processing,            // Processing payments
    Completed,             // All payments processed
    PartiallyCompleted,    // Some payments failed
    Failed,                // Processing failed
    Cancelled              // Cancelled by user
}

/// <summary>
/// Exchange rate type
/// </summary>
public enum ExchangeRateType
{
    Spot,                  // Current market rate
    Forward,               // Future delivery rate
    Fixed,                 // Fixed rate agreement
    Floating,              // Variable rate
    Cross                  // Cross currency rate
}

/// <summary>
/// Payment validation result
/// </summary>
public enum PaymentValidationResult
{
    Valid,                 // Payment is valid
    InvalidAmount,         // Invalid amount
    InsufficientFunds,     // Not enough balance
    InvalidAccount,        // Account doesn't exist
    AccountFrozen,         // Account is frozen
    LimitExceeded,         // Transaction limit exceeded
    InvalidBeneficiary,    // Beneficiary details invalid
    DuplicatePayment,      // Duplicate payment detected
    InvalidCurrency,       // Currency not supported
    InvalidDate,           // Invalid value date
    SystemError            // System validation error
}

/// <summary>
/// Payment notification type
/// </summary>
public enum PaymentNotificationType
{
    Sms,                   // SMS notification
    Email,                 // Email notification
    Push,                  // Push notification
    InApp,                 // In-app notification
    None                   // No notification
}

/// <summary>
/// Payment settlement type
/// </summary>
public enum SettlementType
{
    Immediate,             // Real-time settlement
    SameDay,               // Same day settlement
    NextDay,               // Next business day
    Deferred,              // Deferred settlement
    Batch                  // Batch settlement
}

/// <summary>
/// Payment reversal reason
/// </summary>
public enum ReversalReason
{
    CustomerRequest,       // Customer requested reversal
    SystemError,           // System error occurred
    FraudSuspicion,        // Suspected fraud
    InsufficientFunds,     // Insufficient funds at settlement
    AccountClosed,         // Account closed
    InvalidBeneficiary,    // Beneficiary account invalid
    RegulatoryRequirement, // Regulatory compliance
    BankError,             // Bank processing error
    TechnicalFailure,      // Technical system failure
    Other                  // Other reason
}