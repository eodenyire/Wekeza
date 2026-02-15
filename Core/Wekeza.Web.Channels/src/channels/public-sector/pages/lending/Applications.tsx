import React, { useState, useEffect } from 'react';
import { LoanApplication } from '../../types';
import { LoadingSpinner, ErrorAlert, EmptyState } from '../../components';
import { useNavigate } from 'react-router-dom';

type FilterStatus = 'ALL' | 'PENDING' | 'UNDER_REVIEW' | 'APPROVED' | 'REJECTED';

export const Applications: React.FC = () => {
  const navigate = useNavigate();
  const [applications, setApplications] = useState<LoanApplication[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filterStatus, setFilterStatus] = useState<FilterStatus>('ALL');

  useEffect(() => {
    fetchApplications();
  }, []);

  const fetchApplications = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/public-sector/loans/applications', {
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

  const filteredApplications = applications.filter(app => 
    filterStatus === 'ALL' || app.status === filterStatus
  );

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'PENDING': return 'bg-yellow-100 text-yellow-800';
      case 'UNDER_REVIEW': return 'bg-blue-100 text-blue-800';
      case 'APPROVED': return 'bg-green-100 text-green-800';
      case 'REJECTED': return 'bg-red-100 text-red-800';
      case 'DISBURSED': return 'bg-purple-100 text-purple-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  if (loading) return <LoadingSpinner />;
  if (error) return <ErrorAlert message={error} onRetry={fetchApplications} />;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Loan Applications</h1>
      </div>

      {/* Filters */}
      <div className="bg-white p-4 rounded-lg shadow">
        <div className="flex gap-2">
          {(['ALL', 'PENDING', 'UNDER_REVIEW', 'APPROVED', 'REJECTED'] as FilterStatus[]).map(status => (
            <button
              key={status}
              onClick={() => setFilterStatus(status)}
              className={`px-4 py-2 rounded-lg font-medium transition-colors ${
                filterStatus === status
                  ? 'bg-blue-600 text-white'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
            >
              {status.replace('_', ' ')}
            </button>
          ))}
        </div>
      </div>

      {/* Applications List */}
      {filteredApplications.length === 0 ? (
        <EmptyState message="No loan applications found" />
      ) : (
        <div className="grid gap-4">
          {filteredApplications.map(application => (
            <div
              key={application.id}
              className="bg-white p-6 rounded-lg shadow hover:shadow-md transition-shadow cursor-pointer"
              onClick={() => navigate(`/public-sector/lending/applications/${application.id}`)}
            >
              <div className="flex justify-between items-start">
                <div className="flex-1">
                  <div className="flex items-center gap-3 mb-2">
                    <h3 className="text-lg font-semibold text-gray-900">
                      {application.applicationNumber}
                    </h3>
                    <span className={`px-3 py-1 rounded-full text-xs font-medium ${getStatusColor(application.status)}`}>
                      {application.status.replace('_', ' ')}
                    </span>
                  </div>
                  
                  <div className="grid grid-cols-2 gap-4 mt-4">
                    <div>
                      <p className="text-sm text-gray-500">Government Entity</p>
                      <p className="font-medium text-gray-900">{application.governmentEntity.name}</p>
                      <p className="text-sm text-gray-600">{application.governmentEntity.type}</p>
                    </div>
                    
                    <div>
                      <p className="text-sm text-gray-500">Loan Type</p>
                      <p className="font-medium text-gray-900">{application.loanType.replace('_', ' ')}</p>
                    </div>
                    
                    <div>
                      <p className="text-sm text-gray-500">Requested Amount</p>
                      <p className="font-medium text-gray-900">
                        KES {application.requestedAmount.toLocaleString()}
                      </p>
                    </div>
                    
                    <div>
                      <p className="text-sm text-gray-500">Tenor</p>
                      <p className="font-medium text-gray-900">{application.tenor} months</p>
                    </div>
                  </div>

                  <div className="mt-4">
                    <p className="text-sm text-gray-500">Purpose</p>
                    <p className="text-gray-700 line-clamp-2">{application.purpose}</p>
                  </div>

                  {application.creditAssessment && (
                    <div className="mt-4 p-3 bg-gray-50 rounded">
                      <p className="text-sm font-medium text-gray-700">Credit Assessment</p>
                      <p className="text-sm text-gray-600">
                        Recommendation: <span className="font-medium">{application.creditAssessment.recommendation}</span>
                      </p>
                      <p className="text-sm text-gray-600">
                        Debt Service Ratio: {(application.creditAssessment.debtServiceRatio * 100).toFixed(2)}%
                      </p>
                    </div>
                  )}
                </div>

                <div className="text-right">
                  <p className="text-sm text-gray-500">Submitted</p>
                  <p className="text-sm font-medium text-gray-900">
                    {new Date(application.submittedDate).toLocaleDateString()}
                  </p>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
