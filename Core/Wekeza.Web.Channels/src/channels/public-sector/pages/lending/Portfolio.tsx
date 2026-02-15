import React, { useState, useEffect, useMemo } from 'react';
import { Loan } from '../../types';
import { LoadingSpinner, ErrorAlert } from '../../components';
import { useNavigate } from 'react-router-dom';

export const Portfolio: React.FC = () => {
  const navigate = useNavigate();
  const [loans, setLoans] = useState<Loan[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedLoan, setSelectedLoan] = useState<Loan | null>(null);

  useEffect(() => {
    fetchLoanPortfolio();
  }, []);

  const fetchLoanPortfolio = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/public-sector/loans/portfolio', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to fetch loan portfolio');
      
      const data = await response.json();
      setLoans(data.data || []);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const fetchRepaymentSchedule = async (loanId: string) => {
    try {
      const response = await fetch(`/api/public-sector/loans/${loanId}/schedule`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to fetch repayment schedule');
      
      const data = await response.json();
      const loan = loans.find(l => l.id === loanId);
      if (loan) {
        setSelectedLoan({ ...loan, repaymentSchedule: data.data });
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    }
  };

  const calculateMetrics = useMemo(() => {
    const totalOutstanding = loans.reduce((sum, loan) => sum + loan.outstandingBalance, 0);
    const nationalGovt = loans
      .filter(l => l.governmentEntity.type === 'NATIONAL')
      .reduce((sum, loan) => sum + loan.outstandingBalance, 0);
    const countyGovt = loans
      .filter(l => l.governmentEntity.type === 'COUNTY')
      .reduce((sum, loan) => sum + loan.outstandingBalance, 0);
    const nplCount = loans.filter(l => l.status === 'DEFAULT').length;
    const nplRatio = loans.length > 0 ? (nplCount / loans.length) * 100 : 0;

    return { totalOutstanding, nationalGovt, countyGovt, nplRatio };
  }, [loans]);

  const metrics = calculateMetrics;

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'ACTIVE': return 'bg-green-100 text-green-800';
      case 'CLOSED': return 'bg-gray-100 text-gray-800';
      case 'DEFAULT': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  if (loading) return <LoadingSpinner />;
  if (error) return <ErrorAlert message={error} onRetry={fetchLoanPortfolio} />;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Loan Portfolio</h1>

      {/* Metrics Cards */}
      <div className="grid grid-cols-4 gap-4">
        <div className="bg-white p-6 rounded-lg shadow">
          <p className="text-sm text-gray-500">Total Outstanding</p>
          <p className="text-2xl font-bold text-gray-900">
            KES {metrics.totalOutstanding.toLocaleString()}
          </p>
        </div>
        
        <div className="bg-white p-6 rounded-lg shadow">
          <p className="text-sm text-gray-500">National Government</p>
          <p className="text-2xl font-bold text-blue-600">
            KES {metrics.nationalGovt.toLocaleString()}
          </p>
        </div>
        
        <div className="bg-white p-6 rounded-lg shadow">
          <p className="text-sm text-gray-500">County Governments</p>
          <p className="text-2xl font-bold text-green-600">
            KES {metrics.countyGovt.toLocaleString()}
          </p>
        </div>
        
        <div className="bg-white p-6 rounded-lg shadow">
          <p className="text-sm text-gray-500">NPL Ratio</p>
          <p className={`text-2xl font-bold ${metrics.nplRatio > 5 ? 'text-red-600' : 'text-green-600'}`}>
            {metrics.nplRatio.toFixed(2)}%
          </p>
        </div>
      </div>

      {/* Loans Table */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Loan Number
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Government Entity
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Principal
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Outstanding
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Interest Rate
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Maturity Date
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Status
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Actions
              </th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {loans.map(loan => (
              <tr key={loan.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 whitespace-nowrap">
                  <p className="font-medium text-gray-900">{loan.loanNumber}</p>
                </td>
                <td className="px-6 py-4">
                  <p className="font-medium text-gray-900">{loan.governmentEntity.name}</p>
                  <p className="text-sm text-gray-500">{loan.governmentEntity.type}</p>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <p className="text-gray-900">KES {loan.principalAmount.toLocaleString()}</p>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <p className="font-medium text-gray-900">
                    KES {loan.outstandingBalance.toLocaleString()}
                  </p>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <p className="text-gray-900">{loan.interestRate}%</p>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <p className="text-gray-900">
                    {new Date(loan.maturityDate).toLocaleDateString()}
                  </p>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(loan.status)}`}>
                    {loan.status}
                  </span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <button
                    onClick={() => fetchRepaymentSchedule(loan.id)}
                    className="text-blue-600 hover:text-blue-800 text-sm"
                  >
                    View Schedule
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Repayment Schedule Modal */}
      {selectedLoan && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-4xl w-full max-h-[80vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold text-gray-900">
                Repayment Schedule - {selectedLoan.loanNumber}
              </h2>
              <button
                onClick={() => setSelectedLoan(null)}
                className="text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            </div>

            <div className="mb-4 p-4 bg-gray-50 rounded-lg">
              <div className="grid grid-cols-3 gap-4">
                <div>
                  <p className="text-sm text-gray-500">Principal Amount</p>
                  <p className="font-medium text-gray-900">
                    KES {selectedLoan.principalAmount.toLocaleString()}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Outstanding Balance</p>
                  <p className="font-medium text-gray-900">
                    KES {selectedLoan.outstandingBalance.toLocaleString()}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Interest Rate</p>
                  <p className="font-medium text-gray-900">{selectedLoan.interestRate}%</p>
                </div>
              </div>
            </div>

            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                    Installment
                  </th>
                  <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                    Due Date
                  </th>
                  <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                    Principal
                  </th>
                  <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                    Interest
                  </th>
                  <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                    Total
                  </th>
                  <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                    Paid
                  </th>
                  <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                    Status
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {selectedLoan.repaymentSchedule.map(schedule => (
                  <tr key={schedule.installmentNumber}>
                    <td className="px-4 py-2">{schedule.installmentNumber}</td>
                    <td className="px-4 py-2">
                      {new Date(schedule.dueDate).toLocaleDateString()}
                    </td>
                    <td className="px-4 py-2">
                      KES {schedule.principalAmount.toLocaleString()}
                    </td>
                    <td className="px-4 py-2">
                      KES {schedule.interestAmount.toLocaleString()}
                    </td>
                    <td className="px-4 py-2 font-medium">
                      KES {schedule.totalAmount.toLocaleString()}
                    </td>
                    <td className="px-4 py-2">
                      KES {schedule.paidAmount.toLocaleString()}
                    </td>
                    <td className="px-4 py-2">
                      <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                        schedule.status === 'PAID' ? 'bg-green-100 text-green-800' :
                        schedule.status === 'OVERDUE' ? 'bg-red-100 text-red-800' :
                        'bg-yellow-100 text-yellow-800'
                      }`}>
                        {schedule.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
};
