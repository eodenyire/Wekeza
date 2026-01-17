using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.BackgroundJobs;

/// <summary>
/// The Wealth Engine: Automatically liquidates matured Fixed Deposits.
/// Moves funds (Principal + Interest) back to the primary account.
/// 📂 Phase 11: Fixed Deposit Maturity Engine
///When a Fixed Deposit matures, it shouldn't just sit there. The bank must automatically "Liquidate" it—moving the Principal + the Accrued Interest back to the customer's Operating Account.
/// 1. 📂 Infrastructure/BackgroundJobs/FixedDepositMaturityJob.cs
/// This job runs once a day (usually at 00:01 AM) to process all investments that have reached their maturity date.
///
/// </summary>
public class FixedDepositMaturityJob : BackgroundService
{
    private readonly ILogger<FixedDepositMaturityJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    public FixedDepositMaturityJob(ILogger<FixedDepositMaturityJob> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("[WEKEZA WEALTH] Scanning for matured Fixed Deposits...");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var fdRepository = scope.ServiceProvider.GetRequiredService<IFixedDepositRepository>();
                    var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // 1. Fetch all 'Active' deposits where MaturityDate <= Today
                    var maturedDeposits = await fdRepository.GetMaturedDepositsAsync(DateTime.UtcNow, stoppingToken);

                    foreach (var fd in maturedDeposits)
                    {
                        var targetAccount = await accountRepository.GetByIdAsync(fd.SourceAccountId, stoppingToken);
                        if (targetAccount != null)
                        {
                            // 2. Liquidate: Calculate total payout (Principal + Accrued Interest)
                            var totalPayout = fd.Liquidate(); 

                            // 3. Credit the customer's main account
                            targetAccount.Credit(totalPayout);
                            
                            _logger.LogInformation("[WEKEZA WEALTH] Liquidated FD {Id} for Account {Acc}. Payout: {Amount}", 
                                fd.Id, targetAccount.AccountNumber.Value, totalPayout.Amount);
                        }
                    }

                    await unitOfWork.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WEKEZA WEALTH] Critical failure in Maturity Job.");
            }

            // Run once every 24 hours
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
