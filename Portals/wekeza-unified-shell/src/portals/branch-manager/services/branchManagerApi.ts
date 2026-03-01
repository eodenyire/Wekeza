import { apiClient } from '@services/api';

export const branchManagerApi = {
  // Dashboard APIs
  async getDashboard() {
    const response = await apiClient.get('/api/branch-manager/dashboard');
    return response.data;
  },

  async getDailyTransactionSummary(date?: Date) {
    const params = date ? { date: date.toISOString().split('T')[0] } : {};
    const response = await apiClient.get('/api/branch-manager/transactions/daily', { params });
    return response.data;
  },

  async getTellerPerformance(date?: Date) {
    const params = date ? { date: date.toISOString().split('T')[0] } : {};
    const response = await apiClient.get('/api/branch-manager/tellers/performance', { params });
    return response.data;
  },

  // Team Management APIs
  async getStaffList(role?: string, status?: string) {
    const params: Record<string, string> = {};
    if (role) params.role = role;
    if (status) params.status = status;
    const response = await apiClient.get('/api/branch-manager/staff', { params });
    return response.data;
  },

  // Cash Management APIs
  async getCashPosition() {
    const response = await apiClient.get('/api/branch-manager/cash-position');
    return response.data;
  },

  async requestCashReplenishment(amount: number, reason?: string) {
    const response = await apiClient.post('/api/branch-manager/cash-replenishment/request', {
      amount,
      reason
    });
    return response.data;
  },

  // Compliance & Audit APIs
  async getComplianceStatus() {
    const response = await apiClient.get('/api/branch-manager/compliance');
    return response.data;
  },

  async getAuditTrail(startDate?: Date, endDate?: Date, action?: string, page: number = 1, pageSize: number = 20) {
    const params: Record<string, any> = { page, pageSize };
    if (startDate) params.startDate = startDate.toISOString();
    if (endDate) params.endDate = endDate.toISOString();
    if (action) params.action = action;
    const response = await apiClient.get('/api/branch-manager/audit-trail', { params });
    return response.data;
  },

  // Approvals APIs
  async getPendingRequests() {
    const response = await apiClient.get('/api/branch-manager/pending-requests');
    return response.data;
  },

  async approveRequest(requestId: string, reason: string) {
    const response = await apiClient.post(`/api/branch-manager/pending-requests/${requestId}/approve`, { reason });
    return response.data;
  },

  async rejectRequest(requestId: string, reason: string) {
    const response = await apiClient.post(`/api/branch-manager/pending-requests/${requestId}/reject`, { reason });
    return response.data;
  },
};
