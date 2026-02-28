import { apiClient } from '@services/api';

export interface StartSessionRequest {
  tellerId: string;
  branchId: string;
  openingBalance: number;
}

export interface CashDepositRequest {
  accountNumber: string;
  amount: number;
  narration: string;
}

export interface CashWithdrawalRequest {
  accountNumber: string;
  amount: number;
  narration: string;
}

export const tellerApi = {
  async startSession(payload: StartSessionRequest) {
    const response = await apiClient.post('/api/teller/session/start', payload);
    return response.data;
  },

  async endSession(sessionId: string) {
    const response = await apiClient.post('/api/teller/session/end', { sessionId });
    return response.data;
  },

  async getCurrentSession() {
    const response = await apiClient.get('/api/teller/session/current');
    return response.data;
  },

  async getCashDrawerBalance() {
    const response = await apiClient.get('/api/teller/cash-drawer/balance');
    return response.data;
  },

  async processCashDeposit(payload: CashDepositRequest) {
    const response = await apiClient.post('/api/teller/cash-deposit', payload);
    return response.data;
  },

  async processCashWithdrawal(payload: CashWithdrawalRequest) {
    const response = await apiClient.post('/api/teller/cash-withdrawal', payload);
    return response.data;
  },

  async searchCustomers(query: string) {
    const response = await apiClient.get('/api/teller/search/customers', {
      params: { query },
    });
    return response.data;
  },

  async getAccountBalance(accountId: string) {
    const response = await apiClient.get(`/api/teller/account/${accountId}/balance`);
    return response.data;
  },

  async getTransactionHistory(params: {
    accountId?: string;
    page?: number;
    pageSize?: number;
  }) {
    const response = await apiClient.get('/api/teller/transactions/history', { params });
    return response.data;
  },
};
