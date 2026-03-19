import 'package:flutter/foundation.dart';
import '../models/user_model.dart';
import '../services/auth_service.dart';
import '../services/storage_service.dart';

/// Provider for managing authentication state across the app
class AuthProvider extends ChangeNotifier {
  final AuthService _authService = AuthService();
  final StorageService _storageService = StorageService();

  User? _currentUser;
  bool _isAuthenticated = false;
  bool _isLoading = false;
  String? _errorMessage;

  User? get currentUser => _currentUser;
  bool get isAuthenticated => _isAuthenticated;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  /// Login with username and password
  Future<bool> login({
    required String username,
    required String password,
    bool rememberMe = false,
  }) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      final authResponse = await _authService.login(
        username: username,
        password: password,
        rememberMe: rememberMe,
      );

      _currentUser = authResponse.user;
      _isAuthenticated = true;
      return true;
    } catch (e) {
      _errorMessage = e.toString();
      _isAuthenticated = false;
      return false;
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  /// Logout the current user
  Future<void> logout() async {
    _isLoading = true;
    notifyListeners();

    try {
      await _authService.logout();
    } finally {
      _currentUser = null;
      _isAuthenticated = false;
      _isLoading = false;
      notifyListeners();
    }
  }

  /// Load the currently authenticated user from storage
  Future<void> loadUserFromStorage() async {
    _isLoading = true;
    notifyListeners();

    try {
      final isLoggedIn = await _storageService.isLoggedIn();
      if (isLoggedIn) {
        await _authService.loadUserFromStorage();
        _currentUser = _authService.currentUser;
        _isAuthenticated = _currentUser != null;
      } else {
        _isAuthenticated = false;
      }
    } catch (e) {
      _isAuthenticated = false;
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  /// Refresh the current user's data from the API
  Future<void> refreshUser() async {
    try {
      _currentUser = await _authService.getCurrentUser();
      _isAuthenticated = true;
      notifyListeners();
    } catch (e) {
      _isAuthenticated = false;
      notifyListeners();
    }
  }

  /// Clear any stored error message
  void clearError() {
    _errorMessage = null;
    notifyListeners();
  }
}
