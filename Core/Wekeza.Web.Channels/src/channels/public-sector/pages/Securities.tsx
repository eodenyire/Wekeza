import { Routes, Route, Link, useLocation } from 'react-router-dom';
import { TrendingUp, FileText, BarChart3, Briefcase } from 'lucide-react';
import TreasuryBills from './securities/TreasuryBills';
import Bonds from './securities/Bonds';
import Stocks from './securities/Stocks';
import Portfolio from './securities/Portfolio';

export default function Securities() {
  const location = useLocation();
  
  const tabs = [
    { name: 'Treasury Bills', path: '/public-sector/securities/treasury-bills', icon: TrendingUp },
    { name: 'Bonds', path: '/public-sector/securities/bonds', icon: FileText },
    { name: 'Stocks', path: '/public-sector/securities/stocks', icon: BarChart3 },
    { name: 'Portfolio', path: '/public-sector/securities/portfolio', icon: Briefcase },
  ];

  const isActive = (path: string) => location.pathname === path;

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">Securities Trading</h1>
        <p className="text-gray-600">Trade government securities and manage your portfolio</p>
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
                    ? 'border-wekeza-blue text-wekeza-blue'
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
        <Route path="/treasury-bills" element={<TreasuryBills />} />
        <Route path="/bonds" element={<Bonds />} />
        <Route path="/stocks" element={<Stocks />} />
        <Route path="/portfolio" element={<Portfolio />} />
        <Route path="/" element={<TreasuryBills />} />
      </Routes>
    </div>
  );
}
