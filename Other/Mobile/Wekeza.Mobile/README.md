# Wekeza Mobile

A cross-platform mobile banking application for Wekeza Bank, built with .NET MAUI.

## Features

### Authentication
- ✅ Secure login with JWT tokens
- ✅ Persistent session management
- ✅ Automatic token refresh

### Account Management
- ✅ View account balance (available & book balance)
- ✅ Account summary with transaction history
- ✅ Real-time balance updates
- ✅ Multi-currency support

### Money Transfers
- ✅ Internal transfers (Wekeza to Wekeza accounts)
- ✅ External bank transfers
- ✅ Send money to mobile numbers (M-Pesa integration)
- ✅ Transfer confirmation and receipts

### Mobile Money
- ✅ Send money to own mobile number
- ✅ Send money to other mobile numbers
- ✅ M-Pesa integration
- ✅ Instant transaction processing

### Airtime Purchase
- ✅ Buy airtime for self or others
- ✅ Quick amount selection (10, 20, 50, 100, 200, 500, 1000 KES)
- ✅ Custom amount entry
- ✅ Instant airtime delivery

### Bill Payments
- ✅ Pay bills via Paybill numbers
- ✅ Pay bills via Till numbers
- ✅ Recent paybill management
- ✅ Transaction receipts

### Transaction History
- ✅ View all transactions
- ✅ Filter by date, type, status
- ✅ Transaction details with receipts
- ✅ Pull-to-refresh
- ✅ Infinite scroll/pagination

## Architecture

The app follows the MVVM (Model-View-ViewModel) pattern with clean separation of concerns:

```
Wekeza.Mobile/
├── Models/              # Data models
│   ├── AuthModels.cs
│   ├── AccountModels.cs
│   ├── TransactionModels.cs
│   └── MobileMoneyModels.cs
│
├── Services/            # Business logic & API calls
│   ├── ApiService.cs
│   ├── AuthenticationService.cs
│   ├── AccountService.cs
│   ├── TransactionService.cs
│   ├── MobileMoneyService.cs
│   ├── BillPaymentService.cs
│   └── SecureStorageService.cs
│
├── ViewModels/          # MVVM ViewModels
│   ├── LoginViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── BalanceViewModel.cs
│   ├── TransferViewModel.cs
│   ├── MobileMoneyViewModel.cs
│   ├── AirtimeViewModel.cs
│   ├── BillPaymentViewModel.cs
│   └── TransactionHistoryViewModel.cs
│
├── Views/               # XAML UI pages
│   ├── LoginPage.xaml
│   ├── DashboardPage.xaml
│   ├── BalancePage.xaml
│   ├── TransferPage.xaml
│   ├── MobileMoneyPage.xaml
│   ├── AirtimePage.xaml
│   ├── BillPaymentPage.xaml
│   └── TransactionHistoryPage.xaml
│
├── Converters/          # Value converters
├── Resources/           # Assets, fonts, styles
└── Platforms/           # Platform-specific code
```

## Technology Stack

- **.NET MAUI 8.0** - Cross-platform framework
- **CommunityToolkit.Mvvm** - MVVM helpers
- **CommunityToolkit.Maui** - Additional UI controls
- **Newtonsoft.Json** - JSON serialization
- **SecureStorage** - Encrypted storage for tokens

## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 (Windows) or VS Code (macOS/Linux)
- Android SDK (for Android development)
- Xcode (for iOS development on macOS)

## Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/eodenyire/Wekeza.git
cd Wekeza/Mobile/Wekeza.Mobile
```

### 2. Install .NET MAUI Workload

```bash
dotnet workload install maui
```

### 3. Configure API Endpoint

Update the `BaseUrl` in `Services/ApiService.cs`:

```csharp
private const string BaseUrl = "https://your-api-url.com";
// or for local development:
// private const string BaseUrl = "http://localhost:5001";
```

### 4. Restore Dependencies

```bash
dotnet restore
```

### 5. Build the Project

```bash
# For Android
dotnet build -f net8.0-android

# For iOS (macOS only)
dotnet build -f net8.0-ios

# For Windows
dotnet build -f net8.0-windows10.0.19041.0
```

### 6. Run the Application

```bash
# For Android
dotnet run -f net8.0-android

# For iOS (macOS only)
dotnet run -f net8.0-ios

# For Windows
dotnet run -f net8.0-windows10.0.19041.0
```

## API Integration

The mobile app connects to the Wekeza Core Banking API. Ensure the following endpoints are available:

### Authentication
- `POST /api/authentication/login` - User login

### Accounts
- `GET /api/accounts/{accountNumber}/balance` - Get account balance
- `GET /api/accounts/{accountNumber}/summary` - Get account summary
- `GET /api/accounts/user/accounts` - Get user's accounts

### Transactions
- `POST /api/transactions/transfer` - Transfer funds
- `GET /api/transactions/statement/{accountNumber}` - Get transaction history

### Mobile Money
- `POST /api/mobilemoney/send` - Send money to mobile
- `POST /api/mobilemoney/airtime` - Buy airtime

### Bill Payments
- `POST /api/bills/pay` - Pay bills
- `GET /api/bills/recent` - Get recent paybills

## Security

- **JWT Authentication**: All API requests use Bearer token authentication
- **Secure Storage**: Tokens and sensitive data stored encrypted
- **HTTPS Only**: All API communication over HTTPS
- **Session Management**: Automatic session timeout and renewal
- **Input Validation**: Client-side validation before API calls

## User Interface

### Design Principles
- **Material Design** inspired UI
- **Intuitive Navigation** with bottom navigation and gestures
- **Responsive Layouts** for different screen sizes
- **Accessibility** support with screen readers
- **Dark Mode** support (future enhancement)

### Color Scheme
- **Primary**: Green (#2E7D32) - Trust, growth, money
- **Secondary**: Light Green (#66BB6A) - Friendly, approachable
- **Accent**: Various for different states (success, warning, error)

## Testing

### Run Unit Tests
```bash
dotnet test
```

### Manual Testing Checklist
- [ ] Login with valid credentials
- [ ] View account balance
- [ ] Perform internal transfer
- [ ] Send money to mobile number
- [ ] Buy airtime
- [ ] Pay a bill
- [ ] View transaction history
- [ ] Logout

## Deployment

### Android
1. Generate signing key
2. Update `AndroidManifest.xml`
3. Build release APK/AAB
4. Upload to Google Play Store

### iOS
1. Configure provisioning profiles
2. Update `Info.plist`
3. Build for App Store
4. Upload via Xcode or Transporter

### Windows
1. Create MSIX package
2. Submit to Microsoft Store

## Troubleshooting

### Common Issues

**Build Errors**
- Ensure .NET 8.0 SDK is installed
- Run `dotnet workload install maui`
- Clear bin/obj folders and rebuild

**API Connection Issues**
- Verify API URL in `ApiService.cs`
- Check network connectivity
- Ensure API is running and accessible

**Login Failures**
- Verify credentials
- Check API authentication endpoint
- Review JWT token configuration

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Write/update tests
5. Submit a pull request

## License

Proprietary - © 2026 Wekeza Bank. All rights reserved.

## Support

For issues or questions:
- Email: mobile-support@wekeza.com
- Documentation: https://docs.wekeza.com/mobile
- Issues: https://github.com/eodenyire/Wekeza/issues

---

**Version**: 1.0.0  
**Last Updated**: January 2026  
**Maintained by**: Wekeza Mobile Team
