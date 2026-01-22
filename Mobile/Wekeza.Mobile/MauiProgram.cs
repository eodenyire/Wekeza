using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Wekeza.Mobile.Services;
using Wekeza.Mobile.ViewModels;
using Wekeza.Mobile.Views;

namespace Wekeza.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Services
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<IAccountService, AccountService>();
        builder.Services.AddSingleton<ITransactionService, TransactionService>();
        builder.Services.AddSingleton<IMobileMoneyService, MobileMoneyService>();
        builder.Services.AddSingleton<IBillPaymentService, BillPaymentService>();
        builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();

        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<BalanceViewModel>();
        builder.Services.AddTransient<TransferViewModel>();
        builder.Services.AddTransient<MobileMoneyViewModel>();
        builder.Services.AddTransient<AirtimeViewModel>();
        builder.Services.AddTransient<BillPaymentViewModel>();
        builder.Services.AddTransient<TransactionHistoryViewModel>();

        // Register Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<BalancePage>();
        builder.Services.AddTransient<TransferPage>();
        builder.Services.AddTransient<MobileMoneyPage>();
        builder.Services.AddTransient<AirtimePage>();
        builder.Services.AddTransient<BillPaymentPage>();
        builder.Services.AddTransient<TransactionHistoryPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
