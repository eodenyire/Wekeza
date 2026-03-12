import React from 'react';
import { Card, Col, Row, Typography, Button, Space } from 'antd';
import {
  BankOutlined,
  SettingOutlined,
  DashboardOutlined,
  UserOutlined,
  SafetyOutlined,
  TeamOutlined,
  AlertOutlined,
  DollarOutlined,
  ContainerOutlined,
  AppstoreOutlined,
  SwapOutlined,
  MobileOutlined,
  IdcardOutlined,
  DeploymentUnitOutlined,
  LoginOutlined,
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { PORTAL_CONFIGS } from '@config/portals';

const { Title, Text } = Typography;

const ICON_MAP: Record<string, React.ReactNode> = {
  SettingOutlined:       <SettingOutlined />,
  DashboardOutlined:     <DashboardOutlined />,
  BankOutlined:          <BankOutlined />,
  SafetyOutlined:        <SafetyOutlined />,
  UserOutlined:          <UserOutlined />,
  TeamOutlined:          <TeamOutlined />,
  AlertOutlined:         <AlertOutlined />,
  DollarOutlined:        <DollarOutlined />,
  ContainerOutlined:     <ContainerOutlined />,
  AppstoreOutlined:      <AppstoreOutlined />,
  SwapOutlined:          <SwapOutlined />,
  MobileOutlined:        <MobileOutlined />,
  IdcardOutlined:        <IdcardOutlined />,
  DeploymentUnitOutlined:<DeploymentUnitOutlined />,
};

export const LandingPage: React.FC = () => {
  const navigate = useNavigate();

  const handlePortalAccess = (route: string) => {
    navigate('/login', { state: { from: { pathname: route } } });
  };

  return (
    <div
      style={{
        minHeight: '100vh',
        background: 'linear-gradient(135deg, #f0f4ff 0%, #e6f7ff 100%)',
      }}
    >
      {/* Header */}
      <div
        style={{
          background: 'linear-gradient(135deg, #1890ff 0%, #722ed1 100%)',
          padding: '40px 24px',
          textAlign: 'center',
          color: '#fff',
        }}
      >
        <BankOutlined style={{ fontSize: 56, color: '#fff', marginBottom: 16 }} />
        <Title level={1} style={{ color: '#fff', margin: 0 }}>
          Wekeza Bank
        </Title>
        <Text style={{ color: 'rgba(255,255,255,0.85)', fontSize: 18, display: 'block', marginTop: 8 }}>
          Tier-1 Core Banking Platform
        </Text>
        <Text style={{ color: 'rgba(255,255,255,0.7)', fontSize: 14, display: 'block', marginTop: 4 }}>
          Select a portal below to sign in, or{' '}
          <Button
            type="link"
            style={{ color: '#fff', fontWeight: 600, padding: 0, height: 'auto' }}
            onClick={() => navigate('/login')}
          >
            sign in to your dashboard
          </Button>
          .
        </Text>
      </div>

      {/* Portal Grid */}
      <div style={{ maxWidth: 1200, margin: '0 auto', padding: '40px 24px' }}>
        <Title level={3} style={{ textAlign: 'center', marginBottom: 32, color: '#333' }}>
          Available Portals
        </Title>

        <Row gutter={[24, 24]}>
          {PORTAL_CONFIGS.map((portal) => (
            <Col key={portal.id} xs={24} sm={12} md={8} lg={6}>
              <Card
                hoverable
                style={{
                  height: '100%',
                  borderTop: `4px solid ${portal.color}`,
                  display: 'flex',
                  flexDirection: 'column',
                }}
                bodyStyle={{ flex: 1, display: 'flex', flexDirection: 'column' }}
              >
                <Space direction="vertical" style={{ width: '100%', flex: 1 }}>
                  <div
                    style={{
                      fontSize: 32,
                      color: portal.color,
                      lineHeight: 1,
                    }}
                  >
                    {ICON_MAP[portal.icon] ?? <BankOutlined />}
                  </div>
                  <div>
                    <Text strong style={{ fontSize: 15 }}>
                      {portal.name}
                    </Text>
                    <br />
                    <Text type="secondary" style={{ fontSize: 12 }}>
                      {portal.description}
                    </Text>
                  </div>
                </Space>
                <Button
                  type="primary"
                  icon={<LoginOutlined />}
                  style={{ marginTop: 16, background: portal.color, borderColor: portal.color }}
                  block
                  onClick={() => handlePortalAccess(portal.route)}
                >
                  Access Portal
                </Button>
              </Card>
            </Col>
          ))}
        </Row>
      </div>

      {/* Footer */}
      <div
        style={{
          textAlign: 'center',
          padding: '24px',
          color: '#888',
          fontSize: 13,
          borderTop: '1px solid #e8e8e8',
        }}
      >
        © {new Date().getFullYear()} Wekeza Bank · Nairobi, Kenya ·{' '}
        <a href="mailto:support@wekeza.com" style={{ color: '#1890ff' }}>
          support@wekeza.com
        </a>
      </div>
    </div>
  );
};
