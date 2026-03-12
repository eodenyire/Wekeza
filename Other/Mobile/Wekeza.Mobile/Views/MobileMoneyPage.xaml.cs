using Wekeza.Mobile.ViewModels;

namespace Wekeza.Mobile.Views;

public partial class MobileMoneyPage : ContentPage
{
    private readonly MobileMoneyViewModel _viewModel;

    public MobileMoneyPage(MobileMoneyViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadUserAccountCommand.ExecuteAsync(null);
    }
}
