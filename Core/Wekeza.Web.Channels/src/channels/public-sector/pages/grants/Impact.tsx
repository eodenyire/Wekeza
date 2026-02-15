import React, { useState, useEffect, useMemo } from 'react';
import { Grant } from '../../types';
import { LoadingSpinner, ErrorAlert } from '../../components';

export const Impact: React.FC = () => {
  const [grants, setGrants] = useState<Grant[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedGrant, setSelectedGrant] = useState<Grant | null>(null);

  useEffect(() => {
    fetchImpactData();
  }, []);

  const fetchImpactData = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/public-sector/grants/impact', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to fetch impact data');
      
      const data = await response.json();
      setGrants(data.data || []);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const calculateMetrics = useMemo(() => {
    const totalDisbursed = grants.reduce((sum, g) => sum + g.disbursedAmount, 0);
    const totalUtilized = grants.reduce((sum, g) => 
      sum + g.utilizationReports.reduce((s, r) => s + r.amountUtilized, 0), 0
    );
    const activeGrants = grants.filter(g => g.complianceStatus !== 'NON_COMPLIANT').length;
    const complianceRate = grants.length > 0 
      ? (grants.filter(g => g.complianceStatus === 'COMPLIANT').length / grants.length) * 100 
      : 0;

    return { totalDisbursed, totalUtilized, activeGrants, complianceRate };
  }, [grants]);

  const metrics = calculateMetrics;

  const getComplianceColor = (status: string) => {
    switch (status) {
      case 'COMPLIANT': return 'bg-green-100 text-green-800';
      case 'NON_COMPLIANT': return 'bg-red-100 text-red-800';
      case 'PENDING_REPORT': return 'bg-yellow-100 text-yellow-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  if (loading) return <LoadingSpinner />;
  if (error) return <ErrorAlert message={error} onRetry={fetchImpactData} />;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Grant Impact & Monitoring</h1>

      {/* Metrics Cards */}
      <div className="grid grid-cols-4 gap-4">
        <div className="bg-white p-6 rounded-lg shadow">
          <p className="text-sm text-gray-500">Total Disbursed</p>
          <p className="text-2xl font-bold text-gray-900">
            KES {metrics.totalDisbursed.toLocaleString()}
          </p>
        </div>
        
        <div className="bg-white p-6 rounded-lg shadow">
          <p className="text-sm text-gray-500">Total Utilized</p>
          <p className="text-2xl font-bold text-blue-600">
            KES {metrics.totalUtilized.toLocaleString()}
          </p>
        </div>
        
        <div className="bg-white p-6 rounded-lg shadow">
          <p className="text-sm text-gray-500">Active Grants</p>
          <p className="text-2xl font-bold text-green-600">
            {metrics.activeGrants}
          </p>
        </div>
        
        <div className="bg-white p-6 rounded-lg shadow">
          <p className="text-sm text-gray-500">Compliance Rate</p>
          <p className="text-2xl font-bold text-purple-600">
            {metrics.complianceRate.toFixed(1)}%
          </p>
        </div>
      </div>

      {/* Grants List */}
      <div className="grid gap-4">
        {grants.map(grant => (
          <div key={grant.id} className="bg-white p-6 rounded-lg shadow">
            <div className="flex justify-between items-start mb-4">
              <div className="flex-1">
                <div className="flex items-center gap-3 mb-2">
                  <h3 className="text-lg font-semibold text-gray-900">{grant.grantNumber}</h3>
                  <span className={`px-3 py-1 rounded-full text-xs font-medium ${getComplianceColor(grant.complianceStatus)}`}>
                    {grant.complianceStatus.replace('_', ' ')}
                  </span>
                </div>
                
                <div className="grid grid-cols-3 gap-4 mt-4">
                  <div>
                    <p className="text-sm text-gray-500">Approved Amount</p>
                    <p className="font-medium text-gray-900">
                      KES {grant.approvedAmount.toLocaleString()}
                    </p>
                  </div>
                  
                  <div>
                    <p className="text-sm text-gray-500">Disbursed Amount</p>
                    <p className="font-medium text-blue-600">
                      KES {grant.disbursedAmount.toLocaleString()}
                    </p>
                  </div>
                  
                  <div>
                    <p className="text-sm text-gray-500">Disbursement Date</p>
                    <p className="font-medium text-gray-900">
                      {new Date(grant.disbursementDate).toLocaleDateString()}
                    </p>
                  </div>
                </div>

                <div className="mt-4">
                  <p className="text-sm font-medium text-gray-700 mb-2">
                    Utilization Reports ({grant.utilizationReports.length})
                  </p>
                  {grant.utilizationReports.length === 0 ? (
                    <p className="text-sm text-gray-500">No reports submitted yet</p>
                  ) : (
                    <div className="space-y-2">
                      {grant.utilizationReports.slice(0, 2).map((report, idx) => (
                        <div key={idx} className="p-3 bg-gray-50 rounded">
                          <div className="flex justify-between items-start mb-2">
                            <p className="text-sm font-medium text-gray-900">{report.reportingPeriod}</p>
                            <p className="text-sm font-medium text-blue-600">
                              KES {report.amountUtilized.toLocaleString()}
                            </p>
                          </div>
                          <p className="text-sm text-gray-600 mb-1">
                            <span className="font-medium">Activities:</span> {report.activities}
                          </p>
                          <p className="text-sm text-gray-600">
                            <span className="font-medium">Outcomes:</span> {report.outcomes}
                          </p>
                          {report.challenges && (
                            <p className="text-sm text-gray-600 mt-1">
                              <span className="font-medium">Challenges:</span> {report.challenges}
                            </p>
                          )}
                          <p className="text-xs text-gray-500 mt-2">
                            Submitted: {new Date(report.submittedDate).toLocaleDateString()}
                          </p>
                        </div>
                      ))}
                      {grant.utilizationReports.length > 2 && (
                        <button
                          onClick={() => setSelectedGrant(grant)}
                          className="text-sm text-blue-600 hover:text-blue-800"
                        >
                          View all {grant.utilizationReports.length} reports
                        </button>
                      )}
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {grants.length === 0 && (
        <div className="bg-white rounded-lg shadow p-12 text-center">
          <p className="text-gray-500">No grant data available</p>
        </div>
      )}

      {/* Full Reports Modal */}
      {selectedGrant && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-4xl w-full max-h-[80vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-xl font-bold text-gray-900">
                Utilization Reports - {selectedGrant.grantNumber}
              </h2>
              <button
                onClick={() => setSelectedGrant(null)}
                className="text-gray-500 hover:text-gray-700 text-2xl"
              >
                ✕
              </button>
            </div>

            <div className="space-y-4">
              {selectedGrant.utilizationReports.map((report, idx) => (
                <div key={idx} className="p-4 border rounded-lg">
                  <div className="flex justify-between items-start mb-3">
                    <h3 className="font-semibold text-gray-900">{report.reportingPeriod}</h3>
                    <p className="font-bold text-blue-600">
                      KES {report.amountUtilized.toLocaleString()}
                    </p>
                  </div>
                  
                  <div className="space-y-2">
                    <div>
                      <p className="text-sm font-medium text-gray-700">Activities</p>
                      <p className="text-sm text-gray-600">{report.activities}</p>
                    </div>
                    
                    <div>
                      <p className="text-sm font-medium text-gray-700">Outcomes</p>
                      <p className="text-sm text-gray-600">{report.outcomes}</p>
                    </div>
                    
                    {report.challenges && (
                      <div>
                        <p className="text-sm font-medium text-gray-700">Challenges</p>
                        <p className="text-sm text-gray-600">{report.challenges}</p>
                      </div>
                    )}
                    
                    <p className="text-xs text-gray-500 mt-2">
                      Submitted: {new Date(report.submittedDate).toLocaleDateString()}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
