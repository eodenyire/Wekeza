using Wekeza.Mobile.ViewModels;

namespace Wekeza.Mobile.Views;

public partial class TransferPage : ContentPage
{
    private readonly TransferViewModel _viewModel;

    public TransferPage(TransferViewModel viewModel)
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
