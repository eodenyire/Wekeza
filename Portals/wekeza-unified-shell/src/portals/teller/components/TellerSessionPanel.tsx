import React from 'react';
import { Card, Form, Input, InputNumber, Button, Space, message } from 'antd';
import { useMutation } from '@tanstack/react-query';
import { tellerApi } from '../services/tellerApi';

export const TellerSessionPanel: React.FC = () => {
  const [form] = Form.useForm();

  const startSessionMutation = useMutation({
    mutationFn: tellerApi.startSession,
    onSuccess: () => {
      message.success('Session started successfully');
      form.resetFields();
    },
    onError: () => {
      message.error('Failed to start session');
    },
  });

  const endSessionMutation = useMutation({
    mutationFn: tellerApi.endSession,
    onSuccess: () => message.success('Session ended successfully'),
    onError: () => message.error('Failed to end session'),
  });

  const onStartSession = (values: any) => {
    startSessionMutation.mutate(values);
  };

  return (
    <Space direction="vertical" style={{ width: '100%' }} size="large">
      <Card title="Start Teller Session">
        <Form form={form} layout="vertical" onFinish={onStartSession}>
          <Form.Item
            name="tellerId"
            label="Teller ID"
            rules={[{ required: true, message: 'Please enter teller ID' }]}
          >
            <Input placeholder="e.g. TELLER001" />
          </Form.Item>

          <Form.Item
            name="branchId"
            label="Branch ID"
            rules={[{ required: true, message: 'Please enter branch ID' }]}
          >
            <Input placeholder="e.g. BR001" />
          </Form.Item>

          <Form.Item
            name="openingBalance"
            label="Opening Balance"
            rules={[{ required: true, message: 'Please enter opening balance' }]}
          >
            <InputNumber style={{ width: '100%' }} min={0} placeholder="0.00" />
          </Form.Item>

          <Button type="primary" htmlType="submit" loading={startSessionMutation.isPending}>
            Start Session
          </Button>
        </Form>
      </Card>

      <Card title="End Teller Session">
        <Form
          layout="inline"
          onFinish={(values) => endSessionMutation.mutate(values.sessionId)}
        >
          <Form.Item
            name="sessionId"
            rules={[{ required: true, message: 'Session ID required' }]}
          >
            <Input placeholder="Session ID" style={{ width: 300 }} />
          </Form.Item>
          <Form.Item>
            <Button danger htmlType="submit" loading={endSessionMutation.isPending}>
              End Session
            </Button>
          </Form.Item>
        </Form>
      </Card>
    </Space>
  );
};
