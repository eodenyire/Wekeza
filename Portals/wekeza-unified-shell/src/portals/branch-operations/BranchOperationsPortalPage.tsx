import React from 'react';
import { Tabs, Card, Col, Row, Statistic, Form, InputNumber, Button, Space } from 'antd';

const BranchOperationsPortalPage: React.FC = () => {
  return (
    <div style={{ padding: '24px' }}>
      <h1>Branch Operations Portal</h1>
      <p style={{ marginBottom: '24px' }}>Vault operations, cash controls, and day-end processing.</p>
      <Tabs
        items={[
          {
            key: '1',
            label: 'Vault',
            children: (
              <Row gutter={[16, 16]}>
                <Col xs={24} md={8}><Card><Statistic title="Vault Balance" value={12500000} prefix="KES " /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="Cash In Transit" value={3} /></Card></Col>
                <Col xs={24} md={8}><Card><Statistic title="Dual-Control Exceptions" value={0} /></Card></Col>
              </Row>
            ),
          },
          {
            key: '2',
            label: 'BOD / EOD',
            children: (
              <Card title="Day Processing">
                <Space>
                  <Button type="primary">Run BOD</Button>
                  <Button danger>Run EOD</Button>
                </Space>
              </Card>
            ),
          },
          {
            key: '3',
            label: 'Cash Transfer',
            children: (
              <Card title="Inter-Branch Cash Transfer">
                <Form layout="vertical">
                  <Form.Item label="Amount"><InputNumber min={1} style={{ width: '100%' }} /></Form.Item>
                  <Form.Item><Button type="primary">Submit Transfer</Button></Form.Item>
                </Form>
              </Card>
            ),
          },
        ]}
      />
    </div>
  );
};

export default BranchOperationsPortalPage;
