/// Account model
class Account {
  final String id;
  final String accountNumber;
  final String accountName;
  final String accountType;
  final String currency;
  final double availableBalance;
  final double bookBalance;
  final String status;
  final DateTime openedDate;
  final String? branchCode;
  final String? customerId;

  Account({
    required this.id,
    required this.accountNumber,
    required this.accountName,
    required this.accountType,
    required this.currency,
    required this.availableBalance,
    required this.bookBalance,
    required this.status,
    required this.openedDate,
    this.branchCode,
    this.customerId,
  });

  bool get isActive => status.toLowerCase() == 'active';
  bool get isFrozen => status.toLowerCase() == 'frozen';
  bool get isClosed => status.toLowerCase() == 'closed';

  factory Account.fromJson(Map<String, dynamic> json) {
    return Account(
      id: json['id'] ?? json['accountId'] ?? '',
      accountNumber: json['accountNumber'] ?? '',
      accountName: json['accountName'] ?? json['name'] ?? '',
      accountType: json['accountType'] ?? json['type'] ?? '',
      currency: json['currency'] ?? 'KES',
      availableBalance: (json['availableBalance'] ?? 0.0).toDouble(),
      bookBalance: (json['bookBalance'] ?? 0.0).toDouble(),
      status: json['status'] ?? 'Active',
      openedDate: json['openedDate'] != null 
          ? DateTime.parse(json['openedDate']) 
          : DateTime.now(),
      branchCode: json['branchCode'],
      customerId: json['customerId'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'accountNumber': accountNumber,
      'accountName': accountName,
      'accountType': accountType,
      'currency': currency,
      'availableBalance': availableBalance,
      'bookBalance': bookBalance,
      'status': status,
      'openedDate': openedDate.toIso8601String(),
      'branchCode': branchCode,
      'customerId': customerId,
    };
  }
}

/// Account balance response
class AccountBalance {
  final String accountNumber;
  final double availableBalance;
  final double bookBalance;
  final double clearedBalance;
  final double unclearedBalance;
  final String currency;
  final DateTime asOfDate;

  AccountBalance({
    required this.accountNumber,
    required this.availableBalance,
    required this.bookBalance,
    required this.clearedBalance,
    required this.unclearedBalance,
    required this.currency,
    required this.asOfDate,
  });

  factory AccountBalance.fromJson(Map<String, dynamic> json) {
    return AccountBalance(
      accountNumber: json['accountNumber'] ?? '',
      availableBalance: (json['availableBalance'] ?? 0.0).toDouble(),
      bookBalance: (json['bookBalance'] ?? 0.0).toDouble(),
      clearedBalance: (json['clearedBalance'] ?? 0.0).toDouble(),
      unclearedBalance: (json['unclearedBalance'] ?? 0.0).toDouble(),
      currency: json['currency'] ?? 'KES',
      asOfDate: json['asOfDate'] != null 
          ? DateTime.parse(json['asOfDate']) 
          : DateTime.now(),
    );
  }
}
