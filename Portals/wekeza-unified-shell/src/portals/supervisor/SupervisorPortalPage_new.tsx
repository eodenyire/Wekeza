import React, { useState, useEffect } from 'react';
import { Tabs, Card, Table, Button, Space, Tag, Spin, Alert } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';

interface TeamMember {
  MemberId: string;
  Name: string;
  Role: string;
  Status: string;
  OnDuty: boolean;
  SessionStarted: string | null;
  TransactionsToday: number;
  ErrorsToday: number;
}

interface ApprovalRequest {
  ApprovalId: string;
  Type: string;
  Amount: number | string;
  Priority: string;
  Status: string;
  SubmittedBy: string;
  SubmittedAt: string;
}

const SupervisorPortalPage: React.FC = () => {
  const [teamData, setTeamData] = useState<TeamMember[]>([]);
  const [approvals, setApprovals] = useState<ApprovalRequest[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchTeamData = async () => {
    setLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem('authToken');
      const headers = { 'Authorization': `Bearer ${token}` };

      // Fetch team data
      const teamResponse = await fetch('/api/supervisor/team', { headers });
      if (!teamResponse.ok) throw new Error('Failed to fetch team data');
      const teamResult = await teamResponse.json();
      setTeamData(teamResult.data || []);

      // Mock approval data for now
      setApprovals([
        {
          ApprovalId: 'APR-001',
          Type: 'Cash Withdrawal Override',
          Amount: 'KES 350,000',
          Priority: 'High',
          Status: 'Pending',
          SubmittedBy: 'John Doe',
          SubmittedAt: new Date().toISOString()
        },
        {
          ApprovalId: 'APR-002',
          Type: 'Account Reactivation',
          Amount: 'N/A',
          Priority: 'Medium',
          Status: 'Pending',
          SubmittedBy: 'Jane Smith',
          SubmittedAt: new Date().toISOString()
        }
      ]);
    } catch (err: any) {
      setError(err.message || 'Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTeamData();
    const interval = setInterval(fetchTeamData, 30000); // Auto-refresh every 30 seconds
    return () => clearInterval(interval);
  }, []);

  if (loading && teamData.length === 0) {
    return (
      <div style={{ padding: '40px 0', textAlign: 'center' }}>
        <Spin size="large" tip="Loading supervisor data..." />
      </div>
    );
  }

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <div>
          <h1>Supervisor Portal</h1>
          <p style={{ marginBottom: 0 }}>Operational approvals, team workload, and service quality control.</p>
        </div>
        <Button icon={<ReloadOutlined />} onClick={fetchTeamData} loading={loading}>
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
            label: 'Pending Approvals',
            children: (
              <Table
                pagination={false}
                dataSource={approvals}
                rowKey="ApprovalId"
                columns={[
                  { title: 'Reference', dataIndex: 'ApprovalId', key: 'ApprovalId' },
                  { title: 'Type', dataIndex: 'Type', key: 'Type' },
                  { title: 'Amount', dataIndex: 'Amount', key: 'Amount' },
                  {
                    title: 'Priority',
                    dataIndex: 'Priority',
                    key: 'Priority',
                    render: (v: string) => <Tag color={v === 'High' ? 'red' : v === 'Medium' ? 'orange' : 'green'}>{v}</Tag>
                  },
                  {
                    title: 'Status',
                    dataIndex: 'Status',
                    key: 'Status',
                    render: (v: string) => <Tag color={v === 'Pending' ? 'blue' : v === 'Approved' ? 'green' : 'red'}>{v}</Tag>
                  },
                  {
                    title: 'Action',
                    key: 'action',
                    render: () => (
                      <Space>
                        <Button type="primary" size="small">Approve</Button>
                        <Button danger size="small">Reject</Button>
                      </Space>
                    )
                  }
                ]}
              />
            ),
          },
          {
            key: '2',
            label: 'Team Queue',
            children: (
              <Card>
                <Table
                  pagination={false}
                  dataSource={teamData}
                  rowKey="MemberId"
                  columns={[
                    { title: 'Staff Name', dataIndex: 'Name', key: 'Name' },
                    { title: 'Role', dataIndex: 'Role', key: 'Role' },
                    {
                      title: 'Status',
                      dataIndex: 'Status',
                      key: 'Status',
                      render: (v: string) => <Tag color={v === 'Active' ? 'green' : 'red'}>{v}</Tag>
                    },
                    {
                      title: 'On Duty',
                      dataIndex: 'OnDuty',
                      key: 'OnDuty',
                      render: (v: boolean) => <Tag color={v ? 'green' : 'gray'}>{v ? 'Yes' : 'No'}</Tag>
                    },
                    {
                      title: 'Sessions Started',
                      dataIndex: 'SessionStarted',
                      key: 'SessionStarted',
                      render: (v: string | null) => v ? new Date(v).toLocaleTimeString() : '-'
                    },
                    { title: 'Transactions', dataIndex: 'TransactionsToday', key: 'TransactionsToday' },
                    {
                      title: 'Errors',
                      dataIndex: 'ErrorsToday',
                      key: 'ErrorsToday',
                      render: (v: number) => <Tag color={v > 0 ? 'red' : 'green'}>{v}</Tag>
                    }
                  ]}
                />
              </Card>
            ),
          },
          {
            key: '3',
            label: 'Controls',
            children: (
              <Card>
                <Space>
                  <Button type="primary" onClick={() => alert('Approving selected...')}>Approve Selected</Button>
                  <Button danger onClick={() => alert('Rejecting selected...')}>Reject Selected</Button>
                </Space>
              </Card>
            ),
          },
        ]}
      />
    </div>
  );
};

export default SupervisorPortalPage;
