# Wekeza Mobile - Flutter Banking Application

<div align="center">
  <h1>🏦 Wekeza Mobile Banking</h1>
  <p>A comprehensive mobile banking application built with Flutter/Dart</p>
  <p>Integrated with Wekeza Core Banking System</p>
</div>

---

## 📱 Overview

Wekeza Mobile is a full-featured mobile banking application developed using Flutter and Dart. It provides customers with seamless access to their Wekeza Bank accounts, enabling them to perform various banking operations from their mobile devices.

The app is designed to work like modern mobile banking apps (KCB, NCBA, Equity Bank) with a clean, intuitive interface and robust functionality.

## ✨ Features

### 🔐 Authentication & Security
- ✅ Secure login with JWT token authentication
- ✅ Automatic session management
- ✅ Secure token storage using device encryption
- ✅ Auto-login for returning users
- ✅ Password validation

### 💼 Account Management
- ✅ View all user accounts
- ✅ Real-time balance display
- ✅ Account details and statements
- ✅ Multiple account support
- ✅ Account status monitoring

### 💸 Transactions
- ✅ View transaction history
- ✅ Filter transactions by account
- ✅ Transaction details with reference numbers
- ✅ Transaction status tracking
- ✅ Paginated transaction lists

### 🔄 Money Transfers
- ✅ Internal transfers (Wekeza to Wekeza)
- ✅ External transfers (to other banks)
- ✅ Transfer confirmation
- ✅ Balance validation
- ✅ Transfer limits enforcement

### 📱 Mobile Money (Coming Soon)
- ⏳ M-Pesa integration
- ⏳ Send money to mobile
- ⏳ Buy airtime
- ⏳ Pay bills (Paybill/Till)

### 🏦 Loan Management
- ✅ View all loans
- ✅ Loan details and status
- ✅ Repayment tracking
- ✅ Loan progress visualization
- ⏳ Apply for new loans
- ⏳ Make loan repayments

### 💳 Card Management
- ✅ View all cards
- ✅ Card details display
- ✅ Beautiful card UI design
- ✅ Card status monitoring
- ⏳ Block/unblock cards
- ⏳ Request new cards
- ⏳ View card transactions

## 🛠️ Technology Stack

