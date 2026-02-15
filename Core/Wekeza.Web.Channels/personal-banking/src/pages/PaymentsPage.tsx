export default function PaymentsPage() {
  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6">Payments</h1>
      
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="text-lg font-semibold mb-4">Pay Bills</h2>
          <form className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1">Biller</label>
              <select className="w-full px-3 py-2 border rounded">
                <option>Select biller</option>
                <option>KPLC (Electricity)</option>
                <option>Nairobi Water</option>
                <option>Safaricom</option>
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Account Number</label>
              <input type="text" className="w-full px-3 py-2 border rounded" />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Amount</label>
              <input type="number" className="w-full px-3 py-2 border rounded" />
            </div>
            <button className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700">
              Pay Bill
            </button>
          </form>
        </div>

        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="text-lg font-semibold mb-4">Buy Airtime</h2>
          <form className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1">Phone Number</label>
              <input type="tel" className="w-full px-3 py-2 border rounded" placeholder="+254..." />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Provider</label>
              <select className="w-full px-3 py-2 border rounded">
                <option>Safaricom</option>
                <option>Airtel</option>
                <option>Telkom</option>
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Amount</label>
              <input type="number" className="w-full px-3 py-2 border rounded" />
            </div>
            <button className="w-full bg-green-600 text-white py-2 rounded hover:bg-green-700">
              Buy Airtime
            </button>
          </form>
        </div>
      </div>
    </div>
  )
}
