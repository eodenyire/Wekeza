import { useQuery } from 'react-query'
import { useNavigate } from 'react-router-dom'
import { loansApi, type Loan } from '../services/api'
import AppLayout from '../components/AppLayout'
import clsx from 'clsx'

export default function LoansPage() {
  const navigate = useNavigate()
  const { data: loans = [], isLoading } = useQuery<Loan[]>('loans', loansApi.getUserLoans)

  return (
    <AppLayout title="My Loans" showBack onBack={() => navigate(-1)}>
      <div className="p-5 space-y-4">
        {isLoading && Array.from({ length: 2 }).map((_, i) => (
          <div key={i} className="skeleton h-36 rounded-2xl" />
        ))}

        {!isLoading && loans.length === 0 && (
          <div className="text-center py-16 text-gray-400">
            <p className="text-4xl mb-3">🏦</p>
            <p>No active loans</p>
          </div>
        )}

        {!isLoading && loans.map((loan) => (
          <LoanCard key={loan.loanId} loan={loan} />
        ))}
      </div>
    </AppLayout>
  )
}

function LoanCard({ loan }: { loan: Loan }) {
  const progressPct = loan.principal > 0
    ? Math.max(0, Math.min(100, ((loan.principal - loan.outstandingBalance) / loan.principal) * 100))
    : 0

  return (
    <div className="card">
      <div className="flex items-start justify-between mb-3">
        <div>
          <h3 className="font-semibold text-gray-900">{loan.loanType}</h3>
          <p className="text-xs text-gray-400 mt-0.5">Loan ID: {loan.loanId}</p>
        </div>
        <span className={clsx(
          'text-xs font-medium px-2.5 py-1 rounded-full',
          loan.status === 'Active' ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-500',
        )}>
          {loan.status}
        </span>
      </div>

      <div className="grid grid-cols-2 gap-3 mb-4">
        <div>
          <p className="text-xs text-gray-400">Principal</p>
          <p className="font-medium text-gray-900 text-sm">
            KES {loan.principal.toLocaleString('en-KE', { minimumFractionDigits: 2 })}
          </p>
        </div>
        <div>
          <p className="text-xs text-gray-400">Outstanding</p>
          <p className="font-medium text-red-600 text-sm">
            KES {loan.outstandingBalance.toLocaleString('en-KE', { minimumFractionDigits: 2 })}
          </p>
        </div>
        <div>
          <p className="text-xs text-gray-400">Interest Rate</p>
          <p className="font-medium text-gray-900 text-sm">{loan.interestRate}% p.a.</p>
        </div>
        {loan.nextPaymentDate && (
          <div>
            <p className="text-xs text-gray-400">Next Payment</p>
            <p className="font-medium text-gray-900 text-sm">
              {new Date(loan.nextPaymentDate).toLocaleDateString('en-KE', {
                day: 'numeric', month: 'short',
              })}
            </p>
          </div>
        )}
      </div>

      {/* Repayment progress */}
      <div>
        <div className="flex justify-between text-xs text-gray-400 mb-1">
          <span>Repaid</span>
          <span>{progressPct.toFixed(0)}%</span>
        </div>
        <div className="h-2 bg-gray-100 rounded-full overflow-hidden">
          <div
            className="h-full bg-primary-700 rounded-full transition-all"
            style={{ width: `${progressPct}%` }}
          />
        </div>
      </div>
    </div>
  )
}
