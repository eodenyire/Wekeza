import '../models/loan_model.dart';
import '../config/app_config.dart';
import 'api_service.dart';

/// Service for handling loan operations
class LoanService {
  static final LoanService _instance = LoanService._internal();
  factory LoanService() => _instance;
  LoanService._internal();

  final ApiService _apiService = ApiService();

  /// Get all loans for current user
  Future<List<Loan>> getUserLoans() async {
    try {
      final response = await _apiService.get(
        '${AppConfig.loansEndpoint}/user/loans',
      );

      if (response is List) {
        return response.map((json) => Loan.fromJson(json)).toList();
      } else if (response is Map && response.containsKey('loans')) {
        final loans = response['loans'] as List;
        return loans.map((json) => Loan.fromJson(json)).toList();
      }
      
      return [];
    } catch (e) {
      rethrow;
    }
  }

  /// Get loan by ID
  Future<Loan> getLoanById(String loanId) async {
    try {
      final response = await _apiService.get(
        '${AppConfig.loansEndpoint}/$loanId',
      );

      return Loan.fromJson(response);
    } catch (e) {
      rethrow;
    }
  }

  /// Apply for a loan
  Future<Loan> applyForLoan(LoanApplicationRequest request) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.loansEndpoint}/apply',
        body: request.toJson(),
      );

      return Loan.fromJson(response);
    } catch (e) {
      rethrow;
    }
  }

  /// Make loan repayment
  Future<Map<String, dynamic>> makeRepayment(LoanRepaymentRequest request) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.loansEndpoint}/repayment',
        body: request.toJson(),
      );

      return response as Map<String, dynamic>;
    } catch (e) {
      rethrow;
    }
  }

  /// Get loan repayment schedule
  Future<List<Map<String, dynamic>>> getRepaymentSchedule(String loanId) async {
    try {
      final response = await _apiService.get(
        '${AppConfig.loansEndpoint}/$loanId/schedule',
      );

      if (response is List) {
        return response.cast<Map<String, dynamic>>();
      } else if (response is Map && response.containsKey('schedule')) {
        return (response['schedule'] as List).cast<Map<String, dynamic>>();
      }
      
      return [];
    } catch (e) {
      rethrow;
    }
  }

  /// Get loan repayment history
  Future<List<Map<String, dynamic>>> getRepaymentHistory(String loanId) async {
    try {
      final response = await _apiService.get(
        '${AppConfig.loansEndpoint}/$loanId/repayments',
      );

      if (response is List) {
        return response.cast<Map<String, dynamic>>();
      } else if (response is Map && response.containsKey('repayments')) {
        return (response['repayments'] as List).cast<Map<String, dynamic>>();
      }
      
      return [];
    } catch (e) {
      rethrow;
    }
  }

  /// Calculate loan eligibility
  Future<Map<String, dynamic>> calculateEligibility({
    required String customerId,
    required double monthlyIncome,
    required double requestedAmount,
    required int tenorMonths,
  }) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.loansEndpoint}/calculate-eligibility',
        body: {
          'customerId': customerId,
          'monthlyIncome': monthlyIncome,
          'requestedAmount': requestedAmount,
          'tenorMonths': tenorMonths,
        },
      );

      return response as Map<String, dynamic>;
    } catch (e) {
      rethrow;
    }
  }

  /// Get loan products
  Future<List<Map<String, dynamic>>> getLoanProducts() async {
    try {
      final response = await _apiService.get(
        '${AppConfig.loansEndpoint}/products',
      );

      if (response is List) {
        return response.cast<Map<String, dynamic>>();
      } else if (response is Map && response.containsKey('products')) {
        return (response['products'] as List).cast<Map<String, dynamic>>();
      }
      
      return [];
    } catch (e) {
      rethrow;
    }
  }

  /// Get loan summary
  Future<Map<String, dynamic>> getLoanSummary(String loanId) async {
    try {
      final response = await _apiService.get(
        '${AppConfig.loansEndpoint}/$loanId/summary',
      );

      return response as Map<String, dynamic>;
    } catch (e) {
      rethrow;
    }
  }
}
