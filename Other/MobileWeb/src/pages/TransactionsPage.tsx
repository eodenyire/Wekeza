import { useState } from 'react'
import { useQuery } from 'react-query'
import { useNavigate } from 'react-router-dom'
import { accountsApi, transactionsApi, type Account, type Transaction } from '../services/api'
import AppLayout from '../components/AppLayout'
import clsx from 'clsx'

export default function TransactionsPage() {
  const navigate = useNavigate()
  const { data: accounts = [] } = useQuery<Account[]>('accounts', accountsApi.getUserAccounts)
  const [selectedAccount, setSelectedAccount] = useState('')

  const activeAccount = selectedAccount || accounts[0]?.accountNumber

  const { data, isLoading } = useQuery(
    ['transactions', activeAccount],
    () => transactionsApi.getStatement(activeAccount),
    { enabled: !!activeAccount },
  )

  const transactions = data?.transactions ?? []

  return (
    <AppLayout title="Transaction History" showBack onBack={() => navigate(-1)}>
      <div className="p-5">
        {/* Account selector */}
        <div className="mb-4">
          <select
            value={selectedAccount}
            onChange={(e) => setSelectedAccount(e.target.value)}
            className="input-field"
          >
            {accounts.map((a) => (
              <option key={a.accountNumber} value={a.accountNumber}>
                {a.accountType} – {a.accountNumber}
              </option>
            ))}
          </select>
        </div>

        {isLoading && Array.from({ length: 5 }).map((_, i) => (
          <div key={i} className="skeleton h-16 rounded-xl mb-3" />
        ))}

        {!isLoading && transactions.length === 0 && (
          <div className="text-center py-16 text-gray-400">
            <p className="text-4xl mb-3">📄</p>
            <p>No transactions found</p>
          </div>
        )}

        {!isLoading && transactions.length > 0 && (
          <div className="space-y-2">
            {transactions.map((tx) => (
              <TransactionRow key={tx.transactionId} transaction={tx} />
            ))}
          </div>
        )}
      </div>
    </AppLayout>
  )
}

function TransactionRow({ transaction: tx }: { transaction: Transaction }) {
  const isCredit = tx.type === 'Credit' || tx.type === 'Deposit'
  return (
    <div className="card flex items-center gap-4 py-3">
      <div className={clsx(
        'w-10 h-10 rounded-xl flex items-center justify-center flex-shrink-0',
        isCredit ? 'bg-green-100' : 'bg-red-100',
      )}>
        <svg className={clsx('w-5 h-5', isCredit ? 'text-green-600' : 'text-red-600')}
          fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
            d={isCredit ? 'M12 4v16m0-16l-4 4m4-4l4 4' : 'M12 4v16m0 0l-4-4m4 4l4-4'} />
        </svg>
      </div>
      <div className="flex-1 min-w-0">
        <p className="font-medium text-gray-900 text-sm truncate">{tx.description}</p>
        <p className="text-gray-400 text-xs">
          {new Date(tx.transactionDate).toLocaleDateString('en-KE', {
            day: 'numeric', month: 'short', year: 'numeric',
          })}
        </p>
      </div>
      <div className="text-right flex-shrink-0">
        <p className={clsx('font-semibold text-sm', isCredit ? 'text-green-600' : 'text-red-600')}>
          {isCredit ? '+' : '-'}KES {tx.amount.toLocaleString('en-KE', { minimumFractionDigits: 2 })}
        </p>
        {tx.balance !== undefined && (
          <p className="text-xs text-gray-400">
            Bal: {tx.balance.toLocaleString('en-KE', { minimumFractionDigits: 2 })}
          </p>
        )}
      </div>
    </div>
  )
}
