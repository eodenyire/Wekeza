import '../models/transaction_model.dart';
import '../config/app_config.dart';
import 'api_service.dart';

/// Service for handling transaction operations
class TransactionService {
  static final TransactionService _instance = TransactionService._internal();
  factory TransactionService() => _instance;
  TransactionService._internal();

  final ApiService _apiService = ApiService();

  /// Transfer funds between accounts
  Future<Map<String, dynamic>> transferFunds(TransferRequest request) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.transactionsEndpoint}/transfer',
        body: request.toJson(),
      );

      return response as Map<String, dynamic>;
    } catch (e) {
      rethrow;
    }
  }

  /// Make internal transfer
  Future<Map<String, dynamic>> internalTransfer({
    required String fromAccountId,
    required String toAccountId,
    required double amount,
    String? description,
  }) async {
    final request = TransferRequest(
      fromAccountId: fromAccountId,
      toAccountId: toAccountId,
      amount: amount,
      description: description,
      transferType: 'Internal',
    );
    return await transferFunds(request);
  }

  /// Make external transfer
  Future<Map<String, dynamic>> externalTransfer({
    required String fromAccountId,
    required String toAccountId,
    required double amount,
    String? description,
  }) async {
    final request = TransferRequest(
      fromAccountId: fromAccountId,
      toAccountId: toAccountId,
      amount: amount,
      description: description,
      transferType: 'External',
    );
    return await transferFunds(request);
  }

  /// Deposit funds
  Future<Map<String, dynamic>> depositFunds({
    required String accountId,
    required double amount,
    String? description,
    String? depositType,
  }) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.transactionsEndpoint}/deposit',
        body: {
          'accountId': accountId,
          'amount': amount,
          'description': description ?? 'Deposit',
          'depositType': depositType ?? 'Cash',
        },
      );

      return response as Map<String, dynamic>;
    } catch (e) {
      rethrow;
    }
  }

  /// Mobile money deposit (M-Pesa)
  Future<Map<String, dynamic>> mobileMoneyDeposit({
    required String accountId,
    required String phoneNumber,
    required double amount,
  }) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.transactionsEndpoint}/deposit/mobile',
        body: {
          'accountId': accountId,
          'phoneNumber': phoneNumber,
          'amount': amount,
        },
      );

      return response as Map<String, dynamic>;
    } catch (e) {
      rethrow;
    }
  }

  /// Withdraw funds
  Future<Map<String, dynamic>> withdrawFunds({
    required String accountId,
    required double amount,
    String? description,
  }) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.transactionsEndpoint}/withdraw',
        body: {
          'accountId': accountId,
          'amount': amount,
          'description': description ?? 'Withdrawal',
        },
      );

      return response as Map<String, dynamic>;
    } catch (e) {
      rethrow;
    }
  }

  /// Get transaction history
  Future<List<Transaction>> getTransactionHistory({
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
        '${AppConfig.transactionsEndpoint}/statement/$accountNumber',
        queryParameters: queryParams,
      );

      if (response is List) {
        return response.map((json) => Transaction.fromJson(json)).toList();
      } else if (response is Map && response.containsKey('transactions')) {
        final transactions = response['transactions'] as List;
        return transactions.map((json) => Transaction.fromJson(json)).toList();
      }
      
      return [];
    } catch (e) {
      rethrow;
    }
  }

  /// Get transaction by ID
  Future<Transaction> getTransactionById(String transactionId) async {
    try {
      final response = await _apiService.get(
        '${AppConfig.transactionsEndpoint}/$transactionId',
      );

      return Transaction.fromJson(response);
    } catch (e) {
      rethrow;
    }
  }

  /// Get transaction by reference number
  Future<Transaction> getTransactionByReference(String referenceNumber) async {
    try {
      final response = await _apiService.get(
        '${AppConfig.transactionsEndpoint}/reference/$referenceNumber',
      );

      return Transaction.fromJson(response);
    } catch (e) {
      rethrow;
    }
  }

  /// Send money to mobile (M-Pesa)
  Future<Map<String, dynamic>> sendToMobile({
    required String fromAccountId,
    required String toPhoneNumber,
    required double amount,
    String? description,
  }) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.transactionsEndpoint}/send-to-mobile',
        body: {
          'fromAccountId': fromAccountId,
          'toPhoneNumber': toPhoneNumber,
          'amount': amount,
          'description': description ?? 'Send to Mobile',
        },
      );

      return response as Map<String, dynamic>;
    } catch (e) {
      rethrow;
    }
  }

  /// Buy airtime
  Future<Map<String, dynamic>> buyAirtime({
    required String fromAccountId,
    required String phoneNumber,
    required double amount,
  }) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.transactionsEndpoint}/airtime',
        body: {
          'fromAccountId': fromAccountId,
          'phoneNumber': phoneNumber,
          'amount': amount,
        },
      );

      return response as Map<String, dynamic>;
    } catch (e) {
      rethrow;
    }
  }

  /// Pay bill (Paybill or Till)
  Future<Map<String, dynamic>> payBill({
    required String fromAccountId,
    required String businessNumber,
    String? accountNumber,
    required double amount,
    String? description,
  }) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.paymentsEndpoint}/bill',
        body: {
          'fromAccountId': fromAccountId,
          'businessNumber': businessNumber,
          'accountNumber': accountNumber,
          'amount': amount,
          'description': description ?? 'Bill Payment',
        },
      );

      return response as Map<String, dynamic>;
    } catch (e) {
      rethrow;
    }
  }
}
