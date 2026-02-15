import { useQuery } from '@tanstack/react-query'
import { customerService } from '../services/customerService'
import { Wallet, TrendingUp, CreditCard, Banknote, ArrowUpRight, ArrowDownRight } from 'lucide-react'

export default function DashboardPage() {
  const { data: accounts, isLoading } = useQuery({
    queryKey: ['accounts'],
    queryFn: customerService.getAccounts,
  })

  const { data: cards } = useQuery({
    queryKey: ['cards'],
    queryFn: customerService.getCards,
  })

  const { data: loans } = useQuery({
    queryKey: ['loans'],
    queryFn: customerService.getLoans,
  })

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-wekeza-primary"></div>
      </div>
    )
  }

  const totalBalance = accounts?.reduce((sum: number, acc: any) => sum + (acc.balance || 0), 0) || 0
  const activeCards = cards?.filter((c: any) => c.status === 'Active').length || 0
  const activeLoans = loans?.filter((l: any) => l.status === 'Active').length || 0

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600 mt-1">Welcome back! Here's your financial overview.</p>
      </div>

      {/* Quick Stats */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Total Balance</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">
                KES {totalBalance.toLocaleString()}
              </p>
            </div>
            <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
              <Wallet className="w-6 h-6 text-green-600" />
            </div>
          </div>
          <div className="flex items-center mt-4 text-sm text-green-600">
            <TrendingUp className="w-4 h-4 mr-1" />
            <span>+2.5% from last month</span>
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Active Accounts</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">{accounts?.length || 0}</p>
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
              <Wallet className="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Active Cards</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">{activeCards}</p>
            </div>
            <div className="w-12 h-12 bg-purple-100 rounded-full flex items-center justify-center">
              <CreditCard className="w-6 h-6 text-purple-600" />
            </div>
          </div>
        </div>

        <div className="card">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Active Loans</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">{activeLoans}</p>
            </div>
            <div className="w-12 h-12 bg-orange-100 rounded-full flex items-center justify-center">
              <Banknote className="w-6 h-6 text-orange-600" />
            </div>
          </div>
        </div>
      </div>

      {/* Accounts Overview */}
      <div className="card">
        <h2 className="text-xl font-semibold text-gray-900 mb-4">Your Accounts</h2>
        <div className="space-y-4">
          {accounts?.map((account: any) => (
            <div
              key={account.id}
              className="flex items-center justify-between p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
            >
              <div className="flex items-center space-x-4">
                <div className="w-10 h-10 bg-wekeza-primary rounded-full flex items-center justify-center">
                  <Wallet className="w-5 h-5 text-white" />
                </div>
                <div>
                  <p className="font-medium text-gray-900">{account.accountName || 'Savings Account'}</p>
                  <p className="text-sm text-gray-600">{account.accountNumber}</p>
                </div>
              </div>
              <div className="text-right">
                <p className="font-semibold text-gray-900">
                  KES {(account.balance || 0).toLocaleString()}
                </p>
                <p className="text-sm text-gray-600">{account.accountType}</p>
              </div>
            </div>
          ))}
          {(!accounts || accounts.length === 0) && (
            <div className="text-center py-8 text-gray-500">
              <p>No accounts found</p>
              <button className="mt-4 btn-primary">Open New Account</button>
            </div>
          )}
        </div>
      </div>

      {/* Recent Transactions */}
      <div className="card">
        <h2 className="text-xl font-semibold text-gray-900 mb-4">Recent Transactions</h2>
        <div className="space-y-3">
          {[
            { type: 'credit', desc: 'Salary Payment', amount: 150000, date: '2024-02-10' },
            { type: 'debit', desc: 'Rent Payment', amount: -45000, date: '2024-02-09' },
            { type: 'debit', desc: 'Grocery Shopping', amount: -8500, date: '2024-02-08' },
          ].map((txn, idx) => (
            <div key={idx} className="flex items-center justify-between p-3 hover:bg-gray-50 rounded-lg">
              <div className="flex items-center space-x-3">
                <div
                  className={`w-10 h-10 rounded-full flex items-center justify-center ${
                    txn.type === 'credit' ? 'bg-green-100' : 'bg-red-100'
                  }`}
                >
                  {txn.type === 'credit' ? (
                    <ArrowDownRight className="w-5 h-5 text-green-600" />
                  ) : (
                    <ArrowUpRight className="w-5 h-5 text-red-600" />
                  )}
                </div>
                <div>
                  <p className="font-medium text-gray-900">{txn.desc}</p>
                  <p className="text-sm text-gray-600">{txn.date}</p>
                </div>
              </div>
              <p
                className={`font-semibold ${
                  txn.type === 'credit' ? 'text-green-600' : 'text-red-600'
                }`}
              >
                {txn.type === 'credit' ? '+' : ''}KES {Math.abs(txn.amount).toLocaleString()}
              </p>
            </div>
          ))}
        </div>
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <button className="card hover:shadow-md transition-shadow text-center">
          <ArrowUpRight className="w-8 h-8 text-wekeza-primary mx-auto mb-2" />
          <p className="font-medium text-gray-900">Transfer</p>
        </button>
        <button className="card hover:shadow-md transition-shadow text-center">
          <Receipt className="w-8 h-8 text-wekeza-primary mx-auto mb-2" />
          <p className="font-medium text-gray-900">Pay Bill</p>
        </button>
        <button className="card hover:shadow-md transition-shadow text-center">
          <CreditCard className="w-8 h-8 text-wekeza-primary mx-auto mb-2" />
          <p className="font-medium text-gray-900">Cards</p>
        </button>
        <button className="card hover:shadow-md transition-shadow text-center">
          <Banknote className="w-8 h-8 text-wekeza-primary mx-auto mb-2" />
          <p className="font-medium text-gray-900">Loans</p>
        </button>
      </div>
    </div>
  )
}
