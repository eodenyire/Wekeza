using Wekeza.Mobile.Models;

namespace Wekeza.Mobile.Services;

public interface IMobileMoneyService
{
    Task<MobileMoneyResponse?> SendMoneyAsync(MobileMoneyRequest request);
    Task<MobileMoneyResponse?> BuyAirtimeAsync(AirtimeRequest request);
}

public class MobileMoneyService : IMobileMoneyService
{
    private readonly IApiService _apiService;

    public MobileMoneyService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<MobileMoneyResponse?> SendMoneyAsync(MobileMoneyRequest request)
    {
        try
        {
            return await _apiService.PostAsync<MobileMoneyRequest, MobileMoneyResponse>(
                "/api/mobilemoney/send",
                request
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Send money error: {ex.Message}");
            return new MobileMoneyResponse { Success = false, Message = $"Error: {ex.Message}" };
        }
    }

    public async Task<MobileMoneyResponse?> BuyAirtimeAsync(AirtimeRequest request)
    {
        try
        {
            return await _apiService.PostAsync<AirtimeRequest, MobileMoneyResponse>(
                "/api/mobilemoney/airtime",
                request
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Buy airtime error: {ex.Message}");
            return new MobileMoneyResponse { Success = false, Message = $"Error: {ex.Message}" };
        }
    }
}
