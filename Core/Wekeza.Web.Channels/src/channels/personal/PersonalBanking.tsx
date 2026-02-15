import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext';
import Login from './Login';
import Dashboard from './pages/Dashboard';
import Accounts from './pages/Accounts';
import Transfer from './pages/Transfer';
import Payments from './pages/Payments';
import Cards from './pages/Cards';
import Loans from './pages/Loans';
import Profile from './pages/Profile';
import Layout from './Layout';

export default function PersonalBanking() {
  const { isAuthenticated } = useAuth();

  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route
        path="/*"
        element={
          isAuthenticated ? (
            <Layout>
              <Routes>
                <Route path="/" element={<Dashboard />} />
                <Route path="/dashboard" element={<Dashboard />} />
                <Route path="/accounts" element={<Accounts />} />
                <Route path="/transfer" element={<Transfer />} />
                <Route path="/payments" element={<Payments />} />
                <Route path="/cards" element={<Cards />} />
                <Route path="/loans" element={<Loans />} />
                <Route path="/profile" element={<Profile />} />
              </Routes>
            </Layout>
          ) : (
            <Navigate to="/personal/login" replace />
          )
        }
      />
    </Routes>
  );
}
