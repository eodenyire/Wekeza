/// Transaction model
class Transaction {
  final String id;
  final String transactionType;
  final String? referenceNumber;
  final double amount;
  final String currency;
  final String? fromAccount;
  final String? toAccount;
  final String? description;
  final String status;
  final DateTime transactionDate;
  final DateTime? valueDate;
  final String? initiatedBy;
  final Map<String, dynamic>? metadata;

  Transaction({
    required this.id,
    required this.transactionType,
    this.referenceNumber,
    required this.amount,
    required this.currency,
    this.fromAccount,
    this.toAccount,
    this.description,
    required this.status,
    required this.transactionDate,
    this.valueDate,
    this.initiatedBy,
    this.metadata,
  });

  bool get isDebit => transactionType.toLowerCase().contains('withdrawal') ||
                      transactionType.toLowerCase().contains('transfer') ||
                      transactionType.toLowerCase().contains('payment');
  
  bool get isCredit => transactionType.toLowerCase().contains('deposit') ||
                       transactionType.toLowerCase().contains('credit');
  
  bool get isCompleted => status.toLowerCase() == 'completed' || 
                          status.toLowerCase() == 'success';
  
  bool get isPending => status.toLowerCase() == 'pending';
  
  bool get isFailed => status.toLowerCase() == 'failed';

  factory Transaction.fromJson(Map<String, dynamic> json) {
    return Transaction(
      id: json['id'] ?? json['transactionId'] ?? '',
      transactionType: json['transactionType'] ?? json['type'] ?? '',
      referenceNumber: json['referenceNumber'] ?? json['reference'],
      amount: (json['amount'] ?? 0.0).toDouble(),
      currency: json['currency'] ?? 'KES',
      fromAccount: json['fromAccount'] ?? json['fromAccountId'],
      toAccount: json['toAccount'] ?? json['toAccountId'],
      description: json['description'] ?? json['narration'],
      status: json['status'] ?? 'Pending',
      transactionDate: json['transactionDate'] != null 
          ? DateTime.parse(json['transactionDate']) 
          : DateTime.now(),
      valueDate: json['valueDate'] != null 
          ? DateTime.parse(json['valueDate']) 
          : null,
      initiatedBy: json['initiatedBy'] ?? json['createdBy'],
      metadata: json['metadata'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'transactionType': transactionType,
      'referenceNumber': referenceNumber,
      'amount': amount,
      'currency': currency,
      'fromAccount': fromAccount,
      'toAccount': toAccount,
      'description': description,
      'status': status,
      'transactionDate': transactionDate.toIso8601String(),
      'valueDate': valueDate?.toIso8601String(),
      'initiatedBy': initiatedBy,
      'metadata': metadata,
    };
  }
}

/// Transfer request model
class TransferRequest {
  final String fromAccountId;
  final String toAccountId;
  final double amount;
  final String currency;
  final String? description;
  final String transferType;

  TransferRequest({
    required this.fromAccountId,
    required this.toAccountId,
    required this.amount,
    this.currency = 'KES',
    this.description,
    this.transferType = 'Internal',
  });

  Map<String, dynamic> toJson() {
    return {
      'fromAccountId': fromAccountId,
      'toAccountId': toAccountId,
      'amount': amount,
      'currency': currency,
      'description': description,
      'type': transferType,
    };
  }
}
