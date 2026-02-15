import { Routes, Route, Link, useLocation } from 'react-router-dom';
import { Building2, CreditCard, TrendingUp, FileText, CheckCircle, DollarSign } from 'lucide-react';
import { Accounts } from './banking/Accounts';
import { Payments } from './banking/Payments';
import { Revenues } from './banking/Revenues';
import { Reports } from './banking/Reports';
import { PaymentWorkflow } from './banking/PaymentWorkflow';
import { BudgetControl } from './banking/BudgetControl';

export default function Banking() {
  const location = useLocation();
  
  const tabs = [
    { name: 'Accounts', path: '/public-sector/banking/accounts', icon: Building2 },
    { name: 'Bulk Payments', path: '/public-sector/banking/payments', icon: CreditCard },
    { name: 'Payment Workflow', path: '/public-sector/banking/workflow', icon: CheckCircle },
    { name: 'Budget Control', path: '/public-sector/banking/budget', icon: DollarSign },
    { name: 'Revenues', path: '/public-sector/banking/revenues', icon: TrendingUp },
    { name: 'Reports', path: '/public-sector/banking/reports', icon: FileText },
  ];

  const isActive = (path: string) => location.pathname === path;

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">Government Banking Services</h1>
        <p className="text-gray-600">Manage government accounts, payments, and revenues</p>
      </div>

      {/* Navigation Tabs */}
      <div className="border-b border-gray-200">
        <nav className="-mb-px flex space-x-8 overflow-x-auto">
          {tabs.map((tab) => {
            const Icon = tab.icon;
            return (
              <Link
                key={tab.path}
                to={tab.path}
                className={`
                  flex items-center py-4 px-1 border-b-2 font-medium text-sm transition-colors whitespace-nowrap
                  ${isActive(tab.path)
                    ? 'border-blue-600 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  }
                `}
              >
                <Icon className="h-5 w-5 mr-2" />
                {tab.name}
              </Link>
            );
          })}
        </nav>
      </div>

      {/* Content Area */}
      <Routes>
        <Route path="/accounts" element={<Accounts />} />
        <Route path="/payments" element={<Payments />} />
        <Route path="/workflow" element={<PaymentWorkflow />} />
        <Route path="/budget" element={<BudgetControl />} />
        <Route path="/revenues" element={<Revenues />} />
        <Route path="/reports" element={<Reports />} />
        <Route path="/" element={<Accounts />} />
      </Routes>
    </div>
  );
}
