import { useQuery } from 'react-query'
import { useNavigate } from 'react-router-dom'
import { accountsApi, type Account } from '../services/api'
import AppLayout from '../components/AppLayout'
import clsx from 'clsx'

export default function AccountsPage() {
  const navigate = useNavigate()
  const { data: accounts = [], isLoading, error } = useQuery<Account[]>(
    'accounts',
    accountsApi.getUserAccounts,
  )

  return (
    <AppLayout title="My Accounts" showBack onBack={() => navigate(-1)}>
      <div className="p-5 space-y-4">
        {isLoading && Array.from({ length: 3 }).map((_, i) => (
          <div key={i} className="skeleton h-28 rounded-2xl" />
        ))}

        {error && (
          <div className="p-4 bg-red-50 rounded-xl text-red-700 text-sm">
            Failed to load accounts. Please try again.
          </div>
        )}

        {!isLoading && accounts.map((account) => (
          <AccountDetailCard key={account.accountNumber} account={account} />
        ))}

        {!isLoading && accounts.length === 0 && (
          <div className="text-center py-16 text-gray-400">
            <p className="text-4xl mb-3">💳</p>
            <p>No accounts found</p>
          </div>
        )}
      </div>
    </AppLayout>
  )
}

function AccountDetailCard({ account }: { account: Account }) {
  return (
    <div className="card">
      <div className="flex items-start justify-between mb-3">
        <div>
          <h3 className="font-semibold text-gray-900">{account.accountType}</h3>
          <p className="text-gray-400 text-xs mt-0.5">{account.accountNumber}</p>
        </div>
        <span className={clsx(
          'text-xs font-medium px-2.5 py-1 rounded-full',
          account.status === 'Active'
            ? 'bg-green-100 text-green-700'
            : 'bg-gray-100 text-gray-500',
        )}>
          {account.status}
        </span>
      </div>

      <div className="grid grid-cols-2 gap-4 pt-3 border-t border-gray-100">
        <div>
          <p className="text-xs text-gray-400 mb-0.5">Balance</p>
          <p className="font-bold text-gray-900">
            {account.currency} {account.balance.toLocaleString('en-KE', { minimumFractionDigits: 2 })}
          </p>
        </div>
        <div>
          <p className="text-xs text-gray-400 mb-0.5">Available</p>
          <p className="font-bold text-primary-700">
            {account.currency} {account.availableBalance.toLocaleString('en-KE', { minimumFractionDigits: 2 })}
          </p>
        </div>
      </div>
    </div>
  )
}
