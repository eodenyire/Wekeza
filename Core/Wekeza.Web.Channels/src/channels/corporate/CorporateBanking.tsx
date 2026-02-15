import { Link } from 'react-router-dom';

export default function CorporateBanking() {
  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
      <div className="max-w-2xl w-full text-center">
        <h1 className="text-4xl font-bold mb-4">Corporate Banking Portal</h1>
        <p className="text-xl text-gray-600 mb-8">
          Coming Soon - Comprehensive banking solutions for large enterprises
        </p>
        <div className="card">
          <h2 className="text-2xl font-bold mb-4">Features</h2>
          <ul className="text-left space-y-2 text-gray-700">
            <li>• Bulk Payment Processing</li>
            <li>• Trade Finance (Letters of Credit, Bank Guarantees)</li>
            <li>• Treasury Operations (FX Deals, Money Markets)</li>
            <li>• Maker-Checker Approval Workflows</li>
            <li>• Advanced Reporting & Analytics</li>
            <li>• Multi-User Access Control</li>
          </ul>
        </div>
        <Link to="/" className="btn btn-primary mt-6">
          Back to Home
        </Link>
      </div>
    </div>
  );
}
