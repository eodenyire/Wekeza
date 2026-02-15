import { UserRole } from '../types';

/**
 * Permission matrix for public sector user roles
 */
export const ROLE_PERMISSIONS = {
  [UserRole.TreasuryOfficer]: {
    securities: { read: true, write: true },
    lending: { read: false, write: false },
    banking: { read: false, write: false },
    grants: { read: false, write: false },
    dashboard: { read: true, write: false },
  },
  [UserRole.CreditOfficer]: {
    securities: { read: false, write: false },
    lending: { read: true, write: true },
    banking: { read: false, write: false },
    grants: { read: false, write: false },
    dashboard: { read: true, write: false },
  },
  [UserRole.GovernmentFinanceOfficer]: {
    securities: { read: false, write: false },
    lending: { read: false, write: false },
    banking: { read: true, write: true },
    grants: { read: false, write: false },
    dashboard: { read: true, write: false },
  },
  [UserRole.CSRManager]: {
    securities: { read: false, write: false },
    lending: { read: false, write: false },
    banking: { read: false, write: false },
    grants: { read: true, write: true },
    dashboard: { read: true, write: false },
  },
  [UserRole.ComplianceOfficer]: {
    securities: { read: true, write: false },
    lending: { read: true, write: false },
    banking: { read: true, write: false },
    grants: { read: true, write: false },
    dashboard: { read: true, write: false },
  },
  [UserRole.SeniorManagement]: {
    securities: { read: true, write: false },
    lending: { read: true, write: false },
    banking: { read: true, write: false },
    grants: { read: true, write: false },
    dashboard: { read: true, write: false },
  },
} as const;

export type ModuleName = 'securities' | 'lending' | 'banking' | 'grants' | 'dashboard';
export type PermissionType = 'read' | 'write';

/**
 * Check if a user role has permission for a specific module and action
 */
export function hasPermission(
  role: UserRole | string,
  module: ModuleName,
  permission: PermissionType
): boolean {
  // Check if role is a valid UserRole
  if (!Object.values(UserRole).includes(role as UserRole)) {
    return false;
  }

  const rolePermissions = ROLE_PERMISSIONS[role as UserRole];
  if (!rolePermissions) {
    return false;
  }

  return rolePermissions[module]?.[permission] ?? false;
}

/**
 * Check if user has any of the specified roles
 */
export function hasAnyRole(userRoles: string[], allowedRoles: UserRole[]): boolean {
  return userRoles.some(role => allowedRoles.includes(role as UserRole));
}

/**
 * Check if user has all of the specified roles
 */
export function hasAllRoles(userRoles: string[], requiredRoles: UserRole[]): boolean {
  return requiredRoles.every(role => userRoles.includes(role));
}

/**
 * Get the primary role for a user (first public sector role found)
 */
export function getPrimaryRole(userRoles: string[]): UserRole | null {
  const publicSectorRoles = Object.values(UserRole);
  const primaryRole = userRoles.find(role => publicSectorRoles.includes(role as UserRole));
  return primaryRole ? (primaryRole as UserRole) : null;
}

/**
 * JWT token utilities
 */
export interface JWTPayload {
  userId: string;
  username: string;
  email: string;
  roles: string[];
  exp: number;
  iat: number;
}

/**
 * Decode JWT token (without verification - verification happens on backend)
 */
export function decodeJWT(token: string): JWTPayload | null {
  try {
    const parts = token.split('.');
    if (parts.length !== 3) {
      return null;
    }

    const payload = parts[1];
    const decoded = JSON.parse(atob(payload));
    return decoded as JWTPayload;
  } catch (error) {
    console.error('Failed to decode JWT:', error);
    return null;
  }
}

/**
 * Check if JWT token is expired
 */
export function isTokenExpired(token: string): boolean {
  const payload = decodeJWT(token);
  if (!payload) {
    return true;
  }

  const now = Math.floor(Date.now() / 1000);
  return payload.exp < now;
}

/**
 * Get token expiration time in milliseconds
 */
export function getTokenExpirationTime(token: string): number | null {
  const payload = decodeJWT(token);
  if (!payload) {
    return null;
  }

  return payload.exp * 1000;
}

/**
 * Check if token will expire within the specified minutes
 */
export function willTokenExpireSoon(token: string, withinMinutes: number = 5): boolean {
  const expirationTime = getTokenExpirationTime(token);
  if (!expirationTime) {
    return true;
  }

  const now = Date.now();
  const timeUntilExpiration = expirationTime - now;
  const minutesUntilExpiration = timeUntilExpiration / (1000 * 60);

  return minutesUntilExpiration <= withinMinutes;
}

/**
 * Extract roles from JWT token
 */
export function getRolesFromToken(token: string): string[] {
  const payload = decodeJWT(token);
  return payload?.roles ?? [];
}

/**
 * Validate that user has public sector access
 */
export function hasPublicSectorAccess(userRoles: string[]): boolean {
  const publicSectorRoles = Object.values(UserRole);
  return userRoles.some(role => publicSectorRoles.includes(role as UserRole));
}
