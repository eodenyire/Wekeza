using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wekeza.Mobile.Models;
using Wekeza.Mobile.Services;

namespace Wekeza.Mobile.ViewModels;

public partial class BillPaymentViewModel : ObservableObject
{
    private readonly IBillPaymentService _billPaymentService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private string fromAccountNumber = string.Empty;

    [ObservableProperty]
    private string businessNumber = string.Empty;

    [ObservableProperty]
    private string billNumber = string.Empty;

    [ObservableProperty]
    private string amount = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string billType = "Paybill";

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string resultMessage = string.Empty;

    [ObservableProperty]
    private bool showResult;

    public List<string> BillTypes { get; } = new() { "Paybill", "Till" };

    public BillPaymentViewModel(IBillPaymentService billPaymentService, IAuthenticationService authService)
    {
        _billPaymentService = billPaymentService;
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
    private async Task PayBillAsync()
    {
        if (string.IsNullOrWhiteSpace(BusinessNumber) || string.IsNullOrWhiteSpace(Amount))
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
            var request = new BillPaymentRequest
            {
                FromAccountNumber = FromAccountNumber,
                BusinessNumber = BusinessNumber,
                BillNumber = BillNumber,
                BillType = BillType,
                Amount = amountValue,
                Description = Description
            };

            var result = await _billPaymentService.PayBillAsync(request);

            if (result != null && result.Success)
            {
                ResultMessage = $"Bill paid successfully! Receipt: {result.ReceiptNumber}";
                
                // Clear form
                BusinessNumber = string.Empty;
                BillNumber = string.Empty;
                Amount = string.Empty;
                Description = string.Empty;
            }
            else
            {
                ResultMessage = result?.Message ?? "Bill payment failed";
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
