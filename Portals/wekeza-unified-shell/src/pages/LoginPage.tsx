import React from 'react';
import { Form, Input, Button, Card, Typography, Alert } from 'antd';
import { UserOutlined, LockOutlined, BankOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '@store/authStore';
import { authService } from '@services/api';
import type { LoginCredentials } from '@app-types/index';

const { Title, Text } = Typography;

export const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const { setAuth, setLoading, isLoading } = useAuthStore();
  const [error, setError] = React.useState<string | null>(null);

  const onFinish = async (values: LoginCredentials) => {
    try {
      setError(null);
      setLoading(true);
      const response = await authService.login(values);
      setAuth(response);
      navigate('/dashboard');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Invalid credentials. Please try again.');
      setLoading(false);
    }
  };

  return (
    <div
      style={{
        minHeight: '100vh',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        background: 'linear-gradient(135deg, #1890ff 0%, #722ed1 100%)',
      }}
    >
      <Card style={{ width: 420, boxShadow: '0 8px 24px rgba(0,0,0,0.15)' }}>
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

        <Form name="login" initialValues={{ remember: true }} onFinish={onFinish} size="large">
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
    </div>
  );
};
