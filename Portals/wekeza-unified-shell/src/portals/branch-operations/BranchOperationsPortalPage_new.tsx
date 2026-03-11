import React, { useState, useEffect } from 'react';
import { Tabs, Card, Col, Row, Statistic, Form, InputNumber, Button, Space, Spin, Alert, Typography, message, Select } from 'antd';
import {
  BankOutlined,
  ReloadOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
} from '@ant-design/icons';
import { useAuthStore } from '@store/authStore';

const { Text } = Typography;

const BRANCH_OPS_REFRESH_INTERVAL_MS = 30_000;

interface CashPosition {
  cashOnHand: number;
  safeVaultCash: number;
  totalCashAvailable: number;
  cashInTransit: number;
  cashExpectedIncoming: number;
  cashAtCentralBank: number;
  lastUpdated?: string;
}

const BranchOperationsPortalPage: React.FC = () => {
  const { user } = useAuthStore();
  const currentUser = user?.username ?? 'system';

  const [cashPosition, setCashPosition] = useState<CashPosition | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [bodLoading, setBodLoading] = useState(false);
  const [eodLoading, setEodLoading] = useState(false);
  const [transferLoading, setTransferLoading] = useState(false);
  const [transferForm] = Form.useForm();

  const fetchCashPosition = async () => {
    try {
      setLoading(true);
      setError(null);
      const token = localStorage.getItem('authToken');
      const response = await fetch('/api/branch-manager/cash-position', {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!response.ok) throw new Error(`HTTP ${response.status}`);
      const data = await response.json();
      const payload = data?.value ?? data?.Value ?? data;
      setCashPosition({
        cashOnHand: payload?.cashOnHand ?? payload?.CashOnHand ?? 0,
        safeVaultCash: payload?.safeVaultCash ?? payload?.SafeVaultCash ?? 0,
        totalCashAvailable: payload?.totalCashAvailable ?? payload?.TotalCashAvailable ?? 0,
        cashInTransit: payload?.cashInTransit ?? payload?.CashInTransit ?? 0,
        cashExpectedIncoming: payload?.cashExpectedIncoming ?? payload?.CashExpectedIncoming ?? 0,
        cashAtCentralBank: payload?.cashAtCentralBank ?? payload?.CashAtCentralBank ?? 0,
        lastUpdated: new Date().toLocaleTimeString(),
      });
    } catch (err: any) {
      setError(err.message || 'Failed to load cash position');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCashPosition();
    const interval = setInterval(fetchCashPosition, BRANCH_OPS_REFRESH_INTERVAL_MS);
    return () => clearInterval(interval);
  }, []);

  const runBOD = async () => {
    setBodLoading(true);
    try {
      const token = localStorage.getItem('authToken');
      const response = await fetch(
        `/api/branchoperations/bod?branchId=00000000-0000-0000-0000-000000000001&processedBy=${encodeURIComponent(currentUser)}`,
        { method: 'POST', headers: { Authorization: `Bearer ${token}` } },
      );
      if (!response.ok) throw new Error(`HTTP ${response.status}`);
      message.success('BOD processed successfully');
    } catch (err: any) {
      message.error(`BOD failed: ${err.message}`);
    } finally {
      setBodLoading(false);
    }
  };

  const runEOD = async () => {
    setEodLoading(true);
    try {
      const token = localStorage.getItem('authToken');
      const response = await fetch('/api/branchoperations/eod', {
        method: 'POST',
        headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
        body: JSON.stringify({
          BranchId: '00000000-0000-0000-0000-000000000001',
          ProcessedBy: currentUser,
          EODDate: new Date().toISOString().split('T')[0],
        }),
      });
      if (!response.ok) throw new Error(`HTTP ${response.status}`);
      message.success('EOD processed successfully');
    } catch (err: any) {
      message.error(`EOD failed: ${err.message}`);
    } finally {
      setEodLoading(false);
    }
  };

  const submitTransfer = async (values: { amount: number; targetBranch: string }) => {
    setTransferLoading(true);
    try {
      const token = localStorage.getItem('authToken');
      const response = await fetch('/api/branch-manager/cash-replenishment/request', {
        method: 'POST',
        headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
        body: JSON.stringify({
          Amount: values.amount,
          TargetBranchCode: values.targetBranch,
          RequestedBy: currentUser,
          Reason: 'Inter-branch cash transfer via portal',
        }),
      });
      if (!response.ok) throw new Error(`HTTP ${response.status}`);
      message.success('Cash transfer request submitted successfully');
      transferForm.resetFields();
      fetchCashPosition();
    } catch (err: any) {
      message.error(`Transfer failed: ${err.message}`);
    } finally {
      setTransferLoading(false);
    }
  };

  const fmt = (v: number) =>
    `KES ${v.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <div>
          <h1 style={{ margin: 0 }}>Branch Operations Portal</h1>
          <Text type="secondary">Vault operations, cash controls, and day-end processing</Text>
        </div>
        <Button icon={<ReloadOutlined />} onClick={fetchCashPosition} loading={loading}>
          Refresh
        </Button>
      </div>

      {error && (
        <Alert
          message="Error loading data"
          description={error}
          type="error"
          closable
          onClose={() => setError(null)}
          style={{ marginBottom: 16 }}
        />
      )}

      <Tabs
        items={[
          {
            key: 'vault',
            label: (
              <span>
                <BankOutlined />
                Vault
              </span>
            ),
            children: loading && !cashPosition ? (
              <div style={{ padding: '40px 0', textAlign: 'center' }}>
                <Spin size="large" tip="Loading vault data..." />
              </div>
            ) : (
              <>
                <Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic
                        title="Cash On Hand"
                        value={cashPosition?.cashOnHand ?? 0}
                        formatter={(v) => fmt(Number(v))}
                        prefix={<BankOutlined />}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic
                        title="Safe Vault Cash"
                        value={cashPosition?.safeVaultCash ?? 0}
                        formatter={(v) => fmt(Number(v))}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic
                        title="Total Cash Available"
                        value={cashPosition?.totalCashAvailable ?? 0}
                        formatter={(v) => fmt(Number(v))}
                      />
                    </Card>
                  </Col>
                </Row>
                <Row gutter={[16, 16]}>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic
                        title="Cash In Transit"
                        value={cashPosition?.cashInTransit ?? 0}
                        formatter={(v) => fmt(Number(v))}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic
                        title="Expected Incoming"
                        value={cashPosition?.cashExpectedIncoming ?? 0}
                        formatter={(v) => fmt(Number(v))}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic
                        title="Cash at Central Bank"
                        value={cashPosition?.cashAtCentralBank ?? 0}
                        formatter={(v) => fmt(Number(v))}
                      />
                    </Card>
                  </Col>
                </Row>
                {cashPosition?.lastUpdated && (
                  <Text type="secondary" style={{ marginTop: 8, display: 'block' }}>
                    Last updated: {cashPosition.lastUpdated}
                  </Text>
                )}
              </>
            ),
          },
          {
            key: 'bod-eod',
            label: 'BOD / EOD',
            children: (
              <Card title="Day Processing">
                <Space direction="vertical" size="middle" style={{ width: '100%' }}>
                  <Text type="secondary">
                    Run Beginning of Day (BOD) or End of Day (EOD) processing for the branch.
                  </Text>
                  <Space size="middle">
                    <Button
                      type="primary"
                      icon={<CheckCircleOutlined />}
                      loading={bodLoading}
                      onClick={runBOD}
                    >
                      Run BOD
                    </Button>
                    <Button
                      danger
                      icon={<CloseCircleOutlined />}
                      loading={eodLoading}
                      onClick={runEOD}
                    >
                      Run EOD
                    </Button>
                  </Space>
                </Space>
              </Card>
            ),
          },
          {
            key: 'transfer',
            label: 'Cash Transfer',
            children: (
              <Card title="Inter-Branch Cash Transfer Request">
                <Form
                  form={transferForm}
                  layout="vertical"
                  onFinish={submitTransfer}
                  style={{ maxWidth: 480 }}
                >
                  <Form.Item
                    label="Target Branch Code"
                    name="targetBranch"
                    rules={[{ required: true, message: 'Please select a target branch' }]}
                  >
                    <Select placeholder="Select branch">
                      <Select.Option value="BR100001">BR100001 - Main Branch</Select.Option>
                      <Select.Option value="HQ">HQ - Headquarters</Select.Option>
                      <Select.Option value="BR100002">BR100002 - Branch 2</Select.Option>
                    </Select>
                  </Form.Item>
                  <Form.Item
                    label="Amount (KES)"
                    name="amount"
                    rules={[
                      { required: true, message: 'Please enter an amount' },
                      { type: 'number', min: 1, message: 'Amount must be positive' },
                    ]}
                  >
                    <InputNumber
                      min={1}
                      style={{ width: '100%' }}
                      formatter={(v) => `${v}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
                    />
                  </Form.Item>
                  <Form.Item>
                    <Button type="primary" htmlType="submit" loading={transferLoading}>
                      Submit Transfer Request
                    </Button>
                  </Form.Item>
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
