import { useState } from 'react'
import { useMutation, useQuery } from 'react-query'
import { useNavigate } from 'react-router-dom'
import { accountsApi, mobileMoneyApi, type Account, getErrorMessage } from '../services/api'
import AppLayout from '../components/AppLayout'
import clsx from 'clsx'

type Tab = 'mpesa' | 'airtime' | 'send'

export default function MobileMoneyPage() {
  const navigate = useNavigate()
  const [activeTab, setActiveTab] = useState<Tab>('mpesa')

  const { data: accounts = [] } = useQuery<Account[]>('accounts', accountsApi.getUserAccounts)

  return (
    <AppLayout title="Mobile Money" showBack onBack={() => navigate(-1)}>
      {/* Tab bar */}
      <div className="flex border-b border-gray-200 bg-white px-5 sticky top-0 z-10">
        {([
          { key: 'mpesa', label: 'M-Pesa Deposit' },
          { key: 'send', label: 'Send Money' },
          { key: 'airtime', label: 'Airtime' },
        ] as { key: Tab; label: string }[]).map((tab) => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            className={clsx(
              'flex-1 py-3.5 text-sm font-medium border-b-2 transition-colors',
              activeTab === tab.key
                ? 'border-primary-700 text-primary-700'
                : 'border-transparent text-gray-400',
            )}
          >
            {tab.label}
          </button>
        ))}
      </div>

      <div className="p-5">
        {activeTab === 'mpesa' && <MpesaDepositForm accounts={accounts} />}
        {activeTab === 'send' && <SendMoneyForm accounts={accounts} />}
        {activeTab === 'airtime' && <AirtimeForm accounts={accounts} />}
      </div>
    </AppLayout>
  )
}

function MpesaDepositForm({ accounts }: { accounts: Account[] }) {
  const [accountNumber, setAccountNumber] = useState('')
  const [phoneNumber, setPhoneNumber] = useState('')
  const [amount, setAmount] = useState('')
  const [successMsg, setSuccessMsg] = useState('')
  const [errorMsg, setErrorMsg] = useState('')

  const { mutate, isLoading } = useMutation(
    () => mobileMoneyApi.initiateMpesaDeposit({
      phoneNumber,
      amount: parseFloat(amount),
      accountNumber,
    }),
    {
      onSuccess: () => {
        setSuccessMsg('M-Pesa STK push sent! Please check your phone to complete the payment.')
        setAmount('')
      },
      onError: (err) => setErrorMsg(getErrorMessage(err)),
    },
  )

  return (
    <form onSubmit={(e) => { e.preventDefault(); setSuccessMsg(''); setErrorMsg(''); mutate() }} className="space-y-4">
      {successMsg && <div className="p-4 bg-green-50 rounded-xl text-green-700 text-sm">✅ {successMsg}</div>}
      {errorMsg && <div className="p-4 bg-red-50 rounded-xl text-red-700 text-sm">{errorMsg}</div>}

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1.5">Deposit to Account</label>
        <select value={accountNumber} onChange={(e) => setAccountNumber(e.target.value)} className="input-field" required>
          <option value="">Select account</option>
          {accounts.map((a) => <option key={a.accountNumber} value={a.accountNumber}>{a.accountType} – {a.accountNumber}</option>)}
        </select>
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1.5">M-Pesa Phone Number</label>
        <input type="tel" value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)}
          placeholder="e.g. 0712 345 678" className="input-field" required />
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1.5">Amount (KES)</label>
        <input type="number" value={amount} onChange={(e) => setAmount(e.target.value)}
          placeholder="0.00" min="1" className="input-field" required />
      </div>
      <button type="submit" disabled={isLoading} className="btn-primary">
        {isLoading ? <span className="flex items-center gap-2"><span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />Processing...</span> : 'Send STK Push'}
      </button>
    </form>
  )
}

