using Wekeza.Mobile.ViewModels;

namespace Wekeza.Mobile.Views;

public partial class BalancePage : ContentPage
{
    private readonly BalanceViewModel _viewModel;

    public BalancePage(BalanceViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadBalanceCommand.ExecuteAsync(null);
    }
}
