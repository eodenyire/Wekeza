import 'package:flutter/material.dart';
import '../models/transaction_model.dart';
import '../services/transaction_service.dart';
import '../services/account_service.dart';
import '../models/account_model.dart';
import '../config/app_config.dart';
import 'package:intl/intl.dart';

class TransactionsScreen extends StatefulWidget {
  const TransactionsScreen({super.key});

  @override
  State<TransactionsScreen> createState() => _TransactionsScreenState();
}

class _TransactionsScreenState extends State<TransactionsScreen> {
  final _transactionService = TransactionService();
  final _accountService = AccountService();
  List<Transaction> _transactions = [];
  List<Account> _accounts = [];
  Account? _selectedAccount;
  bool _isLoading = true;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      _accounts = await _accountService.getUserAccounts();
      if (_accounts.isNotEmpty) {
        _selectedAccount = _accounts.first;
        await _loadTransactions();
      }
      if (mounted) {
        setState(() {
          _isLoading = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _isLoading = false;
          _errorMessage = 'Failed to load data';
        });
      }
    }
  }

  Future<void> _loadTransactions() async {
    if (_selectedAccount == null) return;

    try {
      _transactions = await _transactionService.getTransactionHistory(
        accountNumber: _selectedAccount!.accountNumber,
        pageSize: 50,
      );
      if (mounted) {
        setState(() {});
      }
    } catch (e) {
      // Handle error silently or show a snackbar
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Transactions'),
        actions: [
          if (_accounts.isNotEmpty)
            PopupMenuButton<Account>(
              icon: const Icon(Icons.filter_list),
              onSelected: (account) {
                setState(() {
                  _selectedAccount = account;
                });
                _loadTransactions();
              },
              itemBuilder: (context) => _accounts.map((account) {
                return PopupMenuItem<Account>(
                  value: account,
                  child: Text(account.accountNumber),
                );
              }).toList(),
            ),
        ],
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
                        onPressed: _loadData,
                        child: const Text('Retry'),
                      ),
                    ],
                  ),
                )
              : Column(
                  children: [
                    if (_selectedAccount != null)
                      Container(
                        padding: const EdgeInsets.all(16),
                        color: const Color(AppConfig.primaryColorValue),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              _selectedAccount!.accountNumber,
                              style: const TextStyle(
                                color: Colors.white,
                                fontSize: 16,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            const SizedBox(height: 4),
                            Text(
                              'Balance: ${_selectedAccount!.currency} ${NumberFormat('#,##0.00').format(_selectedAccount!.availableBalance)}',
                              style: const TextStyle(
                                color: Colors.white,
                                fontSize: 14,
                              ),
                            ),
                          ],
                        ),
                      ),
                    Expanded(
                      child: _transactions.isEmpty
                          ? const Center(
                              child: Text('No transactions found'),
                            )
                          : RefreshIndicator(
                              onRefresh: _loadTransactions,
                              child: ListView.builder(
                                padding: const EdgeInsets.all(16),
                                itemCount: _transactions.length,
                                itemBuilder: (context, index) {
                                  final transaction = _transactions[index];
                                  return _buildTransactionCard(transaction);
                                },
                              ),
                            ),
                    ),
                  ],
                ),
    );
  }

  Widget _buildTransactionCard(Transaction transaction) {
    final isDebit = transaction.isDebit;
    final iconData = isDebit ? Icons.arrow_upward : Icons.arrow_downward;
    final iconColor = isDebit ? Colors.red : Colors.green;
    final amountColor = isDebit ? Colors.red : Colors.green;
    final amountPrefix = isDebit ? '-' : '+';

    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: iconColor.withOpacity(0.1),
          child: Icon(iconData, color: iconColor),
        ),
        title: Text(
          transaction.transactionType,
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (transaction.description != null)
              Text(transaction.description!),
            const SizedBox(height: 4),
            Text(
              DateFormat('dd MMM yyyy, hh:mm a').format(transaction.transactionDate),
              style: TextStyle(fontSize: 12, color: Colors.grey[600]),
            ),
            if (transaction.referenceNumber != null)
              Text(
                'Ref: ${transaction.referenceNumber}',
                style: TextStyle(fontSize: 12, color: Colors.grey[600]),
              ),
          ],
        ),
        trailing: Column(
          crossAxisAlignment: CrossAxisAlignment.end,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              '$amountPrefix${transaction.currency} ${NumberFormat('#,##0.00').format(transaction.amount)}',
              style: TextStyle(
                fontWeight: FontWeight.bold,
                fontSize: 16,
                color: amountColor,
              ),
            ),
            const SizedBox(height: 4),
            _buildStatusChip(transaction.status),
          ],
        ),
      ),
    );
  }

  Widget _buildStatusChip(String status) {
    Color color;
    if (status.toLowerCase() == 'completed' || status.toLowerCase() == 'success') {
      color = Colors.green;
    } else if (status.toLowerCase() == 'pending') {
      color = Colors.orange;
    } else {
      color = Colors.red;
    }

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Text(
        status,
        style: TextStyle(
          fontSize: 10,
          color: color,
          fontWeight: FontWeight.w500,
        ),
      ),
    );
  }
}
