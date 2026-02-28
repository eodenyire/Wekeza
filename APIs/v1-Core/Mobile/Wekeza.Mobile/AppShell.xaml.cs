using Wekeza.Mobile.Views;

namespace Wekeza.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
        Routing.RegisterRoute(nameof(BalancePage), typeof(BalancePage));
        Routing.RegisterRoute(nameof(TransferPage), typeof(TransferPage));
        Routing.RegisterRoute(nameof(MobileMoneyPage), typeof(MobileMoneyPage));
        Routing.RegisterRoute(nameof(AirtimePage), typeof(AirtimePage));
        Routing.RegisterRoute(nameof(BillPaymentPage), typeof(BillPaymentPage));
        Routing.RegisterRoute(nameof(TransactionHistoryPage), typeof(TransactionHistoryPage));
    }
}
