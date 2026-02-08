import 'dart:convert';
import 'dart:io';
import 'package:http/http.dart' as http;
import '../config/app_config.dart';
import 'storage_service.dart';

/// API Service for handling HTTP requests to the backend
class ApiService {
  static final ApiService _instance = ApiService._internal();
  factory ApiService() => _instance;
  ApiService._internal();

  final StorageService _storageService = StorageService();
  
  // Use the base URL from config
  String get baseUrl => AppConfig.apiBaseUrl;

  /// Get authorization headers with JWT token
  Future<Map<String, String>> _getHeaders({bool includeAuth = true}) async {
    final headers = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };

    if (includeAuth) {
      final token = await _storageService.getAuthToken();
      if (token != null && token.isNotEmpty) {
        headers['Authorization'] = 'Bearer $token';
      }
    }

    return headers;
  }

  /// Handle API response
  dynamic _handleResponse(http.Response response) {
    if (response.statusCode >= 200 && response.statusCode < 300) {
      if (response.body.isEmpty) {
        return {'success': true};
      }
      try {
        return json.decode(response.body);
      } catch (e) {
        return {'success': true, 'data': response.body};
      }
    } else {
      String errorMessage = 'Request failed with status: ${response.statusCode}';
      
      try {
        final errorData = json.decode(response.body);
        if (errorData is Map) {
          errorMessage = errorData['message'] ?? 
                        errorData['error'] ?? 
                        errorData['title'] ?? 
                        errorMessage;
        }
      } catch (e) {
        // Use default error message
      }
      
      throw ApiException(
        message: errorMessage,
        statusCode: response.statusCode,
      );
    }
  }

  /// Generic GET request
  Future<dynamic> get(
    String endpoint, {
    Map<String, dynamic>? queryParameters,
    bool requiresAuth = true,
  }) async {
    try {
      var uri = Uri.parse('$baseUrl$endpoint');
      
      if (queryParameters != null && queryParameters.isNotEmpty) {
        uri = uri.replace(queryParameters: queryParameters);
      }

      final headers = await _getHeaders(includeAuth: requiresAuth);
      final response = await http.get(uri, headers: headers)
          .timeout(AppConfig.connectionTimeout);

      return _handleResponse(response);
    } on SocketException {
      throw ApiException(message: 'No internet connection');
    } on HttpException {
      throw ApiException(message: 'Connection error');
    } on FormatException {
      throw ApiException(message: 'Bad response format');
    } catch (e) {
      if (e is ApiException) rethrow;
      throw ApiException(message: 'An error occurred: ${e.toString()}');
    }
  }

  /// Generic POST request
  Future<dynamic> post(
    String endpoint, {
    Map<String, dynamic>? body,
    bool requiresAuth = true,
  }) async {
    try {
      final uri = Uri.parse('$baseUrl$endpoint');
      final headers = await _getHeaders(includeAuth: requiresAuth);
      
      final response = await http.post(
        uri,
        headers: headers,
        body: body != null ? json.encode(body) : null,
      ).timeout(AppConfig.connectionTimeout);

      return _handleResponse(response);
    } on SocketException {
      throw ApiException(message: 'No internet connection');
    } on HttpException {
      throw ApiException(message: 'Connection error');
    } on FormatException {
      throw ApiException(message: 'Bad response format');
    } catch (e) {
      if (e is ApiException) rethrow;
      throw ApiException(message: 'An error occurred: ${e.toString()}');
    }
  }

  /// Generic PUT request
  Future<dynamic> put(
    String endpoint, {
    Map<String, dynamic>? body,
    bool requiresAuth = true,
  }) async {
    try {
      final uri = Uri.parse('$baseUrl$endpoint');
      final headers = await _getHeaders(includeAuth: requiresAuth);
      
      final response = await http.put(
        uri,
        headers: headers,
        body: body != null ? json.encode(body) : null,
      ).timeout(AppConfig.connectionTimeout);

      return _handleResponse(response);
    } on SocketException {
      throw ApiException(message: 'No internet connection');
    } on HttpException {
      throw ApiException(message: 'Connection error');
    } on FormatException {
      throw ApiException(message: 'Bad response format');
    } catch (e) {
      if (e is ApiException) rethrow;
      throw ApiException(message: 'An error occurred: ${e.toString()}');
    }
  }

  /// Generic PATCH request
  Future<dynamic> patch(
    String endpoint, {
    Map<String, dynamic>? body,
    bool requiresAuth = true,
  }) async {
    try {
      final uri = Uri.parse('$baseUrl$endpoint');
      final headers = await _getHeaders(includeAuth: requiresAuth);
      
      final response = await http.patch(
        uri,
        headers: headers,
        body: body != null ? json.encode(body) : null,
      ).timeout(AppConfig.connectionTimeout);

      return _handleResponse(response);
    } on SocketException {
      throw ApiException(message: 'No internet connection');
    } on HttpException {
      throw ApiException(message: 'Connection error');
    } on FormatException {
      throw ApiException(message: 'Bad response format');
    } catch (e) {
      if (e is ApiException) rethrow;
      throw ApiException(message: 'An error occurred: ${e.toString()}');
    }
  }

  /// Generic DELETE request
  Future<dynamic> delete(
    String endpoint, {
    bool requiresAuth = true,
  }) async {
    try {
      final uri = Uri.parse('$baseUrl$endpoint');
      final headers = await _getHeaders(includeAuth: requiresAuth);
      
      final response = await http.delete(uri, headers: headers)
          .timeout(AppConfig.connectionTimeout);

      return _handleResponse(response);
    } on SocketException {
      throw ApiException(message: 'No internet connection');
    } on HttpException {
      throw ApiException(message: 'Connection error');
    } on FormatException {
      throw ApiException(message: 'Bad response format');
    } catch (e) {
      if (e is ApiException) rethrow;
      throw ApiException(message: 'An error occurred: ${e.toString()}');
    }
  }
}

/// API Exception class
class ApiException implements Exception {
  final String message;
  final int? statusCode;

  ApiException({required this.message, this.statusCode});

  @override
  String toString() => message;
}
