import React from 'react';
import { Card, Input, List, Space, Button, Typography, Empty } from 'antd';
import { SearchOutlined } from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import { tellerApi } from '../services/tellerApi';

const { Text } = Typography;

export const CustomerServicesPanel: React.FC = () => {
  const [searchTerm, setSearchTerm] = React.useState('');

  const { data, isLoading, refetch } = useQuery({
    queryKey: ['teller-customers', searchTerm],
    queryFn: () => tellerApi.searchCustomers(searchTerm),
    enabled: false,
  });

  const customers = data?.customers || data?.data || [];

  return (
    <Space direction="vertical" style={{ width: '100%' }} size="large">
      <Card title="Customer Search & Services">
        <Space.Compact style={{ width: '100%', marginBottom: 16 }}>
          <Input
            placeholder="Search by name, account, ID number..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            onPressEnter={() => refetch()}
          />
          <Button type="primary" icon={<SearchOutlined />} onClick={() => refetch()} loading={isLoading}>
            Search
          </Button>
        </Space.Compact>

        {customers.length > 0 ? (
          <List
            dataSource={customers}
            renderItem={(customer: any) => (
              <List.Item
                actions={[
                  <Button key="view" type="link" size="small">View Accounts</Button>,
                  <Button key="onboard" type="link" size="small">Onboard</Button>,
                  <Button key="verify" type="link" size="small">Verify ID</Button>,
                ]}
              >
                <List.Item.Meta
                  title={customer.fullName || customer.name || 'Customer'}
                  description={
                    <Space direction="vertical" size={0}>
                      <Text type="secondary">Customer ID: {customer.customerId || customer.id || 'N/A'}</Text>
                      <Text type="secondary">Account: {customer.accountNumber || 'N/A'}</Text>
                    </Space>
                  }
                />
              </List.Item>
            )}
          />
        ) : (
          <Empty description="No customers found. Search to begin." />
        )}
      </Card>

      <Card title="Quick Actions">
        <Space wrap>
          <Button type="primary">Open Account</Button>
          <Button>Customer Onboarding</Button>
          <Button>Verify Customer Identity</Button>
          <Button>Print Statement</Button>
          <Button danger>Block Account</Button>
        </Space>
      </Card>
    </Space>
  );
};
