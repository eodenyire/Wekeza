import { Navigate, useLocation } from 'react-router-dom';
import { Spin } from 'antd';
import { useAuthStore } from '@store/authStore';
import { PORTAL_CONFIGS } from '@config/portals';
import type { UserRole, PortalType } from '@app-types/index';

interface ProtectedRouteProps {
  children: React.ReactNode;
  portalId?: PortalType;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, portalId }) => {
  const location = useLocation();
  const { isAuthenticated, user, isLoading } = useAuthStore();

  if (isLoading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <Spin size="large" tip="Loading..." />
      </div>
    );
  }

  if (!isAuthenticated || !user) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (portalId) {
    const portal = PORTAL_CONFIGS.find((p) => p.id === portalId);
    if (portal) {
      const hasAccess = user.roles.some((role: UserRole) => portal.allowedRoles.includes(role));
      if (!hasAccess) {
        return <Navigate to="/unauthorized" replace />;
      }
    }
  }

  return <>{children}</>;
};
