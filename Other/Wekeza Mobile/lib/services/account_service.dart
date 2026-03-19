import '../models/account_model.dart';
import '../config/app_config.dart';
import 'api_service.dart';

/// Service for handling account-related operations
class AccountService {
  static final AccountService _instance = AccountService._internal();
  factory AccountService() => _instance;
  AccountService._internal();

  final ApiService _apiService = ApiService();

  /// Get all accounts for current user
  Future<List<Account>> getUserAccounts() async {
    try {
      final response = await _apiService.get(
        '${AppConfig.accountsEndpoint}/user/accounts',
      );

      if (response is List) {
        return response.map((json) => Account.fromJson(json)).toList();
      } else if (response is Map && response.containsKey('accounts')) {
        final accounts = response['accounts'] as List;
        return accounts.map((json) => Account.fromJson(json)).toList();
      }
      
      return [];
    } catch (e) {
      rethrow;
    }
  }

  /// Get account by ID
  Future<Account> getAccountById(String accountId) async {
    try {
      final response = await _apiService.get(
        '${AppConfig.accountsEndpoint}/$accountId',
      );

      return Account.fromJson(response);
    } catch (e) {
      rethrow;
    }
  }

  /// Get account by account number
  Future<Account> getAccountByNumber(String accountNumber) async {
    try {
      final response = await _apiService.get(
        '${AppConfig.accountsEndpoint}/number/$accountNumber',
      );

      return Account.fromJson(response);
    } catch (e) {
      rethrow;
    }
  }

  /// Get account balance
  Future<AccountBalance> getAccountBalance(String accountNumber) async {
    try {
      final response = await _apiService.get(
        '${AppConfig.accountsEndpoint}/$accountNumber/balance',
      );

      return AccountBalance.fromJson(response);
    } catch (e) {
      rethrow;
    }
  }

  /// Get account summary
  Future<Map<String, dynamic>> getAccountSummary(String accountNumber) async {
    try {
      final response = await _apiService.get(
        '${AppConfig.accountsEndpoint}/$accountNumber/summary',
      );

      return response as Map<String, dynamic>;
    } catch (e) {
      rethrow;
    }
  }

  /// Open individual account
  Future<Account> openIndividualAccount({
    required String customerId,
    required String accountType,
    required String currency,
    String? initialDeposit,
  }) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.accountsEndpoint}/individual',
        body: {
          'customerId': customerId,
          'accountType': accountType,
          'currency': currency,
          if (initialDeposit != null) 'initialDeposit': initialDeposit,
        },
      );

      return Account.fromJson(response);
    } catch (e) {
      rethrow;
    }
  }

  /// Open business account
  Future<Account> openBusinessAccount({
    required String customerId,
    required String businessName,
    required String businessType,
    required String accountType,
    required String currency,
    String? initialDeposit,
  }) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.accountsEndpoint}/business',
        body: {
          'customerId': customerId,
          'businessName': businessName,
          'businessType': businessType,
          'accountType': accountType,
          'currency': currency,
          if (initialDeposit != null) 'initialDeposit': initialDeposit,
        },
      );

      return Account.fromJson(response);
    } catch (e) {
      rethrow;
    }
  }

  /// Freeze account
  Future<void> freezeAccount(String accountId, String reason) async {
    try {
      await _apiService.post(
        '${AppConfig.accountsEndpoint}/freeze',
        body: {
          'accountId': accountId,
          'reason': reason,
        },
      );
    } catch (e) {
      rethrow;
    }
  }

  /// Unfreeze account
  Future<void> unfreezeAccount(String accountId) async {
    try {
      await _apiService.post(
        '${AppConfig.accountsEndpoint}/unfreeze',
        body: {
          'accountId': accountId,
        },
      );
    } catch (e) {
      rethrow;
    }
  }

  /// Close account
  Future<void> closeAccount(String accountId, String reason) async {
    try {
      await _apiService.post(
        '${AppConfig.accountsEndpoint}/close',
        body: {
          'accountId': accountId,
          'reason': reason,
        },
      );
    } catch (e) {
      rethrow;
    }
  }

  /// Get account statement
  Future<List<Map<String, dynamic>>> getAccountStatement({
    required String accountNumber,
    DateTime? startDate,
    DateTime? endDate,
    int? pageNumber,
    int? pageSize,
  }) async {
    try {
      final queryParams = <String, dynamic>{};
      
      if (startDate != null) {
        queryParams['startDate'] = startDate.toIso8601String();
      }
      if (endDate != null) {
        queryParams['endDate'] = endDate.toIso8601String();
      }
      if (pageNumber != null) {
        queryParams['pageNumber'] = pageNumber.toString();
      }
      if (pageSize != null) {
        queryParams['pageSize'] = pageSize.toString();
      }

      final response = await _apiService.get(
        '${AppConfig.accountsEndpoint}/$accountNumber/statement',
        queryParameters: queryParams,
      );

      if (response is List) {
        return response.cast<Map<String, dynamic>>();
      } else if (response is Map && response.containsKey('transactions')) {
        return (response['transactions'] as List).cast<Map<String, dynamic>>();
      }
      
      return [];
    } catch (e) {
      rethrow;
    }
  }
}
