import { useQuery } from 'react-query'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { accountsApi, type Account } from '../services/api'
import AppLayout from '../components/AppLayout'
import clsx from 'clsx'

export default function DashboardPage() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login', { replace: true })
  }

  const { data: accounts = [], isLoading } = useQuery<Account[]>(
    'accounts',
    accountsApi.getUserAccounts,
    { staleTime: 60_000 },
  )

  const totalBalance = accounts.reduce((sum, a) => sum + a.balance, 0)
  const primaryAccount = accounts[0]

  const quickActions = [
    { icon: '↗️', label: 'Transfer', path: '/transfer' },
    { icon: '📱', label: 'M-Money', path: '/mobile-money' },
    { icon: '📄', label: 'History', path: '/transactions' },
    { icon: '🏦', label: 'Loans', path: '/loans' },
  ]

  return (
    <div className="flex flex-col h-screen bg-gray-50 max-w-md mx-auto">
      {/* Header */}
      <div className="bg-primary-800 pt-safe-top">
        <div className="px-5 pt-4 pb-6">
          <div className="flex items-center justify-between mb-6">
            <div>
              <p className="text-primary-200 text-sm">Good {getGreeting()},</p>
              <h1 className="text-white text-xl font-bold">
                {user?.firstName ?? user?.username ?? 'Customer'}
              </h1>
            </div>
            <button
              onClick={handleLogout}
              className="w-9 h-9 rounded-full bg-white/10 flex items-center justify-center"
            >
              <svg className="w-5 h-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                  d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
              </svg>
            </button>
          </div>

          {/* Balance card */}
          <div className="bg-white/10 backdrop-blur rounded-2xl p-5">
            <p className="text-primary-200 text-sm mb-1">Total Balance</p>
            {isLoading ? (
              <div className="skeleton h-8 w-40 rounded" />
            ) : (
              <p className="text-white text-3xl font-bold">
                KES {totalBalance.toLocaleString('en-KE', { minimumFractionDigits: 2 })}
              </p>
            )}
            {primaryAccount && (
              <p className="text-primary-200 text-xs mt-2">{primaryAccount.accountNumber}</p>
            )}
          </div>
        </div>
      </div>

      {/* Scrollable content */}
      <main className="flex-1 overflow-y-auto scroll-smooth-ios pb-20">
        {/* Quick actions */}
        <div className="px-5 py-5">
          <h2 className="text-gray-700 font-semibold mb-4">Quick Actions</h2>
          <div className="grid grid-cols-4 gap-3">
            {quickActions.map((action) => (
              <button
                key={action.path}
                onClick={() => navigate(action.path)}
                className="flex flex-col items-center gap-2 py-3 bg-white rounded-2xl shadow-sm border border-gray-100 active:bg-gray-50"
              >
                <span className="text-2xl">{action.icon}</span>
                <span className="text-xs text-gray-600 font-medium">{action.label}</span>
              </button>
            ))}
          </div>
        </div>

        {/* Accounts */}
        <div className="px-5 mb-5">
          <div className="flex items-center justify-between mb-3">
            <h2 className="text-gray-700 font-semibold">My Accounts</h2>
            <button
              onClick={() => navigate('/accounts')}
              className="text-primary-700 text-sm font-medium"
            >
              See all
            </button>
          </div>
          <div className="space-y-3">
            {isLoading
              ? Array.from({ length: 2 }).map((_, i) => (
                  <div key={i} className="skeleton h-20 rounded-2xl" />
                ))
              : accounts.slice(0, 3).map((account) => (
                  <AccountCard key={account.accountNumber} account={account} />
                ))}
            {!isLoading && accounts.length === 0 && (
              <div className="text-center py-8 text-gray-400">
                <p>No accounts found</p>
              </div>
            )}
          </div>
        </div>
      </main>

      {/* Bottom nav */}
      <BottomNavInline />
    </div>
  )
}

function AccountCard({ account }: { account: Account }) {
  return (
    <div className="card flex items-center gap-4">
      <div className={clsx(
        'w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0',
        account.accountType.toLowerCase().includes('saving')
          ? 'bg-green-100'
          : 'bg-blue-100',
      )}>
        <svg className={clsx(
          'w-6 h-6',
          account.accountType.toLowerCase().includes('saving')
            ? 'text-green-600'
            : 'text-blue-600',
        )} fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
            d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
        </svg>
      </div>
      <div className="flex-1 min-w-0">
        <p className="font-medium text-gray-900 truncate">{account.accountType}</p>
        <p className="text-gray-400 text-xs">{account.accountNumber}</p>
      </div>
      <div className="text-right">
        <p className="font-semibold text-gray-900">
          {account.currency} {account.balance.toLocaleString('en-KE', { minimumFractionDigits: 2 })}
        </p>
        <span className={clsx(
          'text-xs font-medium',
          account.status === 'Active' ? 'text-green-600' : 'text-gray-400',
        )}>
          {account.status}
        </span>
      </div>
    </div>
  )
}

// Inline bottom nav for dashboard (avoids circular import issues)
function BottomNavInline() {
  const navigate = useNavigate()
  return (
    <nav className="flex-none bg-white border-t border-gray-200 pb-safe-bottom flex">
      {[
        { to: '/', label: 'Home', icon: '🏠' },
        { to: '/accounts', label: 'Accounts', icon: '💳' },
        { to: '/transfer', label: 'Transfer', icon: '↗️' },
        { to: '/mobile-money', label: 'M-Money', icon: '📱' },
        { to: '/profile', label: 'Profile', icon: '👤' },
      ].map((item) => (
        <button
          key={item.to}
          onClick={() => navigate(item.to)}
          className="flex flex-col items-center flex-1 pt-2 pb-1 text-xs font-medium text-gray-400 active:text-primary-800"
        >
          <span className="text-lg">{item.icon}</span>
          <span className="mt-0.5">{item.label}</span>
        </button>
      ))}
    </nav>
  )
}

function getGreeting(): string {
  const hour = new Date().getHours()
  if (hour < 12) return 'morning'
  if (hour < 17) return 'afternoon'
  return 'evening'
}
