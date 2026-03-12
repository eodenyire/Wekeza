/// Loan model
class Loan {
  final String id;
  final String loanNumber;
  final String accountId;
  final String customerId;
  final String productType;
  final double principalAmount;
  final double outstandingBalance;
  final double interestRate;
  final int tenorMonths;
  final String status;
  final DateTime applicationDate;
  final DateTime? approvalDate;
  final DateTime? disbursementDate;
  final DateTime? maturityDate;
  final String? purpose;
  final double? monthlyRepayment;

  Loan({
    required this.id,
    required this.loanNumber,
    required this.accountId,
    required this.customerId,
    required this.productType,
    required this.principalAmount,
    required this.outstandingBalance,
    required this.interestRate,
    required this.tenorMonths,
    required this.status,
    required this.applicationDate,
    this.approvalDate,
    this.disbursementDate,
    this.maturityDate,
    this.purpose,
    this.monthlyRepayment,
  });

  bool get isActive => status.toLowerCase() == 'active' || status.toLowerCase() == 'disbursed';
  bool get isPending => status.toLowerCase() == 'pending' || status.toLowerCase() == 'submitted';
  bool get isApproved => status.toLowerCase() == 'approved';
  bool get isRejected => status.toLowerCase() == 'rejected' || status.toLowerCase() == 'declined';
  bool get isPaidOff => status.toLowerCase() == 'closed' || status.toLowerCase() == 'paid';

  double get paidAmount => principalAmount - outstandingBalance;
  double get progressPercentage => (paidAmount / principalAmount * 100).clamp(0, 100);

  factory Loan.fromJson(Map<String, dynamic> json) {
    return Loan(
      id: json['id'] ?? json['loanId'] ?? '',
      loanNumber: json['loanNumber'] ?? json['loanAccountNumber'] ?? '',
      accountId: json['accountId'] ?? json['linkedAccountId'] ?? '',
      customerId: json['customerId'] ?? '',
      productType: json['productType'] ?? json['loanType'] ?? '',
      principalAmount: (json['principalAmount'] ?? json['amount'] ?? 0.0).toDouble(),
      outstandingBalance: (json['outstandingBalance'] ?? json['balance'] ?? 0.0).toDouble(),
      interestRate: (json['interestRate'] ?? 0.0).toDouble(),
      tenorMonths: json['tenorMonths'] ?? json['tenor'] ?? 0,
      status: json['status'] ?? 'Pending',
      applicationDate: json['applicationDate'] != null 
          ? DateTime.parse(json['applicationDate']) 
          : DateTime.now(),
      approvalDate: json['approvalDate'] != null 
          ? DateTime.parse(json['approvalDate']) 
          : null,
      disbursementDate: json['disbursementDate'] != null 
          ? DateTime.parse(json['disbursementDate']) 
          : null,
      maturityDate: json['maturityDate'] != null 
          ? DateTime.parse(json['maturityDate']) 
          : null,
      purpose: json['purpose'] ?? json['loanPurpose'],
      monthlyRepayment: json['monthlyRepayment'] != null 
          ? (json['monthlyRepayment']).toDouble() 
          : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'loanNumber': loanNumber,
      'accountId': accountId,
      'customerId': customerId,
      'productType': productType,
      'principalAmount': principalAmount,
      'outstandingBalance': outstandingBalance,
      'interestRate': interestRate,
      'tenorMonths': tenorMonths,
      'status': status,
      'applicationDate': applicationDate.toIso8601String(),
      'approvalDate': approvalDate?.toIso8601String(),
      'disbursementDate': disbursementDate?.toIso8601String(),
      'maturityDate': maturityDate?.toIso8601String(),
      'purpose': purpose,
      'monthlyRepayment': monthlyRepayment,
    };
  }
}

/// Loan application request
class LoanApplicationRequest {
  final String accountId;
  final String customerId;
  final double amount;
  final int tenorMonths;
  final String productType;
  final String purpose;
  final double monthlyIncome;

  LoanApplicationRequest({
    required this.accountId,
    required this.customerId,
    required this.amount,
    required this.tenorMonths,
    required this.productType,
    required this.purpose,
    required this.monthlyIncome,
  });

  Map<String, dynamic> toJson() {
    return {
      'accountId': accountId,
      'customerId': customerId,
      'amount': amount,
      'tenorMonths': tenorMonths,
      'productType': productType,
      'purpose': purpose,
      'monthlyIncome': monthlyIncome,
    };
  }
}

/// Loan repayment request
class LoanRepaymentRequest {
  final String loanId;
  final String accountId;
  final double amount;
  final String? description;

  LoanRepaymentRequest({
    required this.loanId,
    required this.accountId,
    required this.amount,
    this.description,
  });

  Map<String, dynamic> toJson() {
    return {
      'loanId': loanId,
      'accountId': accountId,
      'amount': amount,
      'description': description ?? 'Loan Repayment',
    };
  }
}
