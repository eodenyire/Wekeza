import React from 'react';
import { Tabs, Card, Table, Button, Space, Tag } from 'antd';

const SupervisorPortalPage: React.FC = () => {
  return (
    <div style={{ padding: '24px' }}>
      <h1>Supervisor Portal</h1>
      <p style={{ marginBottom: '24px' }}>Operational approvals, team workload, and service quality control.</p>
      <Tabs
        items={[
          {
            key: '1',
            label: 'Pending Approvals',
            children: (
              <Table
                pagination={false}
                dataSource={[
                  { key: '1', ref: 'APR-001', type: 'Cash Withdrawal Override', amount: 'KES 350,000', priority: 'High' },
                  { key: '2', ref: 'APR-002', type: 'Account Reactivation', amount: 'N/A', priority: 'Medium' },
                ]}
                columns={[
                  { title: 'Reference', dataIndex: 'ref', key: 'ref' },
                  { title: 'Type', dataIndex: 'type', key: 'type' },
                  { title: 'Amount', dataIndex: 'amount', key: 'amount' },
                  { title: 'Priority', dataIndex: 'priority', key: 'priority', render: (v: string) => <Tag color={v === 'High' ? 'red' : 'orange'}>{v}</Tag> },
                ]}
              />
            ),
          },
          {
            key: '2',
            label: 'Team Queue',
            children: <Card>Live team queue distribution by teller and service desk.</Card>,
          },
          {
            key: '3',
            label: 'Controls',
            children: (
              <Card>
                <Space>
                  <Button type="primary">Approve Selected</Button>
                  <Button danger>Reject Selected</Button>
                </Space>
              </Card>
            ),
          },
        ]}
      />
    </div>
  );
};

export default SupervisorPortalPage;
