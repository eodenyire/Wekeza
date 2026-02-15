import { Routes, Route, Link, useLocation } from 'react-router-dom';
import PaymentInitiation from './PaymentInitiation';
import PendingApprovals from './PendingApprovals';

export function PaymentWorkflow() {
  const location = useLocation();

  const tabs = [
    { name: 'Initiate Payment', path: '/public-sector/banking/workflow/initiate' },
    { name: 'Pending Approvals', path: '/public-sector/banking/workflow/pending' },
    { name: 'Approval History', path: '/public-sector/banking/workflow/history' },
  ];

  const isActive = (path: string) => location.pathname === path;

  return (
    <div>
      {/* Sub-navigation */}
      <div className="mb-6 border-b border-gray-200">
        <nav className="-mb-px flex space-x-8">
          {tabs.map((tab) => (
            <Link
              key={tab.path}
              to={tab.path}
              className={`
                py-4 px-1 border-b-2 font-medium text-sm transition-colors
                ${isActive(tab.path)
                  ? 'border-green-600 text-green-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }
              `}
            >
              {tab.name}
            </Link>
          ))}
        </nav>
      </div>

      {/* Content */}
      <Routes>
        <Route path="/initiate" element={<PaymentInitiation />} />
        <Route path="/pending" element={<PendingApprovals />} />
        <Route path="/history" element={<div className="p-6 bg-white rounded-lg shadow-md"><p>Approval History - Coming Soon</p></div>} />
        <Route path="/" element={<PendingApprovals />} />
      </Routes>
    </div>
  );
}
