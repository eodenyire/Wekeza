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

export interface VifCustomerRegistrationRequest {
  firstName: string;
  lastName: string;
  identificationNumber: string;
  email?: string;
  phoneNumber?: string;
  riskRating?: number;
}

export interface VifAccountRegistrationRequest {
  cifNumber: string;
  accountType?: string;
  currency?: string;
  initialDeposit?: number;
  branchCode?: string;
}

export interface VifAmountRequest {
  accountNumber: string;
  amount: number;
  currency?: string;
  description?: string;
}

export interface VifTransferRequest {
  fromAccountNumber: string;
  toAccountNumber: string;
  amount: number;
  currency?: string;
}

export interface VifAirtimeRequest extends VifAmountRequest {
  phoneNumber: string;
  provider?: string;
}

export interface VifMpesaRequest extends VifAmountRequest {
  phoneNumber: string;
}

export interface VifChequeDepositRequest extends VifAmountRequest {
  chequeNumber: string;
  drawerBank: string;
}

export interface VifInvestmentRequest extends VifAmountRequest {
  instrumentCode: string;
  quantity: number;
}

export interface VifTreasuryPurchaseRequest extends VifInvestmentRequest {
  instrumentType?: 'TBill' | 'TBond';
  tenorDays?: number;
}

export interface VifServiceRequest {
  accountNumber: string;
  reason: string;
}

export const tellerApi = {
  async startSession(payload: StartSessionRequest) {
    const branchGuid =
      /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(payload.branchId)
        ? payload.branchId
        : '11111111-1111-1111-1111-111111111111';

    const response = await apiClient.post('/api/teller/session/start', {
      branchId: branchGuid,
      tellerCode: payload.tellerId,
      tellerName: payload.tellerId,
      branchCode: payload.branchId,
      openingCashBalance: payload.openingBalance,
    });
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
    const response = await apiClient.post('/api/vif/transactions/cash-deposit', {
      accountNumber: payload.accountNumber,
      amount: payload.amount,
      currency: 'KES',
      description: payload.narration,
    });
    return response.data;
  },

  async processCashWithdrawal(payload: CashWithdrawalRequest) {
    const response = await apiClient.post('/api/vif/transactions/cash-withdrawal', {
      accountNumber: payload.accountNumber,
      amount: payload.amount,
      currency: 'KES',
      description: payload.narration,
    });
    return response.data;
  },

  async registerVifCustomer(payload: VifCustomerRegistrationRequest) {
    const response = await apiClient.post('/api/vif/customers/register', payload);
    return response.data;
  },

  async registerVifAccount(payload: VifAccountRegistrationRequest) {
    const response = await apiClient.post('/api/vif/accounts/register', payload);
    return response.data;
  },

  async getVifBalance(accountNumber: string) {
    const response = await apiClient.get(`/api/vif/accounts/${accountNumber}/balance`);
    return response.data;
  },

  async getVifStatement(
    accountNumber: string,
    params?: { from?: string; to?: string; pageNumber?: number; pageSize?: number }
  ) {
    const response = await apiClient.get(`/api/vif/transactions/statement/${accountNumber}`, {
      params,
    });
    return response.data;
  },

  async vifTransfer(payload: VifTransferRequest) {
    const response = await apiClient.post('/api/vif/transactions/transfer', payload);
    return response.data;
  },

  async vifAirtime(payload: VifAirtimeRequest) {
    const response = await apiClient.post('/api/vif/transactions/airtime', payload);
    return response.data;
  },

  async vifMpesa(payload: VifMpesaRequest) {
    const response = await apiClient.post('/api/vif/transactions/mpesa', payload);
    return response.data;
  },

  async vifChequeDeposit(payload: VifChequeDepositRequest) {
    const response = await apiClient.post('/api/vif/transactions/cheque-deposit', payload);
    return response.data;
  },

  async vifBuyShares(payload: VifInvestmentRequest) {
    const response = await apiClient.post('/api/vif/investments/shares/buy', payload);
    return response.data;
  },

  async vifBuyTreasury(payload: VifTreasuryPurchaseRequest) {
    const response = await apiClient.post('/api/vif/investments/treasury/buy', payload);
    return response.data;
  },

  async vifLockAtmCard(payload: VifServiceRequest) {
    const response = await apiClient.post('/api/vif/services/atm-card/lock', payload);
    return response.data;
  },

  async vifBlockMobileLoan(payload: VifServiceRequest) {
    const response = await apiClient.post('/api/vif/services/mobile-loan/block', payload);
    return response.data;
  },

  async searchCustomers(query: string) {
    const response = await apiClient.get('/api/teller/customers/search', {
      params: { searchTerm: query },
    });
    return response.data;
  },

  async getAccountBalance(accountId: string) {
    const response = await apiClient.get(`/api/teller/accounts/${accountId}/balance`);
    return response.data;
  },

  async getTransactionHistory(params: {
    accountId?: string;
    page?: number;
    pageSize?: number;
  }) {
    if (params.accountId) {
      const response = await apiClient.get(`/api/teller/accounts/${params.accountId}/transactions`, {
        params: {
          pageNumber: params.page,
          pageSize: params.pageSize,
        },
      });
      return response.data;
    }

    const response = await apiClient.get('/api/teller/transactions/recent');
    return response.data;
  },
};
