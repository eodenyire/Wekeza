import React, { useState, useEffect } from 'react';
import { Tabs, Card, Col, Row, Statistic, Table, Spin, Alert, Button } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';

interface SystemStats {
  totalCustomers: number;
  totalAccounts: number;
  totalTransactions: number;
  activeUsers: number;
  totalBranches: number;
}

const ExecutivePortalPage: React.FC = () => {
  const [stats, setStats] = useState<SystemStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchStats = async () => {
    setLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem('authToken');
      const headers: HeadersInit = token ? { Authorization: `Bearer ${token}` } : {};
      const res = await fetch('/api/administrator/system/stats', { headers });
      if (!res.ok) throw new Error(`Failed to load system stats (${res.status})`);
      const data = await res.json();
      // API wraps in Result<T>; unwrap if needed
      setStats(data.value ?? data);
    } catch (err: any) {
      setError(err.message || 'Failed to load executive data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStats();
  }, []);

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <div>
          <h1>Executive &amp; Board Portal</h1>
          <p style={{ marginBottom: 0 }}>Strategic insights, enterprise KPIs, and decision support.</p>
        </div>
        <Button icon={<ReloadOutlined />} onClick={fetchStats} loading={loading}>
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
            label: 'Enterprise KPIs',
            children: loading && !stats ? (
              <Spin />
            ) : (
              <>
                {stats && (
                  <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
                    <Col xs={24} md={8}>
                      <Card>
                        <Statistic title="Total Customers" value={stats.totalCustomers} />
                      </Card>
                    </Col>
                    <Col xs={24} md={8}>
                      <Card>
                        <Statistic title="Total Accounts" value={stats.totalAccounts} />
                      </Card>
                    </Col>
                    <Col xs={24} md={8}>
                      <Card>
                        <Statistic title="Total Transactions" value={stats.totalTransactions} />
                      </Card>
                    </Col>
                    <Col xs={24} md={8}>
                      <Card>
                        <Statistic title="Active Users" value={stats.activeUsers} />
                      </Card>
                    </Col>
                    <Col xs={24} md={8}>
                      <Card>
                        <Statistic title="Branches" value={stats.totalBranches} />
                      </Card>
                    </Col>
                  </Row>
                )}
                <Row gutter={[16, 16]}>
                  <Col xs={24} md={8}><Card><Statistic title="ROE" value={18.4} suffix="%" /></Card></Col>
                  <Col xs={24} md={8}><Card><Statistic title="NPL Ratio" value={4.1} suffix="%" /></Card></Col>
                  <Col xs={24} md={8}><Card><Statistic title="Cost-to-Income" value={46.2} suffix="%" /></Card></Col>
                </Row>
              </>
            ),
          },
          {
            key: '2',
            label: 'Board Packs',
            children: <Card>Monthly board pack artifacts and approvals.</Card>,
          },
          {
            key: '3',
            label: 'Strategic Risks',
            children: (
              <Table
                pagination={false}
                dataSource={[
                  { key: '1', risk: 'Credit concentration', owner: 'CRO', status: 'Monitoring' },
                  { key: '2', risk: 'FX volatility', owner: 'Treasury', status: 'Mitigating' },
                ]}
                columns={[
                  { title: 'Risk', dataIndex: 'risk', key: 'risk' },
                  { title: 'Owner', dataIndex: 'owner', key: 'owner' },
                  { title: 'Status', dataIndex: 'status', key: 'status' },
                ]}
              />
            ),
          },
        ]}
      />
    </div>
  );
};

export default ExecutivePortalPage;