function SendMoneyForm({ accounts }: { accounts: Account[] }) {
  const [fromAccount, setFromAccount] = useState('')
  const [toPhone, setToPhone] = useState('')
  const [amount, setAmount] = useState('')
  const [provider, setProvider] = useState('Mpesa')
  const [successMsg, setSuccessMsg] = useState('')
  const [errorMsg, setErrorMsg] = useState('')

  const { mutate, isLoading } = useMutation(
    () => mobileMoneyApi.sendToMobile({
      fromAccountNumber: fromAccount,
      toPhoneNumber: toPhone,
      amount: parseFloat(amount),
      provider,
    }),
    {
      onSuccess: () => {
        setSuccessMsg('Money sent successfully!')
        setAmount('')
        setToPhone('')
      },
      onError: (err) => setErrorMsg(getErrorMessage(err)),
    },
  )

  return (
    <form onSubmit={(e) => { e.preventDefault(); setSuccessMsg(''); setErrorMsg(''); mutate() }} className="space-y-4">
      {successMsg && <div className="p-4 bg-green-50 rounded-xl text-green-700 text-sm">✅ {successMsg}</div>}
      {errorMsg && <div className="p-4 bg-red-50 rounded-xl text-red-700 text-sm">{errorMsg}</div>}

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1.5">From Account</label>
        <select value={fromAccount} onChange={(e) => setFromAccount(e.target.value)} className="input-field" required>
          <option value="">Select account</option>
          {accounts.map((a) => <option key={a.accountNumber} value={a.accountNumber}>{a.accountType} – {a.accountNumber}</option>)}
        </select>
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1.5">Provider</label>
        <select value={provider} onChange={(e) => setProvider(e.target.value)} className="input-field">
          <option value="Mpesa">M-Pesa</option>
          <option value="Airtel">Airtel Money</option>
          <option value="TKash">T-Kash</option>
        </select>
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1.5">Recipient Phone</label>
        <input type="tel" value={toPhone} onChange={(e) => setToPhone(e.target.value)} placeholder="e.g. 0712 345 678" className="input-field" required />
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1.5">Amount (KES)</label>
        <input type="number" value={amount} onChange={(e) => setAmount(e.target.value)} placeholder="0.00" min="1" className="input-field" required />
      </div>
      <button type="submit" disabled={isLoading} className="btn-primary">
        {isLoading ? <span className="flex items-center gap-2"><span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />Sending...</span> : 'Send Money'}
      </button>
    </form>
  )
}

function AirtimeForm({ accounts }: { accounts: Account[] }) {
  const [fromAccount, setFromAccount] = useState('')
  const [phone, setPhone] = useState('')
  const [amount, setAmount] = useState('')
  const [provider, setProvider] = useState('Safaricom')
  const [successMsg, setSuccessMsg] = useState('')
  const [errorMsg, setErrorMsg] = useState('')

  const { mutate, isLoading } = useMutation(
    () => mobileMoneyApi.purchaseAirtime({
      fromAccountNumber: fromAccount,
      phoneNumber: phone,
      amount: parseFloat(amount),
      provider,
    }),
    {
      onSuccess: () => {
        setSuccessMsg('Airtime purchased successfully!')
        setAmount('')
      },
      onError: (err) => setErrorMsg(getErrorMessage(err)),
    },
  )

  const quickAmounts = [10, 20, 50, 100, 200, 500]

  return (
    <form onSubmit={(e) => { e.preventDefault(); setSuccessMsg(''); setErrorMsg(''); mutate() }} className="space-y-4">
      {successMsg && <div className="p-4 bg-green-50 rounded-xl text-green-700 text-sm">✅ {successMsg}</div>}
      {errorMsg && <div className="p-4 bg-red-50 rounded-xl text-red-700 text-sm">{errorMsg}</div>}

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1.5">From Account</label>
        <select value={fromAccount} onChange={(e) => setFromAccount(e.target.value)} className="input-field" required>
          <option value="">Select account</option>
          {accounts.map((a) => <option key={a.accountNumber} value={a.accountNumber}>{a.accountType} – {a.accountNumber}</option>)}
        </select>
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1.5">Network Provider</label>
        <select value={provider} onChange={(e) => setProvider(e.target.value)} className="input-field">
          <option value="Safaricom">Safaricom</option>
          <option value="Airtel">Airtel</option>
          <option value="Telkom">Telkom</option>
        </select>
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1.5">Phone Number</label>
        <input type="tel" value={phone} onChange={(e) => setPhone(e.target.value)} placeholder="e.g. 0712 345 678" className="input-field" required />
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1.5">Amount (KES)</label>
        <input type="number" value={amount} onChange={(e) => setAmount(e.target.value)} placeholder="0.00" min="1" className="input-field" required />
        <div className="flex flex-wrap gap-2 mt-2">
          {quickAmounts.map((a) => (
            <button key={a} type="button" onClick={() => setAmount(String(a))}
              className={clsx('px-3 py-1.5 rounded-lg text-sm font-medium border transition-colors',
                amount === String(a)
                  ? 'bg-primary-800 text-white border-primary-800'
                  : 'bg-white text-gray-600 border-gray-200'
              )}>
              {a}
            </button>
          ))}
        </div>
      </div>
      <button type="submit" disabled={isLoading} className="btn-primary">
        {isLoading ? <span className="flex items-center gap-2"><span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />Processing...</span> : 'Buy Airtime'}
      </button>
    </form>
  )
}
