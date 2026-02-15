import { Link } from 'react-router-dom';

export default function SMEBanking() {
  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
      <div className="max-w-2xl w-full text-center">
        <h1 className="text-4xl font-bold mb-4">SME Banking Portal</h1>
        <p className="text-xl text-gray-600 mb-8">
          Coming Soon - Tailored solutions for small and medium enterprises
        </p>
        <div className="card">
          <h2 className="text-2xl font-bold mb-4">Features</h2>
          <ul className="text-left space-y-2 text-gray-700">
            <li>• Business Account Management</li>
            <li>• Working Capital Loans</li>
            <li>• Payroll Management</li>
            <li>• Merchant Services & POS</li>
            <li>• Business Analytics & Insights</li>
            <li>• Invoice Management</li>
          </ul>
        </div>
        <Link to="/" className="btn btn-primary mt-6">
          Back to Home
        </Link>
      </div>
    </div>
  );
}
