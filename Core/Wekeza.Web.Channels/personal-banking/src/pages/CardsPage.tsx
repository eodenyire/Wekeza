export default function CardsPage() {
  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6">Cards</h1>
      
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Card Display */}
        <div className="lg:col-span-2">
          <div className="bg-gradient-to-br from-blue-600 to-blue-800 text-white p-6 rounded-xl shadow-lg mb-6">
            <div className="flex justify-between items-start mb-8">
              <div>
                <p className="text-sm opacity-80">Debit Card</p>
                <p className="text-lg font-semibold">Wekeza Bank</p>
              </div>
              <div className="text-right">
                <p className="text-xs opacity-80">Card Status</p>
                <p className="text-sm font-medium">Active</p>
              </div>
            </div>
            
            <div className="mb-6">
              <p className="text-xs opacity-80 mb-1">Card Number</p>
              <p className="text-xl font-mono tracking-wider">**** **** **** 4532</p>
            </div>
            
            <div className="flex justify-between">
              <div>
                <p className="text-xs opacity-80">Card Holder</p>
                <p className="text-sm font-medium">JOHN DOE</p>
              </div>
              <div>
                <p className="text-xs opacity-80">Expires</p>
                <p className="text-sm font-medium">12/26</p>
              </div>
              <div>
                <p className="text-xs opacity-80">CVV</p>
                <p className="text-sm font-medium">***</p>
              </div>
            </div>
          </div>

          <div className="bg-white p-6 rounded-lg shadow">
            <h2 className="text-lg font-semibold mb-4">Recent Transactions</h2>
            <div className="space-y-3">
              <div className="flex justify-between items-center py-2 border-b">
                <div>
                  <p className="font-medium">Amazon.com</p>
                  <p className="text-sm text-gray-600">Feb 10, 2026</p>
                </div>
                <p className="font-semibold text-red-600">-KES 5,420</p>
              </div>
              <div className="flex justify-between items-center py-2 border-b">
                <div>
                  <p className="font-medium">Carrefour Supermarket</p>
                  <p className="text-sm text-gray-600">Feb 9, 2026</p>
                </div>
                <p className="font-semibold text-red-600">-KES 3,250</p>
              </div>
              <div className="flex justify-between items-center py-2 border-b">
                <div>
                  <p className="font-medium">Shell Petrol Station</p>
                  <p className="text-sm text-gray-600">Feb 8, 2026</p>
                </div>
                <p className="font-semibold text-red-600">-KES 4,500</p>
              </div>
            </div>
          </div>
        </div>

        {/* Card Actions */}
        <div className="space-y-4">
          <div className="bg-white p-6 rounded-lg shadow">
            <h2 className="text-lg font-semibold mb-4">Card Actions</h2>
            <div className="space-y-3">
              <button className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700">
                Request New Card
              </button>
              <button className="w-full bg-green-600 text-white py-2 rounded hover:bg-green-700">
                Request Virtual Card
              </button>
              <button className="w-full bg-yellow-600 text-white py-2 rounded hover:bg-yellow-700">
                Block Card
              </button>
              <button className="w-full bg-gray-600 text-white py-2 rounded hover:bg-gray-700">
                Set PIN
              </button>
            </div>
          </div>

          <div className="bg-white p-6 rounded-lg shadow">
            <h2 className="text-lg font-semibold mb-4">Card Limits</h2>
            <div className="space-y-3 text-sm">
              <div className="flex justify-between">
                <span className="text-gray-600">Daily ATM Limit</span>
                <span className="font-medium">KES 50,000</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Daily POS Limit</span>
                <span className="font-medium">KES 200,000</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Online Limit</span>
                <span className="font-medium">KES 100,000</span>
              </div>
              <button className="text-blue-600 text-sm hover:underline mt-2">
                Modify Limits
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
