import React from 'react';
import { Tabs } from 'antd';
import type { TabsProps } from 'antd';
import { 
  DashboardOutlined, 
  TeamOutlined, 
  DollarOutlined, 
  FileTextOutlined,
  AuditOutlined,
  SettingOutlined
} from '@ant-design/icons';
import BranchDashboard from './components/BranchDashboard';
import TeamManagementPanel from './components/TeamManagementPanel';
import CashManagementPanel from './components/CashManagementPanel';
import TransactionMonitoringPanel from './components/TransactionMonitoringPanel';
import PerformanceReportsPanel from './components/PerformanceReportsPanel';
import BranchSettingsPanel from './components/BranchSettingsPanel';

const BranchManagerPortalPage: React.FC = () => {
  const items: TabsProps['items'] = [
    {
      key: 'dashboard',
      label: (
        <span>
          <DashboardOutlined />
          Dashboard
        </span>
      ),
      children: <BranchDashboard />,
    },
    {
      key: 'team',
      label: (
        <span>
          <TeamOutlined />
          Team Management
        </span>
      ),
      children: <TeamManagementPanel />,
    },
    {
      key: 'cash',
      label: (
        <span>
          <DollarOutlined />
          Cash Management
        </span>
      ),
      children: <CashManagementPanel />,
    },
    {
      key: 'monitoring',
      label: (
        <span>
          <AuditOutlined />
          Transaction Monitoring
        </span>
      ),
      children: <TransactionMonitoringPanel />,
    },
    {
      key: 'reports',
      label: (
        <span>
          <FileTextOutlined />
          Performance Reports
        </span>
      ),
      children: <PerformanceReportsPanel />,
    },
    {
      key: 'settings',
      label: (
        <span>
          <SettingOutlined />
          Branch Settings
        </span>
      ),
      children: <BranchSettingsPanel />,
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <h1>Branch Manager Portal</h1>
      <p style={{ marginBottom: '24px' }}>
        Branch operations oversight, team management, and performance tracking
      </p>
      <Tabs defaultActiveKey="dashboard" items={items} />
    </div>
  );
};

export default BranchManagerPortalPage;
