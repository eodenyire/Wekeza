import React, { useState, useEffect } from 'react';
import { Card, Row, Col, Statistic, Table, Typography, Skeleton, Alert, Button, Space, Tag } from 'antd';
import {
  WalletOutlined,
  TransactionOutlined,
  UserOutlined,
  ClockCircleOutlined,
  ReloadOutlined,
} from '@ant-design/icons';

const { Title } = Typography;

interface DashboardData {
  drawerBalance: number;
  transactionsToday: number;
  customersServed: number;
  sessionDuration: string;
  tellerName: string;
  lastUpdated: string;
}

interface Transaction {
  Id: string;
  Reference: string;
  Type: string;
  AccountNumber: string;
  Amount: number;
  Timestamp: string;
  Description: string;
  Status: string;
}

const transactionColumns = [
  { title: 'Reference', dataIndex: 'Reference', key: 'Reference', width: '15%' },
  { title: 'Type', dataIndex: 'Type', key: 'Type', width: '15%' },
  { title: 'Account', dataIndex: 'AccountNumber', key: 'AccountNumber', width: '15%' },
  { 
    title: 'Amount', 
    dataIndex: 'Amount', 
    key: 'Amount', 
    width: '15%',
    render: (amount: number) => `KES ${amount.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
  },
  { 
    title: 'Time', 
    dataIndex: 'Timestamp', 
    key: 'Timestamp', 
    width: '15%',
    render: (timestamp: string) => new Date(timestamp).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })
  },
  {
    title: 'Status',
    dataIndex: 'Status',
    key: 'Status',
    width: '15%',
    render: (status: string) => {
      const colors: { [key: string]: string } = {
        'Success': 'green',
        'Pending': 'orange',
        'Failed': 'red'
      };
      return <Tag color={colors[status] || 'blue'}>{status}</Tag>;
    }
  },
];

export const TellerDashboard: React.FC = () => {
  const [dashboardData, setDashboardData] = useState<DashboardData | null>(null);
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch dashboard data and transactions
  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);
      
      // Fetch dashboard data
      const dashboardResponse = await fetch('/api/teller/dashboard', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        }
      });
      
      if (!dashboardResponse.ok) {
        throw new Error(`Failed to fetch dashboard data: ${dashboardResponse.statusText}`);
      }
      
      const dashboardResult = await dashboardResponse.json();
      setDashboardData(dashboardResult);
      
      // Fetch recent transactions
      const transactionsResponse = await fetch('/api/teller/transactions/recent', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        }
      });
      
      if (!transactionsResponse.ok) {
        throw new Error(`Failed to fetch transactions: ${transactionsResponse.statusText}`);
      }
      
      const transactionsResult = await transactionsResponse.json();
      setTransactions(transactionsResult.transactions || []);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred while fetching data');
      console.error('Error fetching dashboard data:', err);
    } finally {
      setLoading(false);
    }
  };

  // Fetch data on component mount
  useEffect(() => {
    fetchData();
    
    // Set up auto-refresh every 30 seconds
    const interval = setInterval(fetchData, 30000);
    return () => clearInterval(interval);
  }, []);

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24 }}>
        <Title level={3} style={{ margin: 0 }}>Teller Dashboard</Title>
        <Space>
          {dashboardData?.lastUpdated && (
            <span style={{ fontSize: '12px', color: '#888' }}>
              Last updated: {new Date(dashboardData.lastUpdated).toLocaleTimeString()}
            </span>
          )}
          <Button 
            type="primary" 
            icon={<ReloadOutlined />}
            size="small"
            onClick={fetchData}
            loading={loading}
          >
            Refresh
          </Button>
        </Space>
      </div>

      {error && (
        <Alert 
          message="Error" 
          description={error}
          type="error" 
          showIcon 
          closable 
          style={{ marginBottom: 16 }}
          onClose={() => setError(null)}
        />
      )}

      {loading && !dashboardData ? (
        <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
          {[1, 2, 3, 4].map(i => (
            <Col span={6} key={i}>
              <Card>
                <Skeleton active paragraph={false} />
              </Card>
            </Col>
          ))}
        </Row>
      ) : dashboardData ? (
        <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
          <Col span={6}>
            <Card>
              <Statistic 
                title="Drawer Balance" 
                value={dashboardData.drawerBalance} 
                prefix={<WalletOutlined />} 
                suffix="KES"
                valueStyle={{ color: '#1890ff' }}
                formatter={(value) => {
                  const num = typeof value === 'number' ? value : 0;
                  return num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic 
                title="Transactions Today" 
                value={dashboardData.transactionsToday} 
                prefix={<TransactionOutlined />}
                valueStyle={{ color: '#52c41a' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic 
                title="Customers Served" 
                value={dashboardData.customersServed} 
                prefix={<UserOutlined />}
                valueStyle={{ color: '#faad14' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic 
                title="Session Duration" 
                value={dashboardData.sessionDuration}
                prefix={<ClockCircleOutlined />}
                valueStyle={{ color: '#eb2f96' }}
              />
            </Card>
          </Col>
        </Row>
      ) : null}

      <Card title="Recent Transactions">
        <Table 
          columns={transactionColumns} 
          dataSource={transactions}
          pagination={false} 
          size="small"
          loading={loading}
          rowKey="Id"
          locale={{ emptyText: 'No transactions today' }}
        />
      </Card>
    </div>
  );
};
