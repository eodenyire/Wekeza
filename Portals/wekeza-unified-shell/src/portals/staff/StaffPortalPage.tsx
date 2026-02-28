import React from 'react';
import { Tabs, Card, Table, Button, Space } from 'antd';

const StaffPortalPage: React.FC = () => {
  return (
    <div style={{ padding: '24px' }}>
      <h1>Staff Self-Service Portal</h1>
      <p style={{ marginBottom: '24px' }}>Leave management, payroll access, training, and internal requests.</p>
      <Tabs
        items={[
          {
            key: '1',
            label: 'My Requests',
            children: (
              <Table
                pagination={false}
                dataSource={[
                  { key: '1', type: 'Leave Request', submitted: '2026-02-25', status: 'Pending' },
                  { key: '2', type: 'HR Letter', submitted: '2026-02-20', status: 'Approved' },
                ]}
                columns={[
                  { title: 'Type', dataIndex: 'type', key: 'type' },
                  { title: 'Submitted', dataIndex: 'submitted', key: 'submitted' },
                  { title: 'Status', dataIndex: 'status', key: 'status' },
                ]}
              />
            ),
          },
          {
            key: '2',
            label: 'Payslips',
            children: <Card>View monthly payslips and statutory deductions.</Card>,
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
