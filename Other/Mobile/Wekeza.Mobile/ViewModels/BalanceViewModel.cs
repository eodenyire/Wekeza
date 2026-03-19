using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wekeza.Mobile.Models;
using Wekeza.Mobile.Services;

namespace Wekeza.Mobile.ViewModels;

public partial class BalanceViewModel : ObservableObject
{
    private readonly IAccountService _accountService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private AccountBalance? accountBalance;

    [ObservableProperty]
    private string accountNumber = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isRefreshing;

    public BalanceViewModel(IAccountService accountService, IAuthenticationService authService)
    {
        _accountService = accountService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoadBalanceAsync()
    {
        IsLoading = true;
        try
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user?.AccountNumber != null)
            {
                AccountNumber = user.AccountNumber;
                AccountBalance = await _accountService.GetBalanceAsync(user.AccountNumber);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading balance: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshBalanceAsync()
    {
        IsRefreshing = true;
        await LoadBalanceAsync();
        IsRefreshing = false;
    }
}
