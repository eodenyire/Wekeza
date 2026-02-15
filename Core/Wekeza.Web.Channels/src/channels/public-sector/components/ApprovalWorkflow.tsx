import { CheckCircle, XCircle, Clock, User } from 'lucide-react';

interface Approval {
  approverName: string;
  approverRole: string;
  decision: 'APPROVED' | 'REJECTED' | 'PENDING';
  comments?: string;
  approvedDate?: string;
}

interface ApprovalWorkflowProps {
  approvals: Approval[];
  requiredApprovals?: number;
  onApprove?: (comments: string) => void;
  onReject?: (comments: string) => void;
  canApprove?: boolean;
}

export default function ApprovalWorkflow({
  approvals,
  requiredApprovals = 2,
  onApprove,
  onReject,
  canApprove = false
}: ApprovalWorkflowProps) {
  const approvedCount = approvals.filter(a => a.decision === 'APPROVED').length;
  const rejectedCount = approvals.filter(a => a.decision === 'REJECTED').length;
  const pendingCount = approvals.filter(a => a.decision === 'PENDING').length;

  const getDecisionIcon = (decision: string) => {
    switch (decision) {
      case 'APPROVED':
        return <CheckCircle className="w-5 h-5 text-green-600" />;
      case 'REJECTED':
        return <XCircle className="w-5 h-5 text-red-600" />;
      case 'PENDING':
        return <Clock className="w-5 h-5 text-yellow-600" />;
      default:
        return <Clock className="w-5 h-5 text-gray-400" />;
    }
  };

  const getDecisionColor = (decision: string) => {
    switch (decision) {
      case 'APPROVED': return 'bg-green-50 border-green-200';
      case 'REJECTED': return 'bg-red-50 border-red-200';
      case 'PENDING': return 'bg-yellow-50 border-yellow-200';
      default: return 'bg-gray-50 border-gray-200';
    }
  };

  return (
    <div className="space-y-4">
      {/* Progress Summary */}
      <div className="bg-white rounded-lg shadow-md p-4">
        <h3 className="font-semibold text-gray-900 mb-3">Approval Progress</h3>
        <div className="flex items-center gap-4 mb-3">
          <div className="flex-1">
            <div className="flex justify-between text-sm mb-1">
              <span className="text-gray-600">Approvals</span>
              <span className="font-medium text-gray-900">
                {approvedCount} / {requiredApprovals}
              </span>
            </div>
            <div className="w-full bg-gray-200 rounded-full h-2">
              <div
                className="bg-green-600 h-2 rounded-full transition-all"
                style={{ width: `${(approvedCount / requiredApprovals) * 100}%` }}
              ></div>
            </div>
          </div>
        </div>
        <div className="grid grid-cols-3 gap-3 text-sm">
          <div className="text-center">
            <p className="text-gray-600">Approved</p>
            <p className="text-lg font-bold text-green-600">{approvedCount}</p>
          </div>
          <div className="text-center">
            <p className="text-gray-600">Pending</p>
            <p className="text-lg font-bold text-yellow-600">{pendingCount}</p>
          </div>
          <div className="text-center">
            <p className="text-gray-600">Rejected</p>
            <p className="text-lg font-bold text-red-600">{rejectedCount}</p>
          </div>
        </div>
      </div>

      {/* Approval History */}
      <div className="bg-white rounded-lg shadow-md p-4">
        <h3 className="font-semibold text-gray-900 mb-3">Approval History</h3>
        <div className="space-y-3">
          {approvals.map((approval, index) => (
            <div
              key={index}
              className={`border rounded-lg p-4 ${getDecisionColor(approval.decision)}`}
            >
              <div className="flex items-start justify-between mb-2">
                <div className="flex items-center gap-3">
                  {getDecisionIcon(approval.decision)}
                  <div>
                    <div className="flex items-center gap-2">
                      <User className="w-4 h-4 text-gray-400" />
                      <p className="font-medium text-gray-900">{approval.approverName}</p>
                    </div>
                    <p className="text-sm text-gray-600">{approval.approverRole}</p>
                  </div>
                </div>
                <span className={`px-2 py-1 text-xs font-medium rounded-full ${
                  approval.decision === 'APPROVED' ? 'bg-green-100 text-green-700' :
                  approval.decision === 'REJECTED' ? 'bg-red-100 text-red-700' :
                  'bg-yellow-100 text-yellow-700'
                }`}>
                  {approval.decision}
                </span>
              </div>
              {approval.comments && (
                <p className="text-sm text-gray-700 mt-2 pl-8">{approval.comments}</p>
              )}
              {approval.approvedDate && (
                <p className="text-xs text-gray-500 mt-2 pl-8">
                  {new Date(approval.approvedDate).toLocaleString()}
                </p>
              )}
            </div>
          ))}
        </div>
      </div>

      {/* Action Buttons */}
      {canApprove && (onApprove || onReject) && (
        <div className="bg-white rounded-lg shadow-md p-4">
          <h3 className="font-semibold text-gray-900 mb-3">Your Action Required</h3>
          <div className="flex gap-3">
            {onApprove && (
              <button
                onClick={() => {
                  const comments = prompt('Add approval comments (optional):');
                  if (comments !== null) {
                    onApprove(comments);
                  }
                }}
                className="flex-1 flex items-center justify-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
              >
                <CheckCircle className="w-4 h-4" />
                Approve
              </button>
            )}
            {onReject && (
              <button
                onClick={() => {
                  const comments = prompt('Add rejection reason (required):');
                  if (comments && comments.trim()) {
                    onReject(comments);
                  } else if (comments !== null) {
                    alert('Rejection reason is required');
                  }
                }}
                className="flex-1 flex items-center justify-center gap-2 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
              >
                <XCircle className="w-4 h-4" />
                Reject
              </button>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
