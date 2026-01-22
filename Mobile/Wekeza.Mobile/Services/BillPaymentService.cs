using Wekeza.Mobile.Models;

namespace Wekeza.Mobile.Services;

public interface IBillPaymentService
{
    Task<BillPaymentResponse?> PayBillAsync(BillPaymentRequest request);
    Task<List<string>> GetRecentPaybillsAsync();
}

public class BillPaymentService : IBillPaymentService
{
    private readonly IApiService _apiService;

    public BillPaymentService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<BillPaymentResponse?> PayBillAsync(BillPaymentRequest request)
    {
        try
        {
            return await _apiService.PostAsync<BillPaymentRequest, BillPaymentResponse>(
                "/api/bills/pay",
                request
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Pay bill error: {ex.Message}");
            return new BillPaymentResponse { Success = false, Message = $"Error: {ex.Message}" };
        }
    }

    public async Task<List<string>> GetRecentPaybillsAsync()
    {
        try
        {
            var paybills = await _apiService.GetAsync<List<string>>("/api/bills/recent");
            return paybills ?? new List<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get recent paybills error: {ex.Message}");
            return new List<string>();
        }
    }
}
