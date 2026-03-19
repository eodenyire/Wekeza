namespace Wekeza.Nexus.Domain.Interfaces;

/// <summary>
/// Service for building transaction velocity and relationship metrics
/// Tracks patterns over time windows (10min, 1hr, 24hr, 30day)
/// </summary>
public interface ITransactionVelocityService
{
    /// <summary>
    /// Gets transaction count in the last N minutes for a user
    /// </summary>
    Task<int> GetTransactionCountAsync(Guid userId, int minutes, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets total transaction amount in the last N minutes for a user
    /// </summary>
    Task<decimal> GetTransactionAmountAsync(Guid userId, int minutes, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets user's average transaction amount (30-day baseline)
    /// </summary>
    Task<decimal> GetAverageTransactionAmountAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if this is the first transaction to a beneficiary
    /// </summary>
    Task<bool> IsFirstTimeBeneficiaryAsync(Guid userId, string beneficiaryAccountNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the age of a beneficiary account in days
    /// </summary>
    Task<int?> GetAccountAgeDaysAsync(string accountNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Detects circular transaction patterns (A→B→C→A)
    /// </summary>
    Task<bool> DetectCircularTransactionAsync(string fromAccount, string toAccount, int lookbackHours = 24, CancellationToken cancellationToken = default);
}
