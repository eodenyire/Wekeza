import '../models/user_model.dart';
import '../config/app_config.dart';
import 'api_service.dart';
import 'storage_service.dart';

/// Service for handling authentication
class AuthService {
  static final AuthService _instance = AuthService._internal();
  factory AuthService() => _instance;
  AuthService._internal();

  final ApiService _apiService = ApiService();
  final StorageService _storageService = StorageService();

  User? _currentUser;

  /// Get current user
  User? get currentUser => _currentUser;

  /// Login with username and password
  Future<AuthResponse> login({
    required String username,
    required String password,
    bool rememberMe = false,
  }) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.authEndpoint}/login',
        body: {
          'username': username,
          'password': password,
        },
        requiresAuth: false,
      );

      final authResponse = AuthResponse.fromJson(response);
      
      // Save auth data
      await _storageService.saveAuthToken(authResponse.token);
      await _storageService.saveUserId(authResponse.user.id);
      await _storageService.saveUsername(authResponse.user.username);
      await _storageService.saveUserEmail(authResponse.user.email);
      await _storageService.saveUserRoles(authResponse.user.roles);
      await _storageService.saveRememberMe(rememberMe);

      _currentUser = authResponse.user;

      return authResponse;
    } catch (e) {
      rethrow;
    }
  }

  /// Register new user
  Future<AuthResponse> register({
    required String username,
    required String email,
    required String password,
    required String firstName,
    required String lastName,
    String? phoneNumber,
  }) async {
    try {
      final response = await _apiService.post(
        '${AppConfig.authEndpoint}/register',
        body: {
          'username': username,
          'email': email,
          'password': password,
          'firstName': firstName,
          'lastName': lastName,
          'phoneNumber': phoneNumber,
        },
        requiresAuth: false,
      );

      final authResponse = AuthResponse.fromJson(response);
      
      // Save auth data
      await _storageService.saveAuthToken(authResponse.token);
      await _storageService.saveUserId(authResponse.user.id);
      await _storageService.saveUsername(authResponse.user.username);
      await _storageService.saveUserEmail(authResponse.user.email);
      await _storageService.saveUserRoles(authResponse.user.roles);

      _currentUser = authResponse.user;

      return authResponse;
    } catch (e) {
      rethrow;
    }
  }

  /// Get current user info from API
  Future<User> getCurrentUser() async {
    try {
      final response = await _apiService.get(
        '${AppConfig.authEndpoint}/me',
        requiresAuth: true,
      );

      _currentUser = User.fromJson(response);
      return _currentUser!;
    } catch (e) {
      rethrow;
    }
  }

  /// Check if user is authenticated
  Future<bool> isAuthenticated() async {
    final token = await _storageService.getAuthToken();
    if (token == null || token.isEmpty) {
      return false;
    }

    // Optionally verify token with API
    try {
      await getCurrentUser();
      return true;
    } catch (e) {
      return false;
    }
  }

  /// Logout
  Future<void> logout() async {
    try {
      // Optionally call logout API endpoint
      // await _apiService.post('${AppConfig.authEndpoint}/logout');
      
      await _storageService.logout();
      _currentUser = null;
    } catch (e) {
      // Even if API call fails, clear local data
      await _storageService.logout();
      _currentUser = null;
    }
  }

  /// Change password
  Future<void> changePassword({
    required String currentPassword,
    required String newPassword,
  }) async {
    try {
      await _apiService.post(
        '${AppConfig.authEndpoint}/change-password',
        body: {
          'currentPassword': currentPassword,
          'newPassword': newPassword,
        },
      );
    } catch (e) {
      rethrow;
    }
  }

  /// Request password reset
  Future<void> requestPasswordReset(String email) async {
    try {
      await _apiService.post(
        '${AppConfig.authEndpoint}/forgot-password',
        body: {'email': email},
        requiresAuth: false,
      );
    } catch (e) {
      rethrow;
    }
  }

  /// Reset password with token
  Future<void> resetPassword({
    required String token,
    required String newPassword,
  }) async {
    try {
      await _apiService.post(
        '${AppConfig.authEndpoint}/reset-password',
        body: {
          'token': token,
          'newPassword': newPassword,
        },
        requiresAuth: false,
      );
    } catch (e) {
      rethrow;
    }
  }

  /// Refresh auth token
  Future<String> refreshToken() async {
    try {
      final response = await _apiService.post(
        '${AppConfig.authEndpoint}/refresh-token',
      );

      final newToken = response['token'] as String;
      await _storageService.saveAuthToken(newToken);
      
      return newToken;
    } catch (e) {
      rethrow;
    }
  }

  /// Load user from storage
  Future<void> loadUserFromStorage() async {
    try {
      final userId = await _storageService.getUserId();
      final username = await _storageService.getUsername();
      final email = await _storageService.getUserEmail();
      final roles = await _storageService.getUserRoles();

      if (userId != null && username != null && email != null) {
        _currentUser = User(
          id: userId,
          username: username,
          email: email,
          roles: roles ?? [],
        );
      }
    } catch (e) {
      // Ignore errors
    }
  }
}
