import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import Layout from './Layout';
import { UserRole } from './types';
import * as AuthContext from '@/contexts/AuthContext';

// Mock the usePublicSectorAuth hook
vi.mock('./hooks/usePublicSectorAuth', () => ({
  usePublicSectorAuth: () => {
    const auth = AuthContext.useAuth();
    const userRoles = auth.user?.roles ?? [];
    
    // Determine primary role
    const publicSectorRoles = Object.values(UserRole);
    const primaryRole = userRoles.find(role => publicSectorRoles.includes(role as UserRole)) as UserRole | null;
    
    // Permission check based on role
    const canRead = (module: string): boolean => {
      if (!primaryRole) return false;
      
      const permissions: Record<UserRole, Record<string, boolean>> = {
        [UserRole.TreasuryOfficer]: { securities: true, dashboard: true },
        [UserRole.CreditOfficer]: { lending: true, dashboard: true },
        [UserRole.GovernmentFinanceOfficer]: { banking: true, dashboard: true },
        [UserRole.CSRManager]: { grants: true, dashboard: true },
        [UserRole.ComplianceOfficer]: { securities: true, lending: true, banking: true, grants: true, dashboard: true },
        [UserRole.SeniorManagement]: { securities: true, lending: true, banking: true, grants: true, dashboard: true },
      };
      
      return permissions[primaryRole]?.[module] ?? false;
    };
    
    const isSeniorManagement = () => primaryRole === UserRole.SeniorManagement;
    const isComplianceOfficer = () => primaryRole === UserRole.ComplianceOfficer;
    
    const getRoleDisplayName = () => {
      const roleNames: Record<UserRole, string> = {
        [UserRole.TreasuryOfficer]: 'Treasury Officer',
        [UserRole.CreditOfficer]: 'Credit Officer',
        [UserRole.GovernmentFinanceOfficer]: 'Government Finance Officer',
        [UserRole.CSRManager]: 'CSR Manager',
        [UserRole.ComplianceOfficer]: 'Compliance Officer',
        [UserRole.SeniorManagement]: 'Senior Management',
      };
      return primaryRole ? roleNames[primaryRole] : 'Unknown';
    };
    
    return {
      ...auth,
      canRead,
      isSeniorManagement,
      isComplianceOfficer,
      getRoleDisplayName,
    };
  },
}));

