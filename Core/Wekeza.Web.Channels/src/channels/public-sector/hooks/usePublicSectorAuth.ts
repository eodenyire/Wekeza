import { useAuth } from '@/contexts/AuthContext';
import { UserRole } from '../types';
import {
  hasPermission,
  hasAnyRole,
  hasAllRoles,
  getPrimaryRole,
  hasPublicSectorAccess,
  ModuleName,
  PermissionType,
} from '../utils/auth';

/**
 * Extended authentication hook for public sector portal
 * Provides role-based permission checks and public sector specific utilities
 */
export function usePublicSectorAuth() {
  const auth = useAuth();

  const userRoles = auth.user?.roles ?? [];
  const primaryRole = getPrimaryRole(userRoles);

  /**
   * Check if user has permission for a specific module and action
   */
  const checkPermission = (module: ModuleName, permission: PermissionType): boolean => {
    if (!primaryRole) {
      return false;
    }
    return hasPermission(primaryRole, module, permission);
  };

  /**
   * Check if user can read from a module
   */
  const canRead = (module: ModuleName): boolean => {
    return checkPermission(module, 'read');
  };

  /**
   * Check if user can write to a module
   */
  const canWrite = (module: ModuleName): boolean => {
    return checkPermission(module, 'write');
  };

  /**
   * Check if user has any of the specified roles
   */
  const hasRole = (...roles: UserRole[]): boolean => {
    return hasAnyRole(userRoles, roles);
  };

  /**
   * Check if user has all of the specified roles
   */
  const hasRoles = (...roles: UserRole[]): boolean => {
    return hasAllRoles(userRoles, roles);
  };

  /**
   * Check if user has access to public sector portal
   */
  const hasAccess = (): boolean => {
    return hasPublicSectorAccess(userRoles);
  };

  /**
   * Get user's primary public sector role
   */
  const getRole = (): UserRole | null => {
    return primaryRole;
  };

  /**
   * Get user's role display name
   */
  const getRoleDisplayName = (): string => {
    if (!primaryRole) {
      return 'Unknown';
    }

    const roleNames: Record<UserRole, string> = {
      [UserRole.TreasuryOfficer]: 'Treasury Officer',
      [UserRole.CreditOfficer]: 'Credit Officer',
      [UserRole.GovernmentFinanceOfficer]: 'Government Finance Officer',
      [UserRole.CSRManager]: 'CSR Manager',
      [UserRole.ComplianceOfficer]: 'Compliance Officer',
      [UserRole.SeniorManagement]: 'Senior Management',
    };

    return roleNames[primaryRole] || 'Unknown';
  };

  /**
   * Check if user is a treasury officer
   */
  const isTreasuryOfficer = (): boolean => {
    return hasRole(UserRole.TreasuryOfficer);
  };

  /**
   * Check if user is a credit officer
   */
  const isCreditOfficer = (): boolean => {
    return hasRole(UserRole.CreditOfficer);
  };

  /**
   * Check if user is a government finance officer
   */
  const isGovernmentFinanceOfficer = (): boolean => {
    return hasRole(UserRole.GovernmentFinanceOfficer);
  };

  /**
   * Check if user is a CSR manager
   */
  const isCSRManager = (): boolean => {
    return hasRole(UserRole.CSRManager);
  };

  /**
   * Check if user is a compliance officer
   */
  const isComplianceOfficer = (): boolean => {
    return hasRole(UserRole.ComplianceOfficer);
  };

  /**
   * Check if user is senior management
   */
  const isSeniorManagement = (): boolean => {
    return hasRole(UserRole.SeniorManagement);
  };

  return {
    // Base auth properties
    ...auth,
    
    // Public sector specific properties
    primaryRole,
    userRoles,
    
    // Permission checks
    checkPermission,
    canRead,
    canWrite,
    
    // Role checks
    hasRole,
    hasRoles,
    hasAccess,
    getRole,
    getRoleDisplayName,
    
    // Convenience role checks
    isTreasuryOfficer,
    isCreditOfficer,
    isGovernmentFinanceOfficer,
    isCSRManager,
    isComplianceOfficer,
    isSeniorManagement,
  };
}
