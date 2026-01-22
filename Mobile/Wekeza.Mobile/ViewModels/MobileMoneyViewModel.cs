using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wekeza.Mobile.Models;
using Wekeza.Mobile.Services;

namespace Wekeza.Mobile.ViewModels;

public partial class MobileMoneyViewModel : ObservableObject
{
    private readonly IMobileMoneyService _mobileMoneyService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private string fromAccountNumber = string.Empty;

    [ObservableProperty]
    private string toPhoneNumber = string.Empty;

    [ObservableProperty]
    private string amount = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string resultMessage = string.Empty;

    [ObservableProperty]
    private bool showResult;

    public MobileMoneyViewModel(IMobileMoneyService mobileMoneyService, IAuthenticationService authService)
    {
        _mobileMoneyService = mobileMoneyService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoadUserAccountAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user?.AccountNumber != null)
        {
            FromAccountNumber = user.AccountNumber;
        }
    }

    [RelayCommand]
    private async Task SendMoneyAsync()
    {
        if (string.IsNullOrWhiteSpace(ToPhoneNumber) || string.IsNullOrWhiteSpace(Amount))
        {
            ResultMessage = "Please fill in all required fields";
            ShowResult = true;
            return;
        }

        if (!decimal.TryParse(Amount, out var amountValue) || amountValue <= 0)
        {
            ResultMessage = "Please enter a valid amount";
            ShowResult = true;
            return;
        }

        IsLoading = true;
        ShowResult = false;

        try
        {
            var request = new MobileMoneyRequest
            {
                FromAccountNumber = FromAccountNumber,
                ToPhoneNumber = ToPhoneNumber,
                Amount = amountValue,
                Description = Description,
                Provider = "MPESA"
            };

            var result = await _mobileMoneyService.SendMoneyAsync(request);

            if (result != null && result.Success)
            {
                ResultMessage = $"Money sent successfully! M-Pesa Receipt: {result.MpesaReceiptNumber}";
                
                // Clear form
                ToPhoneNumber = string.Empty;
                Amount = string.Empty;
                Description = string.Empty;
            }
            else
            {
                ResultMessage = result?.Message ?? "Transfer failed";
            }

            ShowResult = true;
        }
        catch (Exception ex)
        {
            ResultMessage = $"Error: {ex.Message}";
            ShowResult = true;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
