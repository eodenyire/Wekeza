import React from 'react';
import { Card, Form, Input, Select, Button, Space } from 'antd';

const SystemConfigurationPanel: React.FC = () => {
  return (
    <Card title="System Configuration">
      <Form
        layout="vertical"
        initialValues={{
          bankName: 'Wekeza Bank',
          timezone: 'Africa/Nairobi',
          currency: 'KES',
          businessDate: '2026-02-28',
        }}
      >
        <Form.Item label="Bank Name" name="bankName">
          <Input />
        </Form.Item>
        <Form.Item label="Timezone" name="timezone">
          <Select options={[{ value: 'Africa/Nairobi' }, { value: 'UTC' }]} />
        </Form.Item>
        <Form.Item label="Base Currency" name="currency">
          <Select options={[{ value: 'KES' }, { value: 'USD' }, { value: 'EUR' }]} />
        </Form.Item>
        <Form.Item label="Business Date" name="businessDate">
          <Input />
        </Form.Item>
        <Space>
          <Button type="primary">Save Configuration</Button>
          <Button>Discard</Button>
        </Space>
      </Form>
    </Card>
  );
};

export default SystemConfigurationPanel;
