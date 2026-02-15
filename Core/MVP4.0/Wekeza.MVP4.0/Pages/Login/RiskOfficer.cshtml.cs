using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Pages.Login;

public class RiskOfficerModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<RiskOfficerModel> _logger;
    [BindProperty] public string Username { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    [BindProperty] public bool RememberMe { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public RiskOfficerModel(IHttpClientFactory httpClientFactory, ILogger<RiskOfficerModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Username and password are required.";
            return Page();
        }
        try
        {
            var loginRequest = new LoginRequest { Username = Username, Password = Password, Role = UserRole.RiskOfficer };
            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var content = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{baseUrl}/api/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true,
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, options);
                if (loginResponse?.Success == true && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    HttpContext.Session.SetString("AuthToken", loginResponse.Token);
                    HttpContext.Session.SetString("Username", Username);
                    HttpContext.Session.SetString("Role", UserRole.RiskOfficer.ToString());
                    return RedirectToPage("/Risk/Index");
                }
            }
            ErrorMessage = "Invalid username or password.";
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for RiskOfficer user {Username}", Username);
            ErrorMessage = "An error occurred during login. Please try again.";
            return Page();
        }
    }
}
