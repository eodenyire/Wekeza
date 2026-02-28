import React from 'react';
import {
  Layout,
  Menu,
  Avatar,
  Dropdown,
  Space,
  Typography,
  Button,
  Badge,
} from 'antd';
import {
  UserOutlined,
  LogoutOutlined,
  BellOutlined,
  DashboardOutlined,
} from '@ant-design/icons';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuthStore } from '@store/authStore';
import { PORTAL_CONFIGS } from '@config/portals';
import type { UserRole } from '@app-types/index';

const { Header, Sider, Content } = Layout;
const { Text } = Typography;

interface AppLayoutProps {
  children: React.ReactNode;
}

export const AppLayout: React.FC<AppLayoutProps> = ({ children }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, clearAuth } = useAuthStore();

  const accessiblePortals = PORTAL_CONFIGS.filter((portal) =>
    user?.roles.some((role: UserRole) => portal.allowedRoles.includes(role))
  );

  const menuItems = [
    {
      key: '/dashboard',
      icon: <DashboardOutlined />,
      label: 'Dashboard',
      onClick: () => navigate('/dashboard'),
    },
    ...accessiblePortals.map((portal) => ({
      key: portal.route,
      label: portal.name,
      onClick: () => navigate(portal.route),
    })),
  ];

  const userMenuItems = [
    {
      key: 'profile',
      label: 'Profile',
      icon: <UserOutlined />,
    },
    {
      key: 'logout',
      label: 'Logout',
      icon: <LogoutOutlined />,
      onClick: () => {
        clearAuth();
        navigate('/login');
      },
    },
  ];

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider width={280} theme="dark">
        <div style={{ padding: '20px', textAlign: 'center', borderBottom: '1px solid #303030' }}>
          <Typography.Title level={4} style={{ color: 'white', margin: 0 }}>
            Wekeza Bank
          </Typography.Title>
          <Text style={{ color: '#8c8c8c', fontSize: '12px' }}>Unified Portal Shell</Text>
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[location.pathname]}
          items={menuItems}
          style={{ borderRight: 0, marginTop: 10 }}
        />
      </Sider>

      <Layout>
        <Header
          style={{
            padding: '0 24px',
            background: '#fff',
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            borderBottom: '1px solid #f0f0f0',
          }}
        >
          <div>
            <Text strong>Tier-1 Core Banking Platform</Text>
          </div>

          <Space size="large">
            <Badge count={5}>
              <Button type="text" icon={<BellOutlined />} />
            </Badge>

            <Dropdown menu={{ items: userMenuItems }} placement="bottomRight">
              <Space style={{ cursor: 'pointer' }}>
                <Avatar icon={<UserOutlined />} />
                <div>
                  <Text strong>{user?.fullName || user?.username}</Text>
                  <br />
                  <Text type="secondary" style={{ fontSize: '12px' }}>
                    {user?.roles?.join(', ')}
                  </Text>
                </div>
              </Space>
            </Dropdown>
          </Space>
        </Header>

        <Content style={{ margin: '24px', overflow: 'initial' }}>{children}</Content>
      </Layout>
    </Layout>
  );
};
