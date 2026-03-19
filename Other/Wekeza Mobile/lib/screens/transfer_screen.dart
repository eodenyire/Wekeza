import 'package:flutter/material.dart';
import '../services/transaction_service.dart';
import '../services/account_service.dart';
import '../services/api_service.dart';
import '../models/account_model.dart';
import '../models/transaction_model.dart';
import '../config/app_config.dart';
import 'package:intl/intl.dart';

class TransferScreen extends StatefulWidget {
  const TransferScreen({super.key});

  @override
  State<TransferScreen> createState() => _TransferScreenState();
}

class _TransferScreenState extends State<TransferScreen> {
  final _formKey = GlobalKey<FormState>();
  final _toAccountController = TextEditingController();
  final _amountController = TextEditingController();
  final _descriptionController = TextEditingController();
  
  final _transactionService = TransactionService();
  final _accountService = AccountService();
  
  List<Account> _accounts = [];
  Account? _selectedAccount;
  bool _isLoading = false;
  bool _isLoadingAccounts = true;
  String? _errorMessage;
  String _transferType = 'Internal';

  @override
  void initState() {
    super.initState();
    _loadAccounts();
  }

  @override
  void dispose() {
    _toAccountController.dispose();
    _amountController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _loadAccounts() async {
    try {
      _accounts = await _accountService.getUserAccounts();
      if (_accounts.isNotEmpty) {
        _selectedAccount = _accounts.first;
      }
      if (mounted) {
        setState(() {
          _isLoadingAccounts = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _isLoadingAccounts = false;
        });
      }
    }
  }

  Future<void> _transfer() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    if (_selectedAccount == null) {
      setState(() {
        _errorMessage = 'Please select an account';
      });
      return;
    }

    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      final amount = double.parse(_amountController.text);
      
      final request = TransferRequest(
        fromAccountId: _selectedAccount!.id,
        toAccountId: _toAccountController.text.trim(),
        amount: amount,
        description: _descriptionController.text.trim(),
        transferType: _transferType,
      );

      final result = await _transactionService.transferFunds(request);

      if (!mounted) return;

      // Show success dialog
      showDialog(
        context: context,
        barrierDismissible: false,
        builder: (context) => AlertDialog(
          icon: const Icon(
            Icons.check_circle,
            color: Colors.green,
            size: 64,
          ),
          title: const Text('Transfer Successful'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                'Amount: ${_selectedAccount!.currency} ${NumberFormat('#,##0.00').format(amount)}',
                style: const TextStyle(fontWeight: FontWeight.bold),
              ),
              if (result['referenceNumber'] != null) ...[
                const SizedBox(height: 8),
                Text('Reference: ${result['referenceNumber']}'),
              ],
            ],
          ),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.of(context).pop();
                Navigator.of(context).pop();
              },
              child: const Text('Done'),
            ),
          ],
        ),
      );
    } on ApiException catch (e) {
      setState(() {
        _errorMessage = e.message;
      });
    } catch (e) {
      setState(() {
        _errorMessage = 'Transfer failed. Please try again.';
      });
    } finally {
      if (mounted) {
        setState(() {
          _isLoading = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Transfer Money'),
      ),
      body: _isLoadingAccounts
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    // Error message
                    if (_errorMessage != null)
                      Container(
                        padding: const EdgeInsets.all(12),
                        margin: const EdgeInsets.only(bottom: 16),
                        decoration: BoxDecoration(
                          color: Colors.red[50],
                          borderRadius: BorderRadius.circular(8),
                          border: Border.all(color: Colors.red[300]!),
                        ),
                        child: Row(
                          children: [
                            Icon(Icons.error_outline, color: Colors.red[700]),
                            const SizedBox(width: 8),
                            Expanded(
                              child: Text(
                                _errorMessage!,
                                style: TextStyle(color: Colors.red[700]),
                              ),
                            ),
                          ],
                        ),
                      ),
                    // From Account
                    const Text(
                      'From Account',
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 8),
                    Card(
                      child: Padding(
                        padding: const EdgeInsets.all(16),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            DropdownButtonFormField<Account>(
                              value: _selectedAccount,
                              decoration: const InputDecoration(
                                border: InputBorder.none,
                                contentPadding: EdgeInsets.zero,
                              ),
                              items: _accounts.map((account) {
                                return DropdownMenuItem(
                                  value: account,
                                  child: Text(account.accountNumber),
                                );
                              }).toList(),
                              onChanged: (account) {
                                setState(() {
                                  _selectedAccount = account;
                                });
                              },
                            ),
                            if (_selectedAccount != null) ...[
                              const SizedBox(height: 8),
                              Text(
                                'Available: ${_selectedAccount!.currency} ${NumberFormat('#,##0.00').format(_selectedAccount!.availableBalance)}',
                                style: TextStyle(
                                  color: Colors.grey[600],
                                  fontSize: 14,
                                ),
                              ),
                            ],
                          ],
                        ),
                      ),
                    ),
                    const SizedBox(height: 24),
                    // Transfer Type
                    const Text(
                      'Transfer Type',
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 8),
                    SegmentedButton<String>(
                      segments: const [
                        ButtonSegment(
                          value: 'Internal',
                          label: Text('Internal'),
                          icon: Icon(Icons.swap_horiz),
                        ),
                        ButtonSegment(
                          value: 'External',
                          label: Text('External'),
                          icon: Icon(Icons.send),
                        ),
                      ],
                      selected: {_transferType},
                      onSelectionChanged: (Set<String> newSelection) {
                        setState(() {
                          _transferType = newSelection.first;
                        });
                      },
                    ),
                    const SizedBox(height: 24),
                    // To Account
                    TextFormField(
                      controller: _toAccountController,
                      decoration: const InputDecoration(
                        labelText: 'To Account Number',
                        prefixIcon: Icon(Icons.account_balance_wallet),
                      ),
                      keyboardType: TextInputType.text,
                      textInputAction: TextInputAction.next,
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Please enter account number';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 16),
                    // Amount
                    TextFormField(
                      controller: _amountController,
                      decoration: InputDecoration(
                        labelText: 'Amount',
                        prefixIcon: const Icon(Icons.attach_money),
                        suffixText: _selectedAccount?.currency ?? 'KES',
                      ),
                      keyboardType: TextInputType.numberWithOptions(decimal: true),
                      textInputAction: TextInputAction.next,
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Please enter amount';
                        }
                        final amount = double.tryParse(value);
                        if (amount == null || amount <= 0) {
                          return 'Please enter a valid amount';
                        }
                        if (_selectedAccount != null && amount > _selectedAccount!.availableBalance) {
                          return 'Insufficient balance';
                        }
                        if (amount > AppConfig.maxTransferAmount) {
                          return 'Amount exceeds maximum limit';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 16),
                    // Description
                    TextFormField(
                      controller: _descriptionController,
                      decoration: const InputDecoration(
                        labelText: 'Description (Optional)',
                        prefixIcon: Icon(Icons.note),
                      ),
                      textInputAction: TextInputAction.done,
                      maxLines: 2,
                    ),
                    const SizedBox(height: 32),
                    // Transfer button
                    ElevatedButton(
                      onPressed: _isLoading ? null : _transfer,
                      style: ElevatedButton.styleFrom(
                        minimumSize: const Size(double.infinity, 56),
                      ),
                      child: _isLoading
                          ? const SizedBox(
                              height: 20,
                              width: 20,
                              child: CircularProgressIndicator(
                                strokeWidth: 2,
                                valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                              ),
                            )
                          : const Text('Transfer'),
                    ),
                  ],
                ),
              ),
            ),
    );
  }
}
