using Wekeza.Nexus.Domain.Entities;
using Wekeza.Nexus.Domain.Interfaces;

namespace Wekeza.Nexus.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of fraud evaluation repository
/// In production, this would use Entity Framework Core with PostgreSQL
/// </summary>
public class InMemoryFraudEvaluationRepository : IFraudEvaluationRepository
{
    private readonly Dictionary<Guid, FraudEvaluation> _evaluations = new();
    private readonly Dictionary<string, Guid> _transactionRefIndex = new();
    private readonly object _lock = new();
    
    public Task<FraudEvaluation?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_evaluations.TryGetValue(id, out var evaluation) 
                ? evaluation 
                : null);
        }
    }
    
    public Task<FraudEvaluation?> GetByTransactionReferenceAsync(
        string transactionReference, 
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_transactionRefIndex.TryGetValue(transactionReference, out var id))
            {
                return GetByIdAsync(id, cancellationToken);
            }
            return Task.FromResult<FraudEvaluation?>(null);
        }
    }
    
    public Task<List<FraudEvaluation>> GetByUserIdAsync(
        Guid userId, 
        int limit = 100, 
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var results = _evaluations.Values
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EvaluatedAt)
                .Take(limit)
                .ToList();
            
            return Task.FromResult(results);
        }
    }
    
    public Task<List<FraudEvaluation>> GetPendingReviewsAsync(
        int limit = 100, 
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var results = _evaluations.Values
                .Where(e => e.RequiresReview && e.AnalystNotes == null)
                .OrderByDescending(e => e.EvaluatedAt)
                .Take(limit)
                .ToList();
            
            return Task.FromResult(results);
        }
    }
    
    public Task AddAsync(
        FraudEvaluation evaluation, 
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _evaluations[evaluation.Id] = evaluation;
            _transactionRefIndex[evaluation.TransactionReference] = evaluation.Id;
        }
        return Task.CompletedTask;
    }
    
    public Task UpdateAsync(
        FraudEvaluation evaluation, 
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _evaluations[evaluation.Id] = evaluation;
        }
        return Task.CompletedTask;
    }
}
