///📂 Wekeza.Core.Infrastructure/BackgroundJobs
/// 1. ChequeClearanceJob.cs (Midnight Settlement)
/// This job is the "Settlement Engine." It looks for cheque transactions that have passed their maturity window (e.g., T+2 days) and triggers the domain logic to make those funds available to the customer.
///
///
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.BackgroundJobs;

/// <summary>
/// Automated job that runs periodically to clear "Uncleared" cheque deposits
/// after their maturity period has ended.
/// </summary>
public class ChequeClearanceJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ChequeClearanceJob> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6); // Runs 4 times a day

    public ChequeClearanceJob(IServiceProvider serviceProvider, ILogger<ChequeClearanceJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("[WEKEZA SETTLEMENT] Starting Cheque Clearance cycle...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // 1. Fetch transactions flagged as 'IsCleared = false' that have matured
                // In a real system, we'd filter by Timestamp <= DateTime.UtcNow.AddDays(-2)
                var maturedCheques = await transactionRepository.GetMaturedChequesAsync(stoppingToken);

                foreach (var cheque in maturedCheques)
                {
                    var account = await accountRepository.GetByIdAsync(cheque.AccountId, stoppingToken);
                    
                    // Domain Logic: Update balance and mark transaction as cleared
                    account.ClearCheque(cheque);
                }

                await unitOfWork.SaveChangesAsync(stoppingToken);
            }

            _logger.LogInformation("[WEKEZA SETTLEMENT] Cycle complete. Waiting for next window.");
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}
