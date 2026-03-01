using Wekeza.Nexus.Domain.Entities;
using Wekeza.Nexus.Domain.ValueObjects;

namespace Wekeza.Nexus.Domain.Interfaces;

/// <summary>
/// Core fraud evaluation service interface
/// This is the "Deep Brain" of Wekeza Nexus
/// </summary>
public interface IFraudEvaluationService
{
    /// <summary>
    /// Evaluates a transaction context and returns a fraud score
    /// Target latency: < 150ms
    /// </summary>
    Task<FraudScore> EvaluateAsync(TransactionContext context, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Re-evaluates after user completes a challenge (OTP, Biometric, etc.)
    /// </summary>
    Task<FraudScore> ReEvaluateAfterChallengeAsync(
        Guid transactionContextId, 
        bool challengePassed,
        CancellationToken cancellationToken = default);
}
