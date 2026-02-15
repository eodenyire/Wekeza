import React, { useState, useEffect } from 'react';
import { LoanApplication } from '../../types';
import { LoadingSpinner, ErrorAlert, SuccessAlert, ConfirmDialog } from '../../components';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';

const disbursementSchema = z.object({
  accountNumber: z.string().min(1, 'Account number is required'),
  accountName: z.string().min(1, 'Account name is required'),
  bankName: z.string().min(1, 'Bank name is required'),
  branchCode: z.string().min(1, 'Branch code is required'),
  narration: z.string().min(1, 'Narration is required')
});

type DisbursementForm = z.infer<typeof disbursementSchema>;

export const Disbursements: React.FC = () => {
  const [applications, setApplications] = useState<LoanApplication[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [selectedLoan, setSelectedLoan] = useState<LoanApplication | null>(null);
  const [showDisbursementDialog, setShowDisbursementDialog] = useState(false);
  const [disbursing, setDisbursing] = useState(false);

  const { register, handleSubmit, formState: { errors }, reset } = useForm<DisbursementForm>({
    resolver: zodResolver(disbursementSchema)
  });

  useEffect(() => {
    fetchApprovedLoans();
  }, []);

  const fetchApprovedLoans = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/public-sector/loans/applications?status=APPROVED', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Failed to fetch approved loans');
      
      const data = await response.json();
      setApplications(data.data || []);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const handleDisburseLoan = (loan: LoanApplication) => {
    setSelectedLoan(loan);
    setShowDisbursementDialog(true);
    reset();
  };

  const onSubmitDisbursement = async (formData: DisbursementForm) => {
    if (!selectedLoan) return;

    try {
      setDisbursing(true);
      const response = await fetch(`/api/public-sector/loans/${selectedLoan.id}/disburse`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(formData)
      });

      if (!response.ok) throw new Error('Failed to disburse loan');
      
      setSuccess(`Loan ${selectedLoan.applicationNumber} disbursed successfully`);
      setShowDisbursementDialog(false);
      setSelectedLoan(null);
      fetchApprovedLoans();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setDisbursing(false);
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Loan Disbursements</h1>
      </div>

      {error && <ErrorAlert message={error} onClose={() => setError(null)} />}
      {success && <SuccessAlert message={success} />}

      {applications.length === 0 ? (
        <div className="bg-white rounded-lg shadow p-12 text-center">
          <p className="text-gray-500">No approved loans pending disbursement</p>
        </div>
      ) : (
        <div className="grid gap-4">
          {applications.map(application => (
            <div key={application.id} className="bg-white p-6 rounded-lg shadow">
              <div className="flex justify-between items-start">
                <div className="flex-1">
                  <h3 className="text-lg font-semibold text-gray-900 mb-2">
                    {application.applicationNumber}
                  </h3>
                  
                  <div className="grid grid-cols-3 gap-4 mt-4">
                    <div>
                      <p className="text-sm text-gray-500">Government Entity</p>
                      <p className="font-medium text-gray-900">{application.governmentEntity.name}</p>
                    </div>
                    
                    <div>
                      <p className="text-sm text-gray-500">Loan Amount</p>
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
                    <p className="text-gray-700">{application.purpose}</p>
                  </div>
                </div>

                <button
                  onClick={() => handleDisburseLoan(application)}
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
                >
                  Disburse
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Disbursement Dialog */}
      {selectedLoan && (
        <ConfirmDialog
          isOpen={showDisbursementDialog}
          title="Disburse Loan"
          message={`Disburse loan ${selectedLoan.applicationNumber} to ${selectedLoan.governmentEntity.name}`}
          onConfirm={handleSubmit(onSubmitDisbursement)}
          onCancel={() => {
            setShowDisbursementDialog(false);
            setSelectedLoan(null);
          }}
          confirmText="Disburse"
          confirmColor="blue"
          loading={disbursing}
        >
          <form className="space-y-4 mt-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Account Number *
              </label>
              <input
                {...register('accountNumber')}
                type="text"
                className="w-full p-2 border rounded-lg"
                placeholder="Enter account number"
              />
              {errors.accountNumber && (
                <p className="text-red-500 text-sm mt-1">{errors.accountNumber.message}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Account Name *
              </label>
              <input
                {...register('accountName')}
                type="text"
                className="w-full p-2 border rounded-lg"
                placeholder="Enter account name"
              />
              {errors.accountName && (
                <p className="text-red-500 text-sm mt-1">{errors.accountName.message}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Bank Name *
              </label>
              <input
                {...register('bankName')}
                type="text"
                className="w-full p-2 border rounded-lg"
                placeholder="Enter bank name"
              />
              {errors.bankName && (
                <p className="text-red-500 text-sm mt-1">{errors.bankName.message}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Branch Code *
              </label>
              <input
                {...register('branchCode')}
                type="text"
                className="w-full p-2 border rounded-lg"
                placeholder="Enter branch code"
              />
              {errors.branchCode && (
                <p className="text-red-500 text-sm mt-1">{errors.branchCode.message}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Narration *
              </label>
              <textarea
                {...register('narration')}
                className="w-full p-2 border rounded-lg"
                rows={3}
                placeholder="Enter disbursement narration"
              />
              {errors.narration && (
                <p className="text-red-500 text-sm mt-1">{errors.narration.message}</p>
              )}
            </div>

            <div className="p-4 bg-blue-50 rounded-lg">
              <p className="text-sm font-medium text-blue-900">Disbursement Amount</p>
              <p className="text-2xl font-bold text-blue-600">
                KES {selectedLoan.requestedAmount.toLocaleString()}
              </p>
            </div>
          </form>
        </ConfirmDialog>
      )}
    </div>
  );
};
