import React from 'react';
import { Button, Card, Space, Table, Tag } from 'antd';

const users = [
	{ key: '1', username: 'admin', role: 'SystemAdministrator', status: 'Active' },
	{ key: '2', username: 'teller', role: 'Teller', status: 'Active' },
	{ key: '3', username: 'riskofficer', role: 'RiskOfficer', status: 'Locked' },
];

const UserManagementPanel: React.FC = () => {
	return (
		<Card
			title="User Management"
			extra={<Button type="primary">Create User</Button>}
		>
			<Table
				dataSource={users}
				pagination={false}
				columns={[
					{ title: 'Username', dataIndex: 'username', key: 'username' },
					{ title: 'Role', dataIndex: 'role', key: 'role' },
					{
						title: 'Status',
						dataIndex: 'status',
						key: 'status',
						render: (value: string) => <Tag color={value === 'Active' ? 'green' : 'red'}>{value}</Tag>,
					},
					{
						title: 'Actions',
						key: 'actions',
						render: () => (
							<Space>
								<Button size="small">Edit</Button>
								<Button size="small" danger>Disable</Button>
							</Space>
						),
					},
				]}
			/>
		</Card>
	);
};

export default UserManagementPanel;
