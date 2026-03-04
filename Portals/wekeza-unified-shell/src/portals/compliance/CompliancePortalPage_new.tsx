import React, { useState, useEffect } from 'react';
import { Tabs, Card, Table, Tag, Row, Col, Statistic, Spin, Alert, Button } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';

interface AMLAlert {
  CaseId: string;
  CustomerName: string;
  RiskLevel: string;
  Status: string;
  DateReported: string;
}

interface RiskMetrics {
  HighRiskCases: number;
  SanctionsHits: number;
  ComplianceScore: number;
  PendingReviews: number;
  RegulatoryAlerts: number;
  SystemHealth: number;
}

const CompliancePortalPage: React.FC = () => {
  const [alerts, setAlerts] = useState<AMLAlert[]>([]);
  const [metrics, setMetrics] = useState<RiskMetrics | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);

      const token = localStorage.getItem('authToken');
      const headers = { 'Authorization': `Bearer ${token}` };

      const [alertsRes, metricsRes] = await Promise.all([
        fetch('/api/compliance-portal/aml-alerts', { headers }),
        fetch('/api/compliance-portal/risk-metrics', { headers })
      ]);

      if (alertsRes.ok) {
        const alertsData = await alertsRes.json();
        setAlerts(alertsData.data || []);
      }

      if (metricsRes.ok) {
        const metricsData = await metricsRes.json();
        setMetrics(metricsData.data);
      }
    } catch (err) {
      setError('Failed to load compliance data');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
    const interval = setInterval(fetchData, 30000);
    return () => clearInterval(interval);
  }, []);

  const columns = [
    { title: 'Case ID', dataIndex: 'CaseId', key: 'CaseId' },
    { title: 'Customer', dataIndex: 'CustomerName', key: 'CustomerName' },
    { 
      title: 'Risk Level', 
      dataIndex: 'RiskLevel', 
      key: 'RiskLevel',
      render: (risk: string) => <Tag color={risk === 'High' ? 'red' : 'orange'}>{risk}</Tag>
    },
    { 
      title: 'Status', 
      dataIndex: 'Status', 
      key: 'Status',
      render: (status: string) => <Tag color={status === 'Open' ? 'blue' : 'green'}>{status}</Tag>
    }
  ];

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
        <h1>Compliance & Risk Portal</h1>
        <Button icon={<ReloadOutlined />} onClick={fetchData} loading={loading}>
          Refresh
        </Button>
      </div>

      {error && <Alert message={error} type="error" showIcon style={{ marginBottom: '16px' }} closable />}
      
      {loading && <Spin size="large" />}
      
      {!loading && (
        <Tabs
          items={[
            {
              key: '1',
              label: 'AML Alerts',
              children: (
                <Table
                  columns={columns}
                  dataSource={alerts}
                  rowKey="CaseId"
                  pagination={false}
                  loading={loading}
                />
              ),
            },
            {
              key: '2',
              label: 'Risk Metrics',
              children: metrics ? (
                <Row gutter={[16, 16]}>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic 
                        title="High Risk Cases" 
                        value={metrics.HighRiskCases}
                        valueStyle={{ color: 'red' }}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic 
                        title="Compliance Score" 
                        value={metrics.ComplianceScore}
                        suffix="%"
                        valueStyle={{ color: metrics.ComplianceScore >= 95 ? 'green' : 'orange' }}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic 
                        title="System Health" 
                        value={metrics.SystemHealth}
                        suffix="%"
                        valueStyle={{ color: 'green' }}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic title="Sanctions Hits" value={metrics.SanctionsHits} />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic title="Pending Reviews" value={metrics.PendingReviews} />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic title="Regulatory Alerts" value={metrics.RegulatoryAlerts} />
                    </Card>
                  </Col>
                </Row>
              ) : null,
            },
            {
              key: '3',
              label: 'Regulatory Reporting',
              children: <Card>Generate and submit scheduled compliance returns.</Card>,
            },
          ]}
        />
      )}
    </div>
  );
};

export default CompliancePortalPage;
