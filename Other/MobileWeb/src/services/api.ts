import axios, { AxiosInstance, AxiosError } from 'axios'

// ─── Configuration ────────────────────────────────────────────────────────────

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? (
  import.meta.env.PROD
    ? (() => { throw new Error('VITE_API_BASE_URL is not set') })()
    : 'http://localhost:5000/api'
)

// Storage keys
const TOKEN_KEY = 'wekeza_auth_token'
const USER_KEY = 'wekeza_user'

// ─── Axios instance ───────────────────────────────────────────────────────────

const api: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30_000,
  headers: {
    'Content-Type': 'application/json',
    Accept: 'application/json',
  },
})

// Attach JWT token to every request
api.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_KEY)
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Redirect to login is handled via React Router in the consuming components.
// Here we dispatch a custom event that AuthContext/ProtectedRoute can listen to.
api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      localStorage.removeItem(TOKEN_KEY)
      localStorage.removeItem(USER_KEY)
      window.dispatchEvent(new Event('wekeza:unauthorized'))
    }
    return Promise.reject(error)
  },
)

// ─── Helper to extract error message ─────────────────────────────────────────

export function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data
    if (typeof data === 'object' && data !== null) {
      return data.message ?? data.error ?? data.title ?? 'Request failed'
    }
    return error.message
  }
  if (error instanceof Error) return error.message
  return 'An unexpected error occurred'
}

// ─── Types ────────────────────────────────────────────────────────────────────

export interface User {
  id: string
  userId?: string
  username: string
  email: string
  firstName?: string
  lastName?: string
  phoneNumber?: string
  roles: string[]
}

export interface AuthResponse {
  token: string
  userId?: string
  id?: string
  username: string
  email: string
  firstName?: string
  lastName?: string
  roles: string[]
  expiresAt?: string
  refreshToken?: string
}

export interface Account {
  accountNumber: string
  accountType: string
  currency: string
  balance: number
  availableBalance: number
  status: string
  openedDate: string
}

export interface Transaction {
  transactionId: string
  accountNumber: string
  type: string
  amount: number
  currency: string
  description: string
  reference: string
  status: string
  transactionDate: string
  balance?: number
}

export interface TransferRequest {
  fromAccountNumber: string
  toAccountNumber: string
  amount: number
  currency?: string
  narration?: string
  reference?: string
}

export interface Loan {
  loanId: string
  accountNumber: string
  loanType: string
  principal: number
  outstandingBalance: number
  interestRate: number
  status: string
  startDate: string
  endDate: string
  nextPaymentDate?: string
  nextPaymentAmount?: number
}

export interface LoanApplication {
  accountNumber: string
  loanType: string
  amount: number
  tenureMonths: number
  purpose: string
}

// ─── Auth API ─────────────────────────────────────────────────────────────────

export const authApi = {
  login: async (username: string, password: string): Promise<AuthResponse> => {
    const { data } = await api.post<AuthResponse>('/authentication/login', {
      username,
      password,
    })
    // Persist token and user
    localStorage.setItem(TOKEN_KEY, data.token)
    localStorage.setItem(USER_KEY, JSON.stringify(data))
    return data
  },

  logout: () => {
    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(USER_KEY)
  },

  me: async (): Promise<User> => {
    const { data } = await api.get<User>('/authentication/me')
    return data
  },

  changePassword: async (currentPassword: string, newPassword: string) => {
    await api.post('/authentication/change-password', {
      currentPassword,
      newPassword,
    })
  },

  forgotPassword: async (email: string) => {
    await api.post('/authentication/forgot-password', { email })
  },

  getStoredToken: () => localStorage.getItem(TOKEN_KEY),

  getStoredUser: (): AuthResponse | null => {
    const raw = localStorage.getItem(USER_KEY)
    return raw ? JSON.parse(raw) : null
  },
}

// ─── Accounts API ─────────────────────────────────────────────────────────────

export const accountsApi = {
  getUserAccounts: async (): Promise<Account[]> => {
    const { data } = await api.get<Account[]>('/accounts/user/accounts')
    return data
  },

  getBalance: async (accountNumber: string): Promise<{ balance: number; availableBalance: number }> => {
    const { data } = await api.get(`/accounts/${accountNumber}/balance`)
    return data
  },

  getSummary: async (accountNumber: string): Promise<Account> => {
    const { data } = await api.get<Account>(`/accounts/${accountNumber}/summary`)
    return data
  },
}

// ─── Transactions API ─────────────────────────────────────────────────────────

export const transactionsApi = {
  getStatement: async (
    accountNumber: string,
    page = 1,
    pageSize = 20,
  ): Promise<{ transactions: Transaction[]; totalCount: number }> => {
    const { data } = await api.get(`/transactions/statement/${accountNumber}`, {
      params: { page, pageSize },
    })
    // Handle both array and paginated response shapes
    if (Array.isArray(data)) return { transactions: data, totalCount: data.length }
    return {
      transactions: data.transactions ?? data.data ?? [],
      totalCount: data.totalCount ?? data.total ?? 0,
    }
  },

  transfer: async (request: TransferRequest): Promise<{ transactionId: string; message: string }> => {
    const { data } = await api.post('/transactions/transfer', request)
    return data
  },

  mobileDeposit: async (accountNumber: string, amount: number, phoneNumber: string) => {
    const { data } = await api.post('/transactions/deposit/mobile', {
      accountNumber,
      amount,
      phoneNumber,
    })
    return data
  },
}

// ─── Loans API ────────────────────────────────────────────────────────────────

export const loansApi = {
  getUserLoans: async (): Promise<Loan[]> => {
    const { data } = await api.get<Loan[]>('/loans/user/loans')
    return data
  },

  applyForLoan: async (application: LoanApplication): Promise<{ loanId: string; message: string }> => {
    const { data } = await api.post('/loans/apply', application)
    return data
  },
}

// ─── Mobile Money API ─────────────────────────────────────────────────────────

export const mobileMoneyApi = {
  initiateMpesaDeposit: async (params: {
    phoneNumber: string
    amount: number
    accountNumber: string
    description?: string
  }) => {
    const { data } = await api.post('/digitalchannels/mpesa/stk-push', params)
    return data
  },

  sendToMobile: async (params: {
    fromAccountNumber: string
    toPhoneNumber: string
    amount: number
    provider: string
    narration?: string
  }) => {
    const { data } = await api.post('/digitalchannels/send', params)
    return data
  },

  purchaseAirtime: async (params: {
    fromAccountNumber: string
    phoneNumber: string
    amount: number
    provider: string
  }) => {
    const { data } = await api.post('/digitalchannels/airtime', params)
    return data
  },

  payBill: async (params: {
    fromAccountNumber: string
    billType: string
    billAccount: string
    amount: number
    narration?: string
  }) => {
    const { data } = await api.post('/payments/bill', params)
    return data
  },
}

export default api
