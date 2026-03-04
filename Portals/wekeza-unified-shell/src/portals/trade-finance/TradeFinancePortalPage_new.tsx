import React, { useState, useEffect } from 'react';
import { Tabs, Table, Spin, Alert, Button } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';

interface LetterOfCredit {
  LCId: string;
  Beneficiary: string;
  Amount: number;
  Status: string;
}

interface BankGuarantee {
  GuaranteeId: string;
  Beneficiary: string;
  Amount: number;
  Type: string;
}

const TradeFinancePortalPage: React.FC = () => {
  const [lcs, setLCs] = useState<LetterOfCredit[]>([]);
  const [guarantees, setGuarantees] = useState<BankGuarantee[]>([]);
  const [collections, setCollections] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);

      const token = localStorage.getItem('authToken');
      const headers = { 'Authorization': `Bearer ${token}` };

      const [lcsRes, guarsRes, collRes] = await Promise.all([
        fetch('/api/trade-finance-portal/letters-of-credit', { headers }),
        fetch('/api/trade-finance-portal/bank-guarantees', { headers }),
        fetch('/api/trade-finance-portal/documentary-collections', { headers })
      ]);

      if (lcsRes.ok) {
        const data = await lcsRes.json();
        setLCs(data.data || []);
      }

      if (guarsRes.ok) {
        const data = await guarsRes.json();
        setGuarantees(data.data || []);
      }

      if (collRes.ok) {
        const data = await collRes.json();
        setCollections(data.data || []);
      }
    } catch (err) {
      setError('Failed to load trade finance data');
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

  const lcColumns = [
    { title: 'LC ID', dataIndex: 'LCId', key: 'LCId' },
    { title: 'Beneficiary', dataIndex: 'Beneficiary', key: 'Beneficiary' },
    { title: 'Amount', dataIndex: 'Amount', key: 'Amount', render: (val: number) => `USD ${val.toLocaleString()}` },
    { title: 'Status', dataIndex: 'Status', key: 'Status' }
  ];

  const guarColumns = [
    { title: 'Guarantee ID', dataIndex: 'GuaranteeId', key: 'GuaranteeId' },
    { title: 'Beneficiary', dataIndex: 'Beneficiary', key: 'Beneficiary' },
    { title: 'Amount', dataIndex: 'Amount', key: 'Amount', render: (val: number) => `KES ${val.toLocaleString()}` },
    { title: 'Type', dataIndex: 'Type', key: 'Type' }
  ];

  const collColumns = [
    { title: 'Collection ID', dataIndex: 'CollectionId', key: 'CollectionId' },
    { title: 'Applicant', dataIndex: 'Applicant', key: 'Applicant' },
    { title: 'Amount', dataIndex: 'Amount', key: 'Amount', render: (val: number) => val.toLocaleString() },
    { title: 'Status', dataIndex: 'Status', key: 'Status' }
  ];

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
        <h1>Trade Finance Portal</h1>
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
              label: 'Letters of Credit',
              children: <Table columns={lcColumns} dataSource={lcs} rowKey="LCId" pagination={false} />
            },
            {
              key: '2',
              label: 'Bank Guarantees',
              children: <Table columns={guarColumns} dataSource={guarantees} rowKey="GuaranteeId" pagination={false} />
            },
            {
              key: '3',
              label: 'Documentary Collections',
              children: <Table columns={collColumns} dataSource={collections} rowKey="CollectionId" pagination={false} />
            }
          ]}
        />
      )}
    </div>
  );
};

export default TradeFinancePortalPage;
