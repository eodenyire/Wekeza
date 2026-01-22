using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wekeza.Mobile.Models;
using Wekeza.Mobile.Services;
using Wekeza.Mobile.Views;

namespace Wekeza.Mobile.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService;
    private readonly IAccountService _accountService;

    [ObservableProperty]
    private UserInfo? currentUser;

    [ObservableProperty]
    private string displayName = "User";

    [ObservableProperty]
    private decimal availableBalance;

    [ObservableProperty]
    private string accountNumber = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    public DashboardViewModel(IAuthenticationService authService, IAccountService accountService)
    {
        _authService = authService;
        _accountService = accountService;
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            CurrentUser = await _authService.GetCurrentUserAsync();
            if (CurrentUser != null)
            {
                DisplayName = CurrentUser.FullName ?? CurrentUser.Username;
                if (!string.IsNullOrEmpty(CurrentUser.AccountNumber))
                {
                    AccountNumber = CurrentUser.AccountNumber;
                    var balance = await _accountService.GetBalanceAsync(CurrentUser.AccountNumber);
                    if (balance != null)
                    {
                        AvailableBalance = balance.AvailableBalance;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToBalanceAsync()
    {
        await Shell.Current.GoToAsync(nameof(BalancePage));
    }

    [RelayCommand]
    private async Task NavigateToTransferAsync()
    {
        await Shell.Current.GoToAsync(nameof(TransferPage));
    }

    [RelayCommand]
    private async Task NavigateToMobileMoneyAsync()
    {
        await Shell.Current.GoToAsync(nameof(MobileMoneyPage));
    }

    [RelayCommand]
    private async Task NavigateToAirtimeAsync()
    {
        await Shell.Current.GoToAsync(nameof(AirtimePage));
    }

    [RelayCommand]
    private async Task NavigateToBillPaymentAsync()
    {
        await Shell.Current.GoToAsync(nameof(BillPaymentPage));
    }

    [RelayCommand]
    private async Task NavigateToTransactionHistoryAsync()
    {
        await Shell.Current.GoToAsync(nameof(TransactionHistoryPage));
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}
