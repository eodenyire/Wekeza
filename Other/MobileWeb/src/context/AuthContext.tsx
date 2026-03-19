import React, { createContext, useCallback, useContext, useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { authApi, type AuthResponse, type User, getErrorMessage } from '../services/api'

interface AuthContextValue {
  user: User | null
  isAuthenticated: boolean
  isLoading: boolean
  errorMessage: string | null
  login: (username: string, password: string) => Promise<boolean>
  logout: () => void
  clearError: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  // navigate is only available inside BrowserRouter so we defer access via ref
  const navigateRef = useRef<ReturnType<typeof useNavigate> | null>(null)

  // On mount: restore session from localStorage
  useEffect(() => {
    const stored = authApi.getStoredUser()
    if (stored && authApi.getStoredToken()) {
      setUser(toUser(stored))
    }
    setIsLoading(false)
  }, [])

  // Handle 401 unauthorized events dispatched by the Axios interceptor
  useEffect(() => {
    const handleUnauthorized = () => {
      setUser(null)
      navigateRef.current?.('/login', { replace: true })
    }
    window.addEventListener('wekeza:unauthorized', handleUnauthorized)
    return () => window.removeEventListener('wekeza:unauthorized', handleUnauthorized)
  }, [])

  const login = useCallback(async (username: string, password: string): Promise<boolean> => {
    setIsLoading(true)
    setErrorMessage(null)
    try {
      const response = await authApi.login(username, password)
      setUser(toUser(response))
      return true
    } catch (err) {
      setErrorMessage(getErrorMessage(err))
      return false
    } finally {
      setIsLoading(false)
    }
  }, [])

  const logout = useCallback(() => {
    authApi.logout()
    setUser(null)
  }, [])

  const clearError = useCallback(() => setErrorMessage(null), [])

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: user !== null,
        isLoading,
        errorMessage,
        login,
        logout,
        clearError,
      }}
    >
      <NavigateCapture navigateRef={navigateRef} />
      {children}
    </AuthContext.Provider>
  )
}

/** Invisible component that captures the navigate function from React Router context */
function NavigateCapture({
  navigateRef,
}: {
  navigateRef: React.MutableRefObject<ReturnType<typeof useNavigate> | null>
}) {
  const navigate = useNavigate()
  navigateRef.current = navigate
  return null
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}

function toUser(r: AuthResponse): User {
  return {
    id: r.userId ?? r.id ?? '',
    username: r.username,
    email: r.email,
    firstName: r.firstName,
    lastName: r.lastName,
    roles: r.roles ?? [],
  }
}
