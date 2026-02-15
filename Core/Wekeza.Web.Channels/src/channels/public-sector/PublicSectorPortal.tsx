import { Routes, Route, Navigate } from 'react-router-dom';
import { lazy, Suspense } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import Login from './Login';
import Layout from './Layout';
import { LoadingSpinner } from './components';
import { I18nProvider } from './I18nProvider';

// Lazy load modules for better performance
const Dashboard = lazy(() => import('./pages/Dashboard'));
const Securities = lazy(() => import('./pages/Securities'));
const Lending = lazy(() => import('./pages/Lending'));
const Banking = lazy(() => import('./pages/Banking'));
const Grants = lazy(() => import('./pages/Grants'));

export default function PublicSectorPortal() {
  const { isAuthenticated } = useAuth();

  return (
    <I18nProvider>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route
          path="/*"
          element={
            isAuthenticated ? (
              <Layout>
                <Suspense fallback={<LoadingSpinner />}>
                  <Routes>
                    <Route path="/" element={<Dashboard />} />
                    <Route path="/dashboard" element={<Dashboard />} />
                    <Route path="/securities/*" element={<Securities />} />
                    <Route path="/lending/*" element={<Lending />} />
                    <Route path="/banking/*" element={<Banking />} />
                    <Route path="/grants/*" element={<Grants />} />
                  </Routes>
                </Suspense>
              </Layout>
            ) : (
              <Navigate to="/public-sector/login" replace />
            )
          }
        />
      </Routes>
    </I18nProvider>
  );
}
