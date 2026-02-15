/**
 * API Client for Public Sector Portal
 * Centralized API communication with authentication
 */

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

interface ApiRequestOptions extends RequestInit {
  requiresAuth?: boolean;
}

class ApiClient {
  private baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  private getAuthHeaders(): HeadersInit {
    const token = localStorage.getItem('auth_token');
    return {
      'Content-Type': 'application/json',
      ...(token ? { 'Authorization': `Bearer ${token}` } : {})
    };
  }

  async request<T>(
    endpoint: string,
    options: ApiRequestOptions = {}
  ): Promise<T> {
    const { requiresAuth = true, ...fetchOptions } = options;

    const url = `${this.baseUrl}${endpoint}`;
    const headers = requiresAuth ? this.getAuthHeaders() : { 'Content-Type': 'application/json' };

    try {
      const response = await fetch(url, {
        ...fetchOptions,
        headers: {
          ...headers,
          ...fetchOptions.headers,
        },
      });

      if (!response.ok) {
        if (response.status === 401) {
          // Unauthorized - redirect to login
          localStorage.removeItem('auth_token');
          window.location.href = '/public-sector/login';
          throw new Error('Unauthorized');
        }
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('API request failed:', error);
      throw error;
    }
  }

  // GET request
  async get<T>(endpoint: string, options?: ApiRequestOptions): Promise<T> {
    return this.request<T>(endpoint, { ...options, method: 'GET' });
  }

  // POST request
  async post<T>(endpoint: string, data?: any, options?: ApiRequestOptions): Promise<T> {
    return this.request<T>(endpoint, {
      ...options,
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  // PUT request
  async put<T>(endpoint: string, data?: any, options?: ApiRequestOptions): Promise<T> {
    return this.request<T>(endpoint, {
      ...options,
      method: 'PUT',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  // DELETE request
  async delete<T>(endpoint: string, options?: ApiRequestOptions): Promise<T> {
    return this.request<T>(endpoint, { ...options, method: 'DELETE' });
  }
}

// Export singleton instance
export const apiClient = new ApiClient(API_BASE_URL);

// Export convenience methods
export const api = {
  // Dashboard
  getDashboardMetrics: () => apiClient.get('/public-sector/dashboard/metrics'),
  getRevenueTrends: () => apiClient.get('/public-sector/dashboard/revenue-trends'),
  getGrantTrends: () => apiClient.get('/public-sector/dashboard/grant-trends'),

  // Securities
  getTreasuryBills: () => apiClient.get('/public-sector/securities/treasury-bills'),
  getBonds: () => apiClient.get('/public-sector/securities/bonds'),
  placeSecurityOrder: (order: any) => apiClient.post('/public-sector/securities/orders', order),

  // Lending
  getLoanApplications: (status?: string) => 
    apiClient.get(`/public-sector/loans/applications${status ? `?status=${status}` : ''}`),

  // Banking
  getAccounts: () => apiClient.get('/public-sector/accounts'),

  // Grants
  getGrantPrograms: () => apiClient.get('/public-sector/grants/programs'),

  // Authentication
  login: (credentials: { username: string; password: string }) =>
    apiClient.post('/authentication/login', credentials, { requiresAuth: false }),
};

export default apiClient;
