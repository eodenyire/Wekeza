import React, { useState, useEffect } from 'react';
import { Tabs, Card, Row, Col, Statistic, Table, Button, Spin, Alert, Tag } from 'antd';
import { ReloadOutlined, WalletOutlined, BankOutlined, TransactionOutlined } from '@ant-design/icons';

interface DashboardData {
  totalBalance: number;
  accountCount: number;
  recentTransactionCount: number;
  accounts: Array<{
    Id: string;
    AccountNumber: string;
    AccountType: string;
    Balance: number;
    Status: string;
    Currency: string;
  }>;
  customerId: string;
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

const CustomerPortalPage: React.FC = () => {
  const [dashboardData, setDashboardData] = useState<DashboardData | null>(null);
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem('authToken');
      const headers = { 'Authorization': `Bearer ${token}` };

      // Fetch dashboard data
      const dashboardResponse = await fetch('/api/customer-portal/dashboard', { headers });
      if (!dashboardResponse.ok) throw new Error('Failed to fetch dashboard data');
      const dashboardResult = await dashboardResponse.json();
      setDashboardData(dashboardResult);

      // Fetch recent transactions
      const transactionsResponse = await fetch('/api/customer-portal/transactions/recent?limit=10', { headers });
      if (!transactionsResponse.ok) throw new Error('Failed to fetch transactions');
      const transactionsResult = await transactionsResponse.json();
      setTransactions(transactionsResult.transactions || []);
    } catch (err: any) {
      setError(err.message || 'Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
    const interval = setInterval(fetchData, 30000); // Auto-refresh every 30 seconds
    return () => clearInterval(interval);
  }, []);

  if (loading && !dashboardData) {
    return (
      <div style={{ padding: '24px', textAlign: 'center' }}>
        <Spin size="large" tip="Loading your accounts..." />
      </div>
    );
  }

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <div>
          <h1>Customer Digital Portal</h1>
          <p style={{ marginBottom: 0 }}>Self-service accounts, transfers, cards, and support requests.</p>
        </div>
        <Button icon={<ReloadOutlined />} onClick={fetchData} loading={loading}>
          Refresh
        </Button>
      </div>

      {error && (
        <Alert
          message="Error"
          description={error}
          type="error"
          closable
          onClose={() => setError(null)}
          style={{ marginBottom: 16 }}
        />
      )}

      <Tabs
        items={[
          {
            key: '1',
            label: 'Accounts',
            children: dashboardData ? (
              <>
                <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic 
                        title="Total Balance" 
                        value={dashboardData.totalBalance} 
                        precision={2} 
                        prefix={<WalletOutlined />}
                        suffix="KES" 
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic 
                        title="Active Accounts" 
                        value={dashboardData.accountCount}
                        prefix={<BankOutlined />}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic 
                        title="Transactions (30 days)" 
                        value={dashboardData.recentTransactionCount}
                        prefix={<TransactionOutlined />}
                      />
                    </Card>
                  </Col>
                </Row>

                <Card title="Your Accounts">
                  {dashboardData.accounts.length > 0 ? (
                    <Table
                      dataSource={dashboardData.accounts}
                      pagination={false}
                      rowKey="Id"
                      columns={[
                        { 
                          title: 'Account Number', 
                          dataIndex: 'AccountNumber', 
                          key: 'AccountNumber' 
                        },
                        { 
                          title: 'Type', 
                          dataIndex: 'AccountType', 
                          key: 'AccountType' 
                        },
                        { 
                          title: 'Balance', 
                          dataIndex: 'Balance', 
                          key: 'Balance',
                          render: (balance: number, record: any) => 
                            `${record.Currency || 'KES'} ${balance.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
                        },
                        { 
                          title: 'Status', 
                          dataIndex: 'Status', 
                          key: 'Status',
                          render: (status: string) => (
                            <Tag color={status === 'Active' ? 'green' : 'red'}>{status}</Tag>
                          )
                        },
                      ]}
                    />
                  ) : (
                    <div style={{ textAlign: 'center', padding: '40px 0' }}>
                      <p>No accounts found</p>
                      <Button type="primary">Open New Account</Button>
                    </div>
                  )}
                </Card>
              </>
            ) : (
              <Spin />
            ),
          },
          {
            key: '2',
            label: 'Recent Activity',
            children: (
              <Card 
                title="Recent Transactions"
                extra={<small>Last updated: {dashboardData?.lastUpdated ? new Date(dashboardData.lastUpdated).toLocaleTimeString() : ''}</small>}
              >
                {transactions.length > 0 ? (
                  <Table
                    dataSource={transactions}
                    pagination={false}
                    rowKey="Id"
                    columns={[
                      { 
                        title: 'Date', 
                        dataIndex: 'Timestamp', 
                        key: 'Timestamp',
                        render: (date: string) => new Date(date).toLocaleDateString()
                      },
                      { 
                        title: 'Reference', 
                        dataIndex: 'Reference', 
                        key: 'Reference' 
                      },
                      { 
                        title: 'Narration', 
                        dataIndex: 'Description', 
                        key: 'Description',
                        render: (desc: string, record: Transaction) => desc || record.Type
                      },
                      { 
                        title: 'Account', 
                        dataIndex: 'AccountNumber', 
                        key: 'AccountNumber' 
                      },
                      { 
                        title: 'Amount', 
                        dataIndex: 'Amount', 
                        key: 'Amount',
                        render: (amount: number, record: Transaction) => {
                          const isCredit = record.Type.toLowerCase().includes('deposit') || 
                                         record.Type.toLowerCase().includes('credit');
                          return (
                            <span style={{ color: isCredit ? 'green' : 'red' }}>
                              {isCredit ? '+' : '-'}{amount.toLocaleString('en-US', { minimumFractionDigits: 2 })}
                            </span>
                          );
                        }
                      },
                      {
                        title: 'Status',
                        dataIndex: 'Status',
                        key: 'Status',
                        render: (status: string) => (
                          <Tag color={status === 'Success' ? 'green' : status === 'Pending' ? 'orange' : 'red'}>
                            {status}
                          </Tag>
                        )
                      }
                    ]}
                  />
                ) : (
                  <div style={{ textAlign: 'center', padding: '40px 0' }}>
                    <p>No recent transactions</p>
                  </div>
                )}
              </Card>
            ),
          },
          {
            key: '3',
            label: 'Actions',
            children: (
              <Card>
                <Button type="primary" style={{ marginRight: 8 }}>Transfer Funds</Button>
                <Button style={{ marginRight: 8 }}>Pay Bill</Button>
                <Button>Open Ticket</Button>
              </Card>
            ),
          },
        ]}
      />
    </div>
  );
};

export default CustomerPortalPage;
