import React from 'react';
import { Card, Table, Tag, Input, Select, DatePicker, Space, Button } from 'antd';
import { SearchOutlined, FilterOutlined, DownloadOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';

const { RangePicker } = DatePicker;

interface Transaction {
  key: string;
  time: string;
  type: string;
  teller: string;
  customer: string;
  amount: string;
  status: string;
  risk: string;
}

const TransactionMonitoringPanel: React.FC = () => {
  const transactionData: Transaction[] = [
    {
      key: '1',
      time: '10:45 AM',
      type: 'Cash Deposit',
      teller: 'John Teller',
      customer: 'ACC-12345',
      amount: 'KES 450,000',
      status: 'Completed',
      risk: 'Low',
    },
    {
      key: '2',
      time: '10:42 AM',
      type: 'Cash Withdrawal',
      teller: 'Mary Cashier',
      customer: 'ACC-67890',
      amount: 'KES 1,200,000',
      status: 'Pending Approval',
      risk: 'High',
    },
    {
      key: '3',
      time: '10:38 AM',
      type: 'Fund Transfer',
      teller: 'Peter Frontline',
      customer: 'ACC-54321',
      amount: 'KES 85,000',
      status: 'Completed',
      risk: 'Low',
    },
    {
      key: '4',
      time: '10:35 AM',
      type: 'Account Opening',
      teller: 'Sarah Operations',
      customer: 'New Customer',
      amount: 'KES 10,000',
      status: 'In Progress',
      risk: 'Medium',
    },
    {
      key: '5',
      time: '10:30 AM',
      type: 'Cash Deposit',
      teller: 'John Teller',
      customer: 'ACC-98765',
      amount: 'KES 75,000',
      status: 'Completed',
      risk: 'Low',
    },
  ];

  const columns: ColumnsType<Transaction> = [
    {
      title: 'Time',
      dataIndex: 'time',
      key: 'time',
      width: 100,
    },
    {
      title: 'Type',
      dataIndex: 'type',
      key: 'type',
      render: (type: string) => <Tag color="blue">{type}</Tag>,
    },
    {
      title: 'Teller',
      dataIndex: 'teller',
      key: 'teller',
    },
    {
      title: 'Customer',
      dataIndex: 'customer',
      key: 'customer',
    },
    {
      title: 'Amount',
      dataIndex: 'amount',
      key: 'amount',
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => {
        const color = status === 'Completed' ? 'green' : status === 'Pending Approval' ? 'orange' : 'blue';
        return <Tag color={color}>{status}</Tag>;
      },
    },
    {
      title: 'Risk Level',
      dataIndex: 'risk',
      key: 'risk',
      render: (risk: string) => {
        const color = risk === 'Low' ? 'green' : risk === 'Medium' ? 'orange' : 'red';
        return <Tag color={color}>{risk}</Tag>;
      },
    },
    {
      title: 'Actions',
      key: 'actions',
      render: () => (
        <Button type="link" size="small">View Details</Button>
      ),
    },
  ];

  return (
    <Card>
      <Space direction="vertical" style={{ width: '100%', marginBottom: '16px' }}>
        <Space wrap>
          <Input
            placeholder="Search transactions..."
            prefix={<SearchOutlined />}
            style={{ width: '250px' }}
          />
          <Select
            defaultValue="all"
            style={{ width: '150px' }}
            options={[
              { value: 'all', label: 'All Types' },
              { value: 'deposit', label: 'Deposits' },
              { value: 'withdrawal', label: 'Withdrawals' },
              { value: 'transfer', label: 'Transfers' },
            ]}
          />
          <Select
            defaultValue="all"
            style={{ width: '150px' }}
            options={[
              { value: 'all', label: 'All Status' },
              { value: 'completed', label: 'Completed' },
              { value: 'pending', label: 'Pending' },
              { value: 'progress', label: 'In Progress' },
            ]}
          />
          <RangePicker />
          <Button icon={<FilterOutlined />}>Filter</Button>
          <Button icon={<DownloadOutlined />}>Export</Button>
        </Space>
      </Space>
      
      <Table 
        columns={columns} 
        dataSource={transactionData} 
        pagination={{ pageSize: 10 }}
        size="small"
      />
    </Card>
  );
};

export default TransactionMonitoringPanel;
