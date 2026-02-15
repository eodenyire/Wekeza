import api from './api'

export interface LoginRequest {
  username: string
  password: string
  email?: string
}

export interface LoginResponse {
  token: string
  userId: string
  username: string
  roles: string[]
  expiresAt: string
}

export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>('/authentication/login', credentials)
    return response.data
  },

  getCurrentUser: async () => {
    const response = await api.get('/authentication/me')
    return response.data
  },
}
