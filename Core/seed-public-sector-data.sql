-- Seed data for Public Sector Portal

-- Insert admin user
INSERT INTO "Users" ("Id", "Username", "Email", "PasswordHash", "FirstName", "LastName", "Status")
VALUES 
('11111111-1111-1111-1111-111111111111', 'admin', 'admin@wekeza.com', '$2a$11$hashedpassword', 'System', 'Administrator', 'Active')
ON CONFLICT ("Username") DO NOTHING;

-- Insert Government Customers
INSERT INTO "Customers" ("Id", "CustomerNumber", "CustomerType", "Name", "Email", "PhoneNumber", "Status")
VALUES 
('22222222-2222-2222-2222-222222222222', 'GOV001', 'GOVERNMENT', 'National Treasury', 'treasury@gov.ke', '+254700000001', 'Active'),
('33333333-3333-3333-3333-333333333333', 'GOV002', 'GOVERNMENT', 'Nairobi County Government', 'finance@nairobi.go.ke', '+254700000002', 'Active'),
('44444444-4444-4444-4444-444444444444', 'GOV003', 'GOVERNMENT', 'Mombasa County Government', 'finance@mombasa.go.ke', '+254700000003', 'Active'),
('55555555-5555-5555-5555-555555555555', 'GOV004', 'GOVERNMENT', 'Ministry of Education', 'finance@education.go.ke', '+254700000004', 'Active')
ON CONFLICT ("CustomerNumber") DO NOTHING;

-- Insert Government Accounts
INSERT INTO "Accounts" ("Id", "AccountNumber", "CustomerId", "AccountType", "BalanceAmount", "CurrencyCode", "Status", "OpenedDate")
VALUES 
('66666666-6666-6666-6666-666666666666', 'ACC1000000001', '22222222-2222-2222-2222-222222222222', 'CURRENT', 50000000000.00, 'KES', 'Active', '2024-01-01'),
('77777777-7777-7777-7777-777777777777', 'ACC1000000002', '33333333-3333-3333-3333-333333333333', 'CURRENT', 15000000000.00, 'KES', 'Active', '2024-01-01'),
('88888888-8888-8888-8888-888888888888', 'ACC1000000003', '44444444-4444-4444-4444-444444444444', 'CURRENT', 8000000000.00, 'KES', 'Active', '2024-01-01'),
('99999999-9999-9999-9999-999999999999', 'ACC1000000004', '55555555-5555-5555-5555-555555555555', 'CURRENT', 12000000000.00, 'KES', 'Active', '2024-01-01')
ON CONFLICT ("AccountNumber") DO NOTHING;

-- Insert Government Securities (T-Bills)
INSERT INTO "Securities" ("Id", "SecurityCode", "SecurityType", "Name", "IssueDate", "MaturityDate", "CouponRate", "FaceValue", "CurrentPrice", "Status")
VALUES 
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'TBILL-91-001', 'TBILL', '91-Day Treasury Bill', '2026-01-15', '2026-04-16', 12.50, 100000.00, 96875.00, 'Active'),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'TBILL-182-001', 'TBILL', '182-Day Treasury Bill', '2026-01-15', '2026-07-16', 13.00, 100000.00, 93750.00, 'Active'),
('cccccccc-cccc-cccc-cccc-cccccccccccc', 'TBILL-364-001', 'TBILL', '364-Day Treasury Bill', '2026-01-15', '2027-01-14', 13.50, 100000.00, 88235.00, 'Active')
ON CONFLICT ("SecurityCode") DO NOTHING;

-- Insert Government Bonds
INSERT INTO "Securities" ("Id", "SecurityCode", "SecurityType", "Name", "IssueDate", "MaturityDate", "CouponRate", "FaceValue", "CurrentPrice", "Status")
VALUES 
('dddddddd-dddd-dddd-dddd-dddddddddddd', 'BOND-5Y-001', 'BOND', '5-Year Infrastructure Bond', '2024-01-01', '2029-01-01', 14.50, 1000000.00, 1025000.00, 'Active'),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'BOND-10Y-001', 'BOND', '10-Year Development Bond', '2023-06-01', '2033-06-01', 15.00, 1000000.00, 1050000.00, 'Active'),
('ffffffff-ffff-ffff-ffff-ffffffffffff', 'BOND-15Y-001', 'BOND', '15-Year Sovereign Bond', '2022-01-01', '2037-01-01', 15.50, 1000000.00, 1075000.00, 'Active')
ON CONFLICT ("SecurityCode") DO NOTHING;

