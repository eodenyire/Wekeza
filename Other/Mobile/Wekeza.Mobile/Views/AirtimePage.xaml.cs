using Wekeza.Mobile.ViewModels;

namespace Wekeza.Mobile.Views;

public partial class AirtimePage : ContentPage
{
    private readonly AirtimeViewModel _viewModel;

    public AirtimePage(AirtimeViewModel viewModel)
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
