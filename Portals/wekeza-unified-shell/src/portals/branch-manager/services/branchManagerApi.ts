import { apiClient } from '@services/api';

export const branchManagerApi = {
  // Dashboard APIs
  async getBranchMetrics() {
    const response = await apiClient.get('/api/branch/metrics');
    return response.data;
  },

  async getTellerPerformance() {
    const response = await apiClient.get('/api/branch/teller-performance');
    return response.data;
  },

  // Team Management APIs
  async getStaffList() {
    const response = await apiClient.get('/api/branch/staff');
    return response.data;
  },

  async updateStaffMember(staffId: string, data: any) {
    const response = await apiClient.put(`/api/branch/staff/${staffId}`, data);
    return response.data;
  },

  // Cash Management APIs
  async getCashPosition() {
    const response = await apiClient.get('/api/branch/cash-position');
    return response.data;
  },

  async requestCIT(amount: number) {
    const response = await apiClient.post('/api/branch/request-cit', { amount });
    return response.data;
  },

  // Transaction Monitoring APIs
  async getTransactions(filters?: any) {
    const response = await apiClient.get('/api/branch/transactions', { params: filters });
    return response.data;
  },

  // Reports APIs
  async getPerformanceReport(period: string) {
    const response = await apiClient.get(`/api/branch/reports/performance`, { params: { period } });
    return response.data;
  },

  // Settings APIs
  async getBranchSettings() {
    const response = await apiClient.get('/api/branch/settings');
    return response.data;
  },

  async updateBranchSettings(settings: any) {
    const response = await apiClient.put('/api/branch/settings', settings);
    return response.data;
  },
};
