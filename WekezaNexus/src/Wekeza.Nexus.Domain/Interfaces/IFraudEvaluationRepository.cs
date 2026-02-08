using Wekeza.Nexus.Domain.Entities;

namespace Wekeza.Nexus.Domain.Interfaces;

/// <summary>
/// Repository for storing fraud evaluations
/// </summary>
public interface IFraudEvaluationRepository
{
    Task<FraudEvaluation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FraudEvaluation?> GetByTransactionReferenceAsync(string transactionReference, CancellationToken cancellationToken = default);
    Task<List<FraudEvaluation>> GetByUserIdAsync(Guid userId, int limit = 100, CancellationToken cancellationToken = default);
    Task<List<FraudEvaluation>> GetPendingReviewsAsync(int limit = 100, CancellationToken cancellationToken = default);
    Task AddAsync(FraudEvaluation evaluation, CancellationToken cancellationToken = default);
    Task UpdateAsync(FraudEvaluation evaluation, CancellationToken cancellationToken = default);
}
