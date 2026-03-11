import React, { useEffect, useState } from 'react';
import { Card, Row, Col, Statistic, Table, Progress, Badge, Skeleton, message } from 'antd';
import { 
  UserOutlined, 
  DollarOutlined, 
  RiseOutlined, 
  TeamOutlined,
  CheckCircleOutlined,
  ClockCircleOutlined,
  ExclamationOutlined
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { branchManagerApi } from '../services/branchManagerApi';

interface TellerPerformance {
  key: string;
  name: string;
  transactions: number;
  amount: string;
  efficiency: number;
  status: string;
}

const BranchDashboard: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [dashboardData, setDashboardData] = useState<any>(null);
  const [tellerPerformance, setTellerPerformance] = useState<TellerPerformance[]>([]);
  const [pendingApprovals, setPendingApprovals] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      setError(null);

      // Fetch dashboard metrics
      const dashboard = await branchManagerApi.getDashboard();
      setDashboardData(dashboard);

      // Fetch teller performance
      const performance = await branchManagerApi.getTellerPerformance();
      if (performance.data && Array.isArray(performance.data)) {
        const formattedPerformance: TellerPerformance[] = performance.data.map((teller: any, index: number) => ({
          key: String(index),
          name: teller.Name || 'Unknown',
          transactions: teller.Transactions || 0,
          amount: `KES ${(teller.Amount || 0).toLocaleString('en-US', { maximumFractionDigits: 0 })}`,
          efficiency: teller.Efficiency || 0,
          status: teller.Status || 'Active',
        }));
        setTellerPerformance(formattedPerformance);
      }

      // Fetch pending approvals
      try {
        const approvals = await branchManagerApi.getPendingRequests();
        setPendingApprovals(approvals.data || []);
      } catch (err) {
        // Pending requests might not exist yet, that's okay
        setPendingApprovals([]);
      }
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to load dashboard data';
      setError(errorMessage);
      message.error(errorMessage);
      console.error('Dashboard load error:', err);
    } finally {
      setLoading(false);
    }
  };

  const columns: ColumnsType<TellerPerformance> = [
    {
      title: 'Teller Name',
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: 'Transactions',
      dataIndex: 'transactions',
      key: 'transactions',
      sorter: (a, b) => a.transactions - b.transactions,
    },
    {
      title: 'Total Amount',
      dataIndex: 'amount',
      key: 'amount',
    },
    {
      title: 'Efficiency',
      dataIndex: 'efficiency',
      key: 'efficiency',
      render: (efficiency: number) => (
        <Progress 
          percent={efficiency} 
          size="small" 
          status={efficiency >= 90 ? 'success' : efficiency >= 80 ? 'normal' : 'exception'}
        />
      ),
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => (
        <Badge 
          status={status === 'Active' ? 'success' : 'warning'} 
          text={status} 
        />
      ),
    },
  ];

  if (loading) {
    return (
      <div>
        <Row gutter={[16, 16]}>
          {[1, 2, 3, 4].map(i => (
            <Col xs={24} sm={12} lg={6} key={i}>
              <Card>
                <Skeleton active />
              </Card>
            </Col>
          ))}
        </Row>
      </div>
    );
  }

  return (
    <div>
      {error && (
        <Card style={{ marginBottom: 16, borderColor: '#ff4d4f', backgroundColor: '#fff1f0' }}>
          <div style={{ color: '#ff4d4f' }}>
            <ExclamationOutlined /> {error}
          </div>
        </Card>
      )}

      <Row gutter={[16, 16]}>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Active Staff"
              value={dashboardData?.activeTellers ?? dashboardData?.ActiveTellers ?? 0}
              prefix={<TeamOutlined />}
              loading={loading}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Today's Transactions"
              value={dashboardData?.dailyTransactions ?? dashboardData?.DailyTransactions ?? 0}
              prefix={<UserOutlined />}
              valueStyle={{ color: '#3f8600' }}
              loading={loading}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Cash Position"
              value={dashboardData?.cashOnHand ?? dashboardData?.CashOnHand
                ? ((dashboardData?.cashOnHand ?? dashboardData?.CashOnHand) / 1000000).toFixed(1)
                : 0}
              prefix={<DollarOutlined />}
              suffix="M"
              loading={loading}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Branch Health"
              value={dashboardData?.branchHealth ?? dashboardData?.BranchHealth ?? 'Unknown'}
              prefix={<RiseOutlined />}
              valueStyle={{ color: '#3f8600', fontSize: '14px' }}
              loading={loading}
            />
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: '16px' }}>
        <Col xs={24} lg={16}>
          <Card title="Teller Performance Today" size="small">
            <Table 
              columns={columns} 
              dataSource={tellerPerformance} 
              pagination={false}
              size="small"
              loading={loading}
            />
          </Card>
        </Col>
        <Col xs={24} lg={8}>
          <Card title={`Pending Approvals (${pendingApprovals.length})`} size="small">
            {pendingApprovals.length > 0 ? (
              pendingApprovals.slice(0, 5).map((approval: any, index: number) => (
                <div key={index} style={{ marginBottom: '12px' }}>
                  <Badge status="processing" text={approval.Type || 'Pending Request'} />
                  <div style={{ fontSize: '12px', color: '#999', marginLeft: '22px' }}>
                    <ClockCircleOutlined /> {new Date(approval.SubmittedAt).toLocaleTimeString()}
                  </div>
                </div>
              ))
            ) : (
              <div style={{ textAlign: 'center', padding: '20px', color: '#999' }}>
                <CheckCircleOutlined style={{ fontSize: 24, marginRight: 8 }} />
                <p>No pending approvals</p>
              </div>
            )}
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default BranchDashboard;
