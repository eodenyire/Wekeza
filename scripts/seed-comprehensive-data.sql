-- =============================================================
-- Wekeza Bank - Comprehensive Seed Data
-- This script seeds rich, realistic banking data so every
-- portal shows meaningful information from day one.
-- Password for ALL portal users: Admin@123
-- =============================================================

-- ──────────────────────────────────────────────
-- 1. ALL PORTAL USERS (all use password: Admin@123)
-- BCrypt hash: $2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C
-- ──────────────────────────────────────────────
INSERT INTO "Users" (
  "Id", "Username", "Email", "PasswordHash", "FullName", "Role",
  "IsActive", "IsEmailConfirmed", "Department", "CreatedAt", "UpdatedAt"
) VALUES
  ('11111111-0000-0000-0000-000000000001', 'admin',        'admin@wekeza.com',
   '$2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C',
   'System Administrator',      'Administrator',        true, true, 'IT Operations',       NOW(), NOW()),

  ('11111111-0000-0000-0000-000000000002', 'manager1',     'manager1@wekeza.com',
   '$2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C',
   'Jane Smith',                'Manager',              true, true, 'Branch Management',   NOW(), NOW()),

  ('11111111-0000-0000-0000-000000000003', 'teller1',      'teller1@wekeza.com',
   '$2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C',
   'John Teller',               'Teller',               true, true, 'Teller Operations',   NOW(), NOW()),

  ('11111111-0000-0000-0000-000000000004', 'supervisor1',  'supervisor1@wekeza.com',
   '$2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C',
   'Peter Supervisor',          'Supervisor',           true, true, 'Operations',          NOW(), NOW()),

  ('11111111-0000-0000-0000-000000000005', 'compliance1',  'compliance1@wekeza.com',
   '$2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C',
   'Grace Compliance',          'ComplianceOfficer',    true, true, 'Compliance',          NOW(), NOW()),

  ('11111111-0000-0000-0000-000000000006', 'treasury1',    'treasury1@wekeza.com',
   '$2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C',
   'David Treasury',            'TreasuryDealer',       true, true, 'Treasury',            NOW(), NOW()),

  ('11111111-0000-0000-0000-000000000007', 'tradeFinance1','tradefinance1@wekeza.com',
   '$2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C',
   'Sarah TradeFinance',        'TradeFinanceOfficer',  true, true, 'Trade Finance',       NOW(), NOW()),

  ('11111111-0000-0000-0000-000000000008', 'payments1',    'payments1@wekeza.com',
   '$2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C',
   'Michael Payments',          'PaymentsOfficer',      true, true, 'Payments',            NOW(), NOW()),

  ('11111111-0000-0000-0000-000000000009', 'productGL1',   'productgl1@wekeza.com',
   '$2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C',
   'Lisa ProductGL',            'ProductManager',       true, true, 'Product Management',  NOW(), NOW()),

  ('11111111-0000-0000-0000-000000000010', 'customer1',    'customer1@wekeza.com',
   '$2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C',
   'Alice Customer',            'Customer',             true, true, 'Customer Service',    NOW(), NOW()),

  ('11111111-0000-0000-0000-000000000011', 'vaultOfficer1','vaultofficer1@wekeza.com',
   '$2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C',
   'James Vault',               'VaultOfficer',         true, true, 'Vault Operations',    NOW(), NOW()),

  ('11111111-0000-0000-0000-000000000012', 'executive1',   'executive1@wekeza.com',
   '$2b$11$MYEYIvsY9CxZbyecpPeEYOUTQwB4dekhUqZPbrODF5oVhxul88A5C',
   'Dr. James Wekeza',          'CEO',                  true, true, 'Executive',           NOW(), NOW())
ON CONFLICT ("Username") DO UPDATE SET
  "PasswordHash"     = EXCLUDED."PasswordHash",
  "Role"             = EXCLUDED."Role",
  "FullName"         = EXCLUDED."FullName",
  "IsActive"         = true,
  "UpdatedAt"        = NOW();

