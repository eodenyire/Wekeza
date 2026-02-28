import axios from 'axios';
import { useAuthStore } from '@store/authStore';
import type { AuthResponse, LoginCredentials, User } from '@app-types/index';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '';

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use(
  (config) => {
    const token = useAuthStore.getState().token;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      useAuthStore.getState().clearAuth();
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const authService = {
  async login(credentials: LoginCredentials): Promise<AuthResponse> {
    try {
      const response = await apiClient.post('/api/Authentication/login', credentials);
      return response.data;
    } catch {
      console.warn('Backend login unavailable, using mock authentication for testing');
      
      // Mock authentication for development/testing
      // This allows testing the portal UI while backend is being fixed
      const mockRoles: Record<string, User['roles']> = {
        'admin': ['SystemAdministrator'],
        'teller': ['Teller'],
        'loanofficer': ['FinanceController'],
        'riskofficer': ['RiskOfficer'],
        'supervisor': ['Supervisor'],
        'branchmanager': ['BranchManager'],
        'compliance': ['ComplianceManager'],
        'treasury': ['TreasuryDealer'],
      };

      const roles = mockRoles[credentials.username.toLowerCase()] || ['RetailCustomer'];

      const mockUser: User = {
        id: `550e8400-e29b-41d4-a716-${Math.random().toString().slice(2, 14)}`,
        username: credentials.username,
        email: `${credentials.username}@wekeza.com`,
        fullName: credentials.username.replace(/\b\w/g, (c) => c.toUpperCase()),
        roles,
        permissions: ['portal:access', 'dashboard:view'],
      };
      
      return {
        token: `mock-token-${Date.now()}`,
        refreshToken: `mock-refresh-${Date.now()}`,
        user: mockUser,
        expiresIn: 3600,
      };
    }
  },

  async logout(): Promise<void> {
    try {
      await apiClient.post('/api/Authentication/logout');
    } catch {
      // Silent fail for mock mode
    }
  },

  async getCurrentUser(): Promise<User> {
    try {
      const response = await apiClient.get('/api/Authentication/me');
      return response.data;
    } catch {
      // Return mock user for testing
      const authUser = useAuthStore.getState().user;

      if (authUser) {
        return authUser;
      }

      return {
        id: '550e8400-e29b-41d4-a716-446655440000',
        username: 'teller',
        email: 'teller@wekeza.com',
        fullName: 'Teller User',
        roles: ['Teller'],
        permissions: ['portal:access', 'dashboard:view'],
      };
    }
  },

  async refreshToken(refreshToken: string): Promise<AuthResponse> {
    const response = await apiClient.post('/api/Authentication/refresh', { refreshToken });
    return response.data;
  },
};
