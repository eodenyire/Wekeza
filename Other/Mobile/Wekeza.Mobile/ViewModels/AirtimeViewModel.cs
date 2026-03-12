using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wekeza.Mobile.Models;
using Wekeza.Mobile.Services;

namespace Wekeza.Mobile.ViewModels;

public partial class AirtimeViewModel : ObservableObject
{
    private readonly IMobileMoneyService _mobileMoneyService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private string fromAccountNumber = string.Empty;

    [ObservableProperty]
    private string phoneNumber = string.Empty;

    [ObservableProperty]
    private string amount = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string resultMessage = string.Empty;

    [ObservableProperty]
    private bool showResult;

    public List<string> QuickAmounts { get; } = new() { "10", "20", "50", "100", "200", "500", "1000" };

    public AirtimeViewModel(IMobileMoneyService mobileMoneyService, IAuthenticationService authService)
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
    private void SelectQuickAmount(string selectedAmount)
    {
        Amount = selectedAmount;
    }

    [RelayCommand]
    private async Task BuyAirtimeAsync()
    {
        if (string.IsNullOrWhiteSpace(PhoneNumber) || string.IsNullOrWhiteSpace(Amount))
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
            var request = new AirtimeRequest
            {
                FromAccountNumber = FromAccountNumber,
                PhoneNumber = PhoneNumber,
                Amount = amountValue,
                Provider = "MPESA"
            };

            var result = await _mobileMoneyService.BuyAirtimeAsync(request);

            if (result != null && result.Success)
            {
                ResultMessage = $"Airtime purchased successfully! Receipt: {result.MpesaReceiptNumber}";
                
                // Clear form
                PhoneNumber = string.Empty;
                Amount = string.Empty;
            }
            else
            {
                ResultMessage = result?.Message ?? "Airtime purchase failed";
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
