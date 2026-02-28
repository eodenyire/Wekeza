import React from 'react';
import { Card, Form, Input, InputNumber, Switch, Button, Space, Select, TimePicker } from 'antd';
import { SaveOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';

const BranchSettingsPanel: React.FC = () => {
  const [form] = Form.useForm();

  const onFinish = (values: any) => {
    console.log('Branch settings updated:', values);
  };

  return (
    <div>
      <Card title="Branch Configuration">
        <Form
          form={form}
          layout="vertical"
          onFinish={onFinish}
          initialValues={{
            branchName: 'Main Branch - CBD',
            branchCode: 'BR001',
            maxCashLimit: 10000000,
            dailyTransactionLimit: 50000000,
            enableWeekendOps: false,
            openingTime: dayjs('08:00', 'HH:mm'),
            closingTime: dayjs('17:00', 'HH:mm'),
            currency: 'KES',
          }}
        >
          <Form.Item label="Branch Name" name="branchName">
            <Input />
          </Form.Item>

          <Form.Item label="Branch Code" name="branchCode">
            <Input disabled />
          </Form.Item>

          <Form.Item label="Maximum Cash Limit (KES)" name="maxCashLimit">
            <InputNumber
              style={{ width: '100%' }}
              min={0}
              formatter={(value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
            />
          </Form.Item>

          <Form.Item label="Daily Transaction Limit (KES)" name="dailyTransactionLimit">
            <InputNumber
              style={{ width: '100%' }}
              min={0}
              formatter={(value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
            />
          </Form.Item>

          <Form.Item label="Primary Currency" name="currency">
            <Select
              options={[
                { value: 'KES', label: 'Kenyan Shilling (KES)' },
                { value: 'USD', label: 'US Dollar (USD)' },
                { value: 'EUR', label: 'Euro (EUR)' },
              ]}
            />
          </Form.Item>

          <Form.Item label="Opening Time" name="openingTime">
            <TimePicker format="HH:mm" style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item label="Closing Time" name="closingTime">
            <TimePicker format="HH:mm" style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item label="Enable Weekend Operations" name="enableWeekendOps" valuePropName="checked">
            <Switch />
          </Form.Item>

          <Form.Item>
            <Space>
              <Button type="primary" htmlType="submit" icon={<SaveOutlined />}>
                Save Settings
              </Button>
              <Button>Reset</Button>
            </Space>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
};

export default BranchSettingsPanel;
