import { Gift, Calendar, DollarSign, CheckCircle } from 'lucide-react';

interface GrantCardProps {
  programName: string;
  category: 'EDUCATION' | 'HEALTH' | 'INFRASTRUCTURE' | 'ENVIRONMENT' | 'OTHER';
  maxAmount: number;
  deadline: string;
  status: 'OPEN' | 'CLOSED';
  description?: string;
  applicationStatus?: 'SUBMITTED' | 'UNDER_REVIEW' | 'APPROVED' | 'REJECTED' | 'DISBURSED';
  onApply?: () => void;
  onViewDetails?: () => void;
}

export default function GrantCard({
  programName,
  category,
  maxAmount,
  deadline,
  status,
  description,
  applicationStatus,
  onApply,
  onViewDetails
}: GrantCardProps) {
  const getCategoryColor = () => {
    const colors: Record<string, string> = {
      EDUCATION: 'bg-blue-100 text-blue-700',
      HEALTH: 'bg-red-100 text-red-700',
      INFRASTRUCTURE: 'bg-gray-100 text-gray-700',
      ENVIRONMENT: 'bg-green-100 text-green-700',
      OTHER: 'bg-purple-100 text-purple-700'
    };
    return colors[category] || 'bg-gray-100 text-gray-700';
  };

  const getApplicationStatusColor = () => {
    const colors: Record<string, string> = {
      SUBMITTED: 'bg-yellow-100 text-yellow-700',
      UNDER_REVIEW: 'bg-blue-100 text-blue-700',
      APPROVED: 'bg-green-100 text-green-700',
      REJECTED: 'bg-red-100 text-red-700',
      DISBURSED: 'bg-purple-100 text-purple-700'
    };
    return applicationStatus ? colors[applicationStatus] || 'bg-gray-100 text-gray-700' : '';
  };

  const isDeadlineSoon = () => {
    const daysUntilDeadline = Math.ceil((new Date(deadline).getTime() - new Date().getTime()) / (1000 * 60 * 60 * 24));
    return daysUntilDeadline <= 7 && daysUntilDeadline > 0;
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
      <div className="flex justify-between items-start mb-4">
        <div className="flex-1">
          <div className="flex items-center gap-2 mb-2">
            <Gift className="w-5 h-5 text-blue-600" />
            <h3 className="text-lg font-semibold text-gray-900">{programName}</h3>
          </div>
          <div className="flex gap-2">
            <span className={`px-2 py-1 text-xs font-medium rounded-full ${getCategoryColor()}`}>
              {category}
            </span>
            <span className={`px-2 py-1 text-xs font-medium rounded-full ${
              status === 'OPEN' ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'
            }`}>
              {status}
            </span>
            {applicationStatus && (
              <span className={`px-2 py-1 text-xs font-medium rounded-full ${getApplicationStatusColor()}`}>
                {applicationStatus.replace('_', ' ')}
              </span>
            )}
          </div>
        </div>
      </div>

      {description && (
        <p className="text-gray-700 text-sm mb-4 line-clamp-2">{description}</p>
      )}

      <div className="space-y-2 mb-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2 text-sm text-gray-600">
            <DollarSign className="w-4 h-4" />
            <span>Max Amount:</span>
          </div>
          <span className="font-semibold text-gray-900">KES {maxAmount.toLocaleString()}</span>
        </div>

        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2 text-sm text-gray-600">
            <Calendar className="w-4 h-4" />
            <span>Deadline:</span>
          </div>
          <span className={`font-medium ${isDeadlineSoon() ? 'text-red-600' : 'text-gray-900'}`}>
            {new Date(deadline).toLocaleDateString()}
            {isDeadlineSoon() && ' (Soon!)'}
          </span>
        </div>
      </div>

      {(onApply || onViewDetails) && (
        <div className="flex gap-2 pt-4 border-t">
          {onViewDetails && (
            <button
              onClick={onViewDetails}
              className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
            >
              View Details
            </button>
          )}
          {onApply && status === 'OPEN' && (
            <button
              onClick={onApply}
              className="flex-1 flex items-center justify-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
            >
              <CheckCircle className="w-4 h-4" />
              Apply Now
            </button>
          )}
        </div>
      )}
    </div>
  );
}
