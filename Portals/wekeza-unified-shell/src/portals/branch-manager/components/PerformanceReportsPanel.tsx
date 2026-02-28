import React from 'react';
import { Card, Row, Col, Statistic, Button, Select, Space } from 'antd';
import { DownloadOutlined, PrinterOutlined, LineChartOutlined } from '@ant-design/icons';
import { LineChart, Line, BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

const PerformanceReportsPanel: React.FC = () => {
  const weeklyData = [
    { day: 'Mon', transactions: 245, amount: 12.5 },
    { day: 'Tue', transactions: 289, amount: 14.2 },
    { day: 'Wed', transactions: 312, amount: 15.8 },
    { day: 'Thu', transactions: 276, amount: 13.9 },
    { day: 'Fri', transactions: 398, amount: 19.7 },
    { day: 'Sat', transactions: 156, amount: 7.8 },
  ];

  const tellerComparison = [
    { name: 'John T.', transactions: 487, efficiency: 95 },
    { name: 'Mary C.', transactions: 412, efficiency: 88 },
    { name: 'Peter F.', transactions: 523, efficiency: 92 },
    { name: 'Sarah O.', transactions: 445, efficiency: 90 },
  ];

  return (
    <div>
      <Card 
        title="Branch Performance Summary" 
        extra={
          <Space>
            <Select
              defaultValue="week"
              style={{ width: 120 }}
              options={[
                { value: 'today', label: 'Today' },
                { value: 'week', label: 'This Week' },
                { value: 'month', label: 'This Month' },
                { value: 'quarter', label: 'This Quarter' },
              ]}
            />
            <Button icon={<PrinterOutlined />}>Print</Button>
            <Button type="primary" icon={<DownloadOutlined />}>Export Report</Button>
          </Space>
        }
      >
        <Row gutter={[16, 16]}>
          <Col xs={24} md={6}>
            <Statistic
              title="Total Transactions"
              value={1676}
              prefix={<LineChartOutlined />}
              valueStyle={{ color: '#3f8600' }}
            />
          </Col>
          <Col xs={24} md={6}>
            <Statistic
              title="Total Amount"
              value="KES 83.9M"
              valueStyle={{ color: '#3f8600' }}
            />
          </Col>
          <Col xs={24} md={6}>
            <Statistic
              title="Avg per Transaction"
              value="KES 50,060"
            />
          </Col>
          <Col xs={24} md={6}>
            <Statistic
              title="Branch Efficiency"
              value={91.3}
              suffix="%"
              valueStyle={{ color: '#3f8600' }}
            />
          </Col>
        </Row>
      </Card>

      <Row gutter={[16, 16]} style={{ marginTop: '16px' }}>
        <Col xs={24} lg={12}>
          <Card title="Weekly Transaction Trend" size="small">
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={weeklyData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="day" />
                <YAxis yAxisId="left" />
                <YAxis yAxisId="right" orientation="right" />
                <Tooltip />
                <Legend />
                <Line yAxisId="left" type="monotone" dataKey="transactions" stroke="#8884d8" name="Transactions" />
                <Line yAxisId="right" type="monotone" dataKey="amount" stroke="#82ca9d" name="Amount (M)" />
              </LineChart>
            </ResponsiveContainer>
          </Card>
        </Col>
        <Col xs={24} lg={12}>
          <Card title="Teller Performance Comparison" size="small">
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={tellerComparison}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="transactions" fill="#8884d8" name="Transactions" />
                <Bar dataKey="efficiency" fill="#82ca9d" name="Efficiency %" />
              </BarChart>
            </ResponsiveContainer>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default PerformanceReportsPanel;
