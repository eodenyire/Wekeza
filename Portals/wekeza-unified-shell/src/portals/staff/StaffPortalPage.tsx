import React, { useState, useEffect } from 'react';
import { Tabs, Card, Table, Button, Space, Tag, Spin, Alert, Statistic, Row, Col } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';

interface LeaveBalance {
  leaveType: string;
  totalDays: number;
  usedDays: number;
  remainingDays: number;
}

interface LeaveRequest {
  leaveId: string;
  leaveType: string;
  startDate: string;
  endDate: string;
  days: number;
  status: string;
  appliedOn: string;
  reason: string;
}

interface PayrollSummary {
  payPeriod: string;
  basicSalary: number;
  grossPay: number;
  netPay: number;
  totalDeductions: number;
  currency: string;
}

const StaffPortalPage: React.FC = () => {
  const [leaveBalances, setLeaveBalances] = useState<LeaveBalance[]>([]);
  const [leaveHistory, setLeaveHistory] = useState<LeaveRequest[]>([]);
  const [payroll, setPayroll] = useState<PayrollSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem('authToken');
      const headers: HeadersInit = token ? { Authorization: `Bearer ${token}` } : {};

      const [balanceRes, historyRes, payrollRes] = await Promise.all([
        fetch('/api/staff-self-service/leave/balance', { headers }),
        fetch('/api/staff-self-service/leave/history', { headers }),
        fetch('/api/staff-self-service/payroll/current', { headers }),
      ]);

      if (balanceRes.ok) {
        const b = await balanceRes.json();
        setLeaveBalances(b.leaveBalances || []);
      }
      if (historyRes.ok) {
        const h = await historyRes.json();
        setLeaveHistory(h.leaveRequests || []);
      }
      if (payrollRes.ok) {
        const p = await payrollRes.json();
        setPayroll(p);
      }
    } catch (err: any) {
      setError(err.message || 'Failed to load staff data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  if (loading && leaveBalances.length === 0 && !payroll) {
    return (
      <div style={{ padding: '40px 0', textAlign: 'center' }}>
        <Spin size="large" tip="Loading staff data..." />
      </div>
    );
  }

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <div>
          <h1>Staff Self-Service Portal</h1>
          <p style={{ marginBottom: 0 }}>Leave management, payroll access, training, and internal requests.</p>
        </div>
        <Button icon={<ReloadOutlined />} onClick={fetchData} loading={loading}>
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
            label: 'My Requests',
            children: (
              <>
                {leaveBalances.length > 0 && (
                  <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
                    {leaveBalances.map((lb) => (
                      <Col xs={24} md={6} key={lb.leaveType}>
                        <Card>
                          <Statistic
                            title={`${lb.leaveType} Days Left`}
                            value={lb.remainingDays}
                            suffix={`/ ${lb.totalDays}`}
                          />
                        </Card>
                      </Col>
                    ))}
                  </Row>
                )}
                <Table
                  pagination={{ pageSize: 10 }}
                  dataSource={leaveHistory}
                  rowKey="leaveId"
                  locale={{ emptyText: 'No leave requests found' }}
                  columns={[
                    { title: 'Type', dataIndex: 'leaveType', key: 'leaveType' },
                    {
                      title: 'Start',
                      dataIndex: 'startDate',
                      key: 'startDate',
                      render: (v: string) => new Date(v).toLocaleDateString(),
                    },
                    {
                      title: 'End',
                      dataIndex: 'endDate',
                      key: 'endDate',
                      render: (v: string) => new Date(v).toLocaleDateString(),
                    },
                    { title: 'Days', dataIndex: 'days', key: 'days' },
                    {
                      title: 'Status',
                      dataIndex: 'status',
                      key: 'status',
                      render: (v: string) => (
                        <Tag color={v === 'Approved' ? 'green' : v === 'Pending' ? 'orange' : 'red'}>{v}</Tag>
                      ),
                    },
                  ]}
                />
              </>
            ),
          },
          {
            key: '2',
            label: 'Payslips',
            children: payroll ? (
              <Card title={`Payslip: ${payroll.payPeriod}`}>
                <Row gutter={[16, 16]}>
                  <Col xs={24} md={6}>
                    <Statistic title="Basic Salary" value={payroll.basicSalary} precision={2} suffix={payroll.currency} />
                  </Col>
                  <Col xs={24} md={6}>
                    <Statistic title="Gross Pay" value={payroll.grossPay} precision={2} suffix={payroll.currency} />
                  </Col>
                  <Col xs={24} md={6}>
                    <Statistic title="Deductions" value={payroll.totalDeductions} precision={2} suffix={payroll.currency} />
                  </Col>
                  <Col xs={24} md={6}>
                    <Statistic title="Net Pay" value={payroll.netPay} precision={2} suffix={payroll.currency} valueStyle={{ color: '#3f8600' }} />
                  </Col>
                </Row>
              </Card>
            ) : (
              <Card>View monthly payslips and statutory deductions.</Card>
            ),
          },
          {
            key: '3',
            label: 'Actions',
            children: (
              <Card>
                <Space>
                  <Button type="primary">Apply Leave</Button>
                  <Button>Download Payslip</Button>
                  <Button>Request Certificate</Button>
                </Space>
              </Card>
            ),
          },
        ]}
      />
    </div>
  );
};

export default StaffPortalPage;
