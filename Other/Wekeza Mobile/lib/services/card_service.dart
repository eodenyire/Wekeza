import '../models/card_model.dart';
import '../config/app_config.dart';
import 'api_service.dart';

/// Service for handling card operations
class CardService {
  static final CardService _instance = CardService._internal();
  factory CardService() => _instance;
  CardService._internal();

  final ApiService _apiService = ApiService();

  /// Get all cards for current user
  Future<List<BankCard>> getUserCards() async {
    try {
      final response = await _apiService.get(
        '${AppConfig.cardsEndpoint}/user/cards',
      );

      if (response is List) {
        return response.map((json) => BankCard.fromJson(json)).toList();
      } else if (response is Map && response.containsKey('cards')) {
        final cards = response['cards'] as List;
        return cards.map((json) => BankCard.fromJson(json)).toList();
      }
      
      return [];
    } catch (e) {
      rethrow;
    }
  }

  /// Get cards by account ID
  Future<List<BankCard>> getCardsByAccount(String accountId) async {
    try {
      final response = await _apiService.get(
        '${AppConfig.cardsEndpoint}/account/$accountId',
      );

      if (response is List) {
        return response.map((json) => BankCard.fromJson(json)).toList();
      } else if (response is Map && response.containsKey('cards')) {
        final cards = response['cards'] as List;
        return cards.map((json) => BankCard.fromJson(json)).toList();
      }
      
      return [];
    } catch (e) {
      rethrow;
    }
  }

  /// Get card by ID
  Future<BankCard> getCardById(String cardId) async {
    try {
      final response = await _apiService.get(
        '${AppConfig.cardsEndpoint}/$cardId',
      );

      return BankCard.fromJson(response);
    } catch (e) {
      rethrow;
    }
  }

  /// Issue a new card
  Future<BankCard> issueCard(CardIssuanceRequest request) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.cardsEndpoint}/issue',
        body: request.toJson(),
      );

      return BankCard.fromJson(response);
    } catch (e) {
      rethrow;
    }
  }

  /// Activate card
  Future<void> activateCard(String cardId) async {
    try {
      await _apiService.patch(
        '${AppConfig.cardsEndpoint}/$cardId/activate',
      );
    } catch (e) {
      rethrow;
    }
  }

  /// Block/Freeze card
  Future<void> blockCard(String cardId, String reason) async {
    try {
      await _apiService.patch(
        '${AppConfig.cardsEndpoint}/$cardId/block',
        body: {
          'reason': reason,
        },
      );
    } catch (e) {
      rethrow;
    }
  }

  /// Unblock card
  Future<void> unblockCard(String cardId) async {
    try {
      await _apiService.patch(
        '${AppConfig.cardsEndpoint}/$cardId/unblock',
      );
    } catch (e) {
      rethrow;
    }
  }

  /// Cancel/Hotlist card
  Future<void> cancelCard(String cardId, String reason) async {
    try {
      await _apiService.delete(
        '${AppConfig.cardsEndpoint}/$cardId/cancel',
      );
    } catch (e) {
      rethrow;
    }
  }

  /// Update card limits
  Future<void> updateCardLimits({
    required String cardId,
    double? dailyWithdrawalLimit,
    double? dailyPurchaseLimit,
  }) async {
    try {
      await _apiService.patch(
        '${AppConfig.cardsEndpoint}/$cardId/limits',
        body: {
          if (dailyWithdrawalLimit != null) 
            'dailyWithdrawalLimit': dailyWithdrawalLimit,
          if (dailyPurchaseLimit != null) 
            'dailyPurchaseLimit': dailyPurchaseLimit,
        },
      );
    } catch (e) {
      rethrow;
    }
  }

  /// Get card transactions
  Future<List<Map<String, dynamic>>> getCardTransactions({
    required String cardId,
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
        '${AppConfig.cardsEndpoint}/$cardId/transactions',
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

  /// Request card PIN change
  Future<void> changeCardPin({
    required String cardId,
    required String currentPin,
    required String newPin,
  }) async {
    try {
      await _apiService.post(
        '${AppConfig.cardsEndpoint}/$cardId/change-pin',
        body: {
          'currentPin': currentPin,
          'newPin': newPin,
        },
      );
    } catch (e) {
      rethrow;
    }
  }
}
