using Wekeza.Nexus.Domain.ValueObjects;
using Wekeza.Nexus.Domain.Enums;

namespace Wekeza.Nexus.Domain.Entities;

/// <summary>
/// Represents a fraud evaluation performed on a transaction
/// Stores the complete audit trail for compliance and investigation
/// </summary>
public class FraudEvaluation
{
    /// <summary>
    /// Unique identifier for this evaluation
    /// </summary>
    public Guid Id { get; private set; } = Guid.NewGuid();
    
    /// <summary>
    /// Transaction context that was evaluated
    /// </summary>
    public Guid TransactionContextId { get; private set; }
    
    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// Transaction reference (from banking system)
    /// </summary>
    public string TransactionReference { get; private set; } = string.Empty;
    
    /// <summary>
    /// Amount evaluated
    /// </summary>
    public decimal Amount { get; private set; }
    
    /// <summary>
    /// The fraud score calculated
    /// </summary>
    public FraudScore FraudScore { get; private set; } = ValueObjects.FraudScore.Safe();
    
    /// <summary>
    /// When the evaluation was performed
    /// </summary>
    public DateTime EvaluatedAt { get; private set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Processing time in milliseconds
    /// Target: < 150ms for real-time processing
    /// </summary>
    public long ProcessingTimeMs { get; private set; }
    
    /// <summary>
    /// Model version used for evaluation (for A/B testing and auditing)
    /// </summary>
    public string ModelVersion { get; private set; } = "1.0.0";
    
    /// <summary>
    /// Whether the transaction was ultimately allowed
    /// </summary>
    public bool WasAllowed { get; private set; }
    
    /// <summary>
    /// Whether manual review was triggered
    /// </summary>
    public bool RequiresReview { get; private set; }
    
    /// <summary>
    /// Notes from fraud analyst (if reviewed)
    /// </summary>
    public string? AnalystNotes { get; private set; }
    
    /// <summary>
    /// Whether this was a true positive (confirmed fraud)
    /// Used for model training and accuracy metrics
    /// </summary>
    public bool? WasActualFraud { get; private set; }
    
    /// <summary>
    /// Created timestamp
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }
    
    private FraudEvaluation() { }
    
    public static FraudEvaluation Create(
        Guid transactionContextId,
        Guid userId,
        string transactionReference,
        decimal amount,
        FraudScore fraudScore,
        long processingTimeMs,
        string modelVersion = "1.0.0")
    {
        var evaluation = new FraudEvaluation
        {
            TransactionContextId = transactionContextId,
            UserId = userId,
            TransactionReference = transactionReference,
            Amount = amount,
            FraudScore = fraudScore,
            ProcessingTimeMs = processingTimeMs,
            ModelVersion = modelVersion,
            WasAllowed = fraudScore.Decision == FraudDecision.Allow,
            RequiresReview = fraudScore.Decision == FraudDecision.Review
        };
        
        return evaluation;
    }
    
    /// <summary>
    /// Mark evaluation as reviewed by analyst
    /// </summary>
    public void AddAnalystReview(string notes, bool wasActualFraud)
    {
        AnalystNotes = notes;
        WasActualFraud = wasActualFraud;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Update the allowed status (after challenge completion, for example)
    /// </summary>
    public void UpdateAllowedStatus(bool wasAllowed)
    {
        WasAllowed = wasAllowed;
        UpdatedAt = DateTime.UtcNow;
    }
}
