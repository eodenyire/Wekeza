import React, { useState, useEffect } from 'react';
import { GrantApplication, GrantProgram } from '../../types';
import { LoadingSpinner, ErrorAlert, SuccessAlert } from '../../components';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useNavigate, useParams } from 'react-router-dom';

const applicationSchema = z.object({
  programId: z.string().min(1, 'Program is required'),
  applicantName: z.string().min(1, 'Applicant name is required'),
  applicantType: z.enum(['NGO', 'COMMUNITY_GROUP', 'INSTITUTION', 'INDIVIDUAL']),
  requestedAmount: z.number().min(1, 'Requested amount must be greater than 0'),
  projectTitle: z.string().min(1, 'Project title is required'),
  projectDescription: z.string().min(50, 'Project description must be at least 50 characters'),
  expectedImpact: z.string().min(50, 'Expected impact must be at least 50 characters')
});

type ApplicationForm = z.infer<typeof applicationSchema>;

export const Applications: React.FC = () => {
  const navigate = useNavigate();
  const { programId } = useParams<{ programId?: string }>();
  const [applications, setApplications] = useState<GrantApplication[]>([]);
  const [programs, setPrograms] = useState<GrantProgram[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [showApplicationForm, setShowApplicationForm] = useState(!!programId);
  const [documents, setDocuments] = useState<File[]>([]);

  const { register, handleSubmit, formState: { errors }, watch, setValue } = useForm<ApplicationForm>({
    resolver: zodResolver(applicationSchema),
    defaultValues: {
      programId: programId || '',
      applicantType: 'NGO'
    }
  });

  const selectedProgramId = watch('programId');
  const selectedProgram = programs.find(p => p.id === selectedProgramId);

  useEffect(() => {
    fetchApplications();
    fetchPrograms();
  }, []);

  const fetchApplications = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/public-sector/grants/applications', {
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

  const fetchPrograms = async () => {
    try {
      const response = await fetch('/api/public-sector/grants/programs?status=OPEN', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to fetch programs');
      
      const data = await response.json();
      setPrograms(data.data || []);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    }
  };

  const handleFileUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(event.target.files || []);
    setDocuments(prev => [...prev, ...files]);
  };

  const removeDocument = (index: number) => {
    setDocuments(prev => prev.filter((_, i) => i !== index));
  };

  const onSubmit = async (formData: ApplicationForm) => {
    try {
      setSubmitting(true);
      
      const data = new FormData();
      Object.entries(formData).forEach(([key, value]) => {
        data.append(key, value.toString());
      });
      
      documents.forEach((file, index) => {
        data.append(`document_${index}`, file);
      });

      const response = await fetch('/api/public-sector/grants/applications', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: data
      });

      if (!response.ok) throw new Error('Failed to submit application');
      
      setSuccess('Application submitted successfully');
      setShowApplicationForm(false);
      setDocuments([]);
      fetchApplications();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setSubmitting(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'SUBMITTED': return 'bg-blue-100 text-blue-800';
      case 'UNDER_REVIEW': return 'bg-yellow-100 text-yellow-800';
      case 'APPROVED': return 'bg-green-100 text-green-800';
      case 'REJECTED': return 'bg-red-100 text-red-800';
      case 'DISBURSED': return 'bg-purple-100 text-purple-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Grant Applications</h1>
        {!showApplicationForm && (
          <button
            onClick={() => setShowApplicationForm(true)}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          >
            New Application
          </button>
        )}
      </div>

      {error && <ErrorAlert message={error} onClose={() => setError(null)} />}
      {success && <SuccessAlert message={success} />}

      {/* Application Form */}
      {showApplicationForm && (
        <div className="bg-white p-6 rounded-lg shadow">
          <div className="flex justify-between items-center mb-6">
            <h2 className="text-lg font-semibold text-gray-900">New Grant Application</h2>
            <button
              onClick={() => setShowApplicationForm(false)}
              className="text-gray-500 hover:text-gray-700"
            >
              ✕
            </button>
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Grant Program *
              </label>
              <select
                {...register('programId')}
                className="w-full p-2 border rounded-lg"
              >
                <option value="">Select a program...</option>
                {programs.map(program => (
                  <option key={program.id} value={program.id}>
                    {program.name} (Max: KES {program.maxAmount.toLocaleString()})
                  </option>
                ))}
              </select>
              {errors.programId && (
                <p className="text-red-500 text-sm mt-1">{errors.programId.message}</p>
              )}
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Applicant Name *
                </label>
                <input
                  {...register('applicantName')}
                  type="text"
                  className="w-full p-2 border rounded-lg"
                  placeholder="Enter applicant name"
                />
                {errors.applicantName && (
                  <p className="text-red-500 text-sm mt-1">{errors.applicantName.message}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Applicant Type *
                </label>
                <select
                  {...register('applicantType')}
                  className="w-full p-2 border rounded-lg"
                >
                  <option value="NGO">NGO</option>
                  <option value="COMMUNITY_GROUP">Community Group</option>
                  <option value="INSTITUTION">Institution</option>
                  <option value="INDIVIDUAL">Individual</option>
                </select>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Requested Amount *
              </label>
              <input
                {...register('requestedAmount', { valueAsNumber: true })}
                type="number"
                className="w-full p-2 border rounded-lg"
                placeholder="Enter requested amount"
              />
              {selectedProgram && (
                <p className="text-sm text-gray-500 mt-1">
                  Maximum amount for this program: KES {selectedProgram.maxAmount.toLocaleString()}
                </p>
              )}
              {errors.requestedAmount && (
                <p className="text-red-500 text-sm mt-1">{errors.requestedAmount.message}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Project Title *
              </label>
              <input
                {...register('projectTitle')}
                type="text"
                className="w-full p-2 border rounded-lg"
                placeholder="Enter project title"
              />
              {errors.projectTitle && (
                <p className="text-red-500 text-sm mt-1">{errors.projectTitle.message}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Project Description * (minimum 50 characters)
              </label>
              <textarea
                {...register('projectDescription')}
                className="w-full p-2 border rounded-lg"
                rows={5}
                placeholder="Describe your project in detail..."
              />
              {errors.projectDescription && (
                <p className="text-red-500 text-sm mt-1">{errors.projectDescription.message}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Expected Impact * (minimum 50 characters)
              </label>
              <textarea
                {...register('expectedImpact')}
                className="w-full p-2 border rounded-lg"
                rows={5}
                placeholder="Describe the expected impact of your project..."
              />
              {errors.expectedImpact && (
                <p className="text-red-500 text-sm mt-1">{errors.expectedImpact.message}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Supporting Documents
              </label>
              <input
                type="file"
                multiple
                onChange={handleFileUpload}
                className="w-full p-2 border rounded-lg"
              />
              {documents.length > 0 && (
                <div className="mt-2 space-y-1">
                  {documents.map((file, idx) => (
                    <div key={idx} className="flex justify-between items-center p-2 bg-gray-50 rounded">
                      <span className="text-sm text-gray-700">{file.name}</span>
                      <button
                        type="button"
                        onClick={() => removeDocument(idx)}
                        className="text-red-600 hover:text-red-800 text-sm"
                      >
                        Remove
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>

            <div className="flex gap-3">
              <button
                type="submit"
                disabled={submitting}
                className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-400"
              >
                {submitting ? 'Submitting...' : 'Submit Application'}
              </button>
              <button
                type="button"
                onClick={() => setShowApplicationForm(false)}
                className="px-6 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      {/* Applications List */}
      <div className="grid gap-4">
        {applications.map(application => (
          <div key={application.id} className="bg-white p-6 rounded-lg shadow">
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
                
                <h4 className="font-medium text-gray-900 mb-2">{application.projectTitle}</h4>
                
                <div className="grid grid-cols-3 gap-4 mt-4">
                  <div>
                    <p className="text-sm text-gray-500">Applicant</p>
                    <p className="font-medium text-gray-900">{application.applicantName}</p>
                    <p className="text-sm text-gray-600">{application.applicantType.replace('_', ' ')}</p>
                  </div>
                  
                  <div>
                    <p className="text-sm text-gray-500">Requested Amount</p>
                    <p className="font-medium text-gray-900">
                      KES {application.requestedAmount.toLocaleString()}
                    </p>
                  </div>
                  
                  <div>
                    <p className="text-sm text-gray-500">Submitted</p>
                    <p className="font-medium text-gray-900">
                      {new Date(application.submittedDate).toLocaleDateString()}
                    </p>
                  </div>
                </div>

                {application.approvals && application.approvals.length > 0 && (
                  <div className="mt-4 p-3 bg-gray-50 rounded">
                    <p className="text-sm font-medium text-gray-700 mb-2">Approvals</p>
                    {application.approvals.map((approval, idx) => (
                      <div key={idx} className="text-sm text-gray-600">
                        {approval.approverName} ({approval.approverRole}): {approval.decision}
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>

      {applications.length === 0 && !showApplicationForm && (
        <div className="bg-white rounded-lg shadow p-12 text-center">
          <p className="text-gray-500">No applications found</p>
        </div>
      )}
    </div>
  );
};
