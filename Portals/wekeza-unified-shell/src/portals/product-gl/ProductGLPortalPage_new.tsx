import React, { useState, useEffect } from 'react';
import { Tabs, Card, Table, Row, Col, Statistic, Spin, Alert, Button } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';

interface Product {
  ProductId: string;
  Name: string;
  Type: string;
  InterestRate: number;
  Customers: number;
}

interface GLData {
  Assets: { Cash: number; Loans: number; Investments: number; Total: number };
  Liabilities: { Deposits: number; Borrowings: number; Total: number };
  Equity: { Total: number };
}

const ProductGLPortalPage: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [glData, setGLData] = useState<GLData | null>(null);
  const [fees, setFees] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);

      const token = localStorage.getItem('authToken');
      const headers = { 'Authorization': `Bearer ${token}` };

      const [prodRes, glRes, feesRes] = await Promise.all([
        fetch('/api/product-gl-portal/products', { headers }),
        fetch('/api/product-gl-portal/gl-summary', { headers }),
        fetch('/api/product-gl-portal/fees', { headers })
      ]);

      if (prodRes.ok) {
        const data = await prodRes.json();
        setProducts(data.data || []);
      }

      if (glRes.ok) {
        const data = await glRes.json();
        setGLData(data.data);
      }

      if (feesRes.ok) {
        const data = await feesRes.json();
        setFees(data.data || []);
      }
    } catch (err) {
      setError('Failed to load product/GL data');
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

  const prodColumns = [
    { title: 'Product ID', dataIndex: 'ProductId', key: 'ProductId' },
    { title: 'Name', dataIndex: 'Name', key: 'Name' },
    { title: 'Type', dataIndex: 'Type', key: 'Type' },
    { title: 'Interest Rate', dataIndex: 'InterestRate', key: 'InterestRate', render: (val: number) => `${val}%` },
    { title: 'Customers', dataIndex: 'Customers', key: 'Customers' }
  ];

  const feeColumns = [
    { title: 'Fee ID', dataIndex: 'FeeId', key: 'FeeId' },
    { title: 'Description', dataIndex: 'Description', key: 'Description' },
    { title: 'Amount', dataIndex: 'Amount', key: 'Amount', render: (val: number) => `KES ${val.toLocaleString()}` },
    { title: 'Frequency', dataIndex: 'Frequency', key: 'Frequency' }
  ];

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
        <h1>Product & GL Management Portal</h1>
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
              label: 'Products',
              children: <Table columns={prodColumns} dataSource={products} rowKey="ProductId" pagination={false} />
            },
            {
              key: '2',
              label: 'General Ledger',
              children: glData ? (
                <Row gutter={[16, 16]}>
                  <Col xs={24} md={12}>
                    <Card title="Assets">
                      <Statistic title="Cash" value={Math.round(glData.Assets.Cash / 1000000)} suffix="M KES" />
                      <Statistic title="Loans" value={Math.round(glData.Assets.Loans / 1000000)} suffix="M KES" />
                      <Statistic title="Total Assets" value={Math.round(glData.Assets.Total / 1000000)} suffix="M KES" />
                    </Card>
                  </Col>
                  <Col xs={24} md={12}>
                    <Card title="Liabilities & Equity">
                      <Statistic title="Deposits" value={Math.round(glData.Liabilities.Deposits / 1000000)} suffix="M KES" />
                      <Statistic title="Equity" value={Math.round(glData.Equity.Total / 1000000)} suffix="M KES" />
                      <Statistic title="Total Liabilities+Equity" value={Math.round((glData.Liabilities.Total + glData.Equity.Total) / 1000000)} suffix="M KES" />
                    </Card>
                  </Col>
                </Row>
              ) : null
            },
            {
              key: '3',
              label: 'Fee Configuration',
              children: <Table columns={feeColumns} dataSource={fees} rowKey="FeeId" pagination={false} />
            }
          ]}
        />
      )}
    </div>
  );
};

export default ProductGLPortalPage;
