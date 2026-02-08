namespace Wekeza.Nexus.Application.Exceptions;

/// <summary>
/// Exception thrown when a transaction is blocked due to fraud detection
/// </summary>
public class FraudDetectedException : Exception
{
    public int RiskScore { get; }
    public string RiskLevel { get; }
    public Guid TransactionContextId { get; }
    
    public FraudDetectedException(
        string message, 
        int riskScore = 0, 
        string riskLevel = "Unknown",
        Guid transactionContextId = default) 
        : base(message)
    {
        RiskScore = riskScore;
        RiskLevel = riskLevel;
        TransactionContextId = transactionContextId;
    }
}

/// <summary>
/// Exception thrown when step-up authentication is required
/// </summary>
public class StepUpAuthenticationRequiredException : Exception
{
    public Guid TransactionContextId { get; }
    public string ChallengeType { get; }
    
    public StepUpAuthenticationRequiredException(
        string message,
        Guid transactionContextId,
        string challengeType = "OTP") 
        : base(message)
    {
        TransactionContextId = transactionContextId;
        ChallengeType = challengeType;
    }
}
