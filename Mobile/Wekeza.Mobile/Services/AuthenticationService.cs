using Wekeza.Mobile.Models;

namespace Wekeza.Mobile.Services;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(string username, string password);
    Task<bool> LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<UserInfo?> GetCurrentUserAsync();
    Task<string?> GetTokenAsync();
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IApiService _apiService;
    private readonly ISecureStorageService _secureStorage;
    private const string TokenKey = "auth_token";
    private const string UserKey = "user_info";

    public AuthenticationService(IApiService apiService, ISecureStorageService secureStorage)
    {
        _apiService = apiService;
        _secureStorage = secureStorage;
    }

    public async Task<LoginResponse> LoginAsync(string username, string password)
    {
        try
        {
            var request = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var response = await _apiService.PostAsync<LoginRequest, LoginResponse>(
                "/api/authentication/login", 
                request
            );

            if (response != null && response.Success && !string.IsNullOrEmpty(response.Token))
            {
                await _secureStorage.SetAsync(TokenKey, response.Token);
                if (response.User != null)
                {
                    await _secureStorage.SetAsync(UserKey, Newtonsoft.Json.JsonConvert.SerializeObject(response.User));
                }
                _apiService.SetAuthToken(response.Token);
                return response;
            }

            return response ?? new LoginResponse { Success = false, Message = "Login failed" };
        }
        catch (Exception ex)
        {
            return new LoginResponse { Success = false, Message = $"Error: {ex.Message}" };
        }
    }

    public async Task<bool> LogoutAsync()
    {
        try
        {
            await _secureStorage.RemoveAsync(TokenKey);
            await _secureStorage.RemoveAsync(UserKey);
            _apiService.ClearAuthToken();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logout error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _secureStorage.GetAsync(TokenKey);
        if (!string.IsNullOrEmpty(token))
        {
            _apiService.SetAuthToken(token);
            return true;
        }
        return false;
    }

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        try
        {
            var userJson = await _secureStorage.GetAsync(UserKey);
            if (!string.IsNullOrEmpty(userJson))
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(userJson);
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get user error: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _secureStorage.GetAsync(TokenKey);
    }
}
