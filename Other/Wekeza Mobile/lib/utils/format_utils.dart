import 'package:intl/intl.dart';

/// Utility functions for formatting
class FormatUtils {
  /// Format currency with symbol and decimal places
  static String formatCurrency(double amount, {String currency = 'KES', int decimals = 2}) {
    final formatter = NumberFormat('#,##0.${'0' * decimals}');
    return '$currency ${formatter.format(amount)}';
  }

  /// Format date in readable format
  static String formatDate(DateTime date, {String format = 'dd MMM yyyy'}) {
    final formatter = DateFormat(format);
    return formatter.format(date);
  }

  /// Format datetime with time
  static String formatDateTime(DateTime dateTime, {String format = 'dd MMM yyyy, hh:mm a'}) {
    final formatter = DateFormat(format);
    return formatter.format(dateTime);
  }

  /// Format account number with spaces
  static String formatAccountNumber(String accountNumber) {
    if (accountNumber.length <= 4) return accountNumber;
    
    // Format as: XXXX XXXX XXXX
    final buffer = StringBuffer();
    for (int i = 0; i < accountNumber.length; i++) {
      if (i > 0 && i % 4 == 0) {
        buffer.write(' ');
      }
      buffer.write(accountNumber[i]);
    }
    return buffer.toString();
  }

  /// Mask account number for security
  static String maskAccountNumber(String accountNumber) {
    if (accountNumber.length <= 4) return accountNumber;
    
    final visibleDigits = 4;
    final maskedLength = accountNumber.length - visibleDigits;
    final masked = '*' * maskedLength;
    return masked + accountNumber.substring(maskedLength);
  }

  /// Format phone number
  static String formatPhoneNumber(String phoneNumber) {
    // Remove non-numeric characters
    final cleaned = phoneNumber.replaceAll(RegExp(r'\D'), '');
    
    if (cleaned.length == 10) {
      // Format as: 0XXX XXX XXX
      return '${cleaned.substring(0, 4)} ${cleaned.substring(4, 7)} ${cleaned.substring(7)}';
    } else if (cleaned.length == 12) {
      // Format as: +254 XXX XXX XXX
      return '+${cleaned.substring(0, 3)} ${cleaned.substring(3, 6)} ${cleaned.substring(6, 9)} ${cleaned.substring(9)}';
    }
    
    return phoneNumber;
  }

  /// Format percentage
  static String formatPercentage(double value, {int decimals = 1}) {
    return '${value.toStringAsFixed(decimals)}%';
  }

  /// Get relative time (e.g., "2 hours ago")
  static String getRelativeTime(DateTime dateTime) {
    final now = DateTime.now();
    final difference = now.difference(dateTime);

    if (difference.inSeconds < 60) {
      return 'Just now';
    } else if (difference.inMinutes < 60) {
      return '${difference.inMinutes}m ago';
    } else if (difference.inHours < 24) {
      return '${difference.inHours}h ago';
    } else if (difference.inDays < 7) {
      return '${difference.inDays}d ago';
    } else if (difference.inDays < 30) {
      return '${(difference.inDays / 7).floor()}w ago';
    } else if (difference.inDays < 365) {
      return '${(difference.inDays / 30).floor()}mo ago';
    } else {
      return '${(difference.inDays / 365).floor()}y ago';
    }
  }

  /// Format file size
  static String formatFileSize(int bytes) {
    if (bytes < 1024) {
      return '$bytes B';
    } else if (bytes < 1024 * 1024) {
      return '${(bytes / 1024).toStringAsFixed(1)} KB';
    } else if (bytes < 1024 * 1024 * 1024) {
      return '${(bytes / (1024 * 1024)).toStringAsFixed(1)} MB';
    } else {
      return '${(bytes / (1024 * 1024 * 1024)).toStringAsFixed(1)} GB';
    }
  }

  /// Capitalize first letter
  static String capitalize(String text) {
    if (text.isEmpty) return text;
    return text[0].toUpperCase() + text.substring(1);
  }

  /// Truncate text with ellipsis
  static String truncate(String text, int maxLength, {String ellipsis = '...'}) {
    if (text.length <= maxLength) return text;
    return text.substring(0, maxLength - ellipsis.length) + ellipsis;
  }
}

/// Validation utilities
class ValidationUtils {
  /// Validate email format
  static bool isValidEmail(String email) {
    final emailRegex = RegExp(
      r'^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$',
    );
    return emailRegex.hasMatch(email);
  }

  /// Validate phone number (Kenyan format)
  static bool isValidPhoneNumber(String phoneNumber) {
    // Remove non-numeric characters
    final cleaned = phoneNumber.replaceAll(RegExp(r'\D'), '');
    
    // Check for Kenyan phone numbers (07XX XXX XXX or 01XX XXX XXX or +254...)
    return cleaned.length == 10 || cleaned.length == 12;
  }

  /// Validate password strength
  static bool isStrongPassword(String password) {
    // At least 8 characters, one uppercase, one lowercase, one digit, one special char
    if (password.length < 8) return false;
    
    final hasUppercase = password.contains(RegExp(r'[A-Z]'));
    final hasLowercase = password.contains(RegExp(r'[a-z]'));
    final hasDigit = password.contains(RegExp(r'[0-9]'));
    final hasSpecialChar = password.contains(RegExp(r'[!@#$%^&*(),.?":{}|<>]'));
    
    return hasUppercase && hasLowercase && hasDigit && hasSpecialChar;
  }

  /// Validate amount
  static bool isValidAmount(String amount) {
    final amountDouble = double.tryParse(amount);
    return amountDouble != null && amountDouble > 0;
  }

  /// Validate account number
  static bool isValidAccountNumber(String accountNumber) {
    // Remove spaces and check if it's all digits
    final cleaned = accountNumber.replaceAll(' ', '');
    return cleaned.isNotEmpty && RegExp(r'^\d+$').hasMatch(cleaned);
  }
}
