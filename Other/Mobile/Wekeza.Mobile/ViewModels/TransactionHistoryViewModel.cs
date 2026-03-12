using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wekeza.Mobile.Models;
using Wekeza.Mobile.Services;

namespace Wekeza.Mobile.ViewModels;

public partial class TransactionHistoryViewModel : ObservableObject
{
    private readonly ITransactionService _transactionService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private ObservableCollection<Transaction> transactions = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private string accountNumber = string.Empty;

    private int _currentPage = 1;
    private const int PageSize = 20;

    public TransactionHistoryViewModel(ITransactionService transactionService, IAuthenticationService authService)
    {
        _transactionService = transactionService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoadTransactionsAsync()
    {
        IsLoading = true;
        try
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user?.AccountNumber != null)
            {
                AccountNumber = user.AccountNumber;
                var history = await _transactionService.GetTransactionHistoryAsync(
                    user.AccountNumber, 
                    _currentPage, 
                    PageSize
                );

                if (history?.Transactions != null)
                {
                    Transactions.Clear();
                    foreach (var transaction in history.Transactions)
                    {
                        Transactions.Add(transaction);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading transactions: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshTransactionsAsync()
    {
        IsRefreshing = true;
        _currentPage = 1;
        await LoadTransactionsAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task LoadMoreTransactionsAsync()
    {
        _currentPage++;
        await LoadTransactionsAsync();
    }
}
