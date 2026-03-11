import React, { useState, useEffect } from 'react';
import { Tabs, Card, Table, Row, Col, Statistic, Spin, Alert, Button } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';

interface PaymentStatus {
  TotalPayments: number;
  SuccessfulPayments: number;
  SuccessRate: number;
  SystemStatus: string;
}

interface Operation {
  OperationId: string;
  Type: string;
  Destination: string;
  Amount: number;
  Status: string;
}

const PaymentsPortalPage: React.FC = () => {
  const [paymentStatus, setPaymentStatus] = useState<PaymentStatus | null>(null);
  const [rtgsSWIFT, setRTGSSWIFT] = useState<Operation[]>([]);
  const [reconciliation, setReconciliation] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);

      const token = localStorage.getItem('authToken');
      const headers = { 'Authorization': `Bearer ${token}` };

      const [statusRes, rtgsRes, reconRes] = await Promise.all([
        fetch('/api/payments-portal/payment-status', { headers }),
        fetch('/api/payments-portal/rtgs-swift', { headers }),
        fetch('/api/payments-portal/reconciliation', { headers })
      ]);

      if (statusRes.ok) {
        const data = await statusRes.json();
        setPaymentStatus(data.data);
      }

      if (rtgsRes.ok) {
        const data = await rtgsRes.json();
        setRTGSSWIFT(data.data || []);
      }

      if (reconRes.ok) {
        const data = await reconRes.json();
        setReconciliation(data.data);
      }
    } catch (err) {
      setError('Failed to load payments data');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
    const interval = setInterval(fetchData, 30000);
    return () => clearInterval(interval);
  }, []);

  const rtgsColumns = [
    { title: 'Operation ID', dataIndex: "operationId", key: "operationId" },
    { title: 'Type', dataIndex: "type", key: "type" },
    { title: 'Destination', dataIndex: "destination", key: "destination" },
    { title: 'Amount', dataIndex: "amount", key: "amount", render: (val: number) => `KES ${val.toLocaleString()}` },
    { title: 'Status', dataIndex: "status", key: "status" }
  ];

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
        <h1>Payments & Clearing Portal</h1>
        <Button icon={<ReloadOutlined />} onClick={fetchData} loading={loading}>
          Refresh
        </Button>
      </div>

      {error && <Alert message={error} type="error" showIcon style={{ marginBottom: '16px' }} closable />}
      
      {loading && <Spin size="large" />}

      {!loading && (
        <Tabs
          items={[
            {
              key: '1',
              label: 'Payment Status',
              children: paymentStatus ? (
                <Row gutter={[16, 16]}>
                  <Col xs={24} md={6}>
                    <Card>
                      <Statistic 
                        title="Total Payments" 
                        value={paymentStatus.TotalPayments}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={6}>
                    <Card>
                      <Statistic 
                        title="Successful" 
                        value={paymentStatus.SuccessfulPayments}
                        valueStyle={{ color: 'green' }}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={6}>
                    <Card>
                      <Statistic 
                        title="Success Rate" 
                        value={paymentStatus.SuccessRate?.toFixed(1)}
                        suffix="%"
                        valueStyle={{ color: (paymentStatus.SuccessRate ?? 0) >= 99 ? 'green' : 'orange' }}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={6}>
                    <Card>
                      <Statistic 
                        title="System Status" 
                        value={paymentStatus.SystemStatus}
                        valueStyle={{ color: 'green' }}
                      />
                    </Card>
                  </Col>
                </Row>
              ) : null
            },
            {
              key: '2',
              label: 'RTGS/SWIFT',
              children: <Table columns={rtgsColumns} dataSource={rtgsSWIFT} rowKey="operationId" pagination={false} />
            },
            {
              key: '3',
              label: 'Reconciliation',
              children: reconciliation ? (
                <Row gutter={[16, 16]}>
                  <Col xs={24} md={6}>
                    <Card>
                      <Statistic 
                        title="Total Transactions" 
                        value={reconciliation.TotalTransactions}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={6}>
                    <Card>
                      <Statistic 
                        title="Reconciled" 
                        value={reconciliation.ReconciledTransactions}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={6}>
                    <Card>
                      <Statistic 
                        title="Unreconciled" 
                        value={reconciliation.UnreconciledTransactions}
                        valueStyle={{ color: 'orange' }}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={6}>
                    <Card>
                      <Statistic 
                        title="Reconciliation Rate" 
                        value={reconciliation.ReconciliationRate}
                        suffix="%"
                      />
                    </Card>
                  </Col>
                </Row>
              ) : null
            }
          ]}
        />
      )}
    </div>
  );
};

export default PaymentsPortalPage;
