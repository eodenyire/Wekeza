/// Application configuration constants
class AppConfig {
  // API Configuration
  static const String apiBaseUrl = 'http://localhost:5000/api';
  static const String apiBaseUrlHttps = 'https://localhost:5001/api';
  
  // Use this for production
  // static const String apiBaseUrl = 'https://api.wekeza.com/api';
  
  // API Endpoints
  static const String authEndpoint = '/authentication';
  static const String accountsEndpoint = '/accounts';
  static const String transactionsEndpoint = '/transactions';
  static const String loansEndpoint = '/loans';
  static const String cardsEndpoint = '/cards';
  static const String paymentsEndpoint = '/payments';
  static const String depositsEndpoint = '/deposits';
  static const String dashboardEndpoint = '/dashboard';
  
  // App Information
  static const String appName = 'Wekeza Mobile';
  static const String appVersion = '1.0.0';
  static const String companyName = 'Wekeza Bank';
  
  // Security
  static const String jwtSecretKey = 'WekeZa-BaNk-SuPeR-SeCrEt-KeY-2026-MuSt-Be-At-LeAsT-32-ChArS-LoNg!';
  static const int tokenExpiryMinutes = 60;
  
  // Storage Keys
  static const String keyAuthToken = 'auth_token';
  static const String keyUserId = 'user_id';
  static const String keyUsername = 'username';
  static const String keyUserEmail = 'user_email';
  static const String keyUserRoles = 'user_roles';
  static const String keyRememberMe = 'remember_me';
  static const String keyBiometricEnabled = 'biometric_enabled';
  static const String keyPinCode = 'pin_code';
  
  // Timeouts
  static const Duration connectionTimeout = Duration(seconds: 30);
  static const Duration receiveTimeout = Duration(seconds: 30);
  
  // Pagination
  static const int defaultPageSize = 20;
  static const int maxPageSize = 100;
  
  // Currency
  static const String defaultCurrency = 'KES';
  static const String currencySymbol = 'KES';
  
  // Transaction Limits
  static const double maxTransferAmount = 1000000.0;
  static const double minTransferAmount = 1.0;
  
  // Theme Colors
  static const int primaryColorValue = 0xFF2E7D32; // Green
  static const int accentColorValue = 0xFF4CAF50;
  static const int errorColorValue = 0xFFD32F2F;
  static const int successColorValue = 0xFF388E3C;
  static const int warningColorValue = 0xFFFFA000;
  
  // Contact Information
  static const String supportEmail = 'support@wekeza.com';
  static const String supportPhone = '+254-700-123-456';
  static const String websiteUrl = 'https://wekeza.com';
}
