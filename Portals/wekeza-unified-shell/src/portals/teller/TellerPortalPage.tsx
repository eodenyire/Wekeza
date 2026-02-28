import React from 'react';
import { Tabs } from 'antd';
import { TellerDashboard } from './components/TellerDashboard';
import { TellerSessionPanel } from './components/TellerSessionPanel';
import { CashOperationsPanel } from './components/CashOperationsPanel';
import { CustomerServicesPanel } from './components/CustomerServicesPanel';

export const TellerPortalPage: React.FC = () => {
  return (
    <div>
      <Tabs
        defaultActiveKey="dashboard"
        items={[
          {
            key: 'dashboard',
            label: 'Dashboard',
            children: <TellerDashboard />,
          },
          {
            key: 'session',
            label: 'Session Management',
            children: <TellerSessionPanel />,
          },
          {
            key: 'cash',
            label: 'Cash Operations',
            children: <CashOperationsPanel />,
          },
          {
            key: 'customer',
            label: 'Customer Services',
            children: <CustomerServicesPanel />,
          },
        ]}
      />
    </div>
  );
};
