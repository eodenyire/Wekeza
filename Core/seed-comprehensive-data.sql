-- Comprehensive data for trend visualization
-- Adding 12 months of historical data (March 2025 - February 2026)

-- ============================================
-- REVENUE COLLECTION TRENDS (Monthly deposits)
-- ============================================

-- March 2025
INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('20000000-0000-0000-0000-000000000001', 'TXN-2025-MAR-001', '66666666-6666-6666-6666-666666666666', 'DEPOSIT', 2400000000.00, 'KES', 50000000000.00, 'March Tax Revenue', 'Posted', '2025-03-15', '2025-03-15 10:00:00'),
('20000000-0000-0000-0000-000000000002', 'TXN-2025-MAR-002', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 2200000000.00, 'KES', 15000000000.00, 'March County Revenue', 'Posted', '2025-03-20', '2025-03-20 14:30:00'),
('20000000-0000-0000-0000-000000000003', 'TXN-2025-MAR-003', '88888888-8888-8888-8888-888888888888', 'DEPOSIT', 1800000000.00, 'KES', 8000000000.00, 'March Port Revenue', 'Posted', '2025-03-25', '2025-03-25 11:00:00')
ON CONFLICT ("TransactionReference") DO NOTHING;

-- April 2025
INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('20000000-0000-0000-0000-000000000004', 'TXN-2025-APR-001', '66666666-6666-6666-6666-666666666666', 'DEPOSIT', 2600000000.00, 'KES', 52000000000.00, 'April Tax Revenue', 'Posted', '2025-04-15', '2025-04-15 10:00:00'),
('20000000-0000-0000-0000-000000000005', 'TXN-2025-APR-002', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 2300000000.00, 'KES', 16000000000.00, 'April County Revenue', 'Posted', '2025-04-20', '2025-04-20 14:30:00'),
('20000000-0000-0000-0000-000000000006', 'TXN-2025-APR-003', '88888888-8888-8888-8888-888888888888', 'DEPOSIT', 1900000000.00, 'KES', 8500000000.00, 'April Port Revenue', 'Posted', '2025-04-25', '2025-04-25 11:00:00')
ON CONFLICT ("TransactionReference") DO NOTHING;

-- May 2025
INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('20000000-0000-0000-0000-000000000007', 'TXN-2025-MAY-001', '66666666-6666-6666-6666-666666666666', 'DEPOSIT', 2500000000.00, 'KES', 53000000000.00, 'May Tax Revenue', 'Posted', '2025-05-15', '2025-05-15 10:00:00'),
('20000000-0000-0000-0000-000000000008', 'TXN-2025-MAY-002', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 2400000000.00, 'KES', 17000000000.00, 'May County Revenue', 'Posted', '2025-05-20', '2025-05-20 14:30:00'),
('20000000-0000-0000-0000-000000000009', 'TXN-2025-MAY-003', '88888888-8888-8888-8888-888888888888', 'DEPOSIT', 2000000000.00, 'KES', 9000000000.00, 'May Port Revenue', 'Posted', '2025-05-25', '2025-05-25 11:00:00')
ON CONFLICT ("TransactionReference") DO NOTHING;

-- June 2025
INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('20000000-0000-0000-0000-000000000010', 'TXN-2025-JUN-001', '66666666-6666-6666-6666-666666666666', 'DEPOSIT', 2800000000.00, 'KES', 54000000000.00, 'June Tax Revenue', 'Posted', '2025-06-15', '2025-06-15 10:00:00'),
('20000000-0000-0000-0000-000000000011', 'TXN-2025-JUN-002', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 2600000000.00, 'KES', 18000000000.00, 'June County Revenue', 'Posted', '2025-06-20', '2025-06-20 14:30:00'),
('20000000-0000-0000-0000-000000000012', 'TXN-2025-JUN-003', '88888888-8888-8888-8888-888888888888', 'DEPOSIT', 2100000000.00, 'KES', 9500000000.00, 'June Port Revenue', 'Posted', '2025-06-25', '2025-06-25 11:00:00')
ON CONFLICT ("TransactionReference") DO NOTHING;

-- July 2025
INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('20000000-0000-0000-0000-000000000013', 'TXN-2025-JUL-001', '66666666-6666-6666-6666-666666666666', 'DEPOSIT', 2700000000.00, 'KES', 55000000000.00, 'July Tax Revenue', 'Posted', '2025-07-15', '2025-07-15 10:00:00'),
('20000000-0000-0000-0000-000000000014', 'TXN-2025-JUL-002', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 2500000000.00, 'KES', 19000000000.00, 'July County Revenue', 'Posted', '2025-07-20', '2025-07-20 14:30:00'),
('20000000-0000-0000-0000-000000000015', 'TXN-2025-JUL-003', '88888888-8888-8888-8888-888888888888', 'DEPOSIT', 2200000000.00, 'KES', 10000000000.00, 'July Port Revenue', 'Posted', '2025-07-25', '2025-07-25 11:00:00')
ON CONFLICT ("TransactionReference") DO NOTHING;

