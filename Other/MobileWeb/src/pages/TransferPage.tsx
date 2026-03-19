import { useState } from 'react'
import { useMutation, useQuery } from 'react-query'
import { useNavigate } from 'react-router-dom'
import { accountsApi, transactionsApi, type Account, getErrorMessage } from '../services/api'
import AppLayout from '../components/AppLayout'

export default function TransferPage() {
  const navigate = useNavigate()

  const { data: accounts = [] } = useQuery<Account[]>('accounts', accountsApi.getUserAccounts)

  const [fromAccount, setFromAccount] = useState('')
  const [toAccount, setToAccount] = useState('')
  const [amount, setAmount] = useState('')
  const [narration, setNarration] = useState('')
  const [successMsg, setSuccessMsg] = useState('')
  const [errorMsg, setErrorMsg] = useState('')

  const { mutate: transfer, isLoading } = useMutation(
    () =>
      transactionsApi.transfer({
        fromAccountNumber: fromAccount,
        toAccountNumber: toAccount,
        amount: parseFloat(amount),
        narration: narration || 'Wekeza Transfer',
      }),
    {
      onSuccess: (data) => {
        setSuccessMsg(`Transfer successful! Reference: ${data.transactionId}`)
        setToAccount('')
        setAmount('')
        setNarration('')
      },
      onError: (err) => {
        setErrorMsg(getErrorMessage(err))
      },
    },
  )

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    setSuccessMsg('')
    setErrorMsg('')
    if (!fromAccount || !toAccount || !amount || parseFloat(amount) <= 0) return
    transfer()
  }

  return (
    <AppLayout title="Transfer Funds" showBack onBack={() => navigate(-1)}>
      <div className="p-5">
        {successMsg && (
          <div className="mb-4 p-4 bg-green-50 border border-green-200 rounded-xl text-green-700 text-sm">
            ✅ {successMsg}
          </div>
        )}
        {errorMsg && (
          <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm">
            {errorMsg}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1.5">From Account</label>
            <select
              value={fromAccount}
              onChange={(e) => setFromAccount(e.target.value)}
              className="input-field"
              required
            >
              <option value="">Select account</option>
              {accounts.map((a) => (
                <option key={a.accountNumber} value={a.accountNumber}>
                  {a.accountType} – {a.accountNumber} (KES {a.availableBalance.toLocaleString('en-KE', { minimumFractionDigits: 2 })})
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1.5">To Account Number</label>
            <input
              type="text"
              value={toAccount}
              onChange={(e) => setToAccount(e.target.value)}
              placeholder="Enter recipient account number"
              className="input-field"
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1.5">Amount (KES)</label>
            <input
              type="number"
              value={amount}
              onChange={(e) => setAmount(e.target.value)}
              placeholder="0.00"
              min="1"
              max="1000000"
              step="0.01"
              className="input-field"
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1.5">
              Narration <span className="text-gray-400 font-normal">(optional)</span>
            </label>
            <input
              type="text"
              value={narration}
              onChange={(e) => setNarration(e.target.value)}
              placeholder="What is this transfer for?"
              maxLength={100}
              className="input-field"
            />
          </div>

          <div className="pt-2">
            <button
              type="submit"
              disabled={isLoading || !fromAccount || !toAccount || !amount}
              className="btn-primary"
            >
              {isLoading ? (
                <span className="flex items-center gap-2">
                  <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                  Processing...
                </span>
              ) : (
                'Transfer Now'
              )}
            </button>
          </div>
        </form>

        <p className="text-center text-xs text-gray-400 mt-6">
          Maximum transfer: KES 1,000,000 per transaction
        </p>
      </div>
    </AppLayout>
  )
}
