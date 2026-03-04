import React from 'react';
import { Alert, Button, Card, Col, DatePicker, Form, Input, InputNumber, Row, Space, message } from 'antd';
import { tellerApi } from '../services/tellerApi';

const pick = (payload: Record<string, unknown>, ...keys: string[]) => {
  for (const key of keys) {
    if (payload[key] !== undefined && payload[key] !== null) {
      return payload[key];
    }
  }
  return undefined;
};

export const VifAssistedJourneyPanel: React.FC = () => {
  const [busy, setBusy] = React.useState<string | null>(null);
  const [currentCif, setCurrentCif] = React.useState<string>('');
  const [currentAccount, setCurrentAccount] = React.useState<string>('');
  const [balanceText, setBalanceText] = React.useState<string>('');
  const [statementText, setStatementText] = React.useState<string>('');

  const [customerForm] = Form.useForm();
  const [accountForm] = Form.useForm();

  React.useEffect(() => {
    if (currentCif) {
      accountForm.setFieldValue('cifNumber', currentCif);
    }
  }, [currentCif, accountForm]);

  const resolveAccount = (accountNumber?: string) => {
    const account = (accountNumber || currentAccount || '').trim();
    if (!account) {
      throw new Error('Provide account number or register/select an active account first');
    }
    return account;
  };

  const runAction = async (name: string, action: () => Promise<Record<string, unknown>>, successMessage: string) => {
    try {
      setBusy(name);
      const result = await action();
      message.success(successMessage);
      return result;
    } catch (error) {
      const description = error instanceof Error ? error.message : 'Request failed';
      message.error(description);
      return null;
    } finally {
      setBusy(null);
    }
  };

  const onRegisterCustomer = async (values: {
    firstName: string;
    lastName: string;
    identificationNumber: string;
    email?: string;
    phoneNumber?: string;
    riskRating?: number;
  }) => {
    const result = await runAction(
      'register-customer',
      () => tellerApi.registerVifCustomer(values),
      'Customer registered'
    );

    if (!result) {
      return;
    }

    const cif = String(pick(result, 'CifNumber', 'cifNumber') || '');
    if (cif) {
      setCurrentCif(cif);
    }
    customerForm.resetFields();
  };

  const onRegisterAccount = async (values: {
    cifNumber: string;
    accountType?: string;
    currency?: string;
    initialDeposit?: number;
    branchCode?: string;
  }) => {
    const result = await runAction(
      'register-account',
      () => tellerApi.registerVifAccount(values),
      'Account registered'
    );

    if (!result) {
      return;
    }

    const accountNumber = String(pick(result, 'AccountNumber', 'accountNumber') || '');
    if (accountNumber) {
      setCurrentAccount(accountNumber);
    }
    accountForm.resetFields(['initialDeposit']);
  };

  const onCheckBalance = async (values: { accountNumber?: string }) => {
    const accountNumber = resolveAccount(values.accountNumber);
    const result = await runAction('check-balance', () => tellerApi.getVifBalance(accountNumber), 'Balance loaded');
    if (!result) {
      return;
    }

    const balance = pick(result, 'Balance', 'balance');
    const currency = String(pick(result, 'Currency', 'currency') || 'KES');
    setBalanceText(`${accountNumber}: ${currency} ${Number(balance || 0).toLocaleString()}`);
  };

  const onGetStatement = async (values: { accountNumber?: string; range?: any[] }) => {
    const accountNumber = resolveAccount(values.accountNumber);
    const from = values.range?.[0]?.toISOString?.();
    const to = values.range?.[1]?.toISOString?.();

    const result = await runAction(
      'statement',
      () => tellerApi.getVifStatement(accountNumber, { from, to, pageNumber: 1, pageSize: 20 }),
      'Statement loaded'
    );

    if (!result) {
      return;
    }

    const total = Number(pick(result, 'TotalRecords', 'totalRecords') || 0);
    setStatementText(`${accountNumber}: ${total} record(s)`);
  };

  return (
    <Space direction="vertical" size="large" style={{ width: '100%' }}>
      {(currentCif || currentAccount || balanceText || statementText) && (
        <Alert
          type="info"
          showIcon
          message="Active assisted journey context"
          description={
            <div>
              {currentCif && <div>CIF: {currentCif}</div>}
              {currentAccount && <div>Account: {currentAccount}</div>}
              {balanceText && <div>Balance: {balanceText}</div>}
              {statementText && <div>Statement: {statementText}</div>}
            </div>
          }
        />
      )}

      <Row gutter={[16, 16]}>
        <Col xs={24} md={12}>
          <Card title="1) Register Customer + CIF">
            <Form form={customerForm} layout="vertical" onFinish={onRegisterCustomer}>
              <Form.Item name="firstName" label="First Name" rules={[{ required: true }]}>
                <Input />
              </Form.Item>
              <Form.Item name="lastName" label="Last Name" rules={[{ required: true }]}>
                <Input />
              </Form.Item>
              <Form.Item name="identificationNumber" label="ID Number" rules={[{ required: true }]}>
                <Input />
              </Form.Item>
              <Form.Item name="email" label="Email">
                <Input />
              </Form.Item>
              <Form.Item name="phoneNumber" label="Phone Number">
                <Input />
              </Form.Item>
              <Form.Item name="riskRating" label="Risk Rating" initialValue={1}>
                <InputNumber min={0} max={5} style={{ width: '100%' }} />
              </Form.Item>
              <Button type="primary" htmlType="submit" loading={busy === 'register-customer'}>
                Register Customer
              </Button>
            </Form>
          </Card>
        </Col>

        <Col xs={24} md={12}>
          <Card title="2) Register Account">
            <Form form={accountForm} layout="vertical" onFinish={onRegisterAccount}>
              <Form.Item name="cifNumber" label="CIF Number" rules={[{ required: true }]}>
                <Input placeholder="CIF-..." />
              </Form.Item>
              <Form.Item name="accountType" label="Account Type" initialValue="Savings">
                <Input />
              </Form.Item>
              <Form.Item name="currency" label="Currency" initialValue="KES">
                <Input />
              </Form.Item>
              <Form.Item name="initialDeposit" label="Initial Deposit" initialValue={0}>
                <InputNumber min={0} style={{ width: '100%' }} />
              </Form.Item>
              <Form.Item name="branchCode" label="Branch Code" initialValue="BR100001">
                <Input />
              </Form.Item>
              <Button type="primary" htmlType="submit" loading={busy === 'register-account'}>
                Register Account
              </Button>
            </Form>
          </Card>
        </Col>
      </Row>

      <Card title="3) Assisted Operations">
        <Row gutter={[16, 16]}>
          <Col xs={24} md={12}>
            <Card size="small" title="Balance + Statement">
              <Form layout="vertical" onFinish={onCheckBalance}>
                <Form.Item name="accountNumber" label="Account Number (optional if active)">
                  <Input />
                </Form.Item>
                <Button htmlType="submit" loading={busy === 'check-balance'}>
                  Check Balance
                </Button>
              </Form>

              <Form layout="vertical" onFinish={onGetStatement} style={{ marginTop: 16 }}>
                <Form.Item name="accountNumber" label="Account Number (optional if active)">
                  <Input />
                </Form.Item>
                <Form.Item name="range" label="Statement Date Range">
                  <DatePicker.RangePicker style={{ width: '100%' }} />
                </Form.Item>
                <Button htmlType="submit" loading={busy === 'statement'}>
                  Get Statement
                </Button>
              </Form>
            </Card>
          </Col>

          <Col xs={24} md={12}>
            <Card size="small" title="Payments (Transfer / Airtime / M-Pesa)">
              <Form
                layout="vertical"
                onFinish={(values: { fromAccountNumber?: string; toAccountNumber: string; amount: number; currency?: string }) =>
                  runAction(
                    'transfer',
                    () =>
                      tellerApi.vifTransfer({
                        fromAccountNumber: resolveAccount(values.fromAccountNumber),
                        toAccountNumber: values.toAccountNumber,
                        amount: values.amount,
                        currency: values.currency || 'KES',
                      }),
                    'Transfer completed'
                  )
                }
              >
                <Form.Item name="fromAccountNumber" label="From Account (optional if active)">
                  <Input />
                </Form.Item>
                <Form.Item name="toAccountNumber" label="To Account" rules={[{ required: true }]}>
                  <Input />
                </Form.Item>
                <Form.Item name="amount" label="Amount" rules={[{ required: true }]}>
                  <InputNumber min={1} style={{ width: '100%' }} />
                </Form.Item>
                <Form.Item name="currency" label="Currency" initialValue="KES">
                  <Input />
                </Form.Item>
                <Button htmlType="submit" loading={busy === 'transfer'}>
                  Transfer
                </Button>
              </Form>

              <Form
                layout="inline"
                onFinish={(values: { accountNumber?: string; amount: number; phoneNumber: string; provider?: string }) =>
                  runAction(
                    'airtime',
                    () =>
                      tellerApi.vifAirtime({
                        accountNumber: resolveAccount(values.accountNumber),
                        amount: values.amount,
                        phoneNumber: values.phoneNumber,
                        provider: values.provider || 'Safaricom',
                        currency: 'KES',
                      }),
                    'Airtime purchased'
                  )
                }
                style={{ marginTop: 16 }}
              >
                <Form.Item name="accountNumber">
                  <Input placeholder="Account (optional)" />
                </Form.Item>
                <Form.Item name="phoneNumber" rules={[{ required: true }]}>
                  <Input placeholder="Phone" />
                </Form.Item>
                <Form.Item name="amount" rules={[{ required: true }]}>
                  <InputNumber min={1} placeholder="Amount" />
                </Form.Item>
                <Form.Item>
                  <Button htmlType="submit" loading={busy === 'airtime'}>
                    Airtime
                  </Button>
                </Form.Item>
              </Form>

              <Form
                layout="inline"
                onFinish={(values: { accountNumber?: string; amount: number; phoneNumber: string }) =>
                  runAction(
                    'mpesa',
                    () =>
                      tellerApi.vifMpesa({
                        accountNumber: resolveAccount(values.accountNumber),
                        amount: values.amount,
                        phoneNumber: values.phoneNumber,
                        currency: 'KES',
                      }),
                    'M-Pesa transfer sent'
                  )
                }
                style={{ marginTop: 12 }}
              >
                <Form.Item name="accountNumber">
                  <Input placeholder="Account (optional)" />
                </Form.Item>
                <Form.Item name="phoneNumber" rules={[{ required: true }]}>
                  <Input placeholder="Phone" />
                </Form.Item>
                <Form.Item name="amount" rules={[{ required: true }]}>
                  <InputNumber min={1} placeholder="Amount" />
                </Form.Item>
                <Form.Item>
                  <Button htmlType="submit" loading={busy === 'mpesa'}>
                    M-Pesa
                  </Button>
                </Form.Item>
              </Form>
            </Card>
          </Col>

          <Col xs={24} md={12}>
            <Card size="small" title="Cheque + Investments">
              <Form
                layout="vertical"
                onFinish={(values: { accountNumber?: string; amount: number; chequeNumber: string; drawerBank: string }) =>
                  runAction(
                    'cheque',
                    () =>
                      tellerApi.vifChequeDeposit({
                        accountNumber: resolveAccount(values.accountNumber),
                        amount: values.amount,
                        chequeNumber: values.chequeNumber,
                        drawerBank: values.drawerBank,
                        currency: 'KES',
                      }),
                    'Cheque deposit captured'
                  )
                }
              >
                <Form.Item name="accountNumber" label="Account (optional if active)">
                  <Input />
                </Form.Item>
                <Form.Item name="chequeNumber" label="Cheque Number" rules={[{ required: true }]}>
                  <Input />
                </Form.Item>
                <Form.Item name="drawerBank" label="Drawer Bank" rules={[{ required: true }]}>
                  <Input />
                </Form.Item>
                <Form.Item name="amount" label="Amount" rules={[{ required: true }]}>
                  <InputNumber min={1} style={{ width: '100%' }} />
                </Form.Item>
                <Button htmlType="submit" loading={busy === 'cheque'}>
                  Cheque Deposit
                </Button>
              </Form>

              <Form
                layout="inline"
                onFinish={(values: { accountNumber?: string; amount: number; instrumentCode: string; quantity: number }) =>
                  runAction(
                    'shares',
                    () =>
                      tellerApi.vifBuyShares({
                        accountNumber: resolveAccount(values.accountNumber),
                        amount: values.amount,
                        instrumentCode: values.instrumentCode,
                        quantity: values.quantity,
                        currency: 'KES',
                      }),
                    'Share purchase posted'
                  )
                }
                style={{ marginTop: 16 }}
              >
                <Form.Item name="accountNumber">
                  <Input placeholder="Account (optional)" />
                </Form.Item>
                <Form.Item name="instrumentCode" rules={[{ required: true }]}>
                  <Input placeholder="Share code" />
                </Form.Item>
                <Form.Item name="quantity" rules={[{ required: true }]}>
                  <InputNumber min={1} placeholder="Qty" />
                </Form.Item>
                <Form.Item name="amount" rules={[{ required: true }]}>
                  <InputNumber min={1} placeholder="Amount" />
                </Form.Item>
                <Form.Item>
                  <Button htmlType="submit" loading={busy === 'shares'}>
                    Buy Shares
                  </Button>
                </Form.Item>
              </Form>

              <Form
                layout="inline"
                onFinish={(values: {
                  accountNumber?: string;
                  amount: number;
                  instrumentCode: string;
                  quantity: number;
                  instrumentType?: 'TBill' | 'TBond';
                  tenorDays?: number;
                }) =>
                  runAction(
                    'treasury',
                    () =>
                      tellerApi.vifBuyTreasury({
                        accountNumber: resolveAccount(values.accountNumber),
                        amount: values.amount,
                        instrumentCode: values.instrumentCode,
                        quantity: values.quantity,
                        instrumentType: values.instrumentType || 'TBill',
                        tenorDays: values.tenorDays || 91,
                        currency: 'KES',
                      }),
                    'Treasury purchase posted'
                  )
                }
                style={{ marginTop: 12 }}
              >
                <Form.Item name="accountNumber">
                  <Input placeholder="Account (optional)" />
                </Form.Item>
                <Form.Item name="instrumentCode" rules={[{ required: true }]}>
                  <Input placeholder="TBILL/TBOND code" />
                </Form.Item>
                <Form.Item name="amount" rules={[{ required: true }]}>
                  <InputNumber min={1} placeholder="Amount" />
                </Form.Item>
                <Form.Item name="tenorDays">
                  <InputNumber min={1} placeholder="Tenor" />
                </Form.Item>
                <Form.Item>
                  <Button htmlType="submit" loading={busy === 'treasury'}>
                    Buy Treasury
                  </Button>
                </Form.Item>
              </Form>
            </Card>
          </Col>

          <Col xs={24} md={12}>
            <Card size="small" title="Service Requests">
              <Form
                layout="vertical"
                onFinish={(values: { accountNumber?: string; reason: string }) =>
                  runAction(
                    'atm-lock',
                    () =>
                      tellerApi.vifLockAtmCard({
                        accountNumber: resolveAccount(values.accountNumber),
                        reason: values.reason,
                      }),
                    'ATM card lock request submitted'
                  )
                }
              >
                <Form.Item name="accountNumber" label="Account (optional if active)">
                  <Input />
                </Form.Item>
                <Form.Item name="reason" label="ATM Lock Reason" rules={[{ required: true }]}>
                  <Input.TextArea rows={2} />
                </Form.Item>
                <Button htmlType="submit" loading={busy === 'atm-lock'}>
                  Lock ATM Card
                </Button>
              </Form>

              <Form
                layout="vertical"
                onFinish={(values: { accountNumber?: string; reason: string }) =>
                  runAction(
                    'loan-block',
                    () =>
                      tellerApi.vifBlockMobileLoan({
                        accountNumber: resolveAccount(values.accountNumber),
                        reason: values.reason,
                      }),
                    'Mobile loan block request submitted'
                  )
                }
                style={{ marginTop: 16 }}
              >
                <Form.Item name="accountNumber" label="Account (optional if active)">
                  <Input />
                </Form.Item>
                <Form.Item name="reason" label="Mobile Loan Block Reason" rules={[{ required: true }]}>
                  <Input.TextArea rows={2} />
                </Form.Item>
                <Button danger htmlType="submit" loading={busy === 'loan-block'}>
                  Block Mobile Loan
                </Button>
              </Form>
            </Card>
          </Col>
        </Row>
      </Card>
    </Space>
  );
};
