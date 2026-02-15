import { ReactNode, useMemo, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { usePublicSectorAuth } from './hooks/usePublicSectorAuth';
import { useKeyboardNavigation } from './hooks/useKeyboardNavigation';
import { 
  LayoutDashboard, 
  TrendingUp,
  Banknote,
  Building,
  Gift,
  LogOut,
  Building2,
  Menu,
  X,
  BarChart3,
  Keyboard
} from 'lucide-react';
import { useState } from 'react';

interface LayoutProps {
  children: ReactNode;
}

interface NavigationItem {
  name: string;
  href: string;
  icon: any;
  module: 'dashboard' | 'securities' | 'lending' | 'banking' | 'grants' | 'analytics';
}

export default function Layout({ children }: LayoutProps) {
  const { user, logout, canRead, isSeniorManagement, isComplianceOfficer, getRoleDisplayName } = usePublicSectorAuth();
  const location = useLocation();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [showKeyboardHelp, setShowKeyboardHelp] = useState(false);
  
  // Enable keyboard navigation
  useKeyboardNavigation();

  // All possible navigation items
  const allNavigationItems: NavigationItem[] = [
    { name: 'Dashboard', href: '/public-sector/dashboard', icon: LayoutDashboard, module: 'dashboard' },
    { name: 'Securities', href: '/public-sector/securities', icon: TrendingUp, module: 'securities' },
    { name: 'Lending', href: '/public-sector/lending', icon: Banknote, module: 'lending' },
    { name: 'Banking', href: '/public-sector/banking', icon: Building, module: 'banking' },
    { name: 'Grants', href: '/public-sector/grants', icon: Gift, module: 'grants' },
    { name: 'Analytics', href: '/public-sector/analytics', icon: BarChart3, module: 'analytics' },
  ];

  // Filter navigation items based on user role permissions
  const navigation = useMemo(() => {
    if (!user) {
      return [];
    }

    return allNavigationItems.filter(item => {
      // Dashboard is accessible to all roles
      if (item.module === 'dashboard') {
        return true;
      }

      // Analytics is only for Senior Management
      if (item.module === 'analytics') {
        return isSeniorManagement();
      }

      // Senior Management: All modules (read-only) + Analytics
      if (isSeniorManagement()) {
        return true; // Has access to all modules including Analytics
      }

      // Compliance Officer: All modules except Analytics (read-only)
      if (isComplianceOfficer()) {
        return item.module !== 'analytics'; // Has access to all modules except Analytics
      }

      // For other roles, check specific module permissions
      // Treasury Officer: Securities + Dashboard
      // Credit Officer: Lending + Dashboard
      // Government Finance Officer: Banking + Dashboard
      // CSR Manager: Grants + Dashboard
      return canRead(item.module as any);
    });
  }, [user, canRead, isSeniorManagement, isComplianceOfficer]);

  const isActive = (path: string) => location.pathname.startsWith(path);

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm sticky top-0 z-40">
        <div className="px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <button
                onClick={() => setSidebarOpen(!sidebarOpen)}
                className="lg:hidden mr-4"
              >
                {sidebarOpen ? <X className="h-6 w-6" /> : <Menu className="h-6 w-6" />}
              </button>
              <Link to="/" className="flex items-center space-x-2">
                <Building2 className="h-8 w-8 text-green-700" />
                <span className="text-xl font-bold text-green-700">Wekeza Bank</span>
              </Link>
              <span className="ml-4 text-sm text-gray-500 hidden sm:block">Public Sector Portal</span>
            </div>
            <div className="flex items-center space-x-4">
              <div className="text-right hidden sm:block">
                <div className="text-sm text-gray-700">Welcome, {user?.username}</div>
                <div className="text-xs text-gray-500">{getRoleDisplayName()}</div>
              </div>
              <button
                onClick={logout}
                className="flex items-center text-gray-700 hover:text-green-700"
                title="Logout"
              >
                <LogOut className="h-5 w-5" />
              </button>
            </div>
          </div>
        </div>
      </header>

      <div className="flex">
        {/* Sidebar */}
        <aside className={`
          fixed lg:static inset-y-0 left-0 z-30 w-64 bg-white shadow-lg transform transition-transform duration-200 ease-in-out
          ${sidebarOpen ? 'translate-x-0' : '-translate-x-full lg:translate-x-0'}
        `}>
          <nav className="mt-5 px-4 space-y-1">
            {navigation.map((item) => {
              const Icon = item.icon;
              return (
                <Link
                  key={item.name}
                  to={item.href}
                  onClick={() => setSidebarOpen(false)}
                  className={`
                    flex items-center px-4 py-3 text-sm font-medium rounded-lg transition-colors
                    ${isActive(item.href)
                      ? 'bg-green-700 text-white'
                      : 'text-gray-700 hover:bg-gray-100'
                    }
                  `}
                >
                  <Icon className="h-5 w-5 mr-3" />
                  {item.name}
                </Link>
              );
            })}
          </nav>
        </aside>

        {/* Main Content */}
        <main className="flex-1 p-6 lg:p-8">
          {children}
        </main>
      </div>

      {/* Mobile sidebar overlay */}
      {sidebarOpen && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 z-20 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}
    </div>
  );
}
