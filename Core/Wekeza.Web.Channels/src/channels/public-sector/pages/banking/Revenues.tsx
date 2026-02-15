import React, { useState, useEffect, useMemo } from 'react';
import { RevenueCollection } from '../../types';
import { LoadingSpinner, ErrorAlert, SuccessAlert } from '../../components';

export const Revenues: React.FC = () => {
  const [revenues, setRevenues] = useState<RevenueCollection[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [filterType, setFilterType] = useState<string>('ALL');
  const [filterReconciled, setFilterReconciled] = useState<string>('ALL');
  const [dateFrom, setDateFrom] = useState('');
  const [dateTo, setDateTo] = useState('');

  useEffect(() => {
    fetchRevenues();
  }, []);

  const fetchRevenues = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/public-sector/revenues', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to fetch revenues');
      
      const data = await response.json();
      setRevenues(data.data || []);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const handleReconcile = async (revenueId: string) => {
    try {
      const response = await fetch('/api/public-sector/revenues/reconcile', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ revenueId })
      });

      if (!response.ok) throw new Error('Failed to reconcile revenue');
      
      setSuccess('Revenue reconciled successfully');
      fetchRevenues();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    }
  };

  const filteredRevenues = useMemo(() => {
    return revenues.filter(rev => {
      if (filterType !== 'ALL' && rev.revenueType !== filterType) return false;
      if (filterReconciled === 'RECONCILED' && !rev.reconciled) return false;
      if (filterReconciled === 'UNRECONCILED' && rev.reconciled) return false;
      if (dateFrom && new Date(rev.collectionDate) < new Date(dateFrom)) return false;
      if (dateTo && new Date(rev.collectionDate) > new Date(dateTo)) return false;
      return true;
    });
  }, [revenues, filterType, filterReconciled, dateFrom, dateTo]);

  const calculateMetrics = useMemo(() => {
    const total = filteredRevenues.reduce((sum, rev) => sum + rev.amount, 0);
    const reconciled = filteredRevenues.filter(r => r.reconciled).reduce((sum, rev) => sum + rev.amount, 0);
    const unreconciled = filteredRevenues.filter(r => !r.reconciled).reduce((sum, rev) => sum + rev.amount, 0);
    
    const byType = {
      TAX: filteredRevenues.filter(r => r.revenueType === 'TAX').reduce((sum, rev) => sum + rev.amount, 0),
      FEE: filteredRevenues.filter(r => r.revenueType === 'FEE').reduce((sum, rev) => sum + rev.amount, 0),
      LICENSE: filteredRevenues.filter(r => r.revenueType === 'LICENSE').reduce((sum, rev) => sum + rev.amount, 0),
      FINE: filteredRevenues.filter(r => r.revenueType === 'FINE').reduce((sum, rev) => sum + rev.amount, 0),
      OTHER: filteredRevenues.filter(r => r.revenueType === 'OTHER').reduce((sum, rev) => sum + rev.amount, 0)
    };

    return { total, reconciled, unreconciled, byType };
  }, [filteredRevenues]);

  const metrics = calculateMetrics;

  if (loading) return <LoadingSpinner />;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Revenue Collection</h1>

      {error && <ErrorAlert message={error} onClose={() => setError(null)} />}
      {success && <SuccessAlert message={success} />}

      {/* Metrics Cards */}
      <div className="grid grid-cols-4 gap-4">
        <div className="bg-white p-6 rounded-lg shadow">
          <p className="text-sm text-gray-500">Total Revenue</p>
          <p className="text-2xl font-bold text-gray-900">
            KES {metrics.total.toLocaleString()}
          </p>
        </div>
        
        <div className="bg-white p-6 rounded-lg shadow">
          <p className="text-sm text-gray-500">Reconciled</p>
          <p className="text-2xl font-bold text-green-600">
            KES {metrics.reconciled.toLocaleString()}
          </p>
        </div>
        
        <div className="bg-white p-6 rounded-lg shadow">
          <p className="text-sm text-gray-500">Unreconciled</p>
          <p className="text-2xl font-bold text-yellow-600">
            KES {metrics.unreconciled.toLocaleString()}
          </p>
        </div>
        
        <div className="bg-white p-6 rounded-lg shadow">
          <p className="text-sm text-gray-500">Collection Count</p>
          <p className="text-2xl font-bold text-blue-600">
            {filteredRevenues.length}
          </p>
        </div>
      </div>

      {/* Revenue by Type */}
      <div className="bg-white p-6 rounded-lg shadow">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Revenue by Type</h2>
        <div className="grid grid-cols-5 gap-4">
          {Object.entries(metrics.byType).map(([type, amount]) => (
            <div key={type} className="text-center p-4 bg-gray-50 rounded-lg">
              <p className="text-sm text-gray-500">{type}</p>
              <p className="text-lg font-bold text-gray-900">
                KES {amount.toLocaleString()}
              </p>
            </div>
          ))}
        </div>
      </div>

      {/* Filters */}
      <div className="bg-white p-4 rounded-lg shadow">
        <div className="grid grid-cols-4 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Revenue Type
            </label>
            <select
              value={filterType}
              onChange={(e) => setFilterType(e.target.value)}
              className="w-full p-2 border rounded-lg"
            >
              <option value="ALL">All Types</option>
              <option value="TAX">Tax</option>
              <option value="FEE">Fee</option>
              <option value="LICENSE">License</option>
              <option value="FINE">Fine</option>
              <option value="OTHER">Other</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Reconciliation Status
            </label>
            <select
              value={filterReconciled}
              onChange={(e) => setFilterReconciled(e.target.value)}
              className="w-full p-2 border rounded-lg"
            >
              <option value="ALL">All</option>
              <option value="RECONCILED">Reconciled</option>
              <option value="UNRECONCILED">Unreconciled</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Date From
            </label>
            <input
              type="date"
              value={dateFrom}
              onChange={(e) => setDateFrom(e.target.value)}
              className="w-full p-2 border rounded-lg"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Date To
            </label>
            <input
              type="date"
              value={dateTo}
              onChange={(e) => setDateTo(e.target.value)}
              className="w-full p-2 border rounded-lg"
            />
          </div>
        </div>
      </div>

      {/* Revenue Collections Table */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Collection Date
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Revenue Type
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Payer Name
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Reference
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Amount
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
            {filteredRevenues.map(revenue => (
              <tr key={revenue.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 whitespace-nowrap">
                  <p className="text-sm text-gray-900">
                    {new Date(revenue.collectionDate).toLocaleDateString()}
                  </p>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <span className="px-2 py-1 bg-blue-100 text-blue-800 rounded-full text-xs font-medium">
                    {revenue.revenueType}
                  </span>
                </td>
                <td className="px-6 py-4">
                  <p className="text-sm font-medium text-gray-900">{revenue.payerName}</p>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <p className="text-sm text-gray-900">{revenue.payerReference}</p>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <p className="text-sm font-medium text-gray-900">
                    KES {revenue.amount.toLocaleString()}
                  </p>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  {revenue.reconciled ? (
                    <span className="px-2 py-1 bg-green-100 text-green-800 rounded-full text-xs font-medium">
                      Reconciled
                    </span>
                  ) : (
                    <span className="px-2 py-1 bg-yellow-100 text-yellow-800 rounded-full text-xs font-medium">
                      Pending
                    </span>
                  )}
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  {!revenue.reconciled && (
                    <button
                      onClick={() => handleReconcile(revenue.id)}
                      className="text-blue-600 hover:text-blue-800 text-sm"
                    >
                      Reconcile
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};
