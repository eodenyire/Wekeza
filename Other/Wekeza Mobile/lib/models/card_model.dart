/// Card model
class BankCard {
  final String id;
  final String cardNumber;
  final String cardType;
  final String accountId;
  final String cardholderName;
  final DateTime expiryDate;
  final String status;
  final DateTime issuedDate;
  final double? dailyWithdrawalLimit;
  final double? dailyPurchaseLimit;
  final String? cvv; // Only returned during issuance
  final bool isVirtual;
  final String? customerId;

  BankCard({
    required this.id,
    required this.cardNumber,
    required this.cardType,
    required this.accountId,
    required this.cardholderName,
    required this.expiryDate,
    required this.status,
    required this.issuedDate,
    this.dailyWithdrawalLimit,
    this.dailyPurchaseLimit,
    this.cvv,
    this.isVirtual = false,
    this.customerId,
  });

  bool get isActive => status.toLowerCase() == 'active';
  bool get isBlocked => status.toLowerCase() == 'blocked' || status.toLowerCase() == 'hotlisted';
  bool get isExpired => DateTime.now().isAfter(expiryDate);
  bool get isPending => status.toLowerCase() == 'pending' || status.toLowerCase() == 'inactive';
  
  String get maskedCardNumber {
    if (cardNumber.length < 16) return cardNumber;
    return '${cardNumber.substring(0, 4)} **** **** ${cardNumber.substring(cardNumber.length - 4)}';
  }

  String get cardBrand {
    if (cardNumber.isEmpty) return 'Unknown';
    final firstDigit = cardNumber[0];
    switch (firstDigit) {
      case '4':
        return 'Visa';
      case '5':
        return 'Mastercard';
      case '3':
        return 'American Express';
      case '6':
        return 'Discover';
      default:
        return 'Unknown';
    }
  }

  factory BankCard.fromJson(Map<String, dynamic> json) {
    return BankCard(
      id: json['id'] ?? json['cardId'] ?? '',
      cardNumber: json['cardNumber'] ?? '',
      cardType: json['cardType'] ?? json['type'] ?? 'Debit',
      accountId: json['accountId'] ?? json['linkedAccountId'] ?? '',
      cardholderName: json['cardholderName'] ?? json['holderName'] ?? '',
      expiryDate: json['expiryDate'] != null 
          ? DateTime.parse(json['expiryDate']) 
          : DateTime.now().add(const Duration(days: 1095)), // 3 years
      status: json['status'] ?? 'Pending',
      issuedDate: json['issuedDate'] != null 
          ? DateTime.parse(json['issuedDate']) 
          : DateTime.now(),
      dailyWithdrawalLimit: json['dailyWithdrawalLimit'] != null 
          ? (json['dailyWithdrawalLimit']).toDouble() 
          : null,
      dailyPurchaseLimit: json['dailyPurchaseLimit'] != null 
          ? (json['dailyPurchaseLimit']).toDouble() 
          : null,
      cvv: json['cvv'],
      isVirtual: json['isVirtual'] ?? false,
      customerId: json['customerId'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'cardNumber': cardNumber,
      'cardType': cardType,
      'accountId': accountId,
      'cardholderName': cardholderName,
      'expiryDate': expiryDate.toIso8601String(),
      'status': status,
      'issuedDate': issuedDate.toIso8601String(),
      'dailyWithdrawalLimit': dailyWithdrawalLimit,
      'dailyPurchaseLimit': dailyPurchaseLimit,
      'isVirtual': isVirtual,
      'customerId': customerId,
    };
  }
}

/// Card issuance request
class CardIssuanceRequest {
  final String accountId;
  final String cardType;
  final String cardholderName;
  final bool isVirtual;
  final double? dailyWithdrawalLimit;
  final double? dailyPurchaseLimit;

  CardIssuanceRequest({
    required this.accountId,
    required this.cardType,
    required this.cardholderName,
    this.isVirtual = false,
    this.dailyWithdrawalLimit,
    this.dailyPurchaseLimit,
  });

  Map<String, dynamic> toJson() {
    return {
      'accountId': accountId,
      'cardType': cardType,
      'cardholderName': cardholderName,
      'isVirtual': isVirtual,
      'dailyWithdrawalLimit': dailyWithdrawalLimit,
      'dailyPurchaseLimit': dailyPurchaseLimit,
    };
  }
}
