-- Create sample customers
INSERT INTO "Customers" ("Id", "CustomerNumber", "FullName", "Email", "PhoneNumber", "Address", "IdNumber", "IdType", "DateOfBirth", "Gender", "Occupation", "EmployerName", "EmployerAddress", "KycStatus", "KycExpiryDate", "CustomerStatus", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy") VALUES
('11111111-1111-1111-1111-111111111111', 'CUS001', 'John Doe', 'john.doe@email.com', '+256701234567', '123 Main Street, Kampala', 'CM12345678', 'National ID', '1985-05-15', 'Male', 'Software Engineer', 'Tech Solutions Ltd', '456 Business Ave, Kampala', 'Approved', '2025-05-15', 'Active', '2024-01-01 10:00:00', '2024-01-01 10:00:00', 'system', 'system'),
('22222222-2222-2222-2222-222222222222', 'CUS002', 'Jane Smith', 'jane.smith@email.com', '+256702345678', '789 Oak Road, Entebbe', 'CM23456789', 'National ID', '1990-08-22', 'Female', 'Teacher', 'Entebbe Primary School', '321 School Lane, Entebbe', 'Approved', '2025-08-22', 'Active', '2024-01-02 11:00:00', '2024-01-02 11:00:00', 'system', 'system'),
('33333333-3333-3333-3333-333333333333', 'CUS003', 'Michael Johnson', 'michael.johnson@email.com', '+256703456789', '456 Pine Street, Jinja', 'CM34567890', 'National ID', '1988-12-10', 'Male', 'Business Owner', 'Johnson Enterprises', '789 Commerce St, Jinja', 'Pending', '2025-12-10', 'Active', '2024-01-03 12:00:00', '2024-01-03 12:00:00', 'system', 'system'),
('44444444-4444-4444-4444-444444444444', 'CUS004', 'Sarah Wilson', 'sarah.wilson@email.com', '+256704567890', '321 Cedar Ave, Mbarara', 'CM45678901', 'National ID', '1992-03-18', 'Female', 'Nurse', 'Mbarara Hospital', '654 Health Rd, Mbarara', 'Approved', '2025-03-18', 'Active', '2024-01-04 13:00:00', '2024-01-04 13:00:00', 'system', 'system'),
('55555555-5555-5555-5555-555555555555', 'CUS005', 'David Brown', 'david.brown@email.com', '+256705678901', '987 Elm Street, Gulu', 'CM56789012', 'National ID', '1987-07-25', 'Male', 'Farmer', 'Brown Farms', '123 Rural Rd, Gulu', 'Expired', '2024-07-25', 'Dormant', '2024-01-05 14:00:00', '2024-01-05 14:00:00', 'system', 'system');

-- Create sample accounts
INSERT INTO "Accounts" ("Id", "AccountNumber", "AccountName", "AccountType", "Balance", "AvailableBalance", "Currency", "Status", "OpenedDate", "LastTransactionDate", "IsDormant", "IsFrozen", "BranchCode", "CustomerId") VALUES
('aaaa1111-1111-1111-1111-111111111111', 'ACC001001', 'John Doe - Savings', 'Savings', 150000.00, 148500.00, 'UGX', 'Active', '2024-01-01', '2026-01-20', false, false, 'BR001', '11111111-1111-1111-1111-111111111111'),
('aaaa2222-2222-2222-2222-222222222222', 'ACC001002', 'John Doe - Current', 'Current', 75000.00, 70000.00, 'UGX', 'Active', '2024-01-01', '2026-01-22', false, false, 'BR001', '11111111-1111-1111-1111-111111111111'),
('bbbb1111-1111-1111-1111-111111111111', 'ACC002001', 'Jane Smith - Savings', 'Savings', 250000.00, 245000.00, 'UGX', 'Active', '2024-01-02', '2026-01-21', false, false, 'BR002', '22222222-2222-2222-2222-222222222222'),
('cccc1111-1111-1111-1111-111111111111', 'ACC003001', 'Michael Johnson - Business', 'Business', 500000.00, 480000.00, 'UGX', 'Active', '2024-01-03', '2026-01-23', false, false, 'BR003', '33333333-3333-3333-3333-333333333333'),
('dddd1111-1111-1111-1111-111111111111', 'ACC004001', 'Sarah Wilson - Savings', 'Savings', 180000.00, 175000.00, 'UGX', 'Active', '2024-01-04', '2026-01-19', false, false, 'BR004', '44444444-4444-4444-4444-444444444444'),
('eeee1111-1111-1111-1111-111111111111', 'ACC005001', 'David Brown - Savings', 'Savings', 25000.00, 25000.00, 'UGX', 'Dormant', '2024-01-05', '2025-06-15', true, false, 'BR005', '55555555-5555-5555-5555-555555555555');

-- Create sample transactions
INSERT INTO "Transactions" ("Id", "TransactionId", "AccountNumber", "TransactionType", "Amount", "Currency", "Description", "Reference", "TransactionDate", "ValueDate", "Status", "RunningBalance", "Channel", "InitiatedBy", "AccountId") VALUES
('11111111-1111-1111-1111-111111111111', 'TXN20260120001', 'ACC001001', 'Deposit', 50000.00, 'UGX', 'Cash Deposit', 'DEP001', '2026-01-20 09:30:00', '2026-01-20 09:30:00', 'Completed', 150000.00, 'Teller', 'teller1', 'aaaa1111-1111-1111-1111-111111111111'),
('22222222-2222-2222-2222-222222222222', 'TXN20260120002', 'ACC001001', 'Withdrawal', -10000.00, 'UGX', 'ATM Withdrawal', 'WDL001', '2026-01-20 14:15:00', '2026-01-20 14:15:00', 'Completed', 140000.00, 'ATM', 'customer', 'aaaa1111-1111-1111-1111-111111111111'),
('33333333-3333-3333-3333-333333333333', 'TXN20260121001', 'ACC002001', 'Transfer In', 25000.00, 'UGX', 'Salary Transfer', 'SAL001', '2026-01-21 08:00:00', '2026-01-21 08:00:00', 'Completed', 250000.00, 'Online', 'employer', 'bbbb1111-1111-1111-1111-111111111111'),
('44444444-4444-4444-4444-444444444444', 'TXN20260122001', 'ACC001002', 'Transfer Out', -5000.00, 'UGX', 'Utility Payment', 'UTIL001', '2026-01-22 16:45:00', '2026-01-22 16:45:00', 'Completed', 75000.00, 'Mobile', 'customer', 'aaaa2222-2222-2222-2222-222222222222'),
('55555555-5555-5555-5555-555555555555', 'TXN20260123001', 'ACC003001', 'Deposit', 100000.00, 'UGX', 'Business Revenue', 'BUS001', '2026-01-23 11:20:00', '2026-01-23 11:20:00', 'Completed', 500000.00, 'Teller', 'teller2', 'cccc1111-1111-1111-1111-111111111111');

-- Create sample standing instructions
INSERT INTO "StandingInstructions" ("Id", "InstructionId", "FromAccount", "ToAccount", "BeneficiaryName", "Amount", "Frequency", "StartDate", "EndDate", "NextExecutionDate", "Status", "Description", "AccountId") VALUES
('11111111-1111-1111-1111-111111111112', 'SI001', 'ACC001001', 'ACC999001', 'Utility Company', 50000.00, 'Monthly', '2024-02-01', '2025-02-01', '2026-02-01', 'Active', 'Monthly Utility Payment', 'aaaa1111-1111-1111-1111-111111111111'),
('22222222-2222-2222-2222-222222222223', 'SI002', 'ACC002001', 'ACC999002', 'Insurance Company', 25000.00, 'Monthly', '2024-03-01', null, '2026-02-01', 'Active', 'Monthly Insurance Premium', 'bbbb1111-1111-1111-1111-111111111111'),
('33333333-3333-3333-3333-333333333334', 'SI003', 'ACC003001', 'ACC999003', 'Loan Account', 150000.00, 'Monthly', '2024-01-15', '2026-12-15', '2026-02-15', 'Active', 'Monthly Loan Repayment', 'cccc1111-1111-1111-1111-111111111111');

-- Create sample complaints
INSERT INTO "CustomerComplaints" ("Id", "ComplaintNumber", "Subject", "Description", "Category", "Priority", "Status", "CreatedAt", "ResolvedAt", "CreatedBy", "AssignedTo", "Resolution", "CustomerId") VALUES
('11111111-1111-1111-1111-111111111113', 'CMP20260120001', 'ATM Card Not Working', 'My ATM card is not working at any machine. It gets declined every time I try to withdraw money.', 'Card Issue', 'High', 'Open', '2026-01-20 10:30:00', null, 'jacobodenyire', 'cardteam', '', '11111111-1111-1111-1111-111111111111'),
('22222222-2222-2222-2222-222222222224', 'CMP20260121001', 'Incorrect Balance Display', 'The balance shown on my mobile app does not match my account statement.', 'Account Balance', 'Medium', 'In Progress', '2026-01-21 14:15:00', null, 'jacobodenyire', 'techsupport', '', '22222222-2222-2222-2222-222222222222'),
('33333333-3333-3333-3333-333333333335', 'CMP20260119001', 'Delayed Transfer', 'Money transfer to another bank has been delayed for 3 days now.', 'Transfer Issue', 'High', 'Resolved', '2026-01-19 09:00:00', '2026-01-22 16:30:00', 'jacobodenyire', 'operations', 'Transfer completed successfully. Delay was due to correspondent bank maintenance.', '33333333-3333-3333-3333-333333333333'),
('44444444-4444-4444-4444-444444444445', 'CMP20260122001', 'Statement Request', 'I need my account statement for the last 6 months for visa application.', 'Statement Request', 'Low', 'Resolved', '2026-01-22 11:45:00', '2026-01-22 15:20:00', 'jacobodenyire', 'customercare', 'Statement generated and sent to customer email.', '44444444-4444-4444-4444-444444444444'),
('55555555-5555-5555-5555-555555555556', 'CMP20260123001', 'Account Reactivation', 'My account has been dormant. I want to reactivate it.', 'Account Status', 'Medium', 'Open', '2026-01-23 08:30:00', null, 'jacobodenyire', 'branchmanager', '', '55555555-5555-5555-5555-555555555555');

-- Create sample complaint updates
INSERT INTO "ComplaintUpdates" ("Id", "UpdateText", "CreatedAt", "CreatedBy", "ComplaintId") VALUES
('11111111-1111-1111-1111-111111111114', 'Complaint received and assigned to card team for investigation.', '2026-01-20 10:35:00', 'jacobodenyire', '11111111-1111-1111-1111-111111111113'),
('22222222-2222-2222-2222-222222222225', 'Technical team is investigating the balance discrepancy issue.', '2026-01-21 15:00:00', 'techsupport', '22222222-2222-2222-2222-222222222224'),
('33333333-3333-3333-3333-333333333336', 'Transfer has been processed successfully. Customer notified.', '2026-01-22 16:30:00', 'operations', '33333333-3333-3333-3333-333333333335'),
('44444444-4444-4444-4444-444444444446', 'Statement generated and emailed to customer.', '2026-01-22 15:20:00', 'jacobodenyire', '44444444-4444-4444-4444-444444444445');

-- Create sample customer documents
INSERT INTO "CustomerDocuments" ("Id", "DocumentType", "FileName", "FilePath", "UploadedAt", "ExpiryDate", "Status", "UploadedBy", "CustomerId") VALUES
('cd111111-1111-1111-1111-111111111111', 'National ID', 'john_doe_id.pdf', '/documents/customers/john_doe_id.pdf', '2024-01-01 10:00:00', '2029-05-15', 'Approved', 'system', '11111111-1111-1111-1111-111111111111'),
('cd222222-2222-2222-2222-222222222222', 'Passport Photo', 'john_doe_photo.jpg', '/documents/customers/john_doe_photo.jpg', '2024-01-01 10:05:00', null, 'Approved', 'system', '11111111-1111-1111-1111-111111111111'),
('cd333333-3333-3333-3333-333333333333', 'National ID', 'jane_smith_id.pdf', '/documents/customers/jane_smith_id.pdf', '2024-01-02 11:00:00', '2029-08-22', 'Approved', 'system', '22222222-2222-2222-2222-222222222222'),
('cd444444-4444-4444-4444-444444444444', 'Salary Certificate', 'jane_smith_salary.pdf', '/documents/customers/jane_smith_salary.pdf', '2024-01-02 11:10:00', '2025-01-02', 'Pending', 'jacobodenyire', '22222222-2222-2222-2222-222222222222');

-- Create sample account status requests
INSERT INTO "AccountStatusRequests" ("Id", "RequestNumber", "AccountNumber", "RequestType", "Reason", "Status", "RequestedAt", "ProcessedAt", "RequestedBy", "ProcessedBy", "Comments") VALUES
('11111111-1111-1111-1111-111111111115', 'ASR20260120001', 'ACC005001', 'Activate', 'Customer wants to reactivate dormant account', 'Pending', '2026-01-20 09:00:00', null, 'jacobodenyire', '', 'Customer provided updated KYC documents'),
('22222222-2222-2222-2222-222222222226', 'ASR20260121001', 'ACC001002', 'Freeze', 'Suspected fraudulent activity', 'Approved', '2026-01-21 16:30:00', '2026-01-21 17:00:00', 'jacobodenyire', 'branchmanager1', 'Account frozen pending investigation'),
('33333333-3333-3333-3333-333333333337', 'ASR20260119001', 'ACC002001', 'Unfreeze', 'Investigation completed, no fraud detected', 'Approved', '2026-01-19 10:15:00', '2026-01-19 14:30:00', 'jacobodenyire', 'branchmanager1', 'Account unfrozen after investigation');

-- Create sample card requests
INSERT INTO "CardRequests" ("Id", "RequestNumber", "AccountNumber", "CardNumber", "RequestType", "Reason", "Status", "RequestedAt", "ProcessedAt", "RequestedBy", "ProcessedBy", "Comments") VALUES
('11111111-1111-1111-1111-111111111116', 'CR20260120001', 'ACC001001', '****1234', 'Block', 'Card lost by customer', 'Processed', '2026-01-20 11:00:00', '2026-01-20 11:15:00', 'jacobodenyire', 'cardteam', 'Card blocked successfully'),
('22222222-2222-2222-2222-222222222227', 'CR20260120002', 'ACC001001', '****1234', 'Replace', 'Replacement for blocked card', 'Pending', '2026-01-20 11:20:00', null, 'jacobodenyire', '', 'New card being prepared'),
('33333333-3333-3333-3333-333333333338', 'CR20260121001', 'ACC002001', '****5678', 'PIN Reset', 'Customer forgot PIN', 'Processed', '2026-01-21 13:45:00', '2026-01-21 14:00:00', 'jacobodenyire', 'cardteam', 'PIN reset completed'),
('44444444-4444-4444-4444-444444444447', 'CR20260122001', 'ACC003001', '****9012', 'Unblock', 'Temporary block due to suspicious activity', 'Pending', '2026-01-22 09:30:00', null, 'jacobodenyire', '', 'Awaiting customer verification');