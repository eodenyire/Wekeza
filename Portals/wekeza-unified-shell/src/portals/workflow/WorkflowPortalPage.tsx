import React, { useState, useEffect } from 'react';
import { Tabs, Card, Table, Tag, Spin, Alert, Button, Space } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';

interface PendingApproval {
  approvalId: string;
  type: string;
  staff: string;
  customer: string;
  amount: number;
  reason: string;
  submittedAt: string;
  priority: string;
}

const WorkflowPortalPage: React.FC = () => {
  const [approvals, setApprovals] = useState<PendingApproval[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchApprovals = async () => {
    setLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem('authToken');
      const headers: HeadersInit = token ? { Authorization: `Bearer ${token}` } : {};
      const res = await fetch('/api/supervisor/approvals/pending', { headers });
      if (!res.ok) throw new Error(`Failed to load pending approvals (${res.status})`);
      const data = await res.json();
      setApprovals(data.approvals ?? data ?? []);
    } catch (err: any) {
      setError(err.message || 'Failed to load workflow data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchApprovals();
    const interval = setInterval(fetchApprovals, 30000);
    return () => clearInterval(interval);
  }, []);

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <div>
          <h1>Workflow &amp; Task Portal</h1>
          <p style={{ marginBottom: 0 }}>Approval routing, workflow monitoring, and task execution.</p>
        </div>
        <Button icon={<ReloadOutlined />} onClick={fetchApprovals} loading={loading}>
          Refresh
        </Button>
      </div>

      {error && (
        <Alert
          message="Error"
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
            key: '1',
            label: 'My Tasks',
            children: loading && approvals.length === 0 ? (
              <Spin />
            ) : (
              <Table
                pagination={{ pageSize: 10 }}
                dataSource={approvals}
                rowKey="approvalId"
                locale={{ emptyText: 'No pending approvals' }}
                columns={[
                  { title: 'Reference', dataIndex: 'approvalId', key: 'approvalId' },
                  { title: 'Type', dataIndex: 'type', key: 'type' },
                  { title: 'Submitted By', dataIndex: 'staff', key: 'staff' },
                  { title: 'Customer', dataIndex: 'customer', key: 'customer' },
                  {
                    title: 'Amount',
                    dataIndex: 'amount',
                    key: 'amount',
                    render: (v: number) =>
                      v ? `KES ${v.toLocaleString('en-US', { minimumFractionDigits: 2 })}` : '-',
                  },
                  {
                    title: 'Priority',
                    dataIndex: 'priority',
                    key: 'priority',
                    render: (v: string) => (
                      <Tag color={v === 'High' ? 'red' : v === 'Medium' ? 'orange' : 'green'}>{v}</Tag>
                    ),
                  },
                  {
                    title: 'Submitted',
                    dataIndex: 'submittedAt',
                    key: 'submittedAt',
                    render: (v: string) => new Date(v).toLocaleString(),
                  },
                  {
                    title: 'Action',
                    key: 'action',
                    render: () => (
                      <Space>
                        <Button type="primary" size="small">Approve</Button>
                        <Button danger size="small">Reject</Button>
                      </Space>
                    ),
                  },
                ]}
              />
            ),
          },
          {
            key: '2',
            label: 'Workflow Monitor',
            children: <Card>Track in-flight process instances and bottlenecks.</Card>,
          },
          {
            key: '3',
            label: 'Escalations',
            children: <Card>Escalation queue for breached SLAs and stuck tasks.</Card>,
          },
        ]}
      />
    </div>
  );
};

export default WorkflowPortalPage;
