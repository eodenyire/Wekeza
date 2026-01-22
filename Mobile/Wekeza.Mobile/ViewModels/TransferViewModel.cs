using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wekeza.Mobile.Models;
using Wekeza.Mobile.Services;

namespace Wekeza.Mobile.ViewModels;

public partial class TransferViewModel : ObservableObject
{
    private readonly ITransactionService _transactionService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private string fromAccountNumber = string.Empty;

    [ObservableProperty]
    private string toAccountNumber = string.Empty;

    [ObservableProperty]
    private string amount = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string transferType = "Internal";

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string resultMessage = string.Empty;

    [ObservableProperty]
    private bool showResult;

    public List<string> TransferTypes { get; } = new() { "Internal", "External" };

    public TransferViewModel(ITransactionService transactionService, IAuthenticationService authService)
    {
        _transactionService = transactionService;
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
    private async Task TransferAsync()
    {
        if (string.IsNullOrWhiteSpace(ToAccountNumber) || string.IsNullOrWhiteSpace(Amount))
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
            var request = new TransferRequest
            {
                FromAccountNumber = FromAccountNumber,
                ToAccountNumber = ToAccountNumber,
                Amount = amountValue,
                Description = Description,
                TransferType = TransferType
            };

            var result = await _transactionService.TransferFundsAsync(request);

            if (result != null && result.Success)
            {
                ResultMessage = $"Transfer successful! Reference: {result.ReferenceNumber}";
                
                // Clear form
                ToAccountNumber = string.Empty;
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
