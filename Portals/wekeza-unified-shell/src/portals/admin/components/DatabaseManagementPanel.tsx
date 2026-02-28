import React from 'react';
import { Card, Col, Row, Statistic, Button, Space } from 'antd';
import { DatabaseOutlined, CloudUploadOutlined, CloudDownloadOutlined } from '@ant-design/icons';

const DatabaseManagementPanel: React.FC = () => {
  return (
    <div>
      <Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
        <Col xs={24} md={8}><Card><Statistic title="Database Size" value={28.4} suffix="GB" prefix={<DatabaseOutlined />} /></Card></Col>
        <Col xs={24} md={8}><Card><Statistic title="Active Connections" value={74} /></Card></Col>
        <Col xs={24} md={8}><Card><Statistic title="Replication Lag" value={0.8} suffix="s" /></Card></Col>
      </Row>

      <Card title="Maintenance Actions">
        <Space>
          <Button icon={<CloudDownloadOutlined />}>Run Backup</Button>
          <Button icon={<CloudUploadOutlined />}>Restore Snapshot</Button>
          <Button danger>Vacuum Analyze</Button>
        </Space>
      </Card>
    </div>
  );
};

export default DatabaseManagementPanel;
