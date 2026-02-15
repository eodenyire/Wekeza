-- Add historical transaction data for revenue trends (last 6 months)

-- January 2026 transactions
INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1', 'TXN-2026-JAN-001', '66666666-6666-6666-6666-666666666666', 'DEPOSIT', 2800000000.00, 'KES', 50000000000.00, 'January Tax Revenue', 'Posted', '2026-01-15', '2026-01-15 10:00:00'),
('d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2', 'TXN-2026-JAN-002', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 2700000000.00, 'KES', 15000000000.00, 'January County Revenue', 'Posted', '2026-01-20', '2026-01-20 14:30:00')
ON CONFLICT ("TransactionReference") DO NOTHING;

-- December 2025 transactions
INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1', 'TXN-2025-DEC-001', '66666666-6666-6666-6666-666666666666', 'DEPOSIT', 2600000000.00, 'KES', 48000000000.00, 'December Tax Revenue', 'Posted', '2025-12-15', '2025-12-15 10:00:00'),
('e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2', 'TXN-2025-DEC-002', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 2900000000.00, 'KES', 14000000000.00, 'December County Revenue', 'Posted', '2025-12-20', '2025-12-20 14:30:00')
ON CONFLICT ("TransactionReference") DO NOTHING;

-- November 2025 transactions
INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1', 'TXN-2025-NOV-001', '66666666-6666-6666-6666-666666666666', 'DEPOSIT', 2500000000.00, 'KES', 46000000000.00, 'November Tax Revenue', 'Posted', '2025-11-15', '2025-11-15 10:00:00'),
('f2f2f2f2-f2f2-f2f2-f2f2-f2f2f2f2f2f2', 'TXN-2025-NOV-002', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 2400000000.00, 'KES', 13000000000.00, 'November County Revenue', 'Posted', '2025-11-20', '2025-11-20 14:30:00')
ON CONFLICT ("TransactionReference") DO NOTHING;

-- October 2025 transactions
INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('11111111-1111-1111-1111-111111111101', 'TXN-2025-OCT-001', '66666666-6666-6666-6666-666666666666', 'DEPOSIT', 2700000000.00, 'KES', 44000000000.00, 'October Tax Revenue', 'Posted', '2025-10-15', '2025-10-15 10:00:00'),
('11111111-1111-1111-1111-111111111102', 'TXN-2025-OCT-002', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 2600000000.00, 'KES', 12000000000.00, 'October County Revenue', 'Posted', '2025-10-20', '2025-10-20 14:30:00')
ON CONFLICT ("TransactionReference") DO NOTHING;

-- September 2025 transactions
INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('11111111-1111-1111-1111-111111111103', 'TXN-2025-SEP-001', '66666666-6666-6666-6666-666666666666', 'DEPOSIT', 2800000000.00, 'KES', 42000000000.00, 'September Tax Revenue', 'Posted', '2025-09-15', '2025-09-15 10:00:00'),
('11111111-1111-1111-1111-111111111104', 'TXN-2025-SEP-002', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 2500000000.00, 'KES', 11000000000.00, 'September County Revenue', 'Posted', '2025-09-20', '2025-09-20 14:30:00')
ON CONFLICT ("TransactionReference") DO NOTHING;

-- August 2025 transactions
INSERT INTO "Transactions" ("Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "CurrencyCode", "BalanceAfter", "Description", "Status", "ValueDate", "PostedDate")
VALUES 
('11111111-1111-1111-1111-111111111105', 'TXN-2025-AUG-001', '66666666-6666-6666-6666-666666666666', 'DEPOSIT', 2900000000.00, 'KES', 40000000000.00, 'August Tax Revenue', 'Posted', '2025-08-15', '2025-08-15 10:00:00'),
('11111111-1111-1111-1111-111111111106', 'TXN-2025-AUG-002', '77777777-7777-7777-7777-777777777777', 'DEPOSIT', 2700000000.00, 'KES', 10000000000.00, 'August County Revenue', 'Posted', '2025-08-20', '2025-08-20 14:30:00')
ON CONFLICT ("TransactionReference") DO NOTHING;

-- Add historical grant disbursements for grant trends
INSERT INTO "Grants" ("Id", "GrantNumber", "CustomerId", "GrantType", "Amount", "Purpose", "Status", "ApplicationDate", "ApprovalDate", "DisbursementDate", "ComplianceRate")
VALUES 
('11111111-1111-1111-1111-111111111107', 'GRT-2025-AUG-001', '55555555-5555-5555-5555-555555555555', 'EDUCATION', 600000000.00, 'School Construction', 'Disbursed', '2025-07-01', '2025-07-15', '2025-08-01', 94.00),
('11111111-1111-1111-1111-111111111108', 'GRT-2025-SEP-001', '33333333-3333-3333-3333-333333333333', 'HEALTH', 700000000.00, 'Medical Equipment', 'Disbursed', '2025-08-01', '2025-08-15', '2025-09-01', 96.00),
('11111111-1111-1111-1111-111111111109', 'GRT-2025-OCT-001', '44444444-4444-4444-4444-444444444444', 'INFRASTRUCTURE', 500000000.00, 'Water Supply Project', 'Disbursed', '2025-09-01', '2025-09-15', '2025-10-01', 91.00),
('11111111-1111-1111-1111-111111111110', 'GRT-2025-NOV-001', '55555555-5555-5555-5555-555555555555', 'EDUCATION', 650000000.00, 'Teacher Training', 'Disbursed', '2025-10-01', '2025-10-15', '2025-11-01', 93.50),
('11111111-1111-1111-1111-111111111111', 'GRT-2025-DEC-001', '33333333-3333-3333-3333-333333333333', 'HEALTH', 750000000.00, 'Hospital Expansion', 'Disbursed', '2025-11-01', '2025-11-15', '2025-12-01', 95.00),
('11111111-1111-1111-1111-111111111112', 'GRT-2026-JAN-001', '44444444-4444-4444-4444-444444444444', 'INFRASTRUCTURE', 800000000.00, 'Bridge Construction', 'Disbursed', '2025-12-01', '2025-12-15', '2026-01-01', 92.50)
ON CONFLICT ("GrantNumber") DO NOTHING;