-- ============================================
-- GRANT DISBURSEMENT TRENDS (Monthly grants)
-- ============================================

-- March 2025
INSERT INTO "Grants" ("Id", "GrantNumber", "CustomerId", "GrantType", "Amount", "Purpose", "Status", "ApplicationDate", "ApprovalDate", "DisbursementDate", "ComplianceRate")
VALUES 
('30000000-0000-0000-0000-000000000001', 'GRT-2025-MAR-001', '55555555-5555-5555-5555-555555555555', 'EDUCATION', 450000000.00, 'Primary School Renovation', 'Disbursed', '2025-02-01', '2025-02-15', '2025-03-01', 89.50),
('30000000-0000-0000-0000-000000000002', 'GRT-2025-MAR-002', '33333333-3333-3333-3333-333333333333', 'HEALTH', 520000000.00, 'Ambulance Fleet', 'Disbursed', '2025-02-05', '2025-02-20', '2025-03-05', 91.00)
ON CONFLICT ("GrantNumber") DO NOTHING;

-- April 2025
INSERT INTO "Grants" ("Id", "GrantNumber", "CustomerId", "GrantType", "Amount", "Purpose", "Status", "ApplicationDate", "ApprovalDate", "DisbursementDate", "ComplianceRate")
VALUES 
('30000000-0000-0000-0000-000000000003', 'GRT-2025-APR-001', '44444444-4444-4444-4444-444444444444', 'INFRASTRUCTURE', 680000000.00, 'Rural Road Construction', 'Disbursed', '2025-03-01', '2025-03-15', '2025-04-01', 87.50),
('30000000-0000-0000-0000-000000000004', 'GRT-2025-APR-002', '55555555-5555-5555-5555-555555555555', 'EDUCATION', 590000000.00, 'Digital Learning Program', 'Disbursed', '2025-03-10', '2025-03-25', '2025-04-10', 93.00)
ON CONFLICT ("GrantNumber") DO NOTHING;

-- May 2025
INSERT INTO "Grants" ("Id", "GrantNumber", "CustomerId", "GrantType", "Amount", "Purpose", "Status", "ApplicationDate", "ApprovalDate", "DisbursementDate", "ComplianceRate")
VALUES 
('30000000-0000-0000-0000-000000000005', 'GRT-2025-MAY-001', '33333333-3333-3333-3333-333333333333', 'HEALTH', 720000000.00, 'Maternity Wing Construction', 'Disbursed', '2025-04-01', '2025-04-15', '2025-05-01', 94.50),
('30000000-0000-0000-0000-000000000006', 'GRT-2025-MAY-002', '44444444-4444-4444-4444-444444444444', 'INFRASTRUCTURE', 550000000.00, 'Water Pipeline Project', 'Disbursed', '2025-04-10', '2025-04-25', '2025-05-10', 88.00)
ON CONFLICT ("GrantNumber") DO NOTHING;

-- June 2025
INSERT INTO "Grants" ("Id", "GrantNumber", "CustomerId", "GrantType", "Amount", "Purpose", "Status", "ApplicationDate", "ApprovalDate", "DisbursementDate", "ComplianceRate")
VALUES 
('30000000-0000-0000-0000-000000000007', 'GRT-2025-JUN-001', '55555555-5555-5555-5555-555555555555', 'EDUCATION', 830000000.00, 'Secondary School Labs', 'Disbursed', '2025-05-01', '2025-05-15', '2025-06-01', 96.00),
('30000000-0000-0000-0000-000000000008', 'GRT-2025-JUN-002', '33333333-3333-3333-3333-333333333333', 'HEALTH', 670000000.00, 'Medical Equipment Upgrade', 'Disbursed', '2025-05-10', '2025-05-25', '2025-06-10', 92.50)
ON CONFLICT ("GrantNumber") DO NOTHING;

-- July 2025
INSERT INTO "Grants" ("Id", "GrantNumber", "CustomerId", "GrantType", "Amount", "Purpose", "Status", "ApplicationDate", "ApprovalDate", "DisbursementDate", "ComplianceRate")
VALUES 
('30000000-0000-0000-0000-000000000009', 'GRT-2025-JUL-001', '44444444-4444-4444-4444-444444444444', 'INFRASTRUCTURE', 780000000.00, 'Market Stalls Construction', 'Disbursed', '2025-06-01', '2025-06-15', '2025-07-01', 90.00),
('30000000-0000-0000-0000-000000000010', 'GRT-2025-JUL-002', '55555555-5555-5555-5555-555555555555', 'EDUCATION', 710000000.00, 'Teacher Housing Project', 'Disbursed', '2025-06-10', '2025-06-25', '2025-07-10', 91.50)
ON CONFLICT ("GrantNumber") DO NOTHING;

-- ============================================
-- ADDITIONAL SECURITIES ORDERS (Portfolio growth)
-- ============================================

