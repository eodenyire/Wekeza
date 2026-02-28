import React from 'react';
import { Tabs, Card, Table, Tag } from 'antd';

const TradeFinancePortalPage: React.FC = () => {
  return (
    <div style={{ padding: '24px' }}>
      <h1>Trade Finance Portal</h1>
      <p style={{ marginBottom: '24px' }}>Letters of credit, guarantees, and documentary collections.</p>
      <Tabs
        items={[
          {
            key: '1',
            label: 'Letters of Credit',
            children: (
              <Table
                pagination={false}
                dataSource={[
                  { key: '1', ref: 'LC-9001', applicant: 'Blue Sky Imports', amount: 'USD 450,000', status: 'Issued' },
                  { key: '2', ref: 'LC-9002', applicant: 'Nile Exports', amount: 'EUR 120,000', status: 'Pending' },
                ]}
                columns={[
                  { title: 'Reference', dataIndex: 'ref', key: 'ref' },
                  { title: 'Applicant', dataIndex: 'applicant', key: 'applicant' },
                  { title: 'Amount', dataIndex: 'amount', key: 'amount' },
                  { title: 'Status', dataIndex: 'status', key: 'status', render: (v: string) => <Tag color={v === 'Issued' ? 'green' : 'orange'}>{v}</Tag> },
                ]}
              />
            ),
          },
          {
            key: '2',
            label: 'Guarantees',
            children: <Card>Issue, amend, and monitor bank guarantees.</Card>,
          },
          {
            key: '3',
            label: 'Collections',
            children: <Card>Process documentary collections and discrepancy tracking.</Card>,
          },
        ]}
      />
    </div>
  );
};

export default TradeFinancePortalPage;
