import React, { useState, useEffect } from 'react';
import { GrantProgram } from '../../types';
import { LoadingSpinner, ErrorAlert } from '../../components';
import { useNavigate } from 'react-router-dom';

export const Programs: React.FC = () => {
  const navigate = useNavigate();
  const [programs, setPrograms] = useState<GrantProgram[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filterStatus, setFilterStatus] = useState<'ALL' | 'OPEN' | 'CLOSED'>('ALL');
  const [filterCategory, setFilterCategory] = useState<string>('ALL');

  useEffect(() => {
    fetchPrograms();
  }, []);

  const fetchPrograms = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/public-sector/grants/programs', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to fetch grant programs');
      
      const data = await response.json();
      setPrograms(data.data || []);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const filteredPrograms = programs.filter(program => {
    if (filterStatus !== 'ALL' && program.status !== filterStatus) return false;
    if (filterCategory !== 'ALL' && program.category !== filterCategory) return false;
    return true;
  });

  const getStatusColor = (status: string) => {
    return status === 'OPEN' ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800';
  };

  const getCategoryColor = (category: string) => {
    const colors: Record<string, string> = {
      EDUCATION: 'bg-blue-100 text-blue-800',
      HEALTH: 'bg-red-100 text-red-800',
      INFRASTRUCTURE: 'bg-yellow-100 text-yellow-800',
      ENVIRONMENT: 'bg-green-100 text-green-800',
      OTHER: 'bg-gray-100 text-gray-800'
    };
    return colors[category] || colors.OTHER;
  };

  if (loading) return <LoadingSpinner />;
  if (error) return <ErrorAlert message={error} onRetry={fetchPrograms} />;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Grant Programs</h1>
      </div>

      {/* Filters */}
      <div className="bg-white p-4 rounded-lg shadow">
        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Status
            </label>
            <select
              value={filterStatus}
              onChange={(e) => setFilterStatus(e.target.value as any)}
              className="w-full p-2 border rounded-lg"
            >
              <option value="ALL">All Status</option>
              <option value="OPEN">Open</option>
              <option value="CLOSED">Closed</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Category
            </label>
            <select
              value={filterCategory}
              onChange={(e) => setFilterCategory(e.target.value)}
              className="w-full p-2 border rounded-lg"
            >
              <option value="ALL">All Categories</option>
              <option value="EDUCATION">Education</option>
              <option value="HEALTH">Health</option>
              <option value="INFRASTRUCTURE">Infrastructure</option>
              <option value="ENVIRONMENT">Environment</option>
              <option value="OTHER">Other</option>
            </select>
          </div>
        </div>
      </div>

      {/* Programs Grid */}
      <div className="grid gap-6">
        {filteredPrograms.map(program => (
          <div key={program.id} className="bg-white p-6 rounded-lg shadow hover:shadow-md transition-shadow">
            <div className="flex justify-between items-start mb-4">
              <div className="flex-1">
                <div className="flex items-center gap-3 mb-2">
                  <h3 className="text-xl font-semibold text-gray-900">{program.name}</h3>
                  <span className={`px-3 py-1 rounded-full text-xs font-medium ${getStatusColor(program.status)}`}>
                    {program.status}
                  </span>
                  <span className={`px-3 py-1 rounded-full text-xs font-medium ${getCategoryColor(program.category)}`}>
                    {program.category}
                  </span>
                </div>
                
                <p className="text-gray-700 mb-4">{program.description}</p>

                <div className="grid grid-cols-2 gap-4 mb-4">
                  <div>
                    <p className="text-sm text-gray-500">Maximum Grant Amount</p>
                    <p className="text-lg font-bold text-blue-600">
                      KES {program.maxAmount.toLocaleString()}
                    </p>
                  </div>
                  
                  <div>
                    <p className="text-sm text-gray-500">Application Deadline</p>
                    <p className="font-medium text-gray-900">
                      {new Date(program.applicationDeadline).toLocaleDateString()}
                    </p>
                  </div>
                </div>

                <div>
                  <p className="text-sm font-medium text-gray-700 mb-2">Eligibility Criteria:</p>
                  <ul className="list-disc list-inside space-y-1">
                    {program.eligibilityCriteria.map((criteria, idx) => (
                      <li key={idx} className="text-sm text-gray-600">{criteria}</li>
                    ))}
                  </ul>
                </div>
              </div>
            </div>

            {program.status === 'OPEN' && (
              <div className="flex gap-3 mt-4 pt-4 border-t">
                <button
                  onClick={() => navigate(`/public-sector/grants/apply/${program.id}`)}
                  className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
                >
                  Apply Now
                </button>
                <button
                  onClick={() => navigate(`/public-sector/grants/programs/${program.id}`)}
                  className="px-6 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200"
                >
                  View Details
                </button>
              </div>
            )}
          </div>
        ))}
      </div>

      {filteredPrograms.length === 0 && (
        <div className="bg-white rounded-lg shadow p-12 text-center">
          <p className="text-gray-500">No grant programs found matching your filters</p>
        </div>
      )}
    </div>
  );
};
