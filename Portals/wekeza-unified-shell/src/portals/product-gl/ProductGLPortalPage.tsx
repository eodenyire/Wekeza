import React from 'react';
import { Tabs, Card, Table, Row, Col, Statistic } from 'antd';

const ProductGLPortalPage: React.FC = () => {
  return (
    <div style={{ padding: '24px' }}>
      <h1>Product & GL Management Portal</h1>
      <p style={{ marginBottom: '24px' }}>Banking product setup, pricing control, and ledger governance.</p>
      <Tabs
        items={[
          {
            key: '1',
            label: 'Product Catalog',
            children: (
              <Table
                pagination={false}
                dataSource={[
                  { key: '1', code: 'SAV001', name: 'Retail Savings', status: 'Active' },
                  { key: '2', code: 'LN001', name: 'SME Working Capital Loan', status: 'Active' },
                ]}
                columns={[
                  { title: 'Code', dataIndex: 'code', key: 'code' },
                  { title: 'Name', dataIndex: 'name', key: 'name' },
                  { title: 'Status', dataIndex: 'status', key: 'status' },
                ]}
              />
            ),
          },
          {
            key: '2',
            label: 'GL Controls',
            children: (
              <Row gutter={[16, 16]}>
                <Col xs={24} md={8}><Card><Statistic title="Open GL Exceptions" value={4} /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="Suspense Balance" value={1280000} prefix="KES " /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="Unposted Journals" value={9} /></Card></Col>
              </Row>
            ),
          },
          {
            key: '3',
            label: 'Pricing Rules',
            children: <Card>Interest, fees, and posting rule management.</Card>,
          },
        ]}
      />
    </div>
  );
};

export default ProductGLPortalPage;
