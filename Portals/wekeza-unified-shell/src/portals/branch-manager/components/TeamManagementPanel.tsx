import React from 'react';
import { Card, Table, Button, Tag, Space, Input, Select } from 'antd';
import { SearchOutlined, UserAddOutlined, EditOutlined, LockOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';

interface StaffMember {
  key: string;
  employeeId: string;
  name: string;
  role: string;
  shift: string;
  status: string;
  performance: string;
}

const TeamManagementPanel: React.FC = () => {
  const staffData: StaffMember[] = [
    {
      key: '1',
      employeeId: 'TEL001001',
      name: 'John Teller',
      role: 'Teller',
      shift: 'Morning (8AM-2PM)',
      status: 'Active',
      performance: 'Excellent',
    },
    {
      key: '2',
      employeeId: 'TEL001002',
      name: 'Mary Cashier',
      role: 'Teller',
      shift: 'Afternoon (2PM-8PM)',
      status: 'Active',
      performance: 'Good',
    },
    {
      key: '3',
      employeeId: 'CSR001001',
      name: 'Peter Frontline',
      role: 'Customer Service Rep',
      shift: 'Full Day (8AM-5PM)',
      status: 'On Break',
      performance: 'Excellent',
    },
    {
      key: '4',
      employeeId: 'TEL001003',
      name: 'Sarah Operations',
      role: 'Teller',
      shift: 'Morning (8AM-2PM)',
      status: 'Active',
      performance: 'Good',
    },
    {
      key: '5',
      employeeId: 'LOAN001001',
      name: 'Patricia Loan',
      role: 'Loan Officer',
      shift: 'Full Day (8AM-5PM)',
      status: 'Active',
      performance: 'Excellent',
    },
  ];

  const columns: ColumnsType<StaffMember> = [
    {
      title: 'Employee ID',
      dataIndex: 'employeeId',
      key: 'employeeId',
    },
    {
      title: 'Name',
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: 'Role',
      dataIndex: 'role',
      key: 'role',
      render: (role: string) => <Tag color="blue">{role}</Tag>,
    },
    {
      title: 'Shift',
      dataIndex: 'shift',
      key: 'shift',
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => (
        <Tag color={status === 'Active' ? 'green' : 'orange'}>{status}</Tag>
      ),
    },
    {
      title: 'Performance',
      dataIndex: 'performance',
      key: 'performance',
      render: (performance: string) => (
        <Tag color={performance === 'Excellent' ? 'green' : 'blue'}>{performance}</Tag>
      ),
    },
    {
      title: 'Actions',
      key: 'actions',
      render: () => (
        <Space size="small">
          <Button type="link" size="small" icon={<EditOutlined />}>
            Edit
          </Button>
          <Button type="link" size="small" icon={<LockOutlined />}>
            Suspend
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <Card>
      <div style={{ marginBottom: '16px', display: 'flex', gap: '8px' }}>
        <Input
          placeholder="Search staff by name or ID..."
          prefix={<SearchOutlined />}
          style={{ width: '300px' }}
        />
        <Select
          defaultValue="all"
          style={{ width: '150px' }}
          options={[
            { value: 'all', label: 'All Roles' },
            { value: 'teller', label: 'Tellers' },
            { value: 'csr', label: 'CSR' },
            { value: 'loan', label: 'Loan Officers' },
          ]}
        />
        <Button type="primary" icon={<UserAddOutlined />}>
          Add Staff Member
        </Button>
      </div>
      <Table columns={columns} dataSource={staffData} pagination={{ pageSize: 10 }} />
    </Card>
  );
};

export default TeamManagementPanel;
