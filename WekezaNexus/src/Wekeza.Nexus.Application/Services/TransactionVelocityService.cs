using Wekeza.Nexus.Domain.Interfaces;

namespace Wekeza.Nexus.Application.Services;

/// <summary>
/// Implementation of transaction velocity tracking with actual historical data
/// 
/// This service tracks transaction history in memory and provides real-time
/// velocity analysis for fraud detection. For production deployments with large
/// scale, this can be extended with Redis cache, time-series databases, or direct
/// integration with the Core banking system's transaction repository.
/// </summary>
public class TransactionVelocityService : ITransactionVelocityService
{
    private readonly ITransactionHistoryRepository _historyRepository;
    
    public TransactionVelocityService(ITransactionHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }
    
    public async Task<int> GetTransactionCountAsync(
        Guid userId, 
        int minutes, 
        CancellationToken cancellationToken = default)
    {
        var since = DateTime.UtcNow.AddMinutes(-minutes);
        var transactions = await _historyRepository.GetUserTransactionsAsync(
            userId, since, cancellationToken);
        
        return transactions.Count;
    }
    
    public async Task<decimal> GetTransactionAmountAsync(
        Guid userId, 
        int minutes, 
        CancellationToken cancellationToken = default)
    {
        var since = DateTime.UtcNow.AddMinutes(-minutes);
        var transactions = await _historyRepository.GetUserTransactionsAsync(
            userId, since, cancellationToken);
        
        return transactions.Sum(t => t.Amount);
    }
    
    public async Task<decimal> GetAverageTransactionAmountAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var since = DateTime.UtcNow.AddDays(-30);
        var transactions = await _historyRepository.GetUserTransactionsAsync(
            userId, since, cancellationToken);
        
        if (transactions.Count == 0)
        {
            // Return intelligent default baseline for new users
            return 5000m;
        }
        
        return transactions.Average(t => t.Amount);
    }
    
    public async Task<bool> IsFirstTimeBeneficiaryAsync(
        Guid userId, 
        string beneficiaryAccountNumber, 
        CancellationToken cancellationToken = default)
    {
        var hasHistory = await _historyRepository.HasTransactionHistoryWithBeneficiaryAsync(
            userId, beneficiaryAccountNumber, cancellationToken);
        
        // Return true if no history (first time)
        return !hasHistory;
    }
    
    public async Task<int?> GetAccountAgeDaysAsync(
        string accountNumber, 
        CancellationToken cancellationToken = default)
    {
        var metadata = await _historyRepository.GetAccountMetadataAsync(
            accountNumber, cancellationToken);
        
        return metadata?.AgeDays;
    }
    
    public async Task<bool> DetectCircularTransactionAsync(
        string fromAccount, 
        string toAccount, 
        int lookbackHours = 24, 
        CancellationToken cancellationToken = default)
    {
        var since = DateTime.UtcNow.AddHours(-lookbackHours);
        
        // Build a transaction graph and detect cycles using BFS
        var graph = await BuildTransactionGraphAsync(fromAccount, since, cancellationToken);
        
        // Check if there's a path from toAccount back to fromAccount
        return HasPathBFS(graph, toAccount, fromAccount);
    }
    
    /// <summary>
    /// Builds a transaction graph from historical data
    /// </summary>
    private async Task<Dictionary<string, HashSet<string>>> BuildTransactionGraphAsync(
        string startAccount,
        DateTime since,
        CancellationToken cancellationToken)
    {
        var graph = new Dictionary<string, HashSet<string>>();
        var toVisit = new Queue<string>();
        var visited = new HashSet<string>();
        
        toVisit.Enqueue(startAccount);
        
        // BFS to build graph within lookback window
        while (toVisit.Count > 0 && visited.Count < 100) // Limit to prevent infinite loops
        {
            var currentAccount = toVisit.Dequeue();
            if (visited.Contains(currentAccount))
                continue;
                
            visited.Add(currentAccount);
            
            // Get all transactions from this account
            var transactions = await _historyRepository.GetUserTransactionsAsync(
                Guid.Empty, since, cancellationToken);
            
            var outgoingTxns = transactions
                .Where(t => t.FromAccountNumber == currentAccount)
                .Select(t => t.ToAccountNumber)
                .Distinct()
                .ToList();
            
            if (outgoingTxns.Any())
            {
                if (!graph.ContainsKey(currentAccount))
                    graph[currentAccount] = new HashSet<string>();
                
                foreach (var dest in outgoingTxns)
                {
                    graph[currentAccount].Add(dest);
                    if (!visited.Contains(dest))
                        toVisit.Enqueue(dest);
                }
            }
        }
        
        return graph;
    }
    
    /// <summary>
    /// Uses BFS to detect if there's a path from source to destination
    /// Returns true if circular pattern detected (A→B→...→A)
    /// </summary>
    private bool HasPathBFS(
        Dictionary<string, HashSet<string>> graph,
        string source,
        string destination)
    {
        if (!graph.ContainsKey(source))
            return false;
            
        var queue = new Queue<string>();
        var visited = new HashSet<string>();
        
        queue.Enqueue(source);
        visited.Add(source);
        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            
            if (current == destination)
                return true; // Circular transaction detected!
            
            if (graph.ContainsKey(current))
            {
                foreach (var neighbor in graph[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
        
        return false;
    }
}
