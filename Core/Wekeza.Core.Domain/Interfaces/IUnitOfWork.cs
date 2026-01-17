namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// 📂 Wekeza.Core.Domain/Interfaces
/// 3. IUnitOfWork.cs (The Atomic Guarantee)
/// This is the "Billion Dollar" interface. It ensures that when you transfer money, the debit from Account A and the credit to Account B either both succeed or both fail. This is what separates a student project from a Principal-grade financial system.
/// Coordinates the saving of changes across multiple repositories in a single atomic transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Persists all tracked changes to the underlying storage.
    /// In our case, this will also trigger the Outbox pattern for Domain Events.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
