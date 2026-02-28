import React from 'react';
import { Tabs, Card, Table, Tag } from 'antd';

const WorkflowPortalPage: React.FC = () => {
  return (
    <div style={{ padding: '24px' }}>
      <h1>Workflow & Task Portal</h1>
      <p style={{ marginBottom: '24px' }}>Approval routing, workflow monitoring, and task execution.</p>
      <Tabs
        items={[
          {
            key: '1',
            label: 'My Tasks',
            children: (
              <Table
                pagination={false}
                dataSource={[
                  { key: '1', task: 'Approve high-value transfer', sla: '2h', priority: 'High' },
                  { key: '2', task: 'Review customer KYC exception', sla: '6h', priority: 'Medium' },
                ]}
                columns={[
                  { title: 'Task', dataIndex: 'task', key: 'task' },
                  { title: 'SLA', dataIndex: 'sla', key: 'sla' },
                  { title: 'Priority', dataIndex: 'priority', key: 'priority', render: (v: string) => <Tag color={v === 'High' ? 'red' : 'orange'}>{v}</Tag> },
                ]}
              />
            ),
          },
          {
            key: '2',
            label: 'Workflow Monitor',
            children: <Card>Track in-flight process instances and bottlenecks.</Card>,
          },
          {
            key: '3',
            label: 'Escalations',
            children: <Card>Escalation queue for breached SLAs and stuck tasks.</Card>,
          },
        ]}
      />
    </div>
  );
};

export default WorkflowPortalPage;
