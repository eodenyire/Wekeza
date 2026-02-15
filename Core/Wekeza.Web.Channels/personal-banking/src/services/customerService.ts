import api from './api'

export const customerService = {
  // Profile
  getProfile: async () => {
    const response = await api.get('/customer-portal/profile')
    return response.data
  },

  updateProfile: async (data: any) => {
    const response = await api.put('/customer-portal/profile', data)
    return response.data
  },

  // Accounts
  getAccounts: async () => {
    const response = await api.get('/customer-portal/accounts')
    return response.data
  },

  getAccountBalance: async (accountId: string) => {
    const response = await api.get(`/customer-portal/accounts/${accountId}/balance`)
    return response.data
  },

  getAccountTransactions: async (accountId: string, pageSize = 20, pageNumber = 1) => {
    const response = await api.get(`/customer-portal/accounts/${accountId}/transactions`, {
      params: { pageSize, pageNumber }
    })
    return response.data
  },

  downloadStatement: async (accountId: string, fromDate: string, toDate: string) => {
    const response = await api.post(`/customer-portal/accounts/${accountId}/statement`, {
      fromDate,
      toDate,
      format: 'PDF'
    })
    return response.data
  },

  // Transfers
  transferFunds: async (data: {
    fromAccountId: string
    toAccountNumber: string
    amount: number
    currency: string
    narration: string
  }) => {
    const response = await api.post('/customer-portal/transactions/transfer', data)
    return response.data
  },

  // Payments
  payBill: async (data: {
    accountId: string
    billerCode: string
    accountNumber: string
    amount: number
    currency: string
  }) => {
    const response = await api.post('/customer-portal/transactions/pay-bill', data)
    return response.data
  },

  buyAirtime: async (data: {
    accountId: string
    phoneNumber: string
    amount: number
    provider: string
  }) => {
    const response = await api.post('/customer-portal/transactions/buy-airtime', data)
    return response.data
  },

  // Cards
  getCards: async () => {
    const response = await api.get('/customer-portal/cards')
    return response.data
  },

  requestCard: async (data: {
    accountId: string
    cardType: string
    deliveryAddress: string
  }) => {
    const response = await api.post('/customer-portal/cards/request', data)
    return response.data
  },

  requestVirtualCard: async (data: {
    accountId: string
    cardType: string
  }) => {
    const response = await api.post('/customer-portal/cards/request-virtual', data)
    return response.data
  },

  blockCard: async (cardId: string, reason: string) => {
    const response = await api.post('/customer-portal/cards/block', {
      cardId,
      reason,
      action: 'Block'
    })
    return response.data
  },

  // Loans
  getLoans: async () => {
    const response = await api.get('/customer-portal/loans')
    return response.data
  },

  applyForLoan: async (data: {
    productId: string
    amount: number
    termInMonths: number
    purpose: string
  }) => {
    const response = await api.post('/customer-portal/loans/apply', data)
    return response.data
  },

  repayLoan: async (data: {
    loanId: string
    amount: number
    accountId: string
  }) => {
    const response = await api.post('/customer-portal/loans/repay', data)
    return response.data
  },
}
