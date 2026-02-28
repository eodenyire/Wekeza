import React from 'react';
import { Card, Col, Row, Statistic, Table, Tag } from 'antd';
import { SafetyOutlined, TeamOutlined, DatabaseOutlined, WarningOutlined } from '@ant-design/icons';

const incidents = [
	{ key: '1', id: 'INC-001', severity: 'High', service: 'Auth API', status: 'Open' },
	{ key: '2', id: 'INC-002', severity: 'Medium', service: 'Payments API', status: 'Investigating' },
	{ key: '3', id: 'INC-003', severity: 'Low', service: 'Reporting', status: 'Resolved' },
];

const AdminDashboard: React.FC = () => {
	return (
		<div>
			<Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
				<Col xs={24} md={6}><Card><Statistic title="Active Users" value={284} prefix={<TeamOutlined />} /></Card></Col>
				<Col xs={24} md={6}><Card><Statistic title="Security Alerts" value={7} prefix={<SafetyOutlined />} /></Card></Col>
				<Col xs={24} md={6}><Card><Statistic title="System Health" value={99.8} suffix="%" prefix={<DatabaseOutlined />} /></Card></Col>
				<Col xs={24} md={6}><Card><Statistic title="Open Incidents" value={3} prefix={<WarningOutlined />} /></Card></Col>
			</Row>

			<Card title="Recent Incidents">
				<Table
					dataSource={incidents}
					pagination={false}
					columns={[
						{ title: 'Incident ID', dataIndex: 'id', key: 'id' },
						{
							title: 'Severity',
							dataIndex: 'severity',
							key: 'severity',
							render: (value: string) => <Tag color={value === 'High' ? 'red' : value === 'Medium' ? 'orange' : 'green'}>{value}</Tag>,
						},
						{ title: 'Service', dataIndex: 'service', key: 'service' },
						{ title: 'Status', dataIndex: 'status', key: 'status' },
					]}
				/>
			</Card>
		</div>
	);
};

export default AdminDashboard;
