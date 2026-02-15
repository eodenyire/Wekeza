import React, { useState } from 'react';
import { LoadingSpinner, ErrorAlert, SuccessAlert } from '../../components';
import { exportToExcel, exportToPDF } from '../../utils/export';

type ReportType = 'ACCOUNT_STATEMENT' | 'TRANSACTION_SUMMARY' | 'REVENUE_REPORT' | 'PAYMENT_REPORT' | 'CUSTOM';

export const Reports: React.FC = () => {
  const [reportType, setReportType] = useState<ReportType>('ACCOUNT_STATEMENT');
  const [dateFrom, setDateFrom] = useState('');
  const [dateTo, setDateTo] = useState('');
  const [accountId, setAccountId] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [reportData, setReportData] = useState<any>(null);

  const handleGenerateReport = async () => {
    if (!dateFrom || !dateTo) {
      setError('Please select date range');
      return;
    }

    try {
      setLoading(true);
      const params = new URLSearchParams({
        reportType,
        dateFrom,
        dateTo,
        ...(accountId && { accountId })
      });

      const response = await fetch(`/api/public-sector/reports?${params}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to generate report');
      
      const data = await response.json();
      setReportData(data.data);
      setSuccess('Report generated successfully');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const handleExportExcel = () => {
    if (!reportData) return;
    exportToExcel(reportData, `${reportType}_${dateFrom}_${dateTo}`);
    setSuccess('Report exported to Excel');
  };

  const handleExportPDF = () => {
    if (!reportData) return;
    exportToPDF(reportData, `${reportType}_${dateFrom}_${dateTo}`);
    setSuccess('Report exported to PDF');
  };

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Financial Reports</h1>

      {error && <ErrorAlert message={error} onClose={() => setError(null)} />}
      {success && <SuccessAlert message={success} />}

      {/* Report Configuration */}
      <div className="bg-white p-6 rounded-lg shadow space-y-4">
        <h2 className="text-lg font-semibold text-gray-900">Generate Report</h2>
        
        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Report Type *
            </label>
            <select
              value={reportType}
              onChange={(e) => setReportType(e.target.value as ReportType)}
              className="w-full p-2 border rounded-lg"
            >
              <option value="ACCOUNT_STATEMENT">Account Statement</option>
              <option value="TRANSACTION_SUMMARY">Transaction Summary</option>
              <option value="REVENUE_REPORT">Revenue Report</option>
              <option value="PAYMENT_REPORT">Payment Report</option>
              <option value="CUSTOM">Custom Report</option>
            </select>
          </div>

          {reportType === 'ACCOUNT_STATEMENT' && (
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Account
              </label>
              <input
                type="text"
                value={accountId}
                onChange={(e) => setAccountId(e.target.value)}
                placeholder="Enter account number"
                className="w-full p-2 border rounded-lg"
              />
            </div>
          )}
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Date From *
            </label>
            <input
              type="date"
              value={dateFrom}
              onChange={(e) => setDateFrom(e.target.value)}
              className="w-full p-2 border rounded-lg"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Date To *
            </label>
            <input
              type="date"
              value={dateTo}
              onChange={(e) => setDateTo(e.target.value)}
              className="w-full p-2 border rounded-lg"
            />
          </div>
        </div>

        <button
          onClick={handleGenerateReport}
          disabled={loading}
          className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-400"
        >
          {loading ? 'Generating...' : 'Generate Report'}
        </button>
      </div>

      {/* Report Preview */}
      {reportData && (
        <div className="bg-white p-6 rounded-lg shadow space-y-4">
          <div className="flex justify-between items-center">
            <h2 className="text-lg font-semibold text-gray-900">Report Preview</h2>
            <div className="flex gap-2">
              <button
                onClick={handleExportExcel}
                className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700"
              >
                Export to Excel
              </button>
              <button
                onClick={handleExportPDF}
                className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700"
              >
                Export to PDF
              </button>
            </div>
          </div>

          {/* Report Header */}
          <div className="border-b pb-4">
            <h3 className="text-xl font-bold text-gray-900">{reportType.replace('_', ' ')}</h3>
            <p className="text-sm text-gray-500">
              Period: {new Date(dateFrom).toLocaleDateString()} - {new Date(dateTo).toLocaleDateString()}
            </p>
            <p className="text-sm text-gray-500">
              Generated: {new Date().toLocaleString()}
            </p>
          </div>

          {/* Report Content */}
          <div className="space-y-4">
            {reportType === 'ACCOUNT_STATEMENT' && reportData.transactions && (
              <div>
                <h4 className="font-semibold text-gray-900 mb-2">Account: {reportData.accountNumber}</h4>
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Date</th>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Description</th>
                      <th className="px-4 py-2 text-right text-xs font-medium text-gray-500">Debit</th>
                      <th className="px-4 py-2 text-right text-xs font-medium text-gray-500">Credit</th>
                      <th className="px-4 py-2 text-right text-xs font-medium text-gray-500">Balance</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {reportData.transactions.map((txn: any, idx: number) => (
                      <tr key={idx}>
                        <td className="px-4 py-2 text-sm">{new Date(txn.date).toLocaleDateString()}</td>
                        <td className="px-4 py-2 text-sm">{txn.description}</td>
                        <td className="px-4 py-2 text-sm text-right">{txn.debit ? txn.debit.toLocaleString() : '-'}</td>
                        <td className="px-4 py-2 text-sm text-right">{txn.credit ? txn.credit.toLocaleString() : '-'}</td>
                        <td className="px-4 py-2 text-sm text-right font-medium">{txn.balance.toLocaleString()}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}

            {reportType === 'TRANSACTION_SUMMARY' && reportData.summary && (
              <div className="grid grid-cols-3 gap-4">
                <div className="p-4 bg-gray-50 rounded-lg">
                  <p className="text-sm text-gray-500">Total Transactions</p>
                  <p className="text-2xl font-bold text-gray-900">{reportData.summary.totalCount}</p>
                </div>
                <div className="p-4 bg-gray-50 rounded-lg">
                  <p className="text-sm text-gray-500">Total Debits</p>
                  <p className="text-2xl font-bold text-red-600">
                    KES {reportData.summary.totalDebits.toLocaleString()}
                  </p>
                </div>
                <div className="p-4 bg-gray-50 rounded-lg">
                  <p className="text-sm text-gray-500">Total Credits</p>
                  <p className="text-2xl font-bold text-green-600">
                    KES {reportData.summary.totalCredits.toLocaleString()}
                  </p>
                </div>
              </div>
            )}

            {reportType === 'REVENUE_REPORT' && reportData.revenues && (
              <div>
                <div className="grid grid-cols-4 gap-4 mb-4">
                  {Object.entries(reportData.revenueByType || {}).map(([type, amount]: [string, any]) => (
                    <div key={type} className="p-4 bg-gray-50 rounded-lg">
                      <p className="text-sm text-gray-500">{type}</p>
                      <p className="text-lg font-bold text-gray-900">
                        KES {amount.toLocaleString()}
                      </p>
                    </div>
                  ))}
                </div>
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Date</th>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Type</th>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Payer</th>
                      <th className="px-4 py-2 text-right text-xs font-medium text-gray-500">Amount</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {reportData.revenues.map((rev: any, idx: number) => (
                      <tr key={idx}>
                        <td className="px-4 py-2 text-sm">{new Date(rev.date).toLocaleDateString()}</td>
                        <td className="px-4 py-2 text-sm">{rev.type}</td>
                        <td className="px-4 py-2 text-sm">{rev.payer}</td>
                        <td className="px-4 py-2 text-sm text-right font-medium">
                          KES {rev.amount.toLocaleString()}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}

            {reportType === 'PAYMENT_REPORT' && reportData.payments && (
              <div>
                <div className="grid grid-cols-3 gap-4 mb-4">
                  <div className="p-4 bg-gray-50 rounded-lg">
                    <p className="text-sm text-gray-500">Total Payments</p>
                    <p className="text-2xl font-bold text-gray-900">{reportData.totalCount}</p>
                  </div>
                  <div className="p-4 bg-gray-50 rounded-lg">
                    <p className="text-sm text-gray-500">Total Amount</p>
                    <p className="text-2xl font-bold text-blue-600">
                      KES {reportData.totalAmount.toLocaleString()}
                    </p>
                  </div>
                  <div className="p-4 bg-gray-50 rounded-lg">
                    <p className="text-sm text-gray-500">Success Rate</p>
                    <p className="text-2xl font-bold text-green-600">
                      {reportData.successRate}%
                    </p>
                  </div>
                </div>
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Date</th>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Batch</th>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Type</th>
                      <th className="px-4 py-2 text-right text-xs font-medium text-gray-500">Count</th>
                      <th className="px-4 py-2 text-right text-xs font-medium text-gray-500">Amount</th>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Status</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {reportData.payments.map((payment: any, idx: number) => (
                      <tr key={idx}>
                        <td className="px-4 py-2 text-sm">{new Date(payment.date).toLocaleDateString()}</td>
                        <td className="px-4 py-2 text-sm">{payment.batchNumber}</td>
                        <td className="px-4 py-2 text-sm">{payment.type}</td>
                        <td className="px-4 py-2 text-sm text-right">{payment.count}</td>
                        <td className="px-4 py-2 text-sm text-right font-medium">
                          KES {payment.amount.toLocaleString()}
                        </td>
                        <td className="px-4 py-2 text-sm">{payment.status}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};
