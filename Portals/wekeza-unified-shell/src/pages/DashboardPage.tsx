import React from 'react';
import { Card, Row, Col, Statistic, Typography, List, Tag } from 'antd';
import {
  BankOutlined,
  UserOutlined,
  TransactionOutlined,
  WarningOutlined,
} from '@ant-design/icons';
import { useAuthStore } from '@store/authStore';
import { PORTAL_CONFIGS } from '@config/portals';
import type { UserRole } from '@app-types/index';

const { Title, Text } = Typography;

export const DashboardPage: React.FC = () => {
  const { user } = useAuthStore();

  const accessiblePortals = PORTAL_CONFIGS.filter((portal) =>
    user?.roles.some((role: UserRole) => portal.allowedRoles.includes(role))
  );

  return (
    <div>
      <div style={{ marginBottom: 24 }}>
        <Title level={2}>Welcome, {user?.fullName || user?.username}</Title>
        <Text type="secondary">Unified Banking Portal Dashboard</Text>
      </div>

      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col span={6}>
          <Card>
            <Statistic
              title="Accessible Portals"
              value={accessiblePortals.length}
              prefix={<BankOutlined />}
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="Active Sessions" value={1} prefix={<UserOutlined />} />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="Today's Transactions" value={1247} prefix={<TransactionOutlined />} />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="Pending Alerts" value={5} prefix={<WarningOutlined />} />
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]}>
        <Col span={12}>
          <Card title="Your Roles">
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: 8 }}>
              {user?.roles.map((role) => (
                <Tag key={role} color="blue">
                  {role}
                </Tag>
              ))}
            </div>
          </Card>
        </Col>

        <Col span={12}>
          <Card title="Available Portals">
            <List
              size="small"
              dataSource={accessiblePortals}
              renderItem={(portal) => (
                <List.Item>
                  <div>
                    <Text strong>{portal.name}</Text>
                    <br />
                    <Text type="secondary" style={{ fontSize: 12 }}>
                      {portal.description}
                    </Text>
                  </div>
                </List.Item>
              )}
            />
          </Card>
        </Col>
      </Row>
    </div>
  );
};
