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
  assets: { cash: number; loans: number; investments: number; total: number };
  liabilities: { deposits: number; borrowings: number; total: number };
  equity: { total: number };
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
    { title: 'Product ID', dataIndex: "productId", key: "productId" },
    { title: 'Name', dataIndex: "name", key: "name" },
    { title: 'Type', dataIndex: "type", key: "type" },
    { title: 'Interest Rate', dataIndex: "interestRate", key: "interestRate", render: (val: number) => `${val}%` },
    { title: 'Customers', dataIndex: "customers", key: "customers" }
  ];

  const feeColumns = [
    { title: 'Fee ID', dataIndex: "feeId", key: "feeId" },
    { title: 'Description', dataIndex: "description", key: "description" },
    { title: 'Amount', dataIndex: "amount", key: "amount", render: (val: number) => `KES ${val.toLocaleString()}` },
    { title: 'Frequency', dataIndex: "frequency", key: "frequency" }
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
              children: <Table columns={prodColumns} dataSource={products} rowKey="productId" pagination={false} />
            },
            {
              key: '2',
              label: 'General Ledger',
              children: glData ? (
                <Row gutter={[16, 16]}>
                  <Col xs={24} md={12}>
                    <Card title="Assets">
                      <Statistic title="Cash" value={Math.round((glData.assets?.cash ?? 0) / 1000000)} suffix="M KES" />
                      <Statistic title="Loans" value={Math.round((glData.assets?.loans ?? 0) / 1000000)} suffix="M KES" />
                      <Statistic title="Total Assets" value={Math.round((glData.assets?.total ?? 0) / 1000000)} suffix="M KES" />
                    </Card>
                  </Col>
                  <Col xs={24} md={12}>
                    <Card title="Liabilities & Equity">
                      <Statistic title="Deposits" value={Math.round((glData.liabilities?.deposits ?? 0) / 1000000)} suffix="M KES" />
                      <Statistic title="Equity" value={Math.round((glData.equity?.total ?? 0) / 1000000)} suffix="M KES" />
                      <Statistic title="Total Liabilities+Equity" value={Math.round(((glData.liabilities?.total ?? 0) + (glData.equity?.total ?? 0)) / 1000000)} suffix="M KES" />
                    </Card>
                  </Col>
                </Row>
              ) : null
            },
            {
              key: '3',
              label: 'Fee Configuration',
              children: <Table columns={feeColumns} dataSource={fees} rowKey="feeId" pagination={false} />
            }
          ]}
        />
      )}
    </div>
  );
};

export default ProductGLPortalPage;
