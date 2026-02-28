import React from 'react';
import { Card, Table, Tag } from 'antd';

const data = [
  { key: '1', actor: 'admin', action: 'Updated security policy', module: 'Security', status: 'Success' },
  { key: '2', actor: 'system', action: 'Scheduled backup', module: 'Database', status: 'Success' },
  { key: '3', actor: 'riskofficer', action: 'Failed login attempt', module: 'Authentication', status: 'Warning' },
];

const AuditLogsPanel: React.FC = () => {
  return (
    <Card title="Audit Logs">
      <Table
        dataSource={data}
        pagination={false}
        columns={[
          { title: 'Actor', dataIndex: 'actor', key: 'actor' },
          { title: 'Action', dataIndex: 'action', key: 'action' },
          { title: 'Module', dataIndex: 'module', key: 'module' },
          {
            title: 'Status',
            dataIndex: 'status',
            key: 'status',
            render: (value: string) => <Tag color={value === 'Success' ? 'green' : 'orange'}>{value}</Tag>,
          },
        ]}
      />
    </Card>
  );
};

export default AuditLogsPanel;
