import React from 'react';
import { Tabs, Card, Row, Col, Statistic, Table, Button } from 'antd';

const CustomerPortalPage: React.FC = () => {
  return (
    <div style={{ padding: '24px' }}>
      <h1>Customer Digital Portal</h1>
      <p style={{ marginBottom: '24px' }}>Self-service accounts, transfers, cards, and support requests.</p>
      <Tabs
        items={[
          {
            key: '1',
            label: 'Accounts',
            children: (
              <Row gutter={[16, 16]}>
                <Col xs={24} md={8}><Card><Statistic title="Main Account" value={254320.75} precision={2} prefix="KES " /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="Savings" value={1200500.0} precision={2} prefix="KES " /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="Loan Balance" value={-425000.0} precision={2} prefix="KES " /></Card></Col>
              </Row>
            ),
          },
          {
            key: '2',
            label: 'Recent Activity',
            children: (
              <Table
                pagination={false}
                dataSource={[
                  { key: '1', date: '2026-02-28', narration: 'POS Purchase', amount: '-1,850.00' },
                  { key: '2', date: '2026-02-27', narration: 'Salary Credit', amount: '+95,000.00' },
                ]}
                columns={[
                  { title: 'Date', dataIndex: 'date', key: 'date' },
                  { title: 'Narration', dataIndex: 'narration', key: 'narration' },
                  { title: 'Amount', dataIndex: 'amount', key: 'amount' },
                ]}
              />
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
