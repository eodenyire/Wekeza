import { Building2, DollarSign, Calendar, TrendingUp } from 'lucide-react';

interface LoanCardProps {
  loanNumber: string;
  entityName: string;
  entityType: 'NATIONAL' | 'COUNTY';
  principalAmount: number;
  outstandingBalance: number;
  interestRate: number;
  status: 'ACTIVE' | 'CLOSED' | 'DEFAULT';
  maturityDate: string;
  onViewDetails?: () => void;
  onDisburse?: () => void;
}

export default function LoanCard({
  loanNumber,
  entityName,
  entityType,
  principalAmount,
  outstandingBalance,
  interestRate,
  status,
  maturityDate,
  onViewDetails,
  onDisburse
}: LoanCardProps) {
  const getStatusColor = () => {
    switch (status) {
      case 'ACTIVE': return 'bg-green-100 text-green-700';
      case 'CLOSED': return 'bg-gray-100 text-gray-700';
      case 'DEFAULT': return 'bg-red-100 text-red-700';
      default: return 'bg-gray-100 text-gray-700';
    }
  };

  const getEntityColor = () => {
    return entityType === 'NATIONAL' ? 'bg-indigo-100 text-indigo-700' : 'bg-teal-100 text-teal-700';
  };

  const repaymentProgress = ((principalAmount - outstandingBalance) / principalAmount) * 100;

  return (
    <div className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
      <div className="flex justify-between items-start mb-4">
        <div>
          <h3 className="text-lg font-semibold text-gray-900">{loanNumber}</h3>
          <div className="flex items-center gap-2 mt-1">
            <Building2 className="w-4 h-4 text-gray-400" />
            <span className="text-sm text-gray-600">{entityName}</span>
            <span className={`px-2 py-0.5 text-xs font-medium rounded-full ${getEntityColor()}`}>
              {entityType}
            </span>
          </div>
        </div>
        <span className={`px-3 py-1 text-xs font-medium rounded-full ${getStatusColor()}`}>
          {status}
        </span>
      </div>

      <div className="grid grid-cols-2 gap-4 mb-4">
        <div>
          <p className="text-sm text-gray-600 mb-1">Principal Amount</p>
          <p className="text-lg font-bold text-gray-900">
            KES {principalAmount.toLocaleString()}
          </p>
        </div>
        <div>
          <p className="text-sm text-gray-600 mb-1">Outstanding</p>
          <p className="text-lg font-bold text-gray-900">
            KES {outstandingBalance.toLocaleString()}
          </p>
        </div>
      </div>

      <div className="mb-4">
        <div className="flex justify-between text-sm mb-1">
          <span className="text-gray-600">Repayment Progress</span>
          <span className="font-medium text-gray-900">{repaymentProgress.toFixed(1)}%</span>
        </div>
        <div className="w-full bg-gray-200 rounded-full h-2">
          <div
            className="bg-blue-600 h-2 rounded-full transition-all"
            style={{ width: `${repaymentProgress}%` }}
          ></div>
        </div>
      </div>

      <div className="flex items-center justify-between text-sm mb-4">
        <div className="flex items-center gap-2">
          <TrendingUp className="w-4 h-4 text-gray-400" />
          <span className="text-gray-600">Interest Rate:</span>
          <span className="font-medium text-gray-900">{interestRate.toFixed(2)}%</span>
        </div>
        <div className="flex items-center gap-2">
          <Calendar className="w-4 h-4 text-gray-400" />
          <span className="text-gray-600">{new Date(maturityDate).toLocaleDateString()}</span>
        </div>
      </div>

      {(onViewDetails || onDisburse) && (
        <div className="flex gap-2 pt-4 border-t">
          {onViewDetails && (
            <button
              onClick={onViewDetails}
              className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
            >
              View Details
            </button>
          )}
          {onDisburse && (
            <button
              onClick={onDisburse}
              className="flex-1 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
            >
              Disburse
            </button>
          )}
        </div>
      )}
    </div>
  );
}