-- ──────────────────────────────────────────────
-- 2. CUSTOMERS  (20 realistic individuals)
-- customer1@wekeza.com is linked so the customer portal works
-- ──────────────────────────────────────────────
INSERT INTO "Customers" (
  "Id", "FirstName", "LastName", "Email", "PhoneNumber",
  "IdentificationNumber", "RiskRating", "IsActive", "CreatedAt", "UpdatedAt"
) VALUES
  ('22222222-0000-0000-0000-000000000001', 'Alice',   'Customer',  'customer1@wekeza.com',    '+254700000001', 'KE-ID-00001', 0, true, NOW()-INTERVAL '2 years', NOW()),
  ('22222222-0000-0000-0000-000000000002', 'Bob',     'Kariuki',   'bob.kariuki@email.com',   '+254700000002', 'KE-ID-00002', 0, true, NOW()-INTERVAL '3 years', NOW()),
  ('22222222-0000-0000-0000-000000000003', 'Carol',   'Wanjiku',   'carol.wanjiku@email.com', '+254700000003', 'KE-ID-00003', 1, true, NOW()-INTERVAL '18 months', NOW()),
  ('22222222-0000-0000-0000-000000000004', 'David',   'Omondi',    'david.omondi@email.com',  '+254700000004', 'KE-ID-00004', 0, true, NOW()-INTERVAL '1 year', NOW()),
  ('22222222-0000-0000-0000-000000000005', 'Eve',     'Mutuku',    'eve.mutuku@email.com',    '+254700000005', 'KE-ID-00005', 2, true, NOW()-INTERVAL '6 months', NOW()),
  ('22222222-0000-0000-0000-000000000006', 'Frank',   'Njoroge',   'frank.njoroge@email.com', '+254700000006', 'KE-ID-00006', 0, true, NOW()-INTERVAL '4 years', NOW()),
  ('22222222-0000-0000-0000-000000000007', 'Grace',   'Atieno',    'grace.atieno@email.com',  '+254700000007', 'KE-ID-00007', 1, true, NOW()-INTERVAL '2 years', NOW()),
  ('22222222-0000-0000-0000-000000000008', 'Henry',   'Mwangi',    'henry.mwangi@email.com',  '+254700000008', 'KE-ID-00008', 0, true, NOW()-INTERVAL '5 years', NOW()),
  ('22222222-0000-0000-0000-000000000009', 'Irene',   'Achieng',   'irene.achieng@email.com', '+254700000009', 'KE-ID-00009', 0, true, NOW()-INTERVAL '8 months', NOW()),
  ('22222222-0000-0000-0000-000000000010', 'James',   'Kamau',     'james.kamau@email.com',   '+254700000010', 'KE-ID-00010', 1, true, NOW()-INTERVAL '3 years', NOW()),
  ('22222222-0000-0000-0000-000000000011', 'Karen',   'Mbugua',    'karen.mbugua@email.com',  '+254700000011', 'KE-ID-00011', 0, true, NOW()-INTERVAL '14 months', NOW()),
  ('22222222-0000-0000-0000-000000000012', 'Liam',    'Otieno',    'liam.otieno@email.com',   '+254700000012', 'KE-ID-00012', 0, true, NOW()-INTERVAL '2 years', NOW()),
  ('22222222-0000-0000-0000-000000000013', 'Mary',    'Waweru',    'mary.waweru@email.com',   '+254700000013', 'KE-ID-00013', 2, true, NOW()-INTERVAL '7 months', NOW()),
  ('22222222-0000-0000-0000-000000000014', 'Nathan',  'Kiprotich', 'nathan.k@email.com',      '+254700000014', 'KE-ID-00014', 0, true, NOW()-INTERVAL '1 year', NOW()),
  ('22222222-0000-0000-0000-000000000015', 'Olivia',  'Chebet',    'olivia.chebet@email.com', '+254700000015', 'KE-ID-00015', 1, true, NOW()-INTERVAL '9 months', NOW()),
  ('22222222-0000-0000-0000-000000000016', 'Paul',    'Githaiga',  'paul.githaiga@email.com', '+254700000016', 'KE-ID-00016', 0, true, NOW()-INTERVAL '6 years', NOW()),
  ('22222222-0000-0000-0000-000000000017', 'Quinn',   'Kimani',    'quinn.kimani@email.com',  '+254700000017', 'KE-ID-00017', 0, true, NOW()-INTERVAL '3 months', NOW()),
  ('22222222-0000-0000-0000-000000000018', 'Rose',    'Nderitu',   'rose.nderitu@email.com',  '+254700000018', 'KE-ID-00018', 1, true, NOW()-INTERVAL '2 years', NOW()),
  ('22222222-0000-0000-0000-000000000019', 'Steve',   'Macharia',  'steve.macharia@email.com','+254700000019', 'KE-ID-00019', 0, true, NOW()-INTERVAL '4 years', NOW()),
  ('22222222-0000-0000-0000-000000000020', 'Tina',    'Wairimu',   'tina.wairimu@email.com',  '+254700000020', 'KE-ID-00020', 0, true, NOW()-INTERVAL '1 year', NOW())
