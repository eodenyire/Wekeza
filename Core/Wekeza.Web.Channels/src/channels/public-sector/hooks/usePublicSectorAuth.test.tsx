import { describe, it, expect, vi, beforeEach } from 'vitest';
import { renderHook } from '@testing-library/react';
import { usePublicSectorAuth } from './usePublicSectorAuth';
import { UserRole } from '../types';

// Mock the AuthContext
vi.mock('@/contexts/AuthContext', () => ({
  useAuth: vi.fn(),
}));

import { useAuth } from '@/contexts/AuthContext';

describe('usePublicSectorAuth', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('with Treasury Officer role', () => {
    beforeEach(() => {
      vi.mocked(useAuth).mockReturnValue({
        user: {
          userId: '123',
          username: 'treasury.officer',
          email: 'treasury@wekeza.com',
          roles: [UserRole.TreasuryOfficer],
        },
        isAuthenticated: true,
        isLoading: false,
        login: vi.fn(),
        logout: vi.fn(),
      });
    });

    it('should identify user as treasury officer', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.isTreasuryOfficer()).toBe(true);
      expect(result.current.isCreditOfficer()).toBe(false);
      expect(result.current.primaryRole).toBe(UserRole.TreasuryOfficer);
    });

    it('should have correct permissions for securities module', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.canRead('securities')).toBe(true);
      expect(result.current.canWrite('securities')).toBe(true);
    });

    it('should not have permissions for lending module', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.canRead('lending')).toBe(false);
      expect(result.current.canWrite('lending')).toBe(false);
    });

    it('should have read access to dashboard', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.canRead('dashboard')).toBe(true);
      expect(result.current.canWrite('dashboard')).toBe(false);
    });

    it('should return correct role display name', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.getRoleDisplayName()).toBe('Treasury Officer');
    });

    it('should have public sector access', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.hasAccess()).toBe(true);
    });
  });

  describe('with Compliance Officer role', () => {
    beforeEach(() => {
      vi.mocked(useAuth).mockReturnValue({
        user: {
          userId: '456',
          username: 'compliance.officer',
          email: 'compliance@wekeza.com',
          roles: [UserRole.ComplianceOfficer],
        },
        isAuthenticated: true,
        isLoading: false,
        login: vi.fn(),
        logout: vi.fn(),
      });
    });

    it('should identify user as compliance officer', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.isComplianceOfficer()).toBe(true);
      expect(result.current.primaryRole).toBe(UserRole.ComplianceOfficer);
    });

    it('should have read access to all modules', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.canRead('securities')).toBe(true);
      expect(result.current.canRead('lending')).toBe(true);
      expect(result.current.canRead('banking')).toBe(true);
      expect(result.current.canRead('grants')).toBe(true);
      expect(result.current.canRead('dashboard')).toBe(true);
    });

    it('should not have write access to any module', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.canWrite('securities')).toBe(false);
      expect(result.current.canWrite('lending')).toBe(false);
      expect(result.current.canWrite('banking')).toBe(false);
      expect(result.current.canWrite('grants')).toBe(false);
      expect(result.current.canWrite('dashboard')).toBe(false);
    });
  });

  describe('with multiple roles', () => {
    beforeEach(() => {
      vi.mocked(useAuth).mockReturnValue({
        user: {
          userId: '789',
          username: 'multi.role',
          email: 'multi@wekeza.com',
          roles: [UserRole.TreasuryOfficer, UserRole.ComplianceOfficer],
        },
        isAuthenticated: true,
        isLoading: false,
        login: vi.fn(),
        logout: vi.fn(),
      });
    });

    it('should return first public sector role as primary', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.primaryRole).toBe(UserRole.TreasuryOfficer);
    });

    it('should detect both roles', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.hasRole(UserRole.TreasuryOfficer)).toBe(true);
      expect(result.current.hasRole(UserRole.ComplianceOfficer)).toBe(true);
    });

    it('should use primary role for permissions', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      // Should use TreasuryOfficer permissions (primary role)
      expect(result.current.canWrite('securities')).toBe(true);
      expect(result.current.canWrite('lending')).toBe(false);
    });
  });

  describe('without public sector roles', () => {
    beforeEach(() => {
      vi.mocked(useAuth).mockReturnValue({
        user: {
          userId: '999',
          username: 'regular.user',
          email: 'user@wekeza.com',
          roles: ['CUSTOMER', 'USER'],
        },
        isAuthenticated: true,
        isLoading: false,
        login: vi.fn(),
        logout: vi.fn(),
      });
    });

    it('should not have public sector access', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.hasAccess()).toBe(false);
    });

    it('should have null primary role', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.primaryRole).toBe(null);
    });

    it('should not have any module permissions', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.canRead('securities')).toBe(false);
      expect(result.current.canWrite('securities')).toBe(false);
    });

    it('should return Unknown for role display name', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.getRoleDisplayName()).toBe('Unknown');
    });
  });

  describe('when not authenticated', () => {
    beforeEach(() => {
      vi.mocked(useAuth).mockReturnValue({
        user: null,
        isAuthenticated: false,
        isLoading: false,
        login: vi.fn(),
        logout: vi.fn(),
      });
    });

    it('should not have public sector access', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.hasAccess()).toBe(false);
    });

    it('should have null primary role', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.primaryRole).toBe(null);
    });

    it('should not be authenticated', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.isAuthenticated).toBe(false);
    });
  });

  describe('checkPermission method', () => {
    beforeEach(() => {
      vi.mocked(useAuth).mockReturnValue({
        user: {
          userId: '123',
          username: 'credit.officer',
          email: 'credit@wekeza.com',
          roles: [UserRole.CreditOfficer],
        },
        isAuthenticated: true,
        isLoading: false,
        login: vi.fn(),
        logout: vi.fn(),
      });
    });

    it('should correctly check module permissions', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.checkPermission('lending', 'read')).toBe(true);
      expect(result.current.checkPermission('lending', 'write')).toBe(true);
      expect(result.current.checkPermission('securities', 'read')).toBe(false);
      expect(result.current.checkPermission('securities', 'write')).toBe(false);
    });
  });

  describe('hasRoles method', () => {
    beforeEach(() => {
      vi.mocked(useAuth).mockReturnValue({
        user: {
          userId: '123',
          username: 'multi.role',
          email: 'multi@wekeza.com',
          roles: [UserRole.TreasuryOfficer, UserRole.ComplianceOfficer],
        },
        isAuthenticated: true,
        isLoading: false,
        login: vi.fn(),
        logout: vi.fn(),
      });
    });

    it('should return true when user has all required roles', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.hasRoles(UserRole.TreasuryOfficer, UserRole.ComplianceOfficer)).toBe(true);
    });

    it('should return false when user is missing some required roles', () => {
      const { result } = renderHook(() => usePublicSectorAuth());
      
      expect(result.current.hasRoles(UserRole.TreasuryOfficer, UserRole.CreditOfficer)).toBe(false);
    });
  });
});
