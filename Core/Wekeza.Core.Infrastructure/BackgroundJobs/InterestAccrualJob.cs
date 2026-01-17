///2. InterestAccrualJob.cs (The Revenue Engine)
/// This job ensures that for savings or fixed-deposit accounts, the daily interest is calculated based on the End-of-Day (EOD) balance. This is precision-grade math where rounding errors are forbidden.
///
///
namespace Wekeza.Core.Infrastructure.BackgroundJobs;

public class InterestAccrualJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InterestAccrualJob> _logger;

    public InterestAccrualJob(IServiceProvider serviceProvider, ILogger<InterestAccrualJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Logic to run at 23:59 daily
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // Fetch interest-bearing accounts
                var accounts = await accountRepository.GetInterestBearingAccountsAsync(stoppingToken);

                foreach (var account in accounts)
                {
                    // Domain Math: (Balance * Rate) / 365
                    account.AccrueDailyInterest();
                }

                await unitOfWork.SaveChangesAsync(stoppingToken);
            }
            
            // Wait for the next midnight window
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
