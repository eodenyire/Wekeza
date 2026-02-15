import { Routes, Route, Link, useLocation } from 'react-router-dom';
import { Gift, FileText, CheckCircle, TrendingUp } from 'lucide-react';
import { Programs } from './grants/Programs';
import { Applications } from './grants/Applications';
import { Approvals } from './grants/Approvals';
import { Impact } from './grants/Impact';

export default function Grants() {
  const location = useLocation();
  
  const tabs = [
    { name: 'Programs', path: '/public-sector/grants/programs', icon: Gift },
    { name: 'Applications', path: '/public-sector/grants/applications', icon: FileText },
    { name: 'Approvals', path: '/public-sector/grants/approvals', icon: CheckCircle },
    { name: 'Impact', path: '/public-sector/grants/impact', icon: TrendingUp },
  ];

  const isActive = (path: string) => location.pathname === path;

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">Grants & Philanthropy</h1>
        <p className="text-gray-600">Manage grant programs and philanthropic initiatives</p>
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
        <Route path="/programs" element={<Programs />} />
        <Route path="/applications" element={<Applications />} />
        <Route path="/apply/:programId" element={<Applications />} />
        <Route path="/approvals" element={<Approvals />} />
        <Route path="/impact" element={<Impact />} />
        <Route path="/" element={<Programs />} />
      </Routes>
    </div>
  );
}
