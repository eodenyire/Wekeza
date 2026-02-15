import React, { useState, useEffect } from 'react';
import { BulkPayment, GovernmentAccount } from '../../types';
import { LoadingSpinner, ErrorAlert, SuccessAlert } from '../../components';
import * as XLSX from 'xlsx';

export const Payments: React.FC = () => {
  const [accounts, setAccounts] = useState<GovernmentAccount[]>([]);
  const [payments, setPayments] = useState<BulkPayment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [previewData, setPreviewData] = useState<any[]>([]);
  const [selectedAccount, setSelectedAccount] = useState('');
  const [paymentType, setPaymentType] = useState<'SALARY' | 'SUPPLIER' | 'PENSION' | 'OTHER'>('SUPPLIER');

  useEffect(() => {
    fetchAccounts();
    fetchPayments();
  }, []);

  const fetchAccounts = async () => {
    try {
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
    }
  };

  const fetchPayments = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/public-sector/payments/bulk', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to fetch payments');
      
      const data = await response.json();
      setPayments(data.data || []);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    setSelectedFile(file);
    parseFile(file);
  };

  const parseFile = (file: File) => {
    const reader = new FileReader();
    
    reader.onload = (e) => {
      try {
        const data = e.target?.result;
        const workbook = XLSX.read(data, { type: 'binary' });
        const sheetName = workbook.SheetNames[0];
        const worksheet = workbook.Sheets[sheetName];
        const jsonData = XLSX.utils.sheet_to_json(worksheet);
        
        setPreviewData(jsonData);
      } catch (err) {
        setError('Failed to parse file. Please ensure it is a valid Excel or CSV file.');
      }
    };

    reader.readAsBinaryString(file);
  };

  const handleUploadPayments = async () => {
    if (!selectedFile || !selectedAccount || previewData.length === 0) {
      setError('Please select an account and upload a valid payment file');
      return;
    }

    try {
      setUploading(true);
      
      const formData = new FormData();
      formData.append('file', selectedFile);
      formData.append('accountId', selectedAccount);
      formData.append('paymentType', paymentType);

      const response = await fetch('/api/public-sector/payments/bulk', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: formData
      });

      if (!response.ok) throw new Error('Failed to upload payments');
      
      setSuccess('Payments uploaded successfully and queued for processing');
      setSelectedFile(null);
      setPreviewData([]);
      fetchPayments();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setUploading(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'UPLOADED': return 'bg-blue-100 text-blue-800';
      case 'VALIDATED': return 'bg-yellow-100 text-yellow-800';
      case 'PROCESSING': return 'bg-purple-100 text-purple-800';
      case 'COMPLETED': return 'bg-green-100 text-green-800';
      case 'FAILED': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Bulk Payments</h1>

      {error && <ErrorAlert message={error} onClose={() => setError(null)} />}
      {success && <SuccessAlert message={success} />}

      {/* Upload Section */}
      <div className="bg-white p-6 rounded-lg shadow space-y-4">
        <h2 className="text-lg font-semibold text-gray-900">Upload Payment File</h2>
        
        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Source Account *
            </label>
            <select
              value={selectedAccount}
              onChange={(e) => setSelectedAccount(e.target.value)}
              className="w-full p-2 border rounded-lg"
            >
              <option value="">Select account...</option>
              {accounts.map(account => (
                <option key={account.id} value={account.id}>
                  {account.accountNumber} - {account.accountName} (Balance: {account.currency} {account.balance.toLocaleString()})
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Payment Type *
            </label>
            <select
              value={paymentType}
              onChange={(e) => setPaymentType(e.target.value as any)}
              className="w-full p-2 border rounded-lg"
            >
              <option value="SALARY">Salary</option>
              <option value="SUPPLIER">Supplier</option>
              <option value="PENSION">Pension</option>
              <option value="OTHER">Other</option>
            </select>
          </div>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Payment File (Excel/CSV) *
          </label>
          <input
            type="file"
            accept=".xlsx,.xls,.csv"
            onChange={handleFileSelect}
            className="w-full p-2 border rounded-lg"
          />
          <p className="text-sm text-gray-500 mt-1">
            File should contain columns: Beneficiary Name, Account Number, Bank Name, Amount, Narration
          </p>
        </div>

        {previewData.length > 0 && (
          <div>
            <h3 className="font-medium text-gray-900 mb-2">
              Preview ({previewData.length} payments)
            </h3>
            <div className="max-h-64 overflow-y-auto border rounded-lg">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50 sticky top-0">
                  <tr>
                    {Object.keys(previewData[0]).map(key => (
                      <th key={key} className="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">
                        {key}
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {previewData.slice(0, 10).map((row, idx) => (
                    <tr key={idx}>
                      {Object.values(row).map((value: any, i) => (
                        <td key={i} className="px-4 py-2 text-sm text-gray-900">
                          {value}
                        </td>
                      ))}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            {previewData.length > 10 && (
              <p className="text-sm text-gray-500 mt-2">
                Showing first 10 of {previewData.length} payments
              </p>
            )}
          </div>
        )}

        <button
          onClick={handleUploadPayments}
          disabled={uploading || !selectedFile || !selectedAccount}
          className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed"
        >
          {uploading ? 'Uploading...' : 'Upload & Process Payments'}
        </button>
      </div>

      {/* Payment Batches */}
      <div className="bg-white rounded-lg shadow">
        <div className="p-6 border-b">
          <h2 className="text-lg font-semibold text-gray-900">Payment Batches</h2>
        </div>
        
        <div className="divide-y">
          {payments.length === 0 ? (
            <div className="p-12 text-center text-gray-500">
              No payment batches found
            </div>
          ) : (
            payments.map(payment => (
              <div key={payment.id} className="p-6 hover:bg-gray-50">
                <div className="flex justify-between items-start">
                  <div className="flex-1">
                    <div className="flex items-center gap-3 mb-2">
                      <h3 className="font-semibold text-gray-900">{payment.batchNumber}</h3>
                      <span className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(payment.status)}`}>
                        {payment.status}
                      </span>
                    </div>
                    
                    <div className="grid grid-cols-4 gap-4 mt-4">
                      <div>
                        <p className="text-sm text-gray-500">Payment Type</p>
                        <p className="font-medium text-gray-900">{payment.paymentType}</p>
                      </div>
                      <div>
                        <p className="text-sm text-gray-500">Total Amount</p>
                        <p className="font-medium text-gray-900">KES {payment.totalAmount.toLocaleString()}</p>
                      </div>
                      <div>
                        <p className="text-sm text-gray-500">Payment Count</p>
                        <p className="font-medium text-gray-900">{payment.totalCount}</p>
                      </div>
                      <div>
                        <p className="text-sm text-gray-500">Uploaded</p>
                        <p className="font-medium text-gray-900">
                          {new Date(payment.uploadedDate).toLocaleDateString()}
                        </p>
                      </div>
                    </div>

                    {payment.processedDate && (
                      <p className="text-sm text-gray-500 mt-2">
                        Processed: {new Date(payment.processedDate).toLocaleString()}
                      </p>
                    )}
                  </div>
                </div>
              </div>
            ))
          )}
        </div>
      </div>
    </div>
  );
};
