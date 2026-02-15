import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

interface Account {
  id: string;
  accountNumber: string;
  accountName: string;
  balance: number;
}

interface BudgetAllocation {
  id: string;
  departmentName: string;
  category: string;
  availableAmount: number;
}

export default function PaymentInitiation() {
  const navigate = useNavigate();
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [budgetAllocations, setBudgetAllocations] = useState<BudgetAllocation[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const [formData, setFormData] = useState({
    accountId: '',
    budgetAllocationId: '',
    paymentType: 'SUPPLIER',
    amount: '',
    currency: 'KES',
    beneficiaryName: '',
    beneficiaryAccount: '',
    beneficiaryBank: '',
    purpose: '',
    reference: ''
  });

  useEffect(() => {
    fetchAccounts();
    fetchBudgetAllocations();
  }, []);

  const fetchAccounts = async () => {
    try {
      const token = localStorage.getItem('token');
      const response = await fetch('http://localhost:5000/api/public-sector/accounts', {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      const data = await response.json();
      if (data.success) {
        setAccounts(data.data);
      }
    } catch (err) {
      console.error('Error fetching accounts:', err);
    }
  };

  const fetchBudgetAllocations = async () => {
    try {
      const token = localStorage.getItem('token');
      const response = await fetch('http://localhost:5000/api/public-sector/budget/allocations?fiscalYear=2026', {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      const data = await response.json();
      if (data.success) {
        setBudgetAllocations(data.data);
      }
    } catch (err) {
      console.error('Error fetching budget allocations:', err);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setSuccess('');

    try {
      const token = localStorage.getItem('token');
      const response = await fetch('http://localhost:5000/api/public-sector/payments/initiate', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          customerId: '22222222-2222-2222-2222-222222222222',
          accountId: formData.accountId,
          budgetAllocationId: formData.budgetAllocationId || null,
          paymentType: formData.paymentType,
          amount: parseFloat(formData.amount),
          currency: formData.currency,
          beneficiaryName: formData.beneficiaryName,
          beneficiaryAccount: formData.beneficiaryAccount,
          beneficiaryBank: formData.beneficiaryBank,
          purpose: formData.purpose,
          reference: formData.reference
        })
      });

      const data = await response.json();

      if (data.success) {
        setSuccess(`Payment initiated successfully! Request Number: ${data.data.requestNumber}. Required approvals: ${data.data.requiredApprovals}`);
        setTimeout(() => {
          navigate('/public-sector/banking/pending-approvals');
        }, 2000);
      } else {
        setError(data.message || 'Failed to initiate payment');
      }
    } catch (err: any) {
      setError(err.message || 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const selectedAccount = accounts.find(a => a.id === formData.accountId);
  const selectedBudget = budgetAllocations.find(b => b.id === formData.budgetAllocationId);

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Initiate Payment</h1>
        <p className="text-gray-600">Create a new payment request for approval</p>
      </div>

      {error && (
        <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg">
          <p className="text-red-800">{error}</p>
        </div>
      )}

      {success && (
        <div className="mb-4 p-4 bg-green-50 border border-green-200 rounded-lg">
          <p className="text-green-800">{success}</p>
        </div>
      )}

      <div className="bg-white rounded-lg shadow-md p-6">
        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Account Selection */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Source Account *
            </label>
            <select
              name="accountId"
              value={formData.accountId}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
            >
              <option value="">Select Account</option>
              {accounts.map(account => (
                <option key={account.id} value={account.id}>
                  {account.accountNumber} - {account.accountName} (KES {account.balance.toLocaleString()})
                </option>
              ))}
            </select>
            {selectedAccount && (
              <p className="mt-1 text-sm text-gray-600">
                Available Balance: KES {selectedAccount.balance.toLocaleString()}
              </p>
            )}
          </div>

          {/* Budget Allocation (Optional) */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Budget Allocation (Optional)
            </label>
            <select
              name="budgetAllocationId"
              value={formData.budgetAllocationId}
              onChange={handleChange}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
            >
              <option value="">No Budget Allocation</option>
              {budgetAllocations.map(budget => (
                <option key={budget.id} value={budget.id}>
                  {budget.departmentName} - {budget.category} (Available: KES {budget.availableAmount.toLocaleString()})
                </option>
              ))}
            </select>
            {selectedBudget && (
              <p className="mt-1 text-sm text-gray-600">
                Available Budget: KES {selectedBudget.availableAmount.toLocaleString()}
              </p>
            )}
          </div>

          {/* Payment Type */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Payment Type *
            </label>
            <select
              name="paymentType"
              value={formData.paymentType}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
            >
              <option value="SUPPLIER">Supplier Payment</option>
              <option value="SALARY">Salary Payment</option>
              <option value="PENSION">Pension Payment</option>
              <option value="UTILITY">Utility Payment</option>
              <option value="OTHER">Other</option>
            </select>
          </div>

          {/* Amount */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Amount (KES) *
            </label>
            <input
              type="number"
              name="amount"
              value={formData.amount}
              onChange={handleChange}
              required
              min="0"
              step="0.01"
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
              placeholder="Enter amount"
            />
            {formData.amount && (
              <p className="mt-1 text-sm text-gray-600">
                Approval Levels Required: {
                  parseFloat(formData.amount) <= 10000000 ? '1 (Checker)' :
                  parseFloat(formData.amount) <= 100000000 ? '2 (Checker + Approver)' :
                  '3 (Checker + Approver + Senior Approver)'
                }
              </p>
            )}
          </div>

          {/* Beneficiary Details */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Beneficiary Name *
              </label>
              <input
                type="text"
                name="beneficiaryName"
                value={formData.beneficiaryName}
                onChange={handleChange}
                required
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                placeholder="Enter beneficiary name"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Beneficiary Account *
              </label>
              <input
                type="text"
                name="beneficiaryAccount"
                value={formData.beneficiaryAccount}
                onChange={handleChange}
                required
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                placeholder="Enter account number"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Beneficiary Bank *
            </label>
            <input
              type="text"
              name="beneficiaryBank"
              value={formData.beneficiaryBank}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
              placeholder="Enter bank name"
            />
          </div>

          {/* Purpose */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Purpose *
            </label>
            <textarea
              name="purpose"
              value={formData.purpose}
              onChange={handleChange}
              required
              rows={3}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
              placeholder="Enter payment purpose"
            />
          </div>

          {/* Reference */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Reference
            </label>
            <input
              type="text"
              name="reference"
              value={formData.reference}
              onChange={handleChange}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
              placeholder="Enter reference number (optional)"
            />
          </div>

          {/* Submit Button */}
          <div className="flex gap-4">
            <button
              type="submit"
              disabled={loading}
              className="flex-1 bg-green-600 text-white px-6 py-3 rounded-lg hover:bg-green-700 disabled:bg-gray-400 disabled:cursor-not-allowed font-medium"
            >
              {loading ? 'Submitting...' : 'Submit for Approval'}
            </button>
            <button
              type="button"
              onClick={() => navigate('/public-sector/banking/pending-approvals')}
              className="px-6 py-3 border border-gray-300 rounded-lg hover:bg-gray-50 font-medium"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>

      {/* Info Box */}
      <div className="mt-6 bg-blue-50 border border-blue-200 rounded-lg p-4">
        <h3 className="font-medium text-blue-900 mb-2">Approval Process</h3>
        <ul className="text-sm text-blue-800 space-y-1">
          <li>• Payments ≤ KES 10M require 1 approval (Checker)</li>
          <li>• Payments ≤ KES 100M require 2 approvals (Checker + Approver)</li>
          <li>• Payments > KES 100M require 3 approvals (Checker + Approver + Senior Approver)</li>
          <li>• All payments are subject to account balance and budget availability checks</li>
        </ul>
      </div>
    </div>
  );
}
