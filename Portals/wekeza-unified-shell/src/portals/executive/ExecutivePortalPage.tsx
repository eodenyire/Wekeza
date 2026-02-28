import React from 'react';
import { Tabs, Card, Col, Row, Statistic, Table } from 'antd';

const ExecutivePortalPage: React.FC = () => {
  return (
    <div style={{ padding: '24px' }}>
      <h1>Executive & Board Portal</h1>
      <p style={{ marginBottom: '24px' }}>Strategic insights, enterprise KPIs, and decision support.</p>
      <Tabs
        items={[
          {
            key: '1',
            label: 'Enterprise KPIs',
            children: (
              <Row gutter={[16, 16]}>
                <Col xs={24} md={8}><Card><Statistic title="ROE" value={18.4} suffix="%" /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="NPL Ratio" value={4.1} suffix="%" /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="Cost-to-Income" value={46.2} suffix="%" /></Card></Col>
              </Row>
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
