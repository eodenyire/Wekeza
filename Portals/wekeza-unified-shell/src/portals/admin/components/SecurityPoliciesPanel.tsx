import React from 'react';
import { Card, Form, InputNumber, Select, Switch, Button, Space } from 'antd';

const SecurityPoliciesPanel: React.FC = () => {
	return (
		<Card title="Security Policies">
			<Form layout="vertical" initialValues={{ mfaRequired: true, sessionTimeout: 30, passwordMinLength: 12, lockoutThreshold: 5 }}>
				<Form.Item label="MFA Required" name="mfaRequired" valuePropName="checked">
					<Switch />
				</Form.Item>
				<Form.Item label="Session Timeout (minutes)" name="sessionTimeout">
					<InputNumber min={5} max={240} style={{ width: '100%' }} />
				</Form.Item>
				<Form.Item label="Password Minimum Length" name="passwordMinLength">
					<InputNumber min={8} max={32} style={{ width: '100%' }} />
				</Form.Item>
				<Form.Item label="Account Lockout Threshold" name="lockoutThreshold">
					<InputNumber min={3} max={10} style={{ width: '100%' }} />
				</Form.Item>
				<Form.Item label="Allowed Login Regions" name="regions" initialValue={['KE', 'UG', 'TZ']}>
					<Select mode="multiple" options={[{ value: 'KE' }, { value: 'UG' }, { value: 'TZ' }, { value: 'RW' }]} />
				</Form.Item>
				<Space>
					<Button type="primary">Save Policy</Button>
					<Button>Reset</Button>
				</Space>
			</Form>
		</Card>
	);
};

export default SecurityPoliciesPanel;
