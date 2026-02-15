import { ArrowUpRight, ArrowDownRight, CreditCard, Send, Receipt, Smartphone } from 'lucide-react';

export default function Dashboard() {
  // Mock data - will be replaced with API calls
  const accounts = [
    { id: '1', name: 'Savings Account', number: '****1234', balance: 125000, currency: 'KES' },
    { id: '2', name: 'Current Account', number: '****5678', balance: 45000, currency: 'KES' },
  ];

  const recentTransactions = [
    { id: '1', description: 'Salary Deposit', amount: 50000, type: 'credit', date: '2026-02-10' },
    { id: '2', description: 'Rent Payment', amount: -25000, type: 'debit', date: '2026-02-09' },
    { id: '3', description: 'Grocery Shopping', amount: -5000, type: 'debit', date: '2026-02-08' },
    { id: '4', description: 'Freelance Payment', amount: 15000, type: 'credit', date: '2026-02-07' },
  ];

  const quickActions = [
    { name: 'Transfer', icon: Send, href: '/personal/transfer', color: 'bg-blue-500' },
    { name: 'Pay Bill', icon: Receipt, href: '/personal/payments', color: 'bg-green-500' },
    { name: 'Buy Airtime', icon: Smartphone, href: '/personal/payments', color: 'bg-purple-500' },
    { name: 'Cards', icon: CreditCard, href: '/personal/cards', color: 'bg-orange-500' },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">Dashboard</h1>
        <p className="text-gray-600">Welcome back! Here's your account overview</p>
      </div>

      {/* Account Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {accounts.map((account) => (
          <div key={account.id} className="card bg-gradient-to-br from-wekeza-blue to-blue-600 text-white">
            <div className="flex justify-between items-start mb-4">
              <div>
                <p className="text-blue-100 text-sm">{account.name}</p>
                <p className="text-lg font-semibold">{account.number}</p>
              </div>
              <CreditCard className="h-8 w-8 text-blue-200" />
            </div>
            <div className="mt-6">
              <p className="text-blue-100 text-sm">Available Balance</p>
              <p className="text-3xl font-bold">
                {account.currency} {account.balance.toLocaleString()}
              </p>
            </div>
          </div>
        ))}
      </div>

      {/* Quick Actions */}
      <div>
        <h2 className="text-xl font-bold mb-4">Quick Actions</h2>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          {quickActions.map((action) => {
            const Icon = action.icon;
            return (
              <a
                key={action.name}
                href={action.href}
                className="card hover:shadow-lg transition-shadow text-center"
              >
                <div className={`${action.color} w-12 h-12 rounded-full flex items-center justify-center mx-auto mb-3`}>
                  <Icon className="h-6 w-6 text-white" />
                </div>
                <p className="font-medium">{action.name}</p>
              </a>
            );
          })}
        </div>
      </div>

      {/* Recent Transactions */}
      <div className="card">
        <h2 className="text-xl font-bold mb-4">Recent Transactions</h2>
        <div className="space-y-3">
          {recentTransactions.map((txn) => (
            <div key={txn.id} className="flex justify-between items-center py-3 border-b last:border-0">
              <div className="flex items-center">
                <div className={`w-10 h-10 rounded-full flex items-center justify-center mr-3 ${
                  txn.type === 'credit' ? 'bg-green-100' : 'bg-red-100'
                }`}>
                  {txn.type === 'credit' ? (
                    <ArrowDownRight className="h-5 w-5 text-green-600" />
                  ) : (
                    <ArrowUpRight className="h-5 w-5 text-red-600" />
                  )}
                </div>
                <div>
                  <p className="font-medium">{txn.description}</p>
                  <p className="text-sm text-gray-600">{txn.date}</p>
                </div>
              </div>
              <p className={`font-semibold ${
                txn.type === 'credit' ? 'text-green-600' : 'text-red-600'
              }`}>
                {txn.type === 'credit' ? '+' : ''}{txn.amount.toLocaleString()} KES
              </p>
            </div>
          ))}
        </div>
        <button className="mt-4 text-wekeza-blue hover:underline text-sm font-medium">
          View All Transactions
        </button>
      </div>
    </div>
  );
}
