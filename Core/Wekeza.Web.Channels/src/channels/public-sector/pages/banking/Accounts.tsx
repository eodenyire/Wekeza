import React, { useState, useEffect } from 'react';
import { GovernmentAccount } from '../../types';
import { LoadingSpinner, ErrorAlert, TransactionTable } from '../../components';

export const Accounts: React.FC = () => {
  const [accounts, setAccounts] = useState<GovernmentAccount[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedAccount, setSelectedAccount] = useState<GovernmentAccount | null>(null);
  const [transactions, setTransactions] = useState<any[]>([]);
  const [loadingTransactions, setLoadingTransactions] = useState(false);

  useEffect(() => {
    fetchAccounts();
  }, []);

  const fetchAccounts = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/public-sector/accounts', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to fetch accounts');
      
      const data = await response.json();
      setAccounts(data.data || []);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const fetchTransactions = async (accountId: string) => {
    try {
      setLoadingTransactions(true);
      const response = await fetch(`/api/public-sector/accounts/${accountId}/transactions`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to fetch transactions');
      
      const data = await response.json();
      setTransactions(data.data || []);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoadingTransactions(false);
    }
  };

  const handleViewAccount = (account: GovernmentAccount) => {
    setSelectedAccount(account);
    fetchTransactions(account.id);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'ACTIVE': return 'bg-green-100 text-green-800';
      case 'DORMANT': return 'bg-yellow-100 text-yellow-800';
      case 'CLOSED': return 'bg-gray-100 text-gray-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const totalBalance = accounts.reduce((sum, acc) => sum + acc.balance, 0);

  if (loading) return <LoadingSpinner />;
  if (error && !selectedAccount) return <ErrorAlert message={error} onRetry={fetchAccounts} />;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Government Accounts</h1>
      </div>

      {error && <ErrorAlert message={error} onClose={() => setError(null)} />}

      {/* Summary Card */}
      <div className="bg-gradient-to-r from-blue-600 to-blue-800 text-white p-6 rounded-lg shadow">
        <p className="text-sm opacity-90">Total Balance Across All Accounts</p>
        <p className="text-3xl font-bold mt-2">KES {totalBalance.toLocaleString()}</p>
        <p className="text-sm opacity-90 mt-1">{accounts.length} Active Accounts</p>
      </div>

      {/* Accounts Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {accounts.map(account => (
          <div
            key={account.id}
            className="bg-white p-6 rounded-lg shadow hover:shadow-md transition-shadow cursor-pointer"
            onClick={() => handleViewAccount(account)}
          >
            <div className="flex justify-between items-start mb-4">
              <div>
                <p className="text-sm text-gray-500">Account Number</p>
                <p className="font-mono font-medium text-gray-900">{account.accountNumber}</p>
              </div>
              <span className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(account.status)}`}>
                {account.status}
              </span>
            </div>

            <h3 className="font-semibold text-gray-900 mb-2">{account.accountName}</h3>
            
            <div className="mb-4">
              <p className="text-sm text-gray-500">{account.governmentEntity.name}</p>
              <p className="text-xs text-gray-400">{account.accountType.replace('_', ' ')}</p>
            </div>

            <div className="pt-4 border-t">
              <p className="text-sm text-gray-500">Balance</p>
              <p className="text-2xl font-bold text-blue-600">
                {account.currency} {account.balance.toLocaleString()}
              </p>
            </div>
          </div>
        ))}
      </div>

      {/* Account Details Modal */}
      {selectedAccount && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-6xl w-full max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-6">
              <div>
                <h2 className="text-2xl font-bold text-gray-900">{selectedAccount.accountName}</h2>
                <p className="text-gray-500">{selectedAccount.accountNumber}</p>
              </div>
              <button
                onClick={() => setSelectedAccount(null)}
                className="text-gray-500 hover:text-gray-700 text-2xl"
              >
                ✕
              </button>
            </div>

            {/* Account Info */}
            <div className="grid grid-cols-4 gap-4 mb-6">
              <div className="bg-gray-50 p-4 rounded-lg">
                <p className="text-sm text-gray-500">Balance</p>
                <p className="text-xl font-bold text-gray-900">
                  {selectedAccount.currency} {selectedAccount.balance.toLocaleString()}
                </p>
              </div>
              <div className="bg-gray-50 p-4 rounded-lg">
                <p className="text-sm text-gray-500">Account Type</p>
                <p className="font-medium text-gray-900">
                  {selectedAccount.accountType.replace('_', ' ')}
                </p>
              </div>
              <div className="bg-gray-50 p-4 rounded-lg">
                <p className="text-sm text-gray-500">Status</p>
                <span className={`inline-block px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(selectedAccount.status)}`}>
                  {selectedAccount.status}
                </span>
              </div>
              <div className="bg-gray-50 p-4 rounded-lg">
                <p className="text-sm text-gray-500">Entity</p>
                <p className="font-medium text-gray-900">{selectedAccount.governmentEntity.name}</p>
              </div>
            </div>

            {/* Transactions */}
            <div>
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Transaction History</h3>
              {loadingTransactions ? (
                <LoadingSpinner />
              ) : (
                <TransactionTable transactions={transactions} />
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
