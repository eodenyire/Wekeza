import React from 'react';
import { Form, Input, Button, Card, Typography, Alert, Collapse, Table, Tag } from 'antd';
import { UserOutlined, LockOutlined, BankOutlined, InfoCircleOutlined, ArrowLeftOutlined } from '@ant-design/icons';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { useAuthStore } from '@store/authStore';
import { authService } from '@services/api';
import type { LoginCredentials } from '@app-types/index';

const { Title, Text } = Typography;

const PORTAL_CREDENTIALS = [
  { username: 'admin',         password: 'Admin@123',      portal: 'Admin / Executive',           role: 'SystemAdministrator' },
  { username: 'manager1',      password: 'Manager@123',    portal: 'Branch Manager / Supervisor',  role: 'BranchManager' },
  { username: 'teller1',       password: 'Teller@123',     portal: 'Teller / Staff Self-Service',  role: 'Teller' },
  { username: 'supervisor1',   password: 'Supervisor@123', portal: 'Supervisor / Workflow',        role: 'Supervisor' },
  { username: 'compliance1',   password: 'Compliance@123', portal: 'Compliance & Risk',            role: 'ComplianceOfficer' },
  { username: 'treasury1',     password: 'Treasury@123',   portal: 'Treasury & Markets',           role: 'TreasuryDealer' },
  { username: 'tradeFinance1', password: 'Trade@123',      portal: 'Trade Finance',                role: 'TradeFinanceOfficer' },
  { username: 'payments1',     password: 'Payments@123',   portal: 'Payments & Clearing',          role: 'PaymentsOfficer' },
  { username: 'productGL1',    password: 'Product@123',    portal: 'Product & GL',                 role: 'ProductManager' },
  { username: 'customer1',     password: 'Customer@123',   portal: 'Customer Digital',             role: 'RetailCustomer' },
  { username: 'vaultOfficer1', password: 'Vault@123',      portal: 'Branch Operations',            role: 'VaultOfficer' },
  { username: 'executive1',    password: 'Executive@123',  portal: 'Executive & Board',            role: 'CEO' },
];

const credentialColumns = [
  { title: 'Username',  dataIndex: 'username',  key: 'username',  render: (v: string) => <Text code>{v}</Text> },
  { title: 'Password',  dataIndex: 'password',  key: 'password',  render: (v: string) => <Text code>{v}</Text> },
  { title: 'Portal(s)', dataIndex: 'portal',    key: 'portal' },
  { title: 'Role',      dataIndex: 'role',      key: 'role',      render: (v: string) => <Tag color="blue">{v}</Tag> },
];

export const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { setAuth, setLoading, isLoading } = useAuthStore();
  const [error, setError] = React.useState<string | null>(null);
  const [form] = Form.useForm<LoginCredentials>();

  // Destination to redirect to after a successful login.
  // ProtectedRoute stores the originally requested path in location.state.from.
  // The LandingPage passes the same value directly.
  const from = (location.state as { from?: { pathname: string } } | null)?.from?.pathname || '/dashboard';

  const onFinish = async (values: LoginCredentials) => {
    try {
      setError(null);
      setLoading(true);
      const response = await authService.login(values);
      setAuth(response);
      navigate(from, { replace: true });
    } catch (err: any) {
      setError(err.response?.data?.message || 'Invalid credentials. Please try again.');
      setLoading(false);
    }
  };

  const fillCredentials = (username: string, password: string) => {
    form.setFieldsValue({ username, password });
  };

  return (
    <div
      style={{
        minHeight: '100vh',
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        background: 'linear-gradient(135deg, #1890ff 0%, #722ed1 100%)',
        padding: '24px',
      }}
    >
      <Card style={{ width: 420, boxShadow: '0 8px 24px rgba(0,0,0,0.15)', marginBottom: 16 }}>
        <div style={{ marginBottom: 8 }}>
          <Link to="/">
            <Button type="link" icon={<ArrowLeftOutlined />} style={{ padding: 0 }}>
              All Portals
            </Button>
          </Link>
        </div>
        <div style={{ textAlign: 'center', marginBottom: 32 }}>
          <BankOutlined style={{ fontSize: 48, color: '#1890ff' }} />
          <Title level={2} style={{ margin: '16px 0 8px 0' }}>
            Wekeza Bank
          </Title>
          <Text type="secondary">Tier-1 Core Banking Portal</Text>
        </div>

        {error && (
          <Alert
            message={error}
            type="error"
            showIcon
            style={{ marginBottom: 16 }}
            closable
            onClose={() => setError(null)}
          />
        )}

        <Form form={form} name="login" initialValues={{ remember: true }} onFinish={onFinish} size="large">
          <Form.Item
            name="username"
            rules={[{ required: true, message: 'Please input your username' }]}
          >
            <Input prefix={<UserOutlined />} placeholder="Username" />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[{ required: true, message: 'Please input your password' }]}
          >
            <Input.Password prefix={<LockOutlined />} placeholder="Password" />
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={isLoading} block>
              Sign In
            </Button>
          </Form.Item>
        </Form>

        <div style={{ textAlign: 'center', marginTop: 16 }}>
          <Text type="secondary" style={{ fontSize: 12 }}>
            Connected to Core Banking APIs at APIs/v1-Core
          </Text>
        </div>
      </Card>

      {/* Test Credentials Panel */}
      <div style={{ width: '100%', maxWidth: 960 }}>
        <Collapse
          style={{ background: 'rgba(255,255,255,0.95)', borderRadius: 8, boxShadow: '0 4px 16px rgba(0,0,0,0.12)' }}
          items={[
            {
              key: '1',
              label: (
                <span>
                  <InfoCircleOutlined style={{ marginRight: 8, color: '#1890ff' }} />
                  <Text strong>Portal Test Credentials</Text>
                  <Text type="secondary" style={{ marginLeft: 8, fontSize: 12 }}>
                    (click to expand — for development &amp; UAT use)
                  </Text>
                </span>
              ),
              children: (
                <>
                  <Alert
                    message="UAT / Development Credentials"
                    description="These credentials are for testing only. Each user provides access to specific portals based on their role. Click any row to pre-fill the login form."
                    type="info"
                    showIcon
                    style={{ marginBottom: 16 }}
                  />
                  <Table
                    dataSource={PORTAL_CREDENTIALS}
                    columns={credentialColumns}
                    rowKey="username"
                    size="small"
                    pagination={false}
                    onRow={(record) => ({
                      onClick: () => fillCredentials(record.username, record.password),
                      style: { cursor: 'pointer' },
                    })}
                  />
                </>
              ),
            },
          ]}
        />
      </div>
    </div>
  );
};
