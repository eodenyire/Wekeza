using Wekeza.Mobile.Models;

namespace Wekeza.Mobile.Services;

public interface IAccountService
{
    Task<AccountBalance?> GetBalanceAsync(string accountNumber);
    Task<AccountSummary?> GetAccountSummaryAsync(string accountNumber);
    Task<List<AccountSummary>> GetUserAccountsAsync();
}

public class AccountService : IAccountService
{
    private readonly IApiService _apiService;

    public AccountService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<AccountBalance?> GetBalanceAsync(string accountNumber)
    {
        try
        {
            return await _apiService.GetAsync<AccountBalance>($"/api/accounts/{accountNumber}/balance");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get balance error: {ex.Message}");
            return null;
        }
    }

    public async Task<AccountSummary?> GetAccountSummaryAsync(string accountNumber)
    {
        try
        {
            return await _apiService.GetAsync<AccountSummary>($"/api/accounts/{accountNumber}/summary");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get account summary error: {ex.Message}");
            return null;
        }
    }

    public async Task<List<AccountSummary>> GetUserAccountsAsync()
    {
        try
        {
            var accounts = await _apiService.GetAsync<List<AccountSummary>>("/api/accounts/user/accounts");
            return accounts ?? new List<AccountSummary>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get user accounts error: {ex.Message}");
            return new List<AccountSummary>();
        }
    }
}
