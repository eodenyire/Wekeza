import React from 'react';
import { Card, Row, Col, Statistic, Table, Progress, Badge } from 'antd';
import { 
  UserOutlined, 
  DollarOutlined, 
  RiseOutlined, 
  TeamOutlined,
  CheckCircleOutlined,
  ClockCircleOutlined
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';

interface TellerPerformance {
  key: string;
  name: string;
  transactions: number;
  amount: string;
  efficiency: number;
  status: string;
}

const BranchDashboard: React.FC = () => {
  const performanceData: TellerPerformance[] = [
    {
      key: '1',
      name: 'John Teller',
      transactions: 47,
      amount: 'KES 2.4M',
      efficiency: 95,
      status: 'Active',
    },
    {
      key: '2',
      name: 'Mary Cashier',
      transactions: 39,
      amount: 'KES 1.8M',
      efficiency: 88,
      status: 'Active',
    },
    {
      key: '3',
      name: 'Peter Frontline',
      transactions: 52,
      amount: 'KES 3.1M',
      efficiency: 92,
      status: 'Break',
    },
    {
      key: '4',
      name: 'Sarah Operations',
      transactions: 41,
      amount: 'KES 2.2M',
      efficiency: 90,
      status: 'Active',
    },
  ];

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
          status={efficiency >= 90 ? 'success' : 'normal'}
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

  return (
    <div>
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Active Staff"
              value={8}
              prefix={<TeamOutlined />}
              suffix="/ 10"
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Today's Transactions"
              value={179}
              prefix={<UserOutlined />}
              valueStyle={{ color: '#3f8600' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Branch Cash Position"
              value="KES 9.5M"
              prefix={<DollarOutlined />}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Branch Performance"
              value={93}
              prefix={<RiseOutlined />}
              suffix="%"
              valueStyle={{ color: '#3f8600' }}
            />
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: '16px' }}>
        <Col xs={24} lg={16}>
          <Card title="Teller Performance Today" size="small">
            <Table 
              columns={columns} 
              dataSource={performanceData} 
              pagination={false}
              size="small"
            />
          </Card>
        </Col>
        <Col xs={24} lg={8}>
          <Card title="Pending Approvals" size="small">
            <div style={{ marginBottom: '12px' }}>
              <Badge status="processing" text="Large Cash Withdrawal - KES 1.2M" />
              <div style={{ fontSize: '12px', color: '#999', marginLeft: '22px' }}>
                <ClockCircleOutlined /> 5 min ago
              </div>
            </div>
            <div style={{ marginBottom: '12px' }}>
              <Badge status="processing" text="Account Opening - Corporate" />
              <div style={{ fontSize: '12px', color: '#999', marginLeft: '22px' }}>
                <ClockCircleOutlined /> 12 min ago
              </div>
            </div>
            <div style={{ marginBottom: '12px' }}>
              <Badge status="success" text="Loan Disbursement - KES 500K" />
              <div style={{ fontSize: '12px', color: '#999', marginLeft: '22px' }}>
                <CheckCircleOutlined /> Approved
              </div>
            </div>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default BranchDashboard;