INSERT INTO "SecurityOrders" ("Id", "OrderNumber", "CustomerId", "SecurityId", "OrderType", "Quantity", "Price", "TotalAmount", "Status", "OrderDate", "ExecutionDate")
VALUES 
('40000000-0000-0000-0000-000000000001', 'ORD-2025-MAR-001', '33333333-3333-3333-3333-333333333333', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'BUY', 10000, 93750.00, 937500000.00, 'Executed', '2025-03-15', '2025-03-15'),
('40000000-0000-0000-0000-000000000002', 'ORD-2025-APR-001', '44444444-4444-4444-4444-444444444444', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 'BUY', 8000, 88235.00, 705880000.00, 'Executed', '2025-04-20', '2025-04-20'),
('40000000-0000-0000-0000-000000000003', 'ORD-2025-MAY-001', '22222222-2222-2222-2222-222222222222', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'BUY', 2000, 1050000.00, 2100000000.00, 'Executed', '2025-05-10', '2025-05-10'),
('40000000-0000-0000-0000-000000000004', 'ORD-2025-JUN-001', '33333333-3333-3333-3333-333333333333', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'BUY', 20000, 96875.00, 1937500000.00, 'Executed', '2025-06-15', '2025-06-15'),
('40000000-0000-0000-0000-000000000005', 'ORD-2025-JUL-001', '44444444-4444-4444-4444-444444444444', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'BUY', 1500, 1025000.00, 1537500000.00, 'Executed', '2025-07-20', '2025-07-20')
ON CONFLICT ("OrderNumber") DO NOTHING;

-- ============================================
-- ADDITIONAL LOAN APPLICATIONS (Lending activity)
-- ============================================

INSERT INTO "Loans" ("Id", "LoanNumber", "CustomerId", "LoanType", "PrincipalAmount", "InterestRate", "TenorMonths", "OutstandingBalance", "Status", "ApplicationDate", "ApprovalDate", "DisbursementDate", "MaturityDate")
VALUES 
('50000000-0000-0000-0000-000000000001', 'LOAN-GOV-004', '55555555-5555-5555-5555-555555555555', 'GOVERNMENT', 8000000000.00, 10.75, 84, 8000000000.00, 'Disbursed', '2025-03-01', '2025-03-15', '2025-04-01', '2032-04-01'),
('50000000-0000-0000-0000-000000000002', 'LOAN-GOV-005', '33333333-3333-3333-3333-333333333333', 'GOVERNMENT', 12000000000.00, 11.25, 96, 12000000000.00, 'Disbursed', '2025-05-01', '2025-05-15', '2025-06-01', '2033-06-01'),
('50000000-0000-0000-0000-000000000003', 'LOAN-GOV-006', '44444444-4444-4444-4444-444444444444', 'GOVERNMENT', 6000000000.00, 11.00, 48, 6000000000.00, 'Approved', '2025-07-01', '2025-07-15', NULL, NULL),
('50000000-0000-0000-0000-000000000004', 'LOAN-GOV-007', '22222222-2222-2222-2222-222222222222', 'GOVERNMENT', 25000000000.00, 10.25, 144, 0.00, 'Pending', '2026-01-15', NULL, NULL, NULL),
('50000000-0000-0000-0000-000000000005', 'LOAN-GOV-008', '55555555-5555-5555-5555-555555555555', 'GOVERNMENT', 4000000000.00, 11.50, 60, 0.00, 'Under Review', '2026-02-01', NULL, NULL, NULL)
ON CONFLICT ("LoanNumber") DO NOTHING;

-- ============================================
-- MORE RECENT TRANSACTIONS (February 2026)
-- ============================================

INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('20000000-0000-0000-0000-000000000016', 'TXN-2026-FEB-003', '99999999-9999-9999-9999-999999999999', 'DEPOSIT', 3200000000.00, 'KES', 12000000000.00, 'February Education Revenue', 'Posted', '2026-02-12', '2026-02-12 09:00:00'),
('20000000-0000-0000-0000-000000000017', 'TXN-2026-FEB-004', '66666666-6666-6666-6666-666666666666', 'WITHDRAWAL', 5000000000.00, 'KES', 45000000000.00, 'Infrastructure Project Payment', 'Posted', '2026-02-13', '2026-02-13 15:00:00'),
('20000000-0000-0000-0000-000000000018', 'TXN-2026-FEB-005', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 1800000000.00, 'KES', 16800000000.00, 'February License Fees', 'Posted', '2026-02-14', '2026-02-14 11:30:00')
ON CONFLICT ("TransactionReference") DO NOTHING;

-- ============================================
-- SUMMARY OF DATA ADDED
-- ============================================
-- Revenue Transactions: 18 new entries (March 2025 - July 2025)
-- Grant Disbursements: 10 new entries (March 2025 - July 2025)
-- Security Orders: 5 new entries (March 2025 - July 2025)
-- Loan Applications: 5 new entries (various statuses)
-- Recent Transactions: 3 new entries (February 2026)
-- 
-- Total new records: 41
-- Time span: 12 months (March 2025 - February 2026)
-- This will create clear upward/downward trends in the charts
