import { Routes, Route, Link, useLocation } from 'react-router-dom';
import { FileText, DollarSign, TrendingUp, Briefcase } from 'lucide-react';
import { Applications } from './lending/Applications';
import { LoanDetails } from './lending/LoanDetails';
import { Disbursements } from './lending/Disbursements';
import { Portfolio } from './lending/Portfolio';

export default function Lending() {
  const location = useLocation();
  
  const tabs = [
    { name: 'Applications', path: '/public-sector/lending/applications', icon: FileText },
    { name: 'Disbursements', path: '/public-sector/lending/disbursements', icon: DollarSign },
    { name: 'Portfolio', path: '/public-sector/lending/portfolio', icon: Briefcase },
  ];

  const isActive = (path: string) => location.pathname.startsWith(path);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">Government Lending</h1>
        <p className="text-gray-600">Manage government loan applications and portfolio</p>
      </div>

      {/* Navigation Tabs */}
      <div className="border-b border-gray-200">
        <nav className="-mb-px flex space-x-8">
          {tabs.map((tab) => {
            const Icon = tab.icon;
            return (
              <Link
                key={tab.path}
                to={tab.path}
                className={`
                  flex items-center py-4 px-1 border-b-2 font-medium text-sm transition-colors
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
        <Route path="/applications" element={<Applications />} />
        <Route path="/applications/:id" element={<LoanDetails />} />
        <Route path="/disbursements" element={<Disbursements />} />
        <Route path="/portfolio" element={<Portfolio />} />
        <Route path="/" element={<Applications />} />
      </Routes>
    </div>
  );
}