ON CONFLICT ("Email") DO NOTHING;

-- ──────────────────────────────────────────────
-- 3. ACCOUNTS  (2–3 per customer)
-- ──────────────────────────────────────────────
INSERT INTO "Accounts" (
  "Id", "AccountNumber", "CustomerId", "AccountType",
  "Currency", "Balance", "AvailableBalance", "Status",
  "OpenedDate", "CreatedAt", "UpdatedAt"
) VALUES
  -- Alice Customer (customer1)
  ('33333333-0000-0000-0001-000000000001','ACC0000000001','22222222-0000-0000-0000-000000000001','Savings',   'KES', 142184.50, 142184.50, 'Active', NOW()-INTERVAL '2 years', NOW()-INTERVAL '2 years', NOW()),
  ('33333333-0000-0000-0001-000000000002','ACC0000000002','22222222-0000-0000-0000-000000000001','Current',   'KES',  85000.00,  82000.00, 'Active', NOW()-INTERVAL '2 years', NOW()-INTERVAL '2 years', NOW()),
  -- Bob Kariuki
  ('33333333-0000-0000-0002-000000000001','ACC0000000010','22222222-0000-0000-0000-000000000002','Savings',   'KES', 230000.00, 228000.00, 'Active', NOW()-INTERVAL '3 years', NOW()-INTERVAL '3 years', NOW()),
  ('33333333-0000-0000-0002-000000000002','ACC0000000011','22222222-0000-0000-0000-000000000002','Fixed',     'KES', 500000.00, 500000.00, 'Active', NOW()-INTERVAL '2 years', NOW()-INTERVAL '2 years', NOW()),
  -- Carol Wanjiku
  ('33333333-0000-0000-0003-000000000001','ACC0000000020','22222222-0000-0000-0000-000000000003','Savings',   'KES',  76500.00,  74000.00, 'Active', NOW()-INTERVAL '18 months', NOW()-INTERVAL '18 months', NOW()),
  -- David Omondi
  ('33333333-0000-0000-0004-000000000001','ACC0000000030','22222222-0000-0000-0000-000000000004','Current',   'KES', 320000.00, 315000.00, 'Active', NOW()-INTERVAL '1 year', NOW()-INTERVAL '1 year', NOW()),
  -- Eve Mutuku
  ('33333333-0000-0000-0005-000000000001','ACC0000000040','22222222-0000-0000-0000-000000000005','Savings',   'KES',  42000.00,  40000.00, 'Active', NOW()-INTERVAL '6 months', NOW()-INTERVAL '6 months', NOW()),
  -- Frank Njoroge
  ('33333333-0000-0000-0006-000000000001','ACC0000000050','22222222-0000-0000-0000-000000000006','Savings',   'KES', 890000.00, 885000.00, 'Active', NOW()-INTERVAL '4 years', NOW()-INTERVAL '4 years', NOW()),
  ('33333333-0000-0000-0006-000000000002','ACC0000000051','22222222-0000-0000-0000-000000000006','Current',   'KES', 125000.00, 120000.00, 'Active', NOW()-INTERVAL '4 years', NOW()-INTERVAL '4 years', NOW()),
  -- Grace Atieno
  ('33333333-0000-0000-0007-000000000001','ACC0000000060','22222222-0000-0000-0000-000000000007','Savings',   'KES', 195000.00, 195000.00, 'Active', NOW()-INTERVAL '2 years', NOW()-INTERVAL '2 years', NOW()),
  -- Henry Mwangi
  ('33333333-0000-0000-0008-000000000001','ACC0000000070','22222222-0000-0000-0000-000000000008','Current',   'KES', 460000.00, 455000.00, 'Active', NOW()-INTERVAL '5 years', NOW()-INTERVAL '5 years', NOW()),
  ('33333333-0000-0000-0008-000000000002','ACC0000000071','22222222-0000-0000-0000-000000000008','Fixed',     'KES',1000000.00,1000000.00,'Active', NOW()-INTERVAL '3 years', NOW()-INTERVAL '3 years', NOW()),
  -- Irene Achieng
  ('33333333-0000-0000-0009-000000000001','ACC0000000080','22222222-0000-0000-0000-000000000009','Savings',   'KES',  55000.00,  53000.00, 'Active', NOW()-INTERVAL '8 months', NOW()-INTERVAL '8 months', NOW()),
  -- James Kamau
  ('33333333-0000-0000-0010-000000000001','ACC0000000090','22222222-0000-0000-0000-000000000010','Current',   'KES', 275000.00, 272000.00, 'Active', NOW()-INTERVAL '3 years', NOW()-INTERVAL '3 years', NOW()),
  -- Karen Mbugua
  ('33333333-0000-0000-0011-000000000001','ACC0000000100','22222222-0000-0000-0000-000000000011','Savings',   'KES', 130000.00, 130000.00, 'Active', NOW()-INTERVAL '14 months', NOW()-INTERVAL '14 months', NOW()),
  -- Liam Otieno
  ('33333333-0000-0000-0012-000000000001','ACC0000000110','22222222-0000-0000-0000-000000000012','Savings',   'KES', 215000.00, 213000.00, 'Active', NOW()-INTERVAL '2 years', NOW()-INTERVAL '2 years', NOW()),
  -- Mary Waweru
  ('33333333-0000-0000-0013-000000000001','ACC0000000120','22222222-0000-0000-0000-000000000013','Savings',   'KES',  18000.00,  17500.00, 'Active', NOW()-INTERVAL '7 months', NOW()-INTERVAL '7 months', NOW()),
  -- Nathan Kiprotich
  ('33333333-0000-0000-0014-000000000001','ACC0000000130','22222222-0000-0000-0000-000000000014','Current',   'KES', 340000.00, 335000.00, 'Active', NOW()-INTERVAL '1 year', NOW()-INTERVAL '1 year', NOW()),
  -- Olivia Chebet
  ('33333333-0000-0000-0015-000000000001','ACC0000000140','22222222-0000-0000-0000-000000000015','Savings',   'KES',  92000.00,  90000.00, 'Active', NOW()-INTERVAL '9 months', NOW()-INTERVAL '9 months', NOW()),
  -- Paul Githaiga
  ('33333333-0000-0000-0016-000000000001','ACC0000000150','22222222-0000-0000-0000-000000000016','Current',   'KES', 750000.00, 740000.00, 'Active', NOW()-INTERVAL '6 years', NOW()-INTERVAL '6 years', NOW()),
  ('33333333-0000-0000-0016-000000000002','ACC0000000151','22222222-0000-0000-0000-000000000016','Fixed',     'KES',2000000.00,2000000.00,'Active', NOW()-INTERVAL '5 years', NOW()-INTERVAL '5 years', NOW()),
  -- Quinn Kimani
  ('33333333-0000-0000-0017-000000000001','ACC0000000160','22222222-0000-0000-0000-000000000017','Savings',   'KES',  25000.00,  25000.00, 'Active', NOW()-INTERVAL '3 months', NOW()-INTERVAL '3 months', NOW()),
  -- Rose Nderitu
  ('33333333-0000-0000-0018-000000000001','ACC0000000170','22222222-0000-0000-0000-000000000018','Savings',   'KES', 185000.00, 183000.00, 'Active', NOW()-INTERVAL '2 years', NOW()-INTERVAL '2 years', NOW()),
  -- Steve Macharia
  ('33333333-0000-0000-0019-000000000001','ACC0000000180','22222222-0000-0000-0000-000000000019','Current',   'KES', 620000.00, 615000.00, 'Active', NOW()-INTERVAL '4 years', NOW()-INTERVAL '4 years', NOW()),
  -- Tina Wairimu
  ('33333333-0000-0000-0020-000000000001','ACC0000000190','22222222-0000-0000-0000-000000000020','Savings',   'KES', 105000.00, 103000.00, 'Active', NOW()-INTERVAL '1 year', NOW()-INTERVAL '1 year', NOW())
