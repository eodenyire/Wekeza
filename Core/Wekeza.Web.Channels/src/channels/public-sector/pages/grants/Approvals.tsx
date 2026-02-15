import React, { useState, useEffect } from 'react';
import { GrantApplication } from '../../types';
import { LoadingSpinner, ErrorAlert, SuccessAlert, ConfirmDialog } from '../../components';

export const Approvals: React.FC = () => {
  const [applications, setApplications] = useState<GrantApplication[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [selectedApp, setSelectedApp] = useState<GrantApplication | null>(null);
  const [showApproveDialog, setShowApproveDialog] = useState(false);
  const [showRejectDialog, setShowRejectDialog] = useState(false);
  const [comments, setComments] = useState('');
  const [actionLoading, setActionLoading] = useState(false);

  useEffect(() => {
    fetchPendingApplications();
  }, []);

  const fetchPendingApplications = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/public-sector/grants/applications?status=UNDER_REVIEW', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to fetch applications');
      
      const data = await response.json();
      setApplications(data.data || []);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const handleApprove = async () => {
    if (!selectedApp || !comments.trim()) {
      setError('Please provide approval comments');
      return;
    }

    try {
      setActionLoading(true);
      const response = await fetch(`/api/public-sector/grants/applications/${selectedApp.id}/approve`, {
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
      setSelectedApp(null);
      setComments('');
      fetchPendingApplications();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setActionLoading(false);
    }
  };

  const handleReject = async () => {
    if (!selectedApp || !comments.trim()) {
      setError('Please provide rejection reason');
      return;
    }

    try {
      setActionLoading(true);
      const response = await fetch(`/api/public-sector/grants/applications/${selectedApp.id}/reject`, {
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
      setSelectedApp(null);
      setComments('');
      fetchPendingApplications();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setActionLoading(false);
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Grant Approvals</h1>

      {error && <ErrorAlert message={error} onClose={() => setError(null)} />}
      {success && <SuccessAlert message={success} />}

      {applications.length === 0 ? (
        <div className="bg-white rounded-lg shadow p-12 text-center">
          <p className="text-gray-500">No applications pending approval</p>
        </div>
      ) : (
        <div className="grid gap-4">
          {applications.map(application => (
            <div key={application.id} className="bg-white p-6 rounded-lg shadow">
              <div className="flex justify-between items-start mb-4">
                <div className="flex-1">
                  <h3 className="text-lg font-semibold text-gray-900 mb-2">
                    {application.projectTitle}
                  </h3>
                  <p className="text-sm text-gray-600 mb-4">{application.applicationNumber}</p>
                  
                  <div className="grid grid-cols-2 gap-4 mb-4">
                    <div>
                      <p className="text-sm text-gray-500">Applicant</p>
                      <p className="font-medium text-gray-900">{application.applicantName}</p>
                      <p className="text-sm text-gray-600">{application.applicantType.replace('_', ' ')}</p>
                    </div>
                    
                    <div>
                      <p className="text-sm text-gray-500">Requested Amount</p>
                      <p className="text-lg font-bold text-blue-600">
                        KES {application.requestedAmount.toLocaleString()}
                      </p>
                    </div>
                  </div>

                  <div className="mb-4">
                    <p className="text-sm font-medium text-gray-700">Project Description</p>
                    <p className="text-sm text-gray-600 mt-1">{application.projectDescription}</p>
                  </div>

                  <div className="mb-4">
                    <p className="text-sm font-medium text-gray-700">Expected Impact</p>
                    <p className="text-sm text-gray-600 mt-1">{application.expectedImpact}</p>
                  </div>

                  {application.approvals && application.approvals.length > 0 && (
                    <div className="p-3 bg-blue-50 rounded">
                      <p className="text-sm font-medium text-blue-900 mb-2">
                        Previous Approvals ({application.approvals.length})
                      </p>
                      {application.approvals.map((approval, idx) => (
                        <div key={idx} className="text-sm text-blue-800">
                          {approval.approverName} ({approval.approverRole}): {approval.decision}
                          {approval.comments && ` - ${approval.comments}`}
                        </div>
                      ))}
                    </div>
                  )}
                </div>

                <div className="flex flex-col gap-2 ml-4">
                  <button
                    onClick={() => {
                      setSelectedApp(application);
                      setShowApproveDialog(true);
                    }}
                    className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 whitespace-nowrap"
                  >
                    Approve
                  </button>
                  <button
                    onClick={() => {
                      setSelectedApp(application);
                      setShowRejectDialog(true);
                    }}
                    className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 whitespace-nowrap"
                  >
                    Reject
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Approve Dialog */}
      <ConfirmDialog
        isOpen={showApproveDialog}
        title="Approve Grant Application"
        message={`Approve grant application for ${selectedApp?.applicantName}?`}
        onConfirm={handleApprove}
        onCancel={() => {
          setShowApproveDialog(false);
          setSelectedApp(null);
          setComments('');
        }}
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
        title="Reject Grant Application"
        message={`Reject grant application for ${selectedApp?.applicantName}?`}
        onConfirm={handleReject}
        onCancel={() => {
          setShowRejectDialog(false);
          setSelectedApp(null);
          setComments('');
        }}
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
