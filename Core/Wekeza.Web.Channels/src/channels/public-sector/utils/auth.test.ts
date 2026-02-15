import { describe, it, expect } from 'vitest';
import {
  hasPermission,
  hasAnyRole,
  hasAllRoles,
  getPrimaryRole,
  decodeJWT,
  isTokenExpired,
  getTokenExpirationTime,
  willTokenExpireSoon,
  getRolesFromToken,
  hasPublicSectorAccess,
  ROLE_PERMISSIONS,
} from './auth';
import { UserRole } from '../types';

describe('auth utilities', () => {
  describe('hasPermission', () => {
    it('should return true when role has read permission for module', () => {
      expect(hasPermission(UserRole.TreasuryOfficer, 'securities', 'read')).toBe(true);
      expect(hasPermission(UserRole.CreditOfficer, 'lending', 'read')).toBe(true);
      expect(hasPermission(UserRole.GovernmentFinanceOfficer, 'banking', 'read')).toBe(true);
      expect(hasPermission(UserRole.CSRManager, 'grants', 'read')).toBe(true);
    });

    it('should return true when role has write permission for module', () => {
      expect(hasPermission(UserRole.TreasuryOfficer, 'securities', 'write')).toBe(true);
      expect(hasPermission(UserRole.CreditOfficer, 'lending', 'write')).toBe(true);
      expect(hasPermission(UserRole.GovernmentFinanceOfficer, 'banking', 'write')).toBe(true);
      expect(hasPermission(UserRole.CSRManager, 'grants', 'write')).toBe(true);
    });

    it('should return false when role does not have permission for module', () => {
      expect(hasPermission(UserRole.TreasuryOfficer, 'lending', 'read')).toBe(false);
      expect(hasPermission(UserRole.CreditOfficer, 'securities', 'write')).toBe(false);
      expect(hasPermission(UserRole.GovernmentFinanceOfficer, 'grants', 'read')).toBe(false);
      expect(hasPermission(UserRole.CSRManager, 'banking', 'write')).toBe(false);
    });

    it('should return true for compliance officer read access to all modules', () => {
      expect(hasPermission(UserRole.ComplianceOfficer, 'securities', 'read')).toBe(true);
      expect(hasPermission(UserRole.ComplianceOfficer, 'lending', 'read')).toBe(true);
      expect(hasPermission(UserRole.ComplianceOfficer, 'banking', 'read')).toBe(true);
      expect(hasPermission(UserRole.ComplianceOfficer, 'grants', 'read')).toBe(true);
      expect(hasPermission(UserRole.ComplianceOfficer, 'dashboard', 'read')).toBe(true);
    });

    it('should return false for compliance officer write access to all modules', () => {
      expect(hasPermission(UserRole.ComplianceOfficer, 'securities', 'write')).toBe(false);
      expect(hasPermission(UserRole.ComplianceOfficer, 'lending', 'write')).toBe(false);
      expect(hasPermission(UserRole.ComplianceOfficer, 'banking', 'write')).toBe(false);
      expect(hasPermission(UserRole.ComplianceOfficer, 'grants', 'write')).toBe(false);
    });

    it('should return true for senior management read access to all modules', () => {
      expect(hasPermission(UserRole.SeniorManagement, 'securities', 'read')).toBe(true);
      expect(hasPermission(UserRole.SeniorManagement, 'lending', 'read')).toBe(true);
      expect(hasPermission(UserRole.SeniorManagement, 'banking', 'read')).toBe(true);
      expect(hasPermission(UserRole.SeniorManagement, 'grants', 'read')).toBe(true);
      expect(hasPermission(UserRole.SeniorManagement, 'dashboard', 'read')).toBe(true);
    });

    it('should return false for invalid role', () => {
      expect(hasPermission('INVALID_ROLE', 'securities', 'read')).toBe(false);
      expect(hasPermission('', 'lending', 'write')).toBe(false);
    });

    it('should return false for non-existent module', () => {
      expect(hasPermission(UserRole.TreasuryOfficer, 'nonexistent' as any, 'read')).toBe(false);
    });
  });

  describe('hasAnyRole', () => {
    it('should return true when user has at least one of the allowed roles', () => {
      const userRoles = [UserRole.TreasuryOfficer, 'OTHER_ROLE'];
      expect(hasAnyRole(userRoles, [UserRole.TreasuryOfficer])).toBe(true);
      expect(hasAnyRole(userRoles, [UserRole.CreditOfficer, UserRole.TreasuryOfficer])).toBe(true);
    });

    it('should return false when user has none of the allowed roles', () => {
      const userRoles = [UserRole.TreasuryOfficer];
      expect(hasAnyRole(userRoles, [UserRole.CreditOfficer])).toBe(false);
      expect(hasAnyRole(userRoles, [UserRole.CreditOfficer, UserRole.CSRManager])).toBe(false);
    });

    it('should return false when user has no roles', () => {
      expect(hasAnyRole([], [UserRole.TreasuryOfficer])).toBe(false);
    });

    it('should return false when allowed roles is empty', () => {
      expect(hasAnyRole([UserRole.TreasuryOfficer], [])).toBe(false);
    });
  });

  describe('hasAllRoles', () => {
    it('should return true when user has all required roles', () => {
      const userRoles = [UserRole.TreasuryOfficer, UserRole.ComplianceOfficer];
      expect(hasAllRoles(userRoles, [UserRole.TreasuryOfficer])).toBe(true);
      expect(hasAllRoles(userRoles, [UserRole.TreasuryOfficer, UserRole.ComplianceOfficer])).toBe(true);
    });

    it('should return false when user is missing some required roles', () => {
      const userRoles = [UserRole.TreasuryOfficer];
      expect(hasAllRoles(userRoles, [UserRole.TreasuryOfficer, UserRole.ComplianceOfficer])).toBe(false);
    });

    it('should return true when required roles is empty', () => {
      expect(hasAllRoles([UserRole.TreasuryOfficer], [])).toBe(true);
    });

    it('should return false when user has no roles but roles are required', () => {
      expect(hasAllRoles([], [UserRole.TreasuryOfficer])).toBe(false);
    });
  });

  describe('getPrimaryRole', () => {
    it('should return the first public sector role found', () => {
      expect(getPrimaryRole([UserRole.TreasuryOfficer])).toBe(UserRole.TreasuryOfficer);
      expect(getPrimaryRole(['OTHER_ROLE', UserRole.CreditOfficer])).toBe(UserRole.CreditOfficer);
      expect(getPrimaryRole([UserRole.TreasuryOfficer, UserRole.CreditOfficer])).toBe(UserRole.TreasuryOfficer);
    });

    it('should return null when no public sector role is found', () => {
      expect(getPrimaryRole(['OTHER_ROLE', 'ANOTHER_ROLE'])).toBe(null);
      expect(getPrimaryRole([])).toBe(null);
    });
  });

  describe('JWT utilities', () => {
    // Helper to create a mock JWT token
    const createMockToken = (payload: any): string => {
      const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
      const payloadStr = btoa(JSON.stringify(payload));
      const signature = 'mock-signature';
      return `${header}.${payloadStr}.${signature}`;
    };

    describe('decodeJWT', () => {
      it('should decode a valid JWT token', () => {
        const payload = {
          userId: '123',
          username: 'testuser',
          email: 'test@example.com',
          roles: [UserRole.TreasuryOfficer],
          exp: Math.floor(Date.now() / 1000) + 3600,
          iat: Math.floor(Date.now() / 1000),
        };
        const token = createMockToken(payload);
        const decoded = decodeJWT(token);

        expect(decoded).toEqual(payload);
      });

      it('should return null for invalid token format', () => {
        expect(decodeJWT('invalid-token')).toBe(null);
        expect(decodeJWT('invalid.token')).toBe(null);
        expect(decodeJWT('')).toBe(null);
      });

      it('should return null for malformed payload', () => {
        const token = 'header.invalid-base64.signature';
        expect(decodeJWT(token)).toBe(null);
      });
    });

    describe('isTokenExpired', () => {
      it('should return false for non-expired token', () => {
        const payload = {
          userId: '123',
          username: 'testuser',
          email: 'test@example.com',
          roles: [UserRole.TreasuryOfficer],
          exp: Math.floor(Date.now() / 1000) + 3600, // Expires in 1 hour
          iat: Math.floor(Date.now() / 1000),
        };
        const token = createMockToken(payload);
        expect(isTokenExpired(token)).toBe(false);
      });

      it('should return true for expired token', () => {
        const payload = {
          userId: '123',
          username: 'testuser',
          email: 'test@example.com',
          roles: [UserRole.TreasuryOfficer],
          exp: Math.floor(Date.now() / 1000) - 3600, // Expired 1 hour ago
          iat: Math.floor(Date.now() / 1000) - 7200,
        };
        const token = createMockToken(payload);
        expect(isTokenExpired(token)).toBe(true);
      });

      it('should return true for invalid token', () => {
        expect(isTokenExpired('invalid-token')).toBe(true);
      });
    });

    describe('getTokenExpirationTime', () => {
      it('should return expiration time in milliseconds', () => {
        const expSeconds = Math.floor(Date.now() / 1000) + 3600;
        const payload = {
          userId: '123',
          username: 'testuser',
          email: 'test@example.com',
          roles: [UserRole.TreasuryOfficer],
          exp: expSeconds,
          iat: Math.floor(Date.now() / 1000),
        };
        const token = createMockToken(payload);
        expect(getTokenExpirationTime(token)).toBe(expSeconds * 1000);
      });

      it('should return null for invalid token', () => {
        expect(getTokenExpirationTime('invalid-token')).toBe(null);
      });
    });

    describe('willTokenExpireSoon', () => {
      it('should return false when token expires in more than specified minutes', () => {
        const payload = {
          userId: '123',
          username: 'testuser',
          email: 'test@example.com',
          roles: [UserRole.TreasuryOfficer],
          exp: Math.floor(Date.now() / 1000) + 600, // Expires in 10 minutes
          iat: Math.floor(Date.now() / 1000),
        };
        const token = createMockToken(payload);
        expect(willTokenExpireSoon(token, 5)).toBe(false); // Check if expires within 5 minutes
      });

      it('should return true when token expires within specified minutes', () => {
        const payload = {
          userId: '123',
          username: 'testuser',
          email: 'test@example.com',
          roles: [UserRole.TreasuryOfficer],
          exp: Math.floor(Date.now() / 1000) + 180, // Expires in 3 minutes
          iat: Math.floor(Date.now() / 1000),
        };
        const token = createMockToken(payload);
        expect(willTokenExpireSoon(token, 5)).toBe(true); // Check if expires within 5 minutes
      });

      it('should return true for invalid token', () => {
        expect(willTokenExpireSoon('invalid-token', 5)).toBe(true);
      });
    });

    describe('getRolesFromToken', () => {
      it('should extract roles from valid token', () => {
        const roles = [UserRole.TreasuryOfficer, UserRole.ComplianceOfficer];
        const payload = {
          userId: '123',
          username: 'testuser',
          email: 'test@example.com',
          roles,
          exp: Math.floor(Date.now() / 1000) + 3600,
          iat: Math.floor(Date.now() / 1000),
        };
        const token = createMockToken(payload);
        expect(getRolesFromToken(token)).toEqual(roles);
      });

      it('should return empty array for invalid token', () => {
        expect(getRolesFromToken('invalid-token')).toEqual([]);
      });

      it('should return empty array when token has no roles', () => {
        const payload = {
          userId: '123',
          username: 'testuser',
          email: 'test@example.com',
          exp: Math.floor(Date.now() / 1000) + 3600,
          iat: Math.floor(Date.now() / 1000),
        };
        const token = createMockToken(payload);
        expect(getRolesFromToken(token)).toEqual([]);
      });
    });
  });

  describe('hasPublicSectorAccess', () => {
    it('should return true when user has at least one public sector role', () => {
      expect(hasPublicSectorAccess([UserRole.TreasuryOfficer])).toBe(true);
      expect(hasPublicSectorAccess([UserRole.CreditOfficer])).toBe(true);
      expect(hasPublicSectorAccess([UserRole.GovernmentFinanceOfficer])).toBe(true);
      expect(hasPublicSectorAccess([UserRole.CSRManager])).toBe(true);
      expect(hasPublicSectorAccess([UserRole.ComplianceOfficer])).toBe(true);
      expect(hasPublicSectorAccess([UserRole.SeniorManagement])).toBe(true);
      expect(hasPublicSectorAccess(['OTHER_ROLE', UserRole.TreasuryOfficer])).toBe(true);
    });

    it('should return false when user has no public sector roles', () => {
      expect(hasPublicSectorAccess(['OTHER_ROLE'])).toBe(false);
      expect(hasPublicSectorAccess(['ADMIN', 'USER'])).toBe(false);
      expect(hasPublicSectorAccess([])).toBe(false);
    });
  });

  describe('ROLE_PERMISSIONS matrix', () => {
    it('should have permissions defined for all user roles', () => {
      const allRoles = Object.values(UserRole);
      allRoles.forEach(role => {
        expect(ROLE_PERMISSIONS[role]).toBeDefined();
      });
    });

    it('should have all modules defined for each role', () => {
      const modules = ['securities', 'lending', 'banking', 'grants', 'dashboard'];
      const allRoles = Object.values(UserRole);
      
      allRoles.forEach(role => {
        modules.forEach(module => {
          expect(ROLE_PERMISSIONS[role][module as keyof typeof ROLE_PERMISSIONS[typeof role]]).toBeDefined();
        });
      });
    });

    it('should have read and write permissions defined for each module', () => {
      const modules = ['securities', 'lending', 'banking', 'grants', 'dashboard'];
      const allRoles = Object.values(UserRole);
      
      allRoles.forEach(role => {
        modules.forEach(module => {
          const modulePerms = ROLE_PERMISSIONS[role][module as keyof typeof ROLE_PERMISSIONS[typeof role]];
          expect(modulePerms).toHaveProperty('read');
          expect(modulePerms).toHaveProperty('write');
          expect(typeof modulePerms.read).toBe('boolean');
          expect(typeof modulePerms.write).toBe('boolean');
        });
      });
    });
  });
});
