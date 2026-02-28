import React from 'react';
import { Tabs } from 'antd';
import type { TabsProps } from 'antd';
import { 
  DashboardOutlined, 
  UserOutlined, 
  SafetyOutlined, 
  SettingOutlined,
  DatabaseOutlined,
  AuditOutlined,
  ApiOutlined
} from '@ant-design/icons';
import AdminDashboard from './components/AdminDashboard';
import UserManagementPanel from './components/UserManagementPanel';
import SecurityPoliciesPanel from './components/SecurityPoliciesPanel';
import SystemConfigurationPanel from './components/SystemConfigurationPanel';
import DatabaseManagementPanel from './components/DatabaseManagementPanel';
import AuditLogsPanel from './components/AuditLogsPanel';
import SystemIntegrationsPanel from './components/SystemIntegrationsPanel';

const AdminPortalPage: React.FC = () => {
  const items: TabsProps['items'] = [
    {
      key: 'dashboard',
      label: (
        <span>
          <DashboardOutlined />
          System Overview
        </span>
      ),
      children: <AdminDashboard />,
    },
    {
      key: 'users',
      label: (
        <span>
          <UserOutlined />
          User Management
        </span>
      ),
      children: <UserManagementPanel />,
    },
    {
      key: 'security',
      label: (
        <span>
          <SafetyOutlined />
          Security Policies
        </span>
      ),
      children: <SecurityPoliciesPanel />,
    },
    {
      key: 'config',
      label: (
        <span>
          <SettingOutlined />
          System Configuration
        </span>
      ),
      children: <SystemConfigurationPanel />,
    },
    {
      key: 'database',
      label: (
        <span>
          <DatabaseOutlined />
          Database Management
        </span>
      ),
      children: <DatabaseManagementPanel />,
    },
    {
      key: 'audit',
      label: (
        <span>
          <AuditOutlined />
          Audit Logs
        </span>
      ),
      children: <AuditLogsPanel />,
    },
    {
      key: 'integrations',
      label: (
        <span>
          <ApiOutlined />
          System Integrations
        </span>
      ),
      children: <SystemIntegrationsPanel />,
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <h1>System Administration Portal</h1>
      <p style={{ marginBottom: '24px' }}>
        Central system management, security, and configuration
      </p>
      <Tabs defaultActiveKey="dashboard" items={items} />
    </div>
  );
};

export default AdminPortalPage;
