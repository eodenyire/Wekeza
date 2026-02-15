export default function LoansPage() {
  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6">Loans</h1>
      
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="text-lg font-semibold mb-4">Apply for Loan</h2>
          <form className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1">Loan Type</label>
              <select className="w-full px-3 py-2 border rounded">
                <option>Personal Loan</option>
                <option>Business Loan</option>
                <option>Mortgage</option>
                <option>Car Loan</option>
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Amount (KES)</label>
              <input type="number" className="w-full px-3 py-2 border rounded" placeholder="50000" />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Term (Months)</label>
              <input type="number" className="w-full px-3 py-2 border rounded" placeholder="12" />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Purpose</label>
              <textarea className="w-full px-3 py-2 border rounded" rows={3}></textarea>
            </div>
            <button className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700">
              Apply for Loan
            </button>
          </form>
        </div>

        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="text-lg font-semibold mb-4">My Loans</h2>
          <div className="space-y-4">
            <div className="border-b pb-4">
              <div className="flex justify-between items-start mb-2">
                <div>
                  <p className="font-medium">Personal Loan</p>
                  <p className="text-sm text-gray-600">Loan #PL-2024-001</p>
                </div>
                <span className="px-2 py-1 bg-green-100 text-green-800 text-xs rounded">Active</span>
              </div>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <p className="text-gray-600">Amount</p>
                  <p className="font-medium">KES 100,000</p>
                </div>
                <div>
                  <p className="text-gray-600">Outstanding</p>
                  <p className="font-medium">KES 75,000</p>
                </div>
                <div>
                  <p className="text-gray-600">Interest Rate</p>
                  <p className="font-medium">12% p.a.</p>
                </div>
                <div>
                  <p className="text-gray-600">Next Payment</p>
                  <p className="font-medium">KES 9,500</p>
                </div>
              </div>
              <button className="mt-3 text-blue-600 text-sm hover:underline">
                Make Payment
              </button>
            </div>
            
            <p className="text-gray-500 text-sm text-center py-4">
              No other active loans
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}
