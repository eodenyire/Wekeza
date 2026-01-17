using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Infrastructure.BackgroundJobs;

/// <summary>
/// 📂 Wekeza.Core.Infrastructure/BackgroundJobs
///In a Principal-grade system, we don't just use a simple timer. we use a Background Service that integrates with our Unit of Work and Domain Events.
///1. ChequeClearanceJob.cs
/// This job runs every X minutes (or at midnight) to settle cheques that have passed the "Clearance Window" (e.g., T+2 days).
/// Principal-Grade Background Service: Handles the automated transition 
/// of 'Pending' cheque funds to 'Available' status.
/// </summary>
public class ChequeClearanceJob : BackgroundService
{
    private readonly ILogger<ChequeClearanceJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ChequeClearanceJob(ILogger<ChequeClearanceJob> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[WEKEZA BATCH] Cheque Clearance Job started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                    var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // 1. Fetch all pending cheque transactions older than 2 days
                    // Note: In a billion-dollar bank, this is a 'Set-Based' SQL update for performance.
                    var pendingCheques = await transactionRepository.GetMaturedChequesAsync(days: 2, stoppingToken);

                    foreach (var cheque in pendingCheques)
                    {
                        var account = await accountRepository.GetByIdAsync(cheque.AccountId, stoppingToken);
                        if (account != null)
                        {
                            // 2. Business Logic: Move money from 'Pending' to 'Available'
                            // The Account Aggregate handles the math to ensure integrity.
                            account.ClearChequeFunds(cheque.Amount);
                            
                            // 3. Update transaction status to 'Cleared'
                            cheque.MarkAsCleared(); 
                            
                            _logger.LogInformation("[WEKEZA BATCH] Cleared Cheque {ChequeNo} for Account {Account}", 
                                cheque.Description, account.AccountNumber.Value);
                        }
                    }

                    // 4. Atomic Commit: All cheques in this batch are cleared together
                    if (pendingCheques.Any())
                    {
                        await unitOfWork.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WEKEZA BATCH] Critical failure in Cheque Clearance Job.");
            }

            // Run every hour (configurable)
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