-- Insert Security Orders
INSERT INTO "SecurityOrders" ("Id", "OrderNumber", "CustomerId", "SecurityId", "OrderType", "Quantity", "Price", "TotalAmount", "Status", "OrderDate", "ExecutionDate")
VALUES 
('10101010-1010-1010-1010-101010101010', 'ORD-2026-001', '22222222-2222-2222-2222-222222222222', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'BUY', 50000, 96875.00, 4843750000.00, 'Executed', '2026-01-15', '2026-01-15'),
('20202020-2020-2020-2020-202020202020', 'ORD-2026-002', '22222222-2222-2222-2222-222222222222', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'BUY', 3000, 1025000.00, 3075000000.00, 'Executed', '2026-01-20', '2026-01-20'),
('30303030-3030-3030-3030-303030303030', 'ORD-2026-003', '33333333-3333-3333-3333-333333333333', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'BUY', 15000, 93750.00, 1406250000.00, 'Executed', '2026-01-25', '2026-01-25')
ON CONFLICT ("OrderNumber") DO NOTHING;

-- Insert Government Loans
INSERT INTO "Loans" ("Id", "LoanNumber", "CustomerId", "LoanType", "PrincipalAmount", "InterestRate", "TenorMonths", "OutstandingBalance", "Status", "ApplicationDate", "ApprovalDate", "DisbursementDate", "MaturityDate")
VALUES 
('40404040-4040-4040-4040-404040404040', 'LOAN-GOV-001', '22222222-2222-2222-2222-222222222222', 'GOVERNMENT', 30000000000.00, 10.50, 120, 30000000000.00, 'Disbursed', '2025-01-01', '2025-01-15', '2025-02-01', '2035-02-01'),
('50505050-5050-5050-5050-505050505050', 'LOAN-GOV-002', '33333333-3333-3333-3333-333333333333', 'GOVERNMENT', 10000000000.00, 11.00, 60, 10000000000.00, 'Disbursed', '2025-06-01', '2025-06-15', '2025-07-01', '2030-07-01'),
('60606060-6060-6060-6060-606060606060', 'LOAN-GOV-003', '44444444-4444-4444-4444-444444444444', 'GOVERNMENT', 5000000000.00, 11.50, 36, 5000000000.00, 'Disbursed', '2025-09-01', '2025-09-15', '2025-10-01', '2028-10-01')
ON CONFLICT ("LoanNumber") DO NOTHING;

-- Insert Grants
INSERT INTO "Grants" ("Id", "GrantNumber", "CustomerId", "GrantType", "Amount", "Purpose", "Status", "ApplicationDate", "ApprovalDate", "DisbursementDate", "ComplianceRate")
VALUES 
('70707070-7070-7070-7070-707070707070', 'GRT-2026-001', '55555555-5555-5555-5555-555555555555', 'EDUCATION', 1500000000.00, 'School Infrastructure Development', 'Disbursed', '2025-11-01', '2025-11-15', '2025-12-01', 95.50),
('80808080-8080-8080-8080-808080808080', 'GRT-2026-002', '33333333-3333-3333-3333-333333333333', 'HEALTH', 800000000.00, 'County Hospital Equipment', 'Disbursed', '2025-12-01', '2025-12-15', '2026-01-01', 92.00),
('90909090-9090-9090-9090-909090909090', 'GRT-2026-003', '44444444-4444-4444-4444-444444444444', 'INFRASTRUCTURE', 200000000.00, 'Road Maintenance Program', 'Approved', '2026-01-15', '2026-02-01', NULL, NULL)
ON CONFLICT ("GrantNumber") DO NOTHING;

-- Insert sample transactions
INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1', 'TXN-2026-001', '66666666-6666-6666-6666-666666666666', 'DEPOSIT', 5000000000.00, 'KES', 50000000000.00, 'Tax Revenue Collection', 'Posted', '2026-02-01', '2026-02-01 10:00:00'),
('b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2', 'TXN-2026-002', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 2000000000.00, 'KES', 15000000000.00, 'County Revenue Collection', 'Posted', '2026-02-05', '2026-02-05 14:30:00'),
('c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3', 'TXN-2026-003', '66666666-6666-6666-6666-666666666666', 'WITHDRAWAL', 3000000000.00, 'KES', 47000000000.00, 'Infrastructure Project Payment', 'Posted', '2026-02-10', '2026-02-10 09:15:00')
ON CONFLICT ("TransactionReference") DO NOTHING;