describe('Layout - Role-Based Navigation', () => {
  const mockLogout = vi.fn();
  
  beforeEach(() => {
    vi.clearAllMocks();
  });
  
  const renderLayout = (roles: string[]) => {
    vi.spyOn(AuthContext, 'useAuth').mockReturnValue({
      user: {
        id: '1',
        username: 'testuser',
        email: 'test@example.com',
        roles,
      },
      isAuthenticated: true,
      login: vi.fn(),
      logout: mockLogout,
      loading: false,
    });
    
    return render(
      <BrowserRouter>
        <Layout>
          <div>Test Content</div>
        </Layout>
      </BrowserRouter>
    );
  };
  
  describe('Treasury Officer Navigation', () => {
    it('should show Securities and Dashboard only', () => {
      renderLayout([UserRole.TreasuryOfficer]);
      
      // Should show
      expect(screen.getByText('Securities')).toBeInTheDocument();
      expect(screen.getByText('Dashboard')).toBeInTheDocument();
      
      // Should not show
      expect(screen.queryByText('Lending')).not.toBeInTheDocument();
      expect(screen.queryByText('Banking')).not.toBeInTheDocument();
      expect(screen.queryByText('Grants')).not.toBeInTheDocument();
      expect(screen.queryByText('Analytics')).not.toBeInTheDocument();
    });
    
    it('should display role name correctly', () => {
      renderLayout([UserRole.TreasuryOfficer]);
      expect(screen.getByText('Treasury Officer')).toBeInTheDocument();
    });
  });
  
  describe('Credit Officer Navigation', () => {
    it('should show Lending and Dashboard only', () => {
      renderLayout([UserRole.CreditOfficer]);
      
      // Should show
      expect(screen.getByText('Lending')).toBeInTheDocument();
      expect(screen.getByText('Dashboard')).toBeInTheDocument();
      
      // Should not show
      expect(screen.queryByText('Securities')).not.toBeInTheDocument();
      expect(screen.queryByText('Banking')).not.toBeInTheDocument();
      expect(screen.queryByText('Grants')).not.toBeInTheDocument();
      expect(screen.queryByText('Analytics')).not.toBeInTheDocument();
    });
    
    it('should display role name correctly', () => {
      renderLayout([UserRole.CreditOfficer]);
      expect(screen.getByText('Credit Officer')).toBeInTheDocument();
    });
  });
  
  describe('Government Finance Officer Navigation', () => {
    it('should show Banking and Dashboard only', () => {
      renderLayout([UserRole.GovernmentFinanceOfficer]);
      
      // Should show
      expect(screen.getByText('Banking')).toBeInTheDocument();
      expect(screen.getByText('Dashboard')).toBeInTheDocument();
      
      // Should not show
      expect(screen.queryByText('Securities')).not.toBeInTheDocument();
      expect(screen.queryByText('Lending')).not.toBeInTheDocument();
      expect(screen.queryByText('Grants')).not.toBeInTheDocument();
      expect(screen.queryByText('Analytics')).not.toBeInTheDocument();
    });
    
    it('should display role name correctly', () => {
      renderLayout([UserRole.GovernmentFinanceOfficer]);
      expect(screen.getByText('Government Finance Officer')).toBeInTheDocument();
    });
  });
  
  describe('CSR Manager Navigation', () => {
    it('should show Grants and Dashboard only', () => {
      renderLayout([UserRole.CSRManager]);
      
      // Should show
      expect(screen.getByText('Grants')).toBeInTheDocument();
      expect(screen.getByText('Dashboard')).toBeInTheDocument();
      
      // Should not show
      expect(screen.queryByText('Securities')).not.toBeInTheDocument();
      expect(screen.queryByText('Lending')).not.toBeInTheDocument();
      expect(screen.queryByText('Banking')).not.toBeInTheDocument();
      expect(screen.queryByText('Analytics')).not.toBeInTheDocument();
    });
    
    it('should display role name correctly', () => {
      renderLayout([UserRole.CSRManager]);
      expect(screen.getByText('CSR Manager')).toBeInTheDocument();
    });
  });
  
  describe('Compliance Officer Navigation', () => {
    it('should show all modules (read-only) and Dashboard, but not Analytics', () => {
      renderLayout([UserRole.ComplianceOfficer]);
      
      // Should show all modules
      expect(screen.getByText('Securities')).toBeInTheDocument();
      expect(screen.getByText('Lending')).toBeInTheDocument();
      expect(screen.getByText('Banking')).toBeInTheDocument();
      expect(screen.getByText('Grants')).toBeInTheDocument();
      expect(screen.getByText('Dashboard')).toBeInTheDocument();
      
      // Should not show Analytics (only for Senior Management)
      expect(screen.queryByText('Analytics')).not.toBeInTheDocument();
    });
    
    it('should display role name correctly', () => {
      renderLayout([UserRole.ComplianceOfficer]);
      expect(screen.getByText('Compliance Officer')).toBeInTheDocument();
    });
  });
  
  describe('Senior Management Navigation', () => {
    it('should show Dashboard and Analytics', () => {
      renderLayout([UserRole.SeniorManagement]);
      
      // Should show all modules including Analytics
      expect(screen.getByText('Securities')).toBeInTheDocument();
      expect(screen.getByText('Lending')).toBeInTheDocument();
      expect(screen.getByText('Banking')).toBeInTheDocument();
      expect(screen.getByText('Grants')).toBeInTheDocument();
      expect(screen.getByText('Dashboard')).toBeInTheDocument();
      expect(screen.getByText('Analytics')).toBeInTheDocument();
    });
    
    it('should display role name correctly', () => {
      renderLayout([UserRole.SeniorManagement]);
      expect(screen.getByText('Senior Management')).toBeInTheDocument();
    });
  });
  
  describe('Common Layout Elements', () => {
    it('should display Wekeza Bank branding', () => {
      renderLayout([UserRole.TreasuryOfficer]);
      expect(screen.getByText('Wekeza Bank')).toBeInTheDocument();
      expect(screen.getByText('Public Sector Portal')).toBeInTheDocument();
    });
    
    it('should display username', () => {
      renderLayout([UserRole.TreasuryOfficer]);
      expect(screen.getByText(/Welcome, testuser/)).toBeInTheDocument();
    });
    
    it('should render child content', () => {
      renderLayout([UserRole.TreasuryOfficer]);
      expect(screen.getByText('Test Content')).toBeInTheDocument();
    });
  });
});
