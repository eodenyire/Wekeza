import React from 'react';
import { Card, List, Switch, Tag } from 'antd';

const integrations = [
  { name: 'Core Banking API Gateway', status: 'Connected', enabled: true },
  { name: 'Fraud Monitoring Engine', status: 'Connected', enabled: true },
  { name: 'SWIFT Connector', status: 'Degraded', enabled: true },
  { name: 'SMS Notification Provider', status: 'Disconnected', enabled: false },
];

const SystemIntegrationsPanel: React.FC = () => {
  return (
    <Card title="System Integrations">
      <List
        dataSource={integrations}
        renderItem={(item) => (
          <List.Item actions={[<Switch key={item.name} defaultChecked={item.enabled} />]}>
            <List.Item.Meta title={item.name} description={<Tag color={item.status === 'Connected' ? 'green' : item.status === 'Degraded' ? 'orange' : 'red'}>{item.status}</Tag>} />
          </List.Item>
        )}
      />
    </Card>
  );
};

export default SystemIntegrationsPanel;