- **Flutter** - Cross-platform mobile framework
- **Dart** - Programming language
- **Provider** - State management
- **HTTP/Dio** - API communication
- **Shared Preferences** - Local storage
- **Flutter Secure Storage** - Encrypted storage
- **Intl** - Internationalization and formatting

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- Flutter SDK (3.0.0 or higher)
- Dart SDK (3.0.0 or higher)
- Android Studio / Xcode (for mobile development)
- VS Code or Android Studio IDE
- Git

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/eodenyire/Wekeza.git
cd Wekeza/Wekeza\ Mobile
```

### 2. Install Dependencies

```bash
flutter pub get
```

### 3. Configure API Endpoint

Edit the file `lib/config/app_config.dart` and update the API base URL:

```dart
static const String apiBaseUrl = 'http://your-api-url:5000/api';
```

For local development:
```dart
static const String apiBaseUrl = 'http://localhost:5000/api';
```

For Android emulator accessing localhost:
```dart
static const String apiBaseUrl = 'http://10.0.2.2:5000/api';
```

### 4. Run the Application

#### Android
```bash
flutter run
```

#### iOS
```bash
flutter run --device-id=<your-ios-device-id>
```

#### Chrome (for web)
```bash
flutter run -d chrome
```

## 📱 Building for Production

### Android APK
```bash
flutter build apk --release
```

### Android App Bundle (for Google Play)
```bash
flutter build appbundle --release
```

### iOS
```bash
flutter build ios --release
```

## 🏗️ Project Structure

```
lib/
├── config/
│   └── app_config.dart          # App configuration and constants
├── models/
│   ├── user_model.dart          # User data model
│   ├── account_model.dart       # Account data model
│   ├── transaction_model.dart   # Transaction data model
│   ├── loan_model.dart          # Loan data model
│   └── card_model.dart          # Card data model
├── services/
│   ├── api_service.dart         # HTTP client service
│   ├── auth_service.dart        # Authentication service
│   ├── storage_service.dart     # Local storage service
│   ├── account_service.dart     # Account operations
│   ├── transaction_service.dart # Transaction operations
│   ├── loan_service.dart        # Loan operations
│   └── card_service.dart        # Card operations
├── screens/
│   ├── login_screen.dart        # Login page
│   ├── dashboard_screen.dart    # Main dashboard
│   ├── accounts_screen.dart     # Accounts list
│   ├── transactions_screen.dart # Transaction history
│   ├── transfer_screen.dart     # Money transfer
│   ├── loans_screen.dart        # Loans management
│   └── cards_screen.dart        # Cards management
├── widgets/
│   └── (reusable widgets)
├── utils/
│   └── (helper functions)
└── main.dart                    # App entry point
```

## 🔧 Configuration

### API Endpoints

The app connects to the following API endpoints:

- **Authentication**: `/api/authentication/*`
- **Accounts**: `/api/accounts/*`
- **Transactions**: `/api/transactions/*`
- **Loans**: `/api/loans/*`
- **Cards**: `/api/cards/*`
- **Payments**: `/api/payments/*`

### Security Configuration

JWT tokens are stored securely using Flutter Secure Storage:

- Token encryption at device level
- Automatic token refresh
- Secure session management

### Default Settings

- **Connection Timeout**: 30 seconds
- **Receive Timeout**: 30 seconds
- **Token Expiry**: 60 minutes
- **Currency**: KES (Kenyan Shilling)
- **Max Transfer Amount**: KES 1,000,000

## 🎨 UI/UX Design

### Color Scheme
- **Primary Color**: #2E7D32 (Green)
- **Accent Color**: #4CAF50 (Light Green)
- **Error Color**: #D32F2F (Red)
- **Success Color**: #388E3C (Green)

### Design Principles
- Material Design 3
- Clean and minimalist interface
- Intuitive navigation
- Responsive layouts
- Consistent color scheme

## 🧪 Testing

Run unit tests:
```bash
flutter test
```

Run integration tests:
```bash
flutter test integration_test
```

## 📦 Dependencies

Main dependencies used in the project:

```yaml
dependencies:
  flutter:
    sdk: flutter
  provider: ^6.1.1
  http: ^1.2.0
  dio: ^5.4.0
  shared_preferences: ^2.2.2
  flutter_secure_storage: ^9.0.0
  intl: ^0.19.0
  uuid: ^4.3.3
  cached_network_image: ^3.3.1
```

## 🔐 Security Best Practices

1. **Token Storage**: JWT tokens stored in encrypted storage
2. **HTTPS Only**: All API calls use HTTPS in production
3. **Input Validation**: Client-side validation before API calls
4. **Session Timeout**: Automatic logout after token expiration
5. **No Sensitive Logging**: Passwords and tokens never logged

## 🐛 Troubleshooting

### Common Issues

**Issue**: Cannot connect to API
- **Solution**: Check API base URL in `app_config.dart`
- For Android emulator: Use `http://10.0.2.2:5000/api`
- For iOS simulator: Use `http://localhost:5000/api`

**Issue**: Login fails
- **Solution**: Verify backend is running and accessible
- Check network connectivity
- Verify credentials are correct

**Issue**: Build fails
- **Solution**: Run `flutter clean` then `flutter pub get`
- Update Flutter SDK: `flutter upgrade`

## 📞 Support

For technical support or questions:
- **Email**: support@wekeza.com
- **Phone**: +254-700-123-456
- **GitHub Issues**: [Create an issue](https://github.com/eodenyire/Wekeza/issues)

## 📄 License

Proprietary - © 2026 Wekeza Bank. All rights reserved.

## 👥 Contributors

- **Development Team**: Wekeza Engineering Team
- **Project Lead**: Emmanuel Odenyire

## 🗺️ Roadmap

### Version 2.0 (Planned)
- [ ] Biometric authentication (fingerprint/face ID)
- [ ] Push notifications
- [ ] QR code payments
- [ ] Statement downloads (PDF)
- [ ] Dark mode support
- [ ] Multi-language support (English, Swahili)

### Version 3.0 (Planned)
- [ ] Investment tracking
- [ ] Savings goals
- [ ] Budget management
- [ ] Financial insights
- [ ] Peer-to-peer payments

## 🙏 Acknowledgments

- Flutter team for the amazing framework
- Wekeza Bank for the opportunity
- All contributors and testers

---

**Built with ❤️ by the Wekeza Engineering Team**
