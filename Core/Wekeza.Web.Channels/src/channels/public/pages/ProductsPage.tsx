export default function ProductsPage() {
  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <h1 className="text-4xl font-bold mb-8">Our Products</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        <div className="card">
          <h2 className="text-2xl font-bold mb-4">Savings Account</h2>
          <p className="text-gray-600 mb-4">Earn competitive interest on your savings</p>
          <ul className="list-disc list-inside space-y-2 text-gray-700">
            <li>Up to 5% annual interest</li>
            <li>No minimum balance</li>
            <li>Free mobile banking</li>
          </ul>
        </div>
        <div className="card">
          <h2 className="text-2xl font-bold mb-4">Current Account</h2>
          <p className="text-gray-600 mb-4">Perfect for daily transactions</p>
          <ul className="list-disc list-inside space-y-2 text-gray-700">
            <li>Unlimited transactions</li>
            <li>Free debit card</li>
            <li>Overdraft facility</li>
          </ul>
        </div>
      </div>
    </div>
  );
}
