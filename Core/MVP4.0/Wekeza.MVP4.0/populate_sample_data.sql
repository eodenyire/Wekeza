-- Insert sample authorizations
INSERT INTO "Authorizations" ("Type", "TellerId", "CustomerAccount", "CustomerName", "Amount", "Status", "RequestedAt", "TransactionId", "Description")
VALUES 
    ('High-Value Withdrawal', 'teller1', '1001234567', 'John Smith', 15000.00, 'Pending', NOW() - INTERVAL '30 minutes', 'TXN20260123001', 'Withdrawal exceeds teller limit'),
    ('Account Opening', 'teller2', '1001234568', 'Jane Doe', 5000.00, 'Pending', NOW() - INTERVAL '45 minutes', 'TXN20260123002', 'New account opening requires BM approval'),
    ('System Override', 'teller1', '1001234569', 'Mike Johnson', 2500.00, 'Pending', NOW() - INTERVAL '15 minutes', 'TXN20260123003', 'Insufficient balance override request'),
    ('Limit Breach', 'teller3', '1001234570', 'Sarah Wilson', 8000.00, 'Pending', NOW() - INTERVAL '1 hour', 'TXN20260123004', 'Daily transaction limit exceeded');

-- Insert sample risk alerts
INSERT INTO "RiskAlerts" ("AlertType", "AccountNumber", "Description", "RiskLevel", "Status", "CreatedAt")
VALUES 
    ('Large Cash Transaction', '1001234567', 'Cash deposit of $25,000 exceeds reporting threshold', 'Critical', 'Active', NOW() - INTERVAL '2 hours'),
    ('Multiple Withdrawals', '1001234568', 'Customer made 5 withdrawals in one day', 'Medium', 'Active', NOW() - INTERVAL '3 hours'),
    ('Suspicious Pattern', '1001234569', 'Unusual transaction pattern detected', 'High', 'Active', NOW() - INTERVAL '1 hour'),
    ('AML Flag', '1001234570', 'Transaction flagged for AML review', 'Critical', 'Active', NOW() - INTERVAL '4 hours');

-- Insert sample cash position
INSERT INTO "CashPositions" ("VaultCash", "TellerCash", "ATMCash", "UpdatedBy", "LastUpdated")
VALUES (125000, 45000, 35000, 'System', NOW());

-- Insert sample reports
INSERT INTO "BranchReports" ("Name", "Type", "GeneratedBy", "Status", "GeneratedAt", "FilePath", "FileSize")
VALUES 
    ('Daily Transaction Report - 2026-01-23', 'daily', 'Branch Manager', 'Ready', NOW() - INTERVAL '2 hours', 'Reports/daily_20260123_140000.csv', 2048),
    ('Weekly Performance Summary - Week 4', 'weekly', 'Branch Manager', 'Ready', NOW() - INTERVAL '1 day', 'Reports/weekly_20260122_180000.csv', 4096),
    ('Monthly Revenue Report - January 2026', 'monthly', 'Branch Manager', 'Ready', NOW() - INTERVAL '2 days', 'Reports/monthly_20260121_170000.csv', 8192);