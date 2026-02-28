import React from 'react';
import { Card, Row, Col, Statistic, Table, Typography } from 'antd';
import {
  WalletOutlined,
  TransactionOutlined,
  UserOutlined,
  ClockCircleOutlined,
} from '@ant-design/icons';

const { Title } = Typography;

const transactionColumns = [
  { title: 'Reference', dataIndex: 'reference', key: 'reference' },
  { title: 'Type', dataIndex: 'type', key: 'type' },
  { title: 'Account', dataIndex: 'account', key: 'account' },
  { title: 'Amount', dataIndex: 'amount', key: 'amount' },
  { title: 'Time', dataIndex: 'time', key: 'time' },
];

const transactionData = [
  {
    key: '1',
    reference: 'TXN001',
    type: 'Cash Deposit',
    account: 'ACC001234',
    amount: 'KES 45,000',
    time: '10:23 AM',
  },
  {
    key: '2',
    reference: 'TXN002',
    type: 'Cash Withdrawal',
    account: 'ACC005678',
    amount: 'KES 20,000',
    time: '10:45 AM',
  },
];

export const TellerDashboard: React.FC = () => {
  return (
    <div>
      <Title level={3}>Teller Dashboard</Title>

      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col span={6}>
          <Card>
            <Statistic title="Drawer Balance" value={850000} prefix={<WalletOutlined />} suffix="KES" />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="Transactions Today" value={47} prefix={<TransactionOutlined />} />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="Customers Served" value={39} prefix={<UserOutlined />} />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="Session Duration" value="4h 23m" prefix={<ClockCircleOutlined />} />
          </Card>
        </Col>
      </Row>

      <Card title="Recent Transactions">
        <Table columns={transactionColumns} dataSource={transactionData} pagination={false} size="small" />
      </Card>
    </div>
  );
};
