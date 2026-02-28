namespace Wekeza.Nexus.Domain.Enums;

/// <summary>
/// The decision outcome from the Wekeza Nexus fraud evaluation system
/// </summary>
public enum FraudDecision
{
    /// <summary>
    /// Transaction is safe to proceed without additional verification
    /// </summary>
    Allow = 0,
    
    /// <summary>
    /// Transaction requires additional verification (Step-Up Authentication)
    /// Triggers OTP, Biometric, or other challenge mechanisms
    /// </summary>
    Challenge = 1,
    
    /// <summary>
    /// Transaction is blocked due to high fraud risk
    /// </summary>
    Block = 2,
    
    /// <summary>
    /// Transaction is flagged for manual review by fraud analysts
    /// Proceeds but with monitoring
    /// </summary>
    Review = 3
}
