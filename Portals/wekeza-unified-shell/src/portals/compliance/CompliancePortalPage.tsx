import React from 'react';
import { Tabs, Card, Table, Tag, Row, Col, Statistic } from 'antd';

const CompliancePortalPage: React.FC = () => {
  return (
    <div style={{ padding: '24px' }}>
      <h1>Compliance & Risk Portal</h1>
      <p style={{ marginBottom: '24px' }}>AML monitoring, sanctions, fraud alerts, and control attestations.</p>
      <Tabs
        items={[
          {
            key: '1',
            label: 'AML Alerts',
            children: (
              <Table
                pagination={false}
                dataSource={[
                  { key: '1', caseId: 'AML-1001', customer: 'Acme Traders', risk: 'High', status: 'Open' },
                  { key: '2', caseId: 'AML-1002', customer: 'John Mwangi', risk: 'Medium', status: 'Review' },
                ]}
                columns={[
                  { title: 'Case ID', dataIndex: 'caseId', key: 'caseId' },
                  { title: 'Customer', dataIndex: 'customer', key: 'customer' },
                  { title: 'Risk', dataIndex: 'risk', key: 'risk', render: (v: string) => <Tag color={v === 'High' ? 'red' : 'orange'}>{v}</Tag> },
                  { title: 'Status', dataIndex: 'status', key: 'status' },
                ]}
              />
            ),
          },
          {
            key: '2',
            label: 'Risk Metrics',
            children: (
              <Row gutter={[16, 16]}>
                <Col xs={24} md={8}><Card><Statistic title="High Risk Cases" value={12} /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="Sanctions Hits" value={2} /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="Fraud Score Avg" value={41} /></Card></Col>
              </Row>
            ),
          },
          {
            key: '3',
            label: 'Regulatory Reporting',
            children: <Card>Generate and submit scheduled compliance returns.</Card>,
          },
        ]}
      />
    </div>
  );
};

export default CompliancePortalPage;
