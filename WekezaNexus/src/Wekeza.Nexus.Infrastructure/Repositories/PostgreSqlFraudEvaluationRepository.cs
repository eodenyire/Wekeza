using Microsoft.EntityFrameworkCore;
using Wekeza.Nexus.Domain.Entities;
using Wekeza.Nexus.Domain.Interfaces;
using Wekeza.Nexus.Infrastructure.Data;

namespace Wekeza.Nexus.Infrastructure.Repositories;

/// <summary>
/// PostgreSQL implementation of fraud evaluation repository using Entity Framework Core
/// Provides persistent storage for fraud evaluations in alignment with MVP4.0
/// </summary>
public class PostgreSqlFraudEvaluationRepository : IFraudEvaluationRepository
{
    private readonly NexusDbContext _context;

    public PostgreSqlFraudEvaluationRepository(NexusDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<FraudEvaluation?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await _context.FraudEvaluations
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<FraudEvaluation?> GetByTransactionReferenceAsync(
        string transactionReference, 
        CancellationToken cancellationToken = default)
    {
        return await _context.FraudEvaluations
            .FirstOrDefaultAsync(e => e.TransactionReference == transactionReference, cancellationToken);
    }

    public async Task<List<FraudEvaluation>> GetByUserIdAsync(
        Guid userId, 
        int limit = 100, 
        CancellationToken cancellationToken = default)
    {
        return await _context.FraudEvaluations
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.EvaluatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<FraudEvaluation>> GetPendingReviewsAsync(
        int limit = 100, 
        CancellationToken cancellationToken = default)
    {
        return await _context.FraudEvaluations
            .Where(e => e.RequiresReview && e.AnalystNotes == null)
            .OrderByDescending(e => e.EvaluatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        FraudEvaluation evaluation, 
        CancellationToken cancellationToken = default)
    {
        await _context.FraudEvaluations.AddAsync(evaluation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        FraudEvaluation evaluation, 
        CancellationToken cancellationToken = default)
    {
        _context.FraudEvaluations.Update(evaluation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
