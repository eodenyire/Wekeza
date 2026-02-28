import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ConfigProvider, theme } from 'antd';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ProtectedRoute } from '@components/auth/ProtectedRoute';
import { AppLayout } from '@components/layout/AppLayout';
import { LoginPage } from '@pages/LoginPage';
import { DashboardPage } from '@pages/DashboardPage';
import { UnauthorizedPage } from '@pages/UnauthorizedPage';
import AdminPortalPage from '@portals/admin/AdminPortalPage';
import BranchManagerPortalPage from '@portals/branch-manager/BranchManagerPortalPage';
import BranchOperationsPortalPage from '@portals/branch-operations/BranchOperationsPortalPage';
import CompliancePortalPage from '@portals/compliance/CompliancePortalPage';
import CustomerPortalPage from '@portals/customer/CustomerPortalPage';
import ExecutivePortalPage from '@portals/executive/ExecutivePortalPage';
import PaymentsPortalPage from '@portals/payments/PaymentsPortalPage';
import ProductGLPortalPage from '@portals/product-gl/ProductGLPortalPage';
import StaffPortalPage from '@portals/staff/StaffPortalPage';
import SupervisorPortalPage from '@portals/supervisor/SupervisorPortalPage';
import { TellerPortalPage } from '@portals/teller/TellerPortalPage';
import TradeFinancePortalPage from '@portals/trade-finance/TradeFinancePortalPage';
import TreasuryPortalPage from '@portals/treasury/TreasuryPortalPage';
import WorkflowPortalPage from '@portals/workflow/WorkflowPortalPage';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000,
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ConfigProvider
        theme={{
          algorithm: theme.defaultAlgorithm,
          token: {
            colorPrimary: '#1890ff',
            borderRadius: 6,
          },
        }}
      >
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/unauthorized" element={<UnauthorizedPage />} />

            <Route
              path="/*"
              element={
                <ProtectedRoute>
                  <AppLayout>
                    <Routes>
                      <Route path="/dashboard" element={<DashboardPage />} />

                      <Route
                        path="/portals/teller"
                        element={
                          <ProtectedRoute portalId="teller">
                            <TellerPortalPage />
                          </ProtectedRoute>
                        }
                      />

                      <Route
                        path="/portals/admin"
                        element={
                          <ProtectedRoute portalId="admin">
                            <AdminPortalPage />
                          </ProtectedRoute>
                        }
                      />
                      <Route
                        path="/portals/executive"
                        element={
                          <ProtectedRoute portalId="executive">
                            <ExecutivePortalPage />
                          </ProtectedRoute>
                        }
                      />
                      <Route
                        path="/portals/branch-manager"
                        element={
                          <ProtectedRoute portalId="branch-manager">
                            <BranchManagerPortalPage />
                          </ProtectedRoute>
                        }
                      />
                      <Route
                        path="/portals/branch-operations"
                        element={
                          <ProtectedRoute portalId="branch-operations">
                            <BranchOperationsPortalPage />
                          </ProtectedRoute>
                        }
                      />
                      <Route
                        path="/portals/supervisor"
                        element={
                          <ProtectedRoute portalId="supervisor">
                            <SupervisorPortalPage />
                          </ProtectedRoute>
                        }
                      />
                      <Route
                        path="/portals/compliance"
                        element={
                          <ProtectedRoute portalId="compliance">
                            <CompliancePortalPage />
                          </ProtectedRoute>
                        }
                      />
                      <Route
                        path="/portals/treasury"
                        element={
                          <ProtectedRoute portalId="treasury">
                            <TreasuryPortalPage />
                          </ProtectedRoute>
                        }
                      />
                      <Route
                        path="/portals/trade-finance"
                        element={
                          <ProtectedRoute portalId="trade-finance">
                            <TradeFinancePortalPage />
                          </ProtectedRoute>
                        }
                      />
                      <Route
                        path="/portals/product-gl"
                        element={
                          <ProtectedRoute portalId="product-gl">
                            <ProductGLPortalPage />
                          </ProtectedRoute>
                        }
                      />
                      <Route
                        path="/portals/payments"
                        element={
                          <ProtectedRoute portalId="payments">
                            <PaymentsPortalPage />
                          </ProtectedRoute>
                        }
                      />
                      <Route
                        path="/portals/customer"
                        element={
                          <ProtectedRoute portalId="customer">
                            <CustomerPortalPage />
                          </ProtectedRoute>
                        }
                      />
                      <Route
                        path="/portals/staff"
                        element={
                          <ProtectedRoute portalId="staff">
                            <StaffPortalPage />
                          </ProtectedRoute>
                        }
                      />
                      <Route
                        path="/portals/workflow"
                        element={
                          <ProtectedRoute portalId="workflow">
                            <WorkflowPortalPage />
                          </ProtectedRoute>
                        }
                      />

                      <Route path="/" element={<Navigate to="/dashboard" replace />} />
                      <Route path="*" element={<Navigate to="/dashboard" replace />} />
                    </Routes>
                  </AppLayout>
                </ProtectedRoute>
              }
            />
          </Routes>
        </BrowserRouter>
      </ConfigProvider>
    </QueryClientProvider>
  );
}

export default App;
