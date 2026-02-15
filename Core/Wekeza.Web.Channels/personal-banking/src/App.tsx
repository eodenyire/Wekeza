import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuthStore } from './stores/authStore'
import LoginPage from './pages/LoginPage'
import DashboardPage from './pages/DashboardPage'
import AccountsPage from './pages/AccountsPage'
import TransfersPage from './pages/TransfersPage'
import PaymentsPage from './pages/PaymentsPage'
import LoansPage from './pages/LoansPage'
import CardsPage from './pages/CardsPage'
import ProfilePage from './pages/ProfilePage'
import Layout from './components/Layout'

function App() {
  const { isAuthenticated } = useAuthStore()

  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      
      {isAuthenticated ? (
        <Route path="/" element={<Layout />}>
          <Route index element={<DashboardPage />} />
          <Route path="accounts" element={<AccountsPage />} />
          <Route path="transfers" element={<TransfersPage />} />
          <Route path="payments" element={<PaymentsPage />} />
          <Route path="loans" element={<LoansPage />} />
          <Route path="cards" element={<CardsPage />} />
          <Route path="profile" element={<ProfilePage />} />
        </Route>
      ) : (
        <Route path="*" element={<Navigate to="/login" replace />} />
      )}
    </Routes>
  )
}

export default App