ON CONFLICT ("AccountNumber") DO NOTHING;

-- ──────────────────────────────────────────────
-- 4. TRANSACTIONS (realistic history)
-- ──────────────────────────────────────────────
INSERT INTO "Transactions" (
  "Id", "TransactionReference", "AccountId", "TransactionType",
  "Amount", "Currency", "Status", "Description",
  "CreatedAt", "ProcessedAt", "BalanceAfter"
) VALUES
  -- Alice's Savings account (today)
  (uuid_generate_v4(),'TXN-20260311-0001','33333333-0000-0000-0001-000000000001','Deposit',   50000.00,'KES','Success','Salary credit March 2026',    NOW()-INTERVAL '2 hours', NOW()-INTERVAL '2 hours',  192184.50),
  (uuid_generate_v4(),'TXN-20260311-0002','33333333-0000-0000-0001-000000000001','Withdrawal',25000.00,'KES','Success','ATM Withdrawal - Westlands',   NOW()-INTERVAL '3 hours', NOW()-INTERVAL '3 hours',  142184.50),
  (uuid_generate_v4(),'TXN-20260311-0003','33333333-0000-0000-0001-000000000001','Transfer',   8500.00,'KES','Success','Rent payment via M-PESA',      NOW()-INTERVAL '5 hours', NOW()-INTERVAL '5 hours',  167184.50),
  (uuid_generate_v4(),'TXN-20260311-0004','33333333-0000-0000-0001-000000000002','Deposit',   15000.00,'KES','Success','Cash deposit - branch',        NOW()-INTERVAL '1 hour',  NOW()-INTERVAL '1 hour',  100000.00),
  (uuid_generate_v4(),'TXN-20260311-0005','33333333-0000-0000-0001-000000000002','Payment',    2500.00,'KES','Success','Utility bill - KPLC',          NOW()-INTERVAL '4 hours', NOW()-INTERVAL '4 hours',   85000.00),
  -- Alice (yesterday)
  (uuid_generate_v4(),'TXN-20260310-0001','33333333-0000-0000-0001-000000000001','Deposit',   10000.00,'KES','Success','Mobile banking deposit',       NOW()-INTERVAL '1 day',   NOW()-INTERVAL '1 day',   157184.50),
  (uuid_generate_v4(),'TXN-20260310-0002','33333333-0000-0000-0001-000000000001','Withdrawal', 5000.00,'KES','Success','Supermarket - Carrefour',      NOW()-INTERVAL '1 day',   NOW()-INTERVAL '1 day',   147184.50),
  -- Bob Kariuki (today)
  (uuid_generate_v4(),'TXN-20260311-0010','33333333-0000-0000-0002-000000000001','Deposit',  100000.00,'KES','Success','Rental income',                NOW()-INTERVAL '6 hours', NOW()-INTERVAL '6 hours',  330000.00),
  (uuid_generate_v4(),'TXN-20260311-0011','33333333-0000-0000-0002-000000000001','Transfer',  30000.00,'KES','Success','Transfer to savings',          NOW()-INTERVAL '2 hours', NOW()-INTERVAL '2 hours',  230000.00),
  -- Carol Wanjiku
  (uuid_generate_v4(),'TXN-20260311-0020','33333333-0000-0000-0003-000000000001','Deposit',   20000.00,'KES','Success','Cash deposit',                 NOW()-INTERVAL '7 hours', NOW()-INTERVAL '7 hours',   96500.00),
  (uuid_generate_v4(),'TXN-20260311-0021','33333333-0000-0000-0003-000000000001','Payment',    5000.00,'KES','Success','Insurance premium',            NOW()-INTERVAL '8 hours', NOW()-INTERVAL '8 hours',   76500.00),
  -- David Omondi
  (uuid_generate_v4(),'TXN-20260311-0030','33333333-0000-0000-0004-000000000001','Deposit',   80000.00,'KES','Success','Business revenue',             NOW()-INTERVAL '9 hours', NOW()-INTERVAL '9 hours',  400000.00),
  (uuid_generate_v4(),'TXN-20260311-0031','33333333-0000-0000-0004-000000000001','Withdrawal',80000.00,'KES','Success','Supplier payment',             NOW()-INTERVAL '10 hours',NOW()-INTERVAL '10 hours', 320000.00),
  -- Eve Mutuku
  (uuid_generate_v4(),'TXN-20260311-0040','33333333-0000-0000-0005-000000000001','Deposit',   12000.00,'KES','Success','Salary credit',                NOW()-INTERVAL '2 hours', NOW()-INTERVAL '2 hours',   54000.00),
  -- Frank Njoroge (big corporate)
  (uuid_generate_v4(),'TXN-20260311-0050','33333333-0000-0000-0006-000000000001','Deposit',  500000.00,'KES','Success','Corporate dividend',           NOW()-INTERVAL '3 hours', NOW()-INTERVAL '3 hours', 1390000.00),
  (uuid_generate_v4(),'TXN-20260311-0051','33333333-0000-0000-0006-000000000001','Transfer', 200000.00,'KES','Success','Investment transfer',          NOW()-INTERVAL '4 hours', NOW()-INTERVAL '4 hours',  890000.00),
  -- Grace Atieno
  (uuid_generate_v4(),'TXN-20260311-0060','33333333-0000-0000-0007-000000000001','Deposit',   45000.00,'KES','Success','Salary credit',                NOW()-INTERVAL '2 hours', NOW()-INTERVAL '2 hours',  240000.00),
  (uuid_generate_v4(),'TXN-20260311-0061','33333333-0000-0000-0007-000000000001','Payment',   45000.00,'KES','Success','Mortgage instalment',          NOW()-INTERVAL '3 hours', NOW()-INTERVAL '3 hours',  195000.00),
  -- Henry Mwangi (high value)
  (uuid_generate_v4(),'TXN-20260311-0070','33333333-0000-0000-0008-000000000001','Deposit', 1200000.00,'KES','Success','Property sale proceeds',       NOW()-INTERVAL '5 hours', NOW()-INTERVAL '5 hours', 1660000.00),
  (uuid_generate_v4(),'TXN-20260311-0071','33333333-0000-0000-0008-000000000001','Transfer',1200000.00,'KES','Success','Transfer to fixed deposit',    NOW()-INTERVAL '6 hours', NOW()-INTERVAL '6 hours',  460000.00),
  -- Karen Mbugua
  (uuid_generate_v4(),'TXN-20260311-0100','33333333-0000-0000-0011-000000000001','Deposit',   60000.00,'KES','Success','Salary credit',                NOW()-INTERVAL '1 hour',  NOW()-INTERVAL '1 hour',   190000.00),
  (uuid_generate_v4(),'TXN-20260311-0101','33333333-0000-0000-0011-000000000001','Payment',   60000.00,'KES','Success','Rent payment',                 NOW()-INTERVAL '2 hours', NOW()-INTERVAL '2 hours',  130000.00),
  -- Liam Otieno
  (uuid_generate_v4(),'TXN-20260311-0110','33333333-0000-0000-0012-000000000001','Deposit',   25000.00,'KES','Success','Salary credit',                NOW()-INTERVAL '2 hours', NOW()-INTERVAL '2 hours',  240000.00),
  (uuid_generate_v4(),'TXN-20260311-0111','33333333-0000-0000-0012-000000000001','Withdrawal',25000.00,'KES','Success','Living expenses',              NOW()-INTERVAL '3 hours', NOW()-INTERVAL '3 hours',  215000.00),
  -- Paul Githaiga (wealthy)
  (uuid_generate_v4(),'TXN-20260311-0150','33333333-0000-0000-0016-000000000001','Deposit',  300000.00,'KES','Success','Business income',              NOW()-INTERVAL '4 hours', NOW()-INTERVAL '4 hours', 1050000.00),
  (uuid_generate_v4(),'TXN-20260311-0151','33333333-0000-0000-0016-000000000001','Transfer', 300000.00,'KES','Success','Investment allocation',        NOW()-INTERVAL '5 hours', NOW()-INTERVAL '5 hours',  750000.00),
  -- Steve Macharia
  (uuid_generate_v4(),'TXN-20260311-0180','33333333-0000-0000-0019-000000000001','Deposit',  150000.00,'KES','Success','Invoice payment received',     NOW()-INTERVAL '6 hours', NOW()-INTERVAL '6 hours',  770000.00),
  (uuid_generate_v4(),'TXN-20260311-0181','33333333-0000-0000-0019-000000000001','Payment',  150000.00,'KES','Pending', 'Supplier payment batch',      NOW()-INTERVAL '30 minutes',NULL,  620000.00)
