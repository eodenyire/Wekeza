import React from 'react';
import { Tabs, Card, Row, Col, Statistic, Table } from 'antd';

const TreasuryPortalPage: React.FC = () => {
  return (
    <div style={{ padding: '24px' }}>
      <h1>Treasury & Markets Portal</h1>
      <p style={{ marginBottom: '24px' }}>Liquidity, FX, and money market operations.</p>
      <Tabs
        items={[
          {
            key: '1',
            label: 'Liquidity',
            children: (
              <Row gutter={[16, 16]}>
                <Col xs={24} md={8}><Card><Statistic title="LCR" value={132} suffix="%" /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="NSFR" value={118} suffix="%" /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="Net Position" value={245000000} prefix="KES " /></Card></Col>
              </Row>
            ),
          },
          {
            key: '2',
            label: 'FX Deals',
            children: (
              <Table
                pagination={false}
                dataSource={[
                  { key: '1', dealId: 'FX-001', pair: 'USD/KES', amount: '1,000,000', rate: '158.40' },
                  { key: '2', dealId: 'FX-002', pair: 'EUR/KES', amount: '250,000', rate: '171.05' },
                ]}
                columns={[
                  { title: 'Deal ID', dataIndex: 'dealId', key: 'dealId' },
                  { title: 'Pair', dataIndex: 'pair', key: 'pair' },
                  { title: 'Amount', dataIndex: 'amount', key: 'amount' },
                  { title: 'Rate', dataIndex: 'rate', key: 'rate' },
                ]}
              />
            ),
          },
          {
            key: '3',
            label: 'Money Market',
            children: <Card>Interbank placements, borrowings, and maturity ladder.</Card>,
          },
        ]}
      />
    </div>
  );
};

export default TreasuryPortalPage;
