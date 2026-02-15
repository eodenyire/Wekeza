/// 📂 Wekeza.Core.Infrastructure/Persistence/UnitOfWork.cs
///The UnitOfWork is the professional way to manage transaction boundaries. It wraps our ApplicationDbContext and ensures that we only call SaveChangesAsync once at the end of a handler.
///
///
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence;

/// <summary>
/// The Atomic Guardian: Ensures ACID compliance across all repositories.
/// Every operation in a single Request/Command is committed as one unit.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Commits all changes made in the current transaction to the database.
    /// In 2026, this is where we also hook into Domain Event dispatching.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // PRINCIPAL MOVE: This is the perfect place to intercept 
        // and dispatch Domain Events before the transaction completes.
        return await _context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Explicitly releases the database context.
    /// </summary>
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
