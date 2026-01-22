import 'package:flutter/material.dart';
import '../models/loan_model.dart';
import '../services/loan_service.dart';
import '../config/app_config.dart';
import 'package:intl/intl.dart';

class LoansScreen extends StatefulWidget {
  const LoansScreen({super.key};

  @override
  State<LoansScreen> createState() => _LoansScreenState();
}

class _LoansScreenState extends State<LoansScreen> {
  final _loanService = LoanService();
  List<Loan> _loans = [];
  bool _isLoading = true;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    _loadLoans();
  }

  Future<void> _loadLoans() async {
    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      _loans = await _loanService.getUserLoans();
      if (mounted) {
        setState(() {
          _isLoading = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _isLoading = false;
          _errorMessage = 'Failed to load loans';
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('My Loans'),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _errorMessage != null
              ? Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(Icons.error_outline, size: 48, color: Colors.red),
                      const SizedBox(height: 16),
                      Text(_errorMessage!),
                      const SizedBox(height: 16),
                      ElevatedButton(
                        onPressed: _loadLoans,
                        child: const Text('Retry'),
                      ),
                    ],
                  ),
                )
              : _loans.isEmpty
                  ? Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          const Icon(
                            Icons.account_balance,
                            size: 64,
                            color: Colors.grey,
                          ),
                          const SizedBox(height: 16),
                          const Text('No loans found'),
                          const SizedBox(height: 24),
                          ElevatedButton(
                            onPressed: () {
                              ScaffoldMessenger.of(context).showSnackBar(
                                const SnackBar(content: Text('Apply for loan feature coming soon')),
                              );
                            },
                            child: const Text('Apply for Loan'),
                          ),
                        ],
                      ),
                    )
                  : RefreshIndicator(
                      onRefresh: _loadLoans,
                      child: ListView.builder(
                        padding: const EdgeInsets.all(16),
                        itemCount: _loans.length,
                        itemBuilder: (context, index) {
                          final loan = _loans[index];
                          return _buildLoanCard(loan);
                        },
                      ),
                    ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Apply for loan feature coming soon')),
          );
        },
        icon: const Icon(Icons.add),
        label: const Text('Apply for Loan'),
      ),
    );
  }

  Widget _buildLoanCard(Loan loan) {
    final statusColor = loan.isActive
        ? Colors.green
        : loan.isPending
            ? Colors.orange
            : loan.isRejected
                ? Colors.red
                : Colors.grey;

    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      child: InkWell(
        onTap: () => _showLoanDetails(loan),
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    loan.productType,
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: statusColor.withOpacity(0.1),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Text(
                      loan.status,
                      style: TextStyle(
                        fontSize: 12,
                        color: statusColor,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              Text(
                loan.loanNumber,
                style: TextStyle(
                  fontSize: 14,
                  color: Colors.grey[600],
                ),
              ),
              const SizedBox(height: 16),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Principal Amount',
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey[600],
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        'KES ${NumberFormat('#,##0.00').format(loan.principalAmount)}',
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.end,
                    children: [
                      Text(
                        'Outstanding',
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey[600],
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        'KES ${NumberFormat('#,##0.00').format(loan.outstandingBalance)}',
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: Colors.orange,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
              if (loan.isActive) ...[
                const SizedBox(height: 16),
                LinearProgressIndicator(
                  value: loan.progressPercentage / 100,
                  backgroundColor: Colors.grey[200],
                  color: const Color(AppConfig.primaryColorValue),
                ),
                const SizedBox(height: 4),
                Text(
                  '${loan.progressPercentage.toStringAsFixed(1)}% repaid',
                  style: TextStyle(
                    fontSize: 12,
                    color: Colors.grey[600],
                  ),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }

  void _showLoanDetails(Loan loan) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (context) => DraggableScrollableSheet(
        initialChildSize: 0.7,
        minChildSize: 0.5,
        maxChildSize: 0.95,
        expand: false,
        builder: (context, scrollController) => SingleChildScrollView(
          controller: scrollController,
          child: Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Center(
                  child: Container(
                    width: 40,
                    height: 4,
                    decoration: BoxDecoration(
                      color: Colors.grey[300],
                      borderRadius: BorderRadius.circular(2),
                    ),
                  ),
                ),
                const SizedBox(height: 24),
                const Text(
                  'Loan Details',
                  style: TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 24),
                _buildDetailRow('Loan Number', loan.loanNumber),
                _buildDetailRow('Product Type', loan.productType),
                _buildDetailRow(
                  'Principal Amount',
                  'KES ${NumberFormat('#,##0.00').format(loan.principalAmount)}',
                ),
                _buildDetailRow(
                  'Outstanding Balance',
                  'KES ${NumberFormat('#,##0.00').format(loan.outstandingBalance)}',
                ),
                _buildDetailRow('Interest Rate', '${loan.interestRate}% p.a.'),
                _buildDetailRow('Tenor', '${loan.tenorMonths} months'),
                _buildDetailRow('Status', loan.status),
                _buildDetailRow(
                  'Application Date',
                  DateFormat('dd MMM yyyy').format(loan.applicationDate),
                ),
                if (loan.approvalDate != null)
                  _buildDetailRow(
                    'Approval Date',
                    DateFormat('dd MMM yyyy').format(loan.approvalDate!),
                  ),
                if (loan.disbursementDate != null)
                  _buildDetailRow(
                    'Disbursement Date',
                    DateFormat('dd MMM yyyy').format(loan.disbursementDate!),
                  ),
                if (loan.maturityDate != null)
                  _buildDetailRow(
                    'Maturity Date',
                    DateFormat('dd MMM yyyy').format(loan.maturityDate!),
                  ),
                if (loan.monthlyRepayment != null)
                  _buildDetailRow(
                    'Monthly Repayment',
                    'KES ${NumberFormat('#,##0.00').format(loan.monthlyRepayment)}',
                  ),
                const SizedBox(height: 24),
                if (loan.isActive)
                  ElevatedButton(
                    onPressed: () {
                      Navigator.pop(context);
                      ScaffoldMessenger.of(context).showSnackBar(
                        const SnackBar(content: Text('Make repayment feature coming soon')),
                      );
                    },
                    style: ElevatedButton.styleFrom(
                      minimumSize: const Size(double.infinity, 48),
                    ),
                    child: const Text('Make Repayment'),
                  ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildDetailRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            flex: 2,
            child: Text(
              label,
              style: TextStyle(
                fontSize: 14,
                color: Colors.grey[600],
              ),
            ),
          ),
          Expanded(
            flex: 3,
            child: Text(
              value,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w500,
              ),
              textAlign: TextAlign.right,
            ),
          ),
        ],
      ),
    );
  }
}
