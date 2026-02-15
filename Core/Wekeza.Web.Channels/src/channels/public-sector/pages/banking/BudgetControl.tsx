import { useState, useEffect } from 'react';
import { AlertTriangle, TrendingUp, DollarSign, CheckCircle } from 'lucide-react';

interface BudgetAllocation {
  id: string;
  departmentName: string;
  category: string;
  fiscalYear: number;
  allocatedAmount: number;
  spentAmount: number;
  committedAmount: number;
  availableAmount: number;
  utilizationPercentage: number;
  commitmentPercentage: number;
  alert: string;
}

interface BudgetCommitment {
  id: string;
  commitmentnumber: string;
  amount: number;
  purpose: string;
  status: string;
  createdat: string;
  createdby: string;
  departmentname: string;
  category: string;
}

export function BudgetControl() {
  const [allocations, setAllocations] = useState<BudgetAllocation[]>([]);
  const [commitments, setCommitments] = useState<BudgetCommitment[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCommitmentModal, setShowCommitmentModal] = useState(false);
  const [selectedAllocation, setSelectedAllocation] = useState<BudgetAllocation | null>(null);
  const [commitmentForm, setCommitmentForm] = useState({
    amount: '',
    purpose: '',
    reference: ''
  });
  const [message, setMessage] = useState({ type: '', text: '' });

  useEffect(() => {
    fetchBudgetData();
  }, []);

  const fetchBudgetData = async () => {
    try {
      const token = localStorage.getItem('token');
      
      // Fetch allocations
      const allocResponse = await fetch('http://localhost:5000/api/public-sector/budget/allocations?fiscalYear=2026', {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      const allocData = await allocResponse.json();
      if (allocData.success) {
        setAllocations(allocData.data);
      }

      // Fetch commitments
      const commResponse = await fetch('http://localhost:5000/api/public-sector/budget/commitments', {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      const commData = await commResponse.json();
      if (commData.success) {
        setCommitments(commData.data);
      }
    } catch (err) {
      console.error('Error fetching budget data:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateCommitment = async () => {
    if (!selectedAllocation) return;

    try {
      const token = localStorage.getItem('token');
      const response = await fetch('http://localhost:5000/api/public-sector/budget/commitments', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          allocationId: selectedAllocation.id,
          amount: parseFloat(commitmentForm.amount),
          purpose: commitmentForm.purpose,
          reference: commitmentForm.reference
        })
      });

      const data = await response.json();

      if (data.success) {
        setMessage({ type: 'success', text: `Commitment created: ${data.data.commitmentNumber}` });
        setShowCommitmentModal(false);
        setCommitmentForm({ amount: '', purpose: '', reference: '' });
        fetchBudgetData();
      } else {
        setMessage({ type: 'error', text: data.message });
      }
    } catch (err: any) {
      setMessage({ type: 'error', text: err.message });
    }
  };

  const getAlertColor = (alert: string) => {
    switch (alert) {
      case 'CRITICAL': return 'bg-red-100 text-red-800 border-red-200';
      case 'HIGH': return 'bg-orange-100 text-orange-800 border-orange-200';
      case 'MEDIUM': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      default: return 'bg-green-100 text-green-800 border-green-200';
    }
  };

  const getAlertIcon = (alert: string) => {
    switch (alert) {
      case 'CRITICAL':
      case 'HIGH':
      case 'MEDIUM':
        return <AlertTriangle className="h-5 w-5" />;
      default:
        return <CheckCircle className="h-5 w-5" />;
    }
  };

  if (loading) {
    return <div className="p-6">Loading budget data...</div>;
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Budget Control & Commitments</h2>
        <p className="text-gray-600">Track budget allocations, commitments, and spending</p>
      </div>

      {message.text && (
        <div className={`p-4 rounded-lg ${
          message.type === 'success' ? 'bg-green-50 border border-green-200 text-green-800' : 'bg-red-50 border border-red-200 text-red-800'
        }`}>
          {message.text}
        </div>
      )}

      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-white p-6 rounded-lg shadow-md">
          <div className="flex items-center justify-between mb-2">
            <span className="text-gray-600">Total Allocated</span>
            <DollarSign className="h-5 w-5 text-blue-600" />
          </div>
          <div className="text-2xl font-bold">
            KES {allocations.reduce((sum, a) => sum + a.allocatedAmount, 0).toLocaleString()}
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-md">
          <div className="flex items-center justify-between mb-2">
            <span className="text-gray-600">Total Spent</span>
            <TrendingUp className="h-5 w-5 text-green-600" />
          </div>
          <div className="text-2xl font-bold">
            KES {allocations.reduce((sum, a) => sum + a.spentAmount, 0).toLocaleString()}
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-md">
          <div className="flex items-center justify-between mb-2">
            <span className="text-gray-600">Total Committed</span>
            <CheckCircle className="h-5 w-5 text-orange-600" />
          </div>
          <div className="text-2xl font-bold">
            KES {allocations.reduce((sum, a) => sum + a.committedAmount, 0).toLocaleString()}
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-md">
          <div className="flex items-center justify-between mb-2">
            <span className="text-gray-600">Total Available</span>
            <DollarSign className="h-5 w-5 text-purple-600" />
          </div>
          <div className="text-2xl font-bold">
            KES {allocations.reduce((sum, a) => sum + a.availableAmount, 0).toLocaleString()}
          </div>
        </div>
      </div>

      {/* Budget Allocations */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="p-6 border-b border-gray-200">
          <h3 className="text-lg font-bold">Budget Allocations - FY 2026</h3>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Department</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Category</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Allocated</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Spent</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Committed</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Available</th>
                <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase">Utilization</th>
                <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase">Alert</th>
                <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {allocations.map((allocation) => (
                <tr key={allocation.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {allocation.departmentName}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                    {allocation.category}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-gray-900">
                    {allocation.allocatedAmount.toLocaleString()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-gray-900">
                    {allocation.spentAmount.toLocaleString()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-gray-900">
                    {allocation.committedAmount.toLocaleString()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-right font-medium text-gray-900">
                    {allocation.availableAmount.toLocaleString()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center">
                    <div className="flex items-center justify-center">
                      <div className="w-full max-w-xs bg-gray-200 rounded-full h-2">
                        <div
                          className="bg-blue-600 h-2 rounded-full"
                          style={{ width: `${Math.min(allocation.utilizationPercentage, 100)}%` }}
                        />
                      </div>
                      <span className="ml-2 text-sm text-gray-600">
                        {allocation.utilizationPercentage.toFixed(1)}%
                      </span>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center">
                    <span className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium border ${getAlertColor(allocation.alert)}`}>
                      {getAlertIcon(allocation.alert)}
                      <span className="ml-1">{allocation.alert}</span>
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center text-sm">
                    <button
                      onClick={() => {
                        setSelectedAllocation(allocation);
                        setShowCommitmentModal(true);
                      }}
                      className="text-green-600 hover:text-green-900 font-medium"
                    >
                      Create Commitment
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Recent Commitments */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="p-6 border-b border-gray-200">
          <h3 className="text-lg font-bold">Recent Commitments</h3>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Commitment #</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Department</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Purpose</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Amount</th>
                <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase">Status</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Created</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {commitments.slice(0, 10).map((commitment) => (
                <tr key={commitment.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {commitment.commitmentnumber}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                    {commitment.departmentname}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-900">
                    {commitment.purpose}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-gray-900">
                    KES {commitment.amount.toLocaleString()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center">
                    <span className={`px-2 py-1 text-xs font-medium rounded-full ${
                      commitment.status === 'ACTIVE' ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                    }`}>
                      {commitment.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                    {new Date(commitment.createdat).toLocaleDateString()}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Create Commitment Modal */}
      {showCommitmentModal && selectedAllocation && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-md w-full">
            <h3 className="text-lg font-bold mb-4">Create Budget Commitment</h3>
            <div className="mb-4">
              <p className="text-sm text-gray-600 mb-1">Department:</p>
              <p className="font-medium">{selectedAllocation.departmentName} - {selectedAllocation.category}</p>
            </div>
            <div className="mb-4">
              <p className="text-sm text-gray-600 mb-1">Available Budget:</p>
              <p className="font-medium text-green-600">KES {selectedAllocation.availableAmount.toLocaleString()}</p>
            </div>
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-2">Amount (KES) *</label>
              <input
                type="number"
                value={commitmentForm.amount}
                onChange={(e) => setCommitmentForm({ ...commitmentForm, amount: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500"
                placeholder="Enter amount"
              />
            </div>
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-2">Purpose *</label>
              <textarea
                value={commitmentForm.purpose}
                onChange={(e) => setCommitmentForm({ ...commitmentForm, purpose: e.target.value })}
                rows={3}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500"
                placeholder="Enter purpose"
              />
            </div>
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-2">Reference</label>
              <input
                type="text"
                value={commitmentForm.reference}
                onChange={(e) => setCommitmentForm({ ...commitmentForm, reference: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500"
                placeholder="Enter reference (optional)"
              />
            </div>
            <div className="flex gap-3">
              <button
                onClick={handleCreateCommitment}
                disabled={!commitmentForm.amount || !commitmentForm.purpose}
                className="flex-1 bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 disabled:bg-gray-400"
              >
                Create Commitment
              </button>
              <button
                onClick={() => setShowCommitmentModal(false)}
                className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
