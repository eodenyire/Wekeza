import React from 'react';
import { Tabs, Card, Table, Tag, Statistic, Row, Col } from 'antd';

const PaymentsPortalPage: React.FC = () => {
  return (
    <div style={{ padding: '24px' }}>
      <h1>Payments & Clearing Portal</h1>
      <p style={{ marginBottom: '24px' }}>Process RTGS, ACH, mobile, and card-clearing operations.</p>
      <Tabs
        items={[
          {
            key: '1',
            label: 'Processing Queue',
            children: (
              <Table
                pagination={false}
                dataSource={[
                  { key: '1', ref: 'PAY-1001', channel: 'RTGS', amount: 'KES 4,500,000', status: 'Queued' },
                  { key: '2', ref: 'PAY-1002', channel: 'ACH', amount: 'KES 320,000', status: 'Processing' },
                ]}
                columns={[
                  { title: 'Reference', dataIndex: 'ref', key: 'ref' },
                  { title: 'Channel', dataIndex: 'channel', key: 'channel' },
                  { title: 'Amount', dataIndex: 'amount', key: 'amount' },
                  { title: 'Status', dataIndex: 'status', key: 'status', render: (v: string) => <Tag color={v === 'Queued' ? 'orange' : 'blue'}>{v}</Tag> },
                ]}
              />
            ),
          },
          {
            key: '2',
            label: 'Clearing Metrics',
            children: (
              <Row gutter={[16, 16]}>
                <Col xs={24} md={8}><Card><Statistic title="STP Rate" value={96.3} suffix="%" /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="Failed Payments" value={11} /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="Average Turnaround" value={12} suffix="min" /></Card></Col>
              </Row>
            ),
          },
          {
            key: '3',
            label: 'Returns & Reversals',
            children: <Card>Manage payment returns, disputes, and reversals.</Card>,
          },
        ]}
      />
    </div>
  );
};

export default PaymentsPortalPage;
