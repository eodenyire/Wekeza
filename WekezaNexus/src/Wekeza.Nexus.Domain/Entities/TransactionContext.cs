using Wekeza.Nexus.Domain.ValueObjects;

namespace Wekeza.Nexus.Domain.Entities;

/// <summary>
/// Complete context for a transaction being evaluated for fraud
/// This is the "Identity Graph" node that Wekeza Nexus analyzes
/// Combines transaction data, user context, device info, and behavioral signals
/// </summary>
public class TransactionContext
{
    /// <summary>
    /// Unique identifier for this transaction context
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();
    
    /// <summary>
    /// User/Customer ID initiating the transaction
    /// </summary>
    public Guid UserId { get; init; }
    
    /// <summary>
    /// Source account number
    /// </summary>
    public string FromAccountNumber { get; init; } = string.Empty;
    
    /// <summary>
    /// Destination account number
    /// </summary>
    public string ToAccountNumber { get; init; } = string.Empty;
    
    /// <summary>
    /// Transaction amount
    /// </summary>
    public decimal Amount { get; init; }
    
    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; init; } = string.Empty;
    
    /// <summary>
    /// Transaction type (Transfer, Payment, Withdrawal, etc.)
    /// </summary>
    public string TransactionType { get; init; } = string.Empty;
    
    /// <summary>
    /// Transaction description/narration
    /// </summary>
    public string Description { get; init; } = string.Empty;
    
    /// <summary>
    /// Device fingerprint data
    /// </summary>
    public DeviceFingerprint? DeviceInfo { get; init; }
    
    /// <summary>
    /// Behavioral biometric data
    /// </summary>
    public BehavioralMetrics? BehavioralData { get; init; }
    
    /// <summary>
    /// Transaction timestamp
    /// </summary>
    public DateTime TransactionTime { get; init; } = DateTime.UtcNow;
    
    /// <summary>
    /// Time of day classification (Night transactions are riskier)
    /// </summary>
    public string TimeOfDay { get; init; } = string.Empty;
    
    /// <summary>
    /// Whether this is the first transaction to this beneficiary
    /// </summary>
    public bool IsFirstTimeBeneficiary { get; init; }
    
    /// <summary>
    /// Age of the beneficiary account in days
    /// New accounts (<7 days) are higher risk for mule accounts
    /// </summary>
    public int? BeneficiaryAccountAgeDays { get; init; }
    
    /// <summary>
    /// Number of transactions in the last 10 minutes
    /// </summary>
    public int RecentTransactionCount { get; init; }
    
    /// <summary>
    /// Total amount transacted in the last 10 minutes
    /// </summary>
    public decimal RecentTransactionAmount { get; init; }
    
    /// <summary>
    /// Number of transactions in the last 24 hours
    /// </summary>
    public int DailyTransactionCount { get; init; }
    
    /// <summary>
    /// Total amount transacted in the last 24 hours
    /// </summary>
    public decimal DailyTransactionAmount { get; init; }
    
    /// <summary>
    /// User's average transaction amount (historical baseline)
    /// </summary>
    public decimal AverageTransactionAmount { get; init; }
    
    /// <summary>
    /// Percentage deviation from average amount
    /// </summary>
    public double AmountDeviationPercent { get; init; }
    
    /// <summary>
    /// Additional metadata/context
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();
    
    /// <summary>
    /// Session ID for tracking user session
    /// </summary>
    public string SessionId { get; init; } = string.Empty;
    
    /// <summary>
    /// Channel through which transaction was initiated
    /// </summary>
    public string Channel { get; init; } = string.Empty; // Web, Mobile, ATM, etc.
}
