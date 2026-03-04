import React, { useState, useEffect } from 'react';
import { Tabs, Card, Table, Row, Col, Statistic, Spin, Alert, Button } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';

interface LiquidityData {
  LCR: number;
  NSFR: number;
  NetLiquidityPosition: number;
  CashReserves: number;
  ExcessLiquidity: number;
}

interface FXDeal {
  DealId: string;
  Pair: string;
  Amount: number;
  Rate: number;
  Direction: string;
  Status: string;
}

const TreasuryPortalPage: React.FC = () => {
  const [liquidity, setLiquidity] = useState<LiquidityData | null>(null);
  const [fxDeals, setFxDeals] = useState<FXDeal[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);

      const token = localStorage.getItem('authToken');
      const headers = { 'Authorization': `Bearer ${token}` };

      const [liquidityRes, fxRes] = await Promise.all([
        fetch('/api/treasury-portal/liquidity', { headers }),
        fetch('/api/treasury-portal/fx-deals', { headers })
      ]);

      if (liquidityRes.ok) {
        const data = await liquidityRes.json();
        setLiquidity(data.data);
      }

      if (fxRes.ok) {
        const data = await fxRes.json();
        setFxDeals(data.data || []);
      }
    } catch (err) {
      setError('Failed to load treasury data');
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

  const fxColumns = [
    { title: 'Deal ID', dataIndex: 'DealId', key: 'DealId' },
    { title: 'Pair', dataIndex: 'Pair', key: 'Pair' },
    { title: 'Amount', dataIndex: 'Amount', key: 'Amount', render: (val: number) => val.toLocaleString() },
    { title: 'Rate', dataIndex: 'Rate', key: 'Rate' },
    { title: 'Direction', dataIndex: 'Direction', key: 'Direction' },
    { title: 'Status', dataIndex: 'Status', key: 'Status' }
  ];

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
        <h1>Treasury & Markets Portal</h1>
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
              label: 'Liquidity',
              children: liquidity ? (
                <Row gutter={[16, 16]}>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic 
                        title="LCR" 
                        value={liquidity.LCR.toFixed(2)}
                        suffix="%"
                        valueStyle={{ color: liquidity.LCR >= 100 ? 'green' : 'red' }}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic 
                        title="NSFR" 
                        value={liquidity.NSFR.toFixed(2)}
                        suffix="%"
                      />
                    </Card>
                  </Col>
                  <Col xs={24} md={8}>
                    <Card>
                      <Statistic 
                        title="Net Position" 
                        value={Math.round(liquidity.NetLiquidityPosition / 1000000)}
                        prefix="KES "
                        suffix="M"
                      />
                    </Card>
                  </Col>
                </Row>
              ) : null,
            },
            {
              key: '2',
              label: 'FX Deals',
              children: (
                <Table
                  columns={fxColumns}
                  dataSource={fxDeals}
                  rowKey="DealId"
                  pagination={false}
                  loading={loading}
                />
              ),
            },
            {
              key: '3',
              label: 'Money Market',
              children: <Card>Interbank placements, borrowings, and maturity ladder.</Card>,
            },
          ]}
        />
      )}
    </div>
  );
};

export default TreasuryPortalPage;
