import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../config/app_config.dart';

/// Service for handling local storage (both secure and insecure)
class StorageService {
  static final StorageService _instance = StorageService._internal();
  factory StorageService() => _instance;
  StorageService._internal();

  final FlutterSecureStorage _secureStorage = const FlutterSecureStorage();
  SharedPreferences? _prefs;

  /// Initialize shared preferences
  Future<void> init() async {
    _prefs ??= await SharedPreferences.getInstance();
  }

  /// Ensure SharedPreferences is initialized
  Future<SharedPreferences> get prefs async {
    if (_prefs == null) {
      await init();
    }
    return _prefs!;
  }

  // ========== Secure Storage (for sensitive data) ==========

  /// Save auth token securely
  Future<void> saveAuthToken(String token) async {
    await _secureStorage.write(key: AppConfig.keyAuthToken, value: token);
  }

  /// Get auth token
  Future<String?> getAuthToken() async {
    return await _secureStorage.read(key: AppConfig.keyAuthToken);
  }

  /// Delete auth token
  Future<void> deleteAuthToken() async {
    await _secureStorage.delete(key: AppConfig.keyAuthToken);
  }

  /// Save pin code securely
  Future<void> savePinCode(String pin) async {
    await _secureStorage.write(key: AppConfig.keyPinCode, value: pin);
  }

  /// Get pin code
  Future<String?> getPinCode() async {
    return await _secureStorage.read(key: AppConfig.keyPinCode);
  }

  /// Delete pin code
  Future<void> deletePinCode() async {
    await _secureStorage.delete(key: AppConfig.keyPinCode);
  }

  // ========== Regular Storage (for non-sensitive data) ==========

  /// Save string value
  Future<bool> saveString(String key, String value) async {
    final p = await prefs;
    return await p.setString(key, value);
  }

  /// Get string value
  Future<String?> getString(String key) async {
    final p = await prefs;
    return p.getString(key);
  }

  /// Save int value
  Future<bool> saveInt(String key, int value) async {
    final p = await prefs;
    return await p.setInt(key, value);
  }

  /// Get int value
  Future<int?> getInt(String key) async {
    final p = await prefs;
    return p.getInt(key);
  }

  /// Save bool value
  Future<bool> saveBool(String key, bool value) async {
    final p = await prefs;
    return await p.setBool(key, value);
  }

  /// Get bool value
  Future<bool?> getBool(String key) async {
    final p = await prefs;
    return p.getBool(key);
  }

  /// Save double value
  Future<bool> saveDouble(String key, double value) async {
    final p = await prefs;
    return await p.setDouble(key, value);
  }

  /// Get double value
  Future<double?> getDouble(String key) async {
    final p = await prefs;
    return p.getDouble(key);
  }

  /// Save list of strings
  Future<bool> saveStringList(String key, List<String> value) async {
    final p = await prefs;
    return await p.setStringList(key, value);
  }

  /// Get list of strings
  Future<List<String>?> getStringList(String key) async {
    final p = await prefs;
    return p.getStringList(key);
  }

  /// Save JSON object
  Future<bool> saveJson(String key, Map<String, dynamic> value) async {
    final jsonString = json.encode(value);
    return await saveString(key, jsonString);
  }

  /// Get JSON object
  Future<Map<String, dynamic>?> getJson(String key) async {
    final jsonString = await getString(key);
    if (jsonString == null) return null;
    try {
      return json.decode(jsonString) as Map<String, dynamic>;
    } catch (e) {
      return null;
    }
  }

  /// Remove a key
  Future<bool> remove(String key) async {
    final p = await prefs;
    return await p.remove(key);
  }

  /// Clear all data
  Future<bool> clearAll() async {
    // Clear secure storage
    await _secureStorage.deleteAll();
    // Clear shared preferences
    final p = await prefs;
    return await p.clear();
  }

  /// Check if key exists
  Future<bool> containsKey(String key) async {
    final p = await prefs;
    return p.containsKey(key);
  }

  // ========== User-specific Storage Helpers ==========

  /// Save user ID
  Future<void> saveUserId(String userId) async {
    await saveString(AppConfig.keyUserId, userId);
  }

  /// Get user ID
  Future<String?> getUserId() async {
    return await getString(AppConfig.keyUserId);
  }

  /// Save username
  Future<void> saveUsername(String username) async {
    await saveString(AppConfig.keyUsername, username);
  }

  /// Get username
  Future<String?> getUsername() async {
    return await getString(AppConfig.keyUsername);
  }

  /// Save user email
  Future<void> saveUserEmail(String email) async {
    await saveString(AppConfig.keyUserEmail, email);
  }

  /// Get user email
  Future<String?> getUserEmail() async {
    return await getString(AppConfig.keyUserEmail);
  }

  /// Save user roles
  Future<void> saveUserRoles(List<String> roles) async {
    await saveStringList(AppConfig.keyUserRoles, roles);
  }

  /// Get user roles
  Future<List<String>?> getUserRoles() async {
    return await getStringList(AppConfig.keyUserRoles);
  }

  /// Save remember me preference
  Future<void> saveRememberMe(bool value) async {
    await saveBool(AppConfig.keyRememberMe, value);
  }

  /// Get remember me preference
  Future<bool> getRememberMe() async {
    return await getBool(AppConfig.keyRememberMe) ?? false;
  }

  /// Save biometric enabled
  Future<void> saveBiometricEnabled(bool value) async {
    await saveBool(AppConfig.keyBiometricEnabled, value);
  }

  /// Get biometric enabled
  Future<bool> getBiometricEnabled() async {
    return await getBool(AppConfig.keyBiometricEnabled) ?? false;
  }

  /// Check if user is logged in
  Future<bool> isLoggedIn() async {
    final token = await getAuthToken();
    return token != null && token.isNotEmpty;
  }

  /// Logout (clear all user data)
  Future<void> logout() async {
    await deleteAuthToken();
    await remove(AppConfig.keyUserId);
    await remove(AppConfig.keyUsername);
    await remove(AppConfig.keyUserEmail);
    await remove(AppConfig.keyUserRoles);
    // Keep remember me and biometric preferences
  }
}
