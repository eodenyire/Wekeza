namespace Wekeza.Nexus.Domain.Entities;

/// <summary>
/// Represents a historical transaction record for velocity and pattern analysis
/// Stores essential transaction data needed for fraud detection
/// </summary>
public class TransactionRecord
{
    /// <summary>
    /// Unique identifier for this transaction record
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();
    
    /// <summary>
    /// User/Customer ID who initiated the transaction
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
    /// When the transaction occurred
    /// </summary>
    public DateTime TransactionTime { get; init; } = DateTime.UtcNow;
    
    /// <summary>
    /// Transaction reference from banking system
    /// </summary>
    public string TransactionReference { get; init; } = string.Empty;
    
    /// <summary>
    /// Whether the transaction was allowed or blocked
    /// </summary>
    public bool WasAllowed { get; init; } = true;
    
    /// <summary>
    /// Fraud decision made for this transaction
    /// </summary>
    public string FraudDecision { get; init; } = "Allow";
}
