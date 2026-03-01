import React, { useState, useEffect } from 'react';
import { Card, Col, Row, Statistic, Table, Tag, Button, Spin, Alert } from 'antd';
import { SafetyOutlined, TeamOutlined, DatabaseOutlined, WarningOutlined, ReloadOutlined } from '@ant-design/icons';

interface DashboardStats {
	activeUsers: number;
	totalCustomers: number;
	totalAccounts: number;
	totalBalance: number;
	transactionsToday: number;
	pendingApprovals: number;
	systemHealth: number;
	incidents: Array<{
		Id: string;
		Severity: string;
		Service: string;
		Status: string;
		Timestamp: string;
	}>;
	lastUpdated: string;
}

const AdminDashboard: React.FC = () => {
	const [stats, setStats] = useState<DashboardStats | null>(null);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);

	const fetchStats = async () => {
		setLoading(true);
		setError(null);
		try {
			const token = localStorage.getItem('authToken');
			const response = await fetch('/admin/dashboard/stats', {
				headers: { 'Authorization': `Bearer ${token}` }
			});
			if (!response.ok) throw new Error('Failed to fetch dashboard stats');
			const data = await response.json();
			setStats(data);
		} catch (err: any) {
			setError(err.message || 'Failed to load dashboard data');
		} finally {
			setLoading(false);
		}
	};

	useEffect(() => {
		fetchStats();
		const interval = setInterval(fetchStats, 30000); // Auto-refresh every 30 seconds
		return () => clearInterval(interval);
	}, []);

	if (loading && !stats) {
		return (
			<div style={{ padding: '40px 0', textAlign: 'center' }}>
				<Spin size="large" tip="Loading dashboard..." />
			</div>
		);
	}

	return (
		<div>
			<div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
				<h3 style={{ margin: 0 }}>System Overview</h3>
				<Button icon={<ReloadOutlined />} onClick={fetchStats} loading={loading}>
					Refresh
				</Button>
			</div>

			{error && (
				<Alert
					message="Error"
					description={error}
					type="error"
					closable
					onClose={() => setError(null)}
					style={{ marginBottom: 16 }}
				/>
			)}

			{stats && (
				<>
					<Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
						<Col xs={24} md={6}>
							<Card>
								<Statistic 
									title="Active Users" 
									value={stats.activeUsers} 
									prefix={<TeamOutlined />} 
								/>
							</Card>
						</Col>
						<Col xs={24} md={6}>
							<Card>
								<Statistic 
									title="Total Customers" 
									value={stats.totalCustomers} 
									prefix={<TeamOutlined />} 
								/>
							</Card>
						</Col>
						<Col xs={24} md={6}>
							<Card>
								<Statistic 
									title="System Health" 
									value={stats.systemHealth} 
									suffix="%" 
									prefix={<DatabaseOutlined />}
									valueStyle={{ color: stats.systemHealth >= 95 ? '#3f8600' : '#cf1322' }}
								/>
							</Card>
						</Col>
						<Col xs={24} md={6}>
							<Card>
								<Statistic 
									title="Transactions Today" 
									value={stats.transactionsToday} 
									prefix={<WarningOutlined />} 
								/>
							</Card>
						</Col>
					</Row>

					<Row gutter={[16, 16]}>
						<Col xs={24} md={12}>
							<Card>
								<Statistic 
									title="Total Accounts" 
									value={stats.totalAccounts}
									prefix={<DatabaseOutlined />}
								/>
							</Card>
						</Col>
						<Col xs={24} md={12}>
							<Card>
								<Statistic 
									title="Total Balance" 
									value={stats.totalBalance}
									precision={2}
									suffix="KES"
									prefix={<DatabaseOutlined />}
								/>
							</Card>
						</Col>
					</Row>

					<Card 
						title="Recent System Events" 
						style={{ marginTop: 16 }}
						extra={<small>Last updated: {new Date(stats.lastUpdated).toLocaleTimeString()}</small>}
					>
						{stats.incidents && stats.incidents.length > 0 ? (
							<Table
								dataSource={stats.incidents}
								pagination={false}
								rowKey="Id"
								columns={[
									{ title: 'Event ID', dataIndex: 'Id', key: 'Id' },
									{
										title: 'Severity',
										dataIndex: 'Severity',
										key: 'Severity',
										render: (value: string) => (
											<Tag color={value === 'High' ? 'red' : value === 'Medium' ? 'orange' : 'green'}>
												{value}
											</Tag>
										),
									},
									{ title: 'Service', dataIndex: 'Service', key: 'Service' },
									{ title: 'Status', dataIndex: 'Status', key: 'Status' },
									{
										title: 'Time',
										dataIndex: 'Timestamp',
										key: 'Timestamp',
										render: (time: string) => new Date(time).toLocaleTimeString()
									}
								]}
							/>
						) : (
							<div style={{ textAlign: 'center', padding: '20px 0' }}>
								<p>No recent incidents - System running smoothly</p>
							</div>
						)}
					</Card>
				</>
			)}
		</div>
	);
};

export default AdminDashboard;
