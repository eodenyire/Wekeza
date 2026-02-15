import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { LoanApplication } from '../../types';
import { LoadingSpinner, ErrorAlert, SuccessAlert, ConfirmDialog } from '../../components';

export const LoanDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [application, setApplication] = useState<LoanApplication | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [actionLoading, setActionLoading] = useState(false);
  const [showApproveDialog, setShowApproveDialog] = useState(false);
  const [showRejectDialog, setShowRejectDialog] = useState(false);
  const [comments, setComments] = useState('');

  useEffect(() => {
    fetchApplicationDetails();
  }, [id]);

  const fetchApplicationDetails = async () => {
    try {
      setLoading(true);
      const response = await fetch(`/api/public-sector/loans/applications/${id}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to fetch application details');
      
      const data = await response.json();
      setApplication(data.data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const handleApprove = async () => {
    if (!comments.trim()) {
      setError('Please provide approval comments');
      return;
    }

    try {
      setActionLoading(true);
      const response = await fetch(`/api/public-sector/loans/applications/${id}/approve`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ comments })
      });

      if (!response.ok) throw new Error('Failed to approve application');
      
      setSuccess('Application approved successfully');
      setShowApproveDialog(false);
      setTimeout(() => navigate('/public-sector/lending/applications'), 2000);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setActionLoading(false);
    }
  };

  const handleReject = async () => {
    if (!comments.trim()) {
      setError('Please provide rejection reason');
      return;
    }

    try {
      setActionLoading(true);
      const response = await fetch(`/api/public-sector/loans/applications/${id}/reject`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ comments })
      });

      if (!response.ok) throw new Error('Failed to reject application');
      
      setSuccess('Application rejected');
      setShowRejectDialog(false);
      setTimeout(() => navigate('/public-sector/lending/applications'), 2000);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setActionLoading(false);
    }
  };

  if (loading) return <LoadingSpinner />;
  if (error && !application) return <ErrorAlert message={error} onRetry={fetchApplicationDetails} />;
  if (!application) return <ErrorAlert message="Application not found" />;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <button
            onClick={() => navigate('/public-sector/lending/applications')}
            className="text-blue-600 hover:text-blue-800 mb-2"
          >
            ← Back to Applications
          </button>
          <h1 className="text-2xl font-bold text-gray-900">{application.applicationNumber}</h1>
        </div>
        
        {application.status === 'UNDER_REVIEW' && (
          <div className="flex gap-3">
            <button
              onClick={() => setShowRejectDialog(true)}
              className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700"
            >
              Reject
            </button>
            <button
              onClick={() => setShowApproveDialog(true)}
              className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700"
            >
              Approve
            </button>
          </div>
        )}
      </div>

      {error && <ErrorAlert message={error} onClose={() => setError(null)} />}
      {success && <SuccessAlert message={success} />}

      {/* Application Details */}
      <div className="bg-white rounded-lg shadow p-6 space-y-6">
        <div>
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Government Entity Details</h2>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <p className="text-sm text-gray-500">Entity Name</p>
              <p className="font-medium text-gray-900">{application.governmentEntity.name}</p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Type</p>
              <p className="font-medium text-gray-900">{application.governmentEntity.type}</p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Contact Person</p>
              <p className="font-medium text-gray-900">{application.governmentEntity.contactPerson}</p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Email</p>
              <p className="font-medium text-gray-900">{application.governmentEntity.email}</p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Phone</p>
              <p className="font-medium text-gray-900">{application.governmentEntity.phone}</p>
            </div>
          </div>
        </div>

        <div className="border-t pt-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Loan Details</h2>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <p className="text-sm text-gray-500">Loan Type</p>
              <p className="font-medium text-gray-900">{application.loanType.replace('_', ' ')}</p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Requested Amount</p>
              <p className="font-medium text-gray-900">KES {application.requestedAmount.toLocaleString()}</p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Tenor</p>
              <p className="font-medium text-gray-900">{application.tenor} months</p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Status</p>
              <p className="font-medium text-gray-900">{application.status.replace('_', ' ')}</p>
            </div>
          </div>
          
          <div className="mt-4">
            <p className="text-sm text-gray-500">Purpose</p>
            <p className="text-gray-900 mt-1">{application.purpose}</p>
          </div>
        </div>

        {application.creditAssessment && (
          <div className="border-t pt-6">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">Credit Assessment</h2>
            
            {application.creditAssessment.sovereignRating && (
              <div className="mb-4">
                <p className="text-sm text-gray-500">Sovereign Rating</p>
                <p className="font-medium text-gray-900">{application.creditAssessment.sovereignRating}</p>
              </div>
            )}

            <div className="mb-4">
              <p className="text-sm text-gray-500 mb-2">Revenue Streams</p>
              <div className="space-y-2">
                {application.creditAssessment.revenueStreams.map((stream, idx) => (
                  <div key={idx} className="flex justify-between items-center p-3 bg-gray-50 rounded">
                    <div>
                      <p className="font-medium text-gray-900">{stream.source}</p>
                      <p className="text-sm text-gray-600">Reliability: {stream.reliability}</p>
                    </div>
                    <p className="font-medium text-gray-900">KES {stream.annualAmount.toLocaleString()}</p>
                  </div>
                ))}
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4 mb-4">
              <div>
                <p className="text-sm text-gray-500">Existing Debt</p>
                <p className="font-medium text-gray-900">KES {application.creditAssessment.existingDebt.toLocaleString()}</p>
              </div>
              <div>
                <p className="text-sm text-gray-500">Debt Service Ratio</p>
                <p className="font-medium text-gray-900">{(application.creditAssessment.debtServiceRatio * 100).toFixed(2)}%</p>
              </div>
            </div>

            <div className="mb-4">
              <p className="text-sm text-gray-500">Recommendation</p>
              <p className={`font-medium ${
                application.creditAssessment.recommendation === 'APPROVE' ? 'text-green-600' :
                application.creditAssessment.recommendation === 'REJECT' ? 'text-red-600' :
                'text-yellow-600'
              }`}>
                {application.creditAssessment.recommendation}
              </p>
            </div>

            <div>
              <p className="text-sm text-gray-500">Comments</p>
              <p className="text-gray-900 mt-1">{application.creditAssessment.comments}</p>
            </div>

            <div className="mt-4 text-sm text-gray-500">
              Assessed by {application.creditAssessment.assessedBy} on{' '}
              {new Date(application.creditAssessment.assessedDate).toLocaleDateString()}
            </div>
          </div>
        )}
      </div>

      {/* Approve Dialog */}
      <ConfirmDialog
        isOpen={showApproveDialog}
        title="Approve Loan Application"
        message="Are you sure you want to approve this loan application?"
        onConfirm={handleApprove}
        onCancel={() => setShowApproveDialog(false)}
        confirmText="Approve"
        confirmColor="green"
        loading={actionLoading}
      >
        <textarea
          value={comments}
          onChange={(e) => setComments(e.target.value)}
          placeholder="Enter approval comments..."
          className="w-full p-3 border rounded-lg mt-4"
          rows={4}
        />
      </ConfirmDialog>

      {/* Reject Dialog */}
      <ConfirmDialog
        isOpen={showRejectDialog}
        title="Reject Loan Application"
        message="Are you sure you want to reject this loan application?"
        onConfirm={handleReject}
        onCancel={() => setShowRejectDialog(false)}
        confirmText="Reject"
        confirmColor="red"
        loading={actionLoading}
      >
        <textarea
          value={comments}
          onChange={(e) => setComments(e.target.value)}
          placeholder="Enter rejection reason..."
          className="w-full p-3 border rounded-lg mt-4"
          rows={4}
        />
      </ConfirmDialog>
    </div>
  );
};