ON CONFLICT ("TransactionReference") DO NOTHING;

-- ──────────────────────────────────────────────
-- 5. BRANCHES
-- ──────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS "Branches" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "BranchCode" VARCHAR(20) UNIQUE NOT NULL,
    "BranchName" VARCHAR(100) NOT NULL,
    "Address" VARCHAR(255),
    "City" VARCHAR(100),
    "Region" VARCHAR(100),
    "PhoneNumber" VARCHAR(20),
    "IsActive" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO "Branches" ("BranchCode", "BranchName", "Address", "City", "Region", "IsActive") VALUES
  ('HQ001', 'Wekeza Bank - Headquarters',    'Westlands, Waiyaki Way', 'Nairobi', 'Nairobi',      true),
  ('CBD002', 'Wekeza Bank - CBD Branch',     'Kenyatta Avenue',        'Nairobi', 'Nairobi',      true),
  ('KSM003', 'Wekeza Bank - Kisumu Branch',  'Oginga Odinga Street',   'Kisumu',  'Nyanza',       true),
  ('MSA004', 'Wekeza Bank - Mombasa Branch', 'Moi Avenue',             'Mombasa', 'Coast',        true),
  ('NKR005', 'Wekeza Bank - Nakuru Branch',  'Kenyatta Avenue',        'Nakuru',  'Rift Valley',  true)
ON CONFLICT ("BranchCode") DO NOTHING;

-- ──────────────────────────────────────────────
-- 6. Audit log entry
-- ──────────────────────────────────────────────
INSERT INTO audit.audit_log (table_name, operation, new_data, changed_by)
VALUES ('system', 'SEED',
        '{"message": "Comprehensive banking data seeded - 12 portal users, 20 customers, 28 accounts, 28 transactions, 5 branches"}',
        'system')
ON CONFLICT DO NOTHING;

SELECT
  (SELECT COUNT(*) FROM "Users")        AS users_seeded,
  (SELECT COUNT(*) FROM "Customers")    AS customers_seeded,
  (SELECT COUNT(*) FROM "Accounts")     AS accounts_seeded,
  (SELECT COUNT(*) FROM "Transactions") AS transactions_seeded,
  (SELECT COUNT(*) FROM "Branches")     AS branches_seeded;
