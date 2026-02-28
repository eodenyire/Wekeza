import React from 'react';
import { Card, Row, Col, Statistic, Button, Table, Progress, Space } from 'antd';
import { DollarOutlined, ArrowUpOutlined, ArrowDownOutlined, ReloadOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';

interface CashPosition {
  key: string;
  denomination: string;
  quantity: number;
  value: string;
  percentage: number;
}

const CashManagementPanel: React.FC = () => {
  const cashData: CashPosition[] = [
    { key: '1', denomination: 'KES 1,000', quantity: 3500, value: 'KES 3,500,000', percentage: 37 },
    { key: '2', denomination: 'KES 500', quantity: 4200, value: 'KES 2,100,000', percentage: 22 },
    { key: '3', denomination: 'KES 200', quantity: 5000, value: 'KES 1,000,000', percentage: 11 },
    { key: '4', denomination: 'KES 100', quantity: 8000, value: 'KES 800,000', percentage: 8 },
    { key: '5', denomination: 'KES 50', quantity: 12000, value: 'KES 600,000', percentage: 6 },
    { key: '6', denomination: 'Coins', quantity: 0, value: 'KES 1,500,000', percentage: 16 },
  ];

  const columns: ColumnsType<CashPosition> = [
    {
      title: 'Denomination',
      dataIndex: 'denomination',
      key: 'denomination',
    },
    {
      title: 'Quantity',
      dataIndex: 'quantity',
      key: 'quantity',
      render: (qty: number) => qty.toLocaleString(),
    },
    {
      title: 'Value',
      dataIndex: 'value',
      key: 'value',
    },
    {
      title: 'Distribution',
      dataIndex: 'percentage',
      key: 'percentage',
      render: (percentage: number) => (
        <Progress percent={percentage} size="small" />
      ),
    },
  ];

  return (
    <div>
      <Row gutter={[16, 16]}>
        <Col xs={24} md={8}>
          <Card>
            <Statistic
              title="Total Branch Cash"
              value="KES 9,500,000"
              precision={0}
              valueStyle={{ color: '#3f8600' }}
              prefix={<DollarOutlined />}
            />
          </Card>
        </Col>
        <Col xs={24} md={8}>
          <Card>
            <Statistic
              title="Cash Received Today"
              value="KES 2,340,000"
              precision={0}
              valueStyle={{ color: '#3f8600' }}
              prefix={<ArrowDownOutlined />}
            />
          </Card>
        </Col>
        <Col xs={24} md={8}>
          <Card>
            <Statistic
              title="Cash Paid Today"
              value="KES 1,890,000"
              precision={0}
              valueStyle={{ color: '#cf1322' }}
              prefix={<ArrowUpOutlined />}
            />
          </Card>
        </Col>
      </Row>

      <Card title="Cash Position by Denomination" style={{ marginTop: '16px' }}>
        <Table 
          columns={columns} 
          dataSource={cashData} 
          pagination={false}
          size="small"
        />
      </Card>

      <Row gutter={[16, 16]} style={{ marginTop: '16px' }}>
        <Col xs={24} md={12}>
          <Card 
            title="Vault Management" 
            size="small"
            extra={<Button type="primary" icon={<ReloadOutlined />}>Request CIT</Button>}
          >
            <Space direction="vertical" style={{ width: '100%' }}>
              <div>
                <strong>Opening Balance:</strong> KES 9,050,000
              </div>
              <div>
                <strong>Current Balance:</strong> KES 9,500,000
              </div>
              <div>
                <strong>Variance:</strong> <span style={{ color: '#3f8600' }}>+KES 450,000</span>
              </div>
              <div>
                <strong>Last CIT Delivery:</strong> 2 days ago
              </div>
            </Space>
          </Card>
        </Col>
        <Col xs={24} md={12}>
          <Card title="ATM Cash Management" size="small">
            <Space direction="vertical" style={{ width: '100%' }}>
              <div>
                <strong>ATM 1 (Main Branch):</strong> KES 2,400,000 
                <Progress percent={80} size="small" status="success" />
              </div>
              <div>
                <strong>ATM 2 (Back Office):</strong> KES 890,000
                <Progress percent={30} size="small" status="exception" />
              </div>
              <div>
                <strong>ATM 3 (External):</strong> KES 1,650,000
                <Progress percent={55} size="small" status="normal" />
              </div>
              <Button type="primary" danger size="small">Request ATM Refill</Button>
            </Space>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default CashManagementPanel;
