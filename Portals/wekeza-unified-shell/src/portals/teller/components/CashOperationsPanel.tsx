import React from 'react';
import { Card, Form, Input, InputNumber, Button, Row, Col, message } from 'antd';
import { useMutation } from '@tanstack/react-query';
import { tellerApi } from '../services/tellerApi';

export const CashOperationsPanel: React.FC = () => {
  const [depositForm] = Form.useForm();
  const [withdrawalForm] = Form.useForm();

  const depositMutation = useMutation({
    mutationFn: tellerApi.processCashDeposit,
    onSuccess: () => {
      message.success('Cash deposit processed successfully');
      depositForm.resetFields();
    },
    onError: () => message.error('Failed to process cash deposit'),
  });

  const withdrawalMutation = useMutation({
    mutationFn: tellerApi.processCashWithdrawal,
    onSuccess: () => {
      message.success('Cash withdrawal processed successfully');
      withdrawalForm.resetFields();
    },
    onError: () => message.error('Failed to process cash withdrawal'),
  });

  return (
    <Row gutter={[16, 16]}>
      <Col span={12}>
        <Card title="Cash Deposit">
          <Form form={depositForm} layout="vertical" onFinish={(values) => depositMutation.mutate(values)}>
            <Form.Item
              name="accountNumber"
              label="Account Number"
              rules={[{ required: true, message: 'Please enter account number' }]}
            >
              <Input placeholder="e.g. ACC001234" />
            </Form.Item>

            <Form.Item
              name="amount"
              label="Amount"
              rules={[{ required: true, message: 'Please enter amount' }]}
            >
              <InputNumber style={{ width: '100%' }} min={1} placeholder="0.00" />
            </Form.Item>

            <Form.Item
              name="narration"
              label="Narration"
              rules={[{ required: true, message: 'Please enter narration' }]}
            >
              <Input.TextArea rows={3} placeholder="Deposit narration" />
            </Form.Item>

            <Button type="primary" htmlType="submit" loading={depositMutation.isPending} block>
              Process Deposit
            </Button>
          </Form>
        </Card>
      </Col>

      <Col span={12}>
        <Card title="Cash Withdrawal">
          <Form
            form={withdrawalForm}
            layout="vertical"
            onFinish={(values) => withdrawalMutation.mutate(values)}
          >
            <Form.Item
              name="accountNumber"
              label="Account Number"
              rules={[{ required: true, message: 'Please enter account number' }]}
            >
              <Input placeholder="e.g. ACC005678" />
            </Form.Item>

            <Form.Item
              name="amount"
              label="Amount"
              rules={[{ required: true, message: 'Please enter amount' }]}
            >
              <InputNumber style={{ width: '100%' }} min={1} placeholder="0.00" />
            </Form.Item>

            <Form.Item
              name="narration"
              label="Narration"
              rules={[{ required: true, message: 'Please enter narration' }]}
            >
              <Input.TextArea rows={3} placeholder="Withdrawal narration" />
            </Form.Item>

            <Button danger htmlType="submit" loading={withdrawalMutation.isPending} block>
              Process Withdrawal
            </Button>
          </Form>
        </Card>
      </Col>
    </Row>
  );
};
