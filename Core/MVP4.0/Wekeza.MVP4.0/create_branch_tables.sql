CREATE TABLE IF NOT EXISTS "Authorizations" (
    "Id" SERIAL PRIMARY KEY,
    "TransactionId" VARCHAR(50) NOT NULL,
    "Type" VARCHAR(50) NOT NULL,
    "TellerId" VARCHAR(100) NOT NULL,
    "CustomerAccount" VARCHAR(20) NOT NULL,
    "CustomerName" VARCHAR(200),
    "Amount" DECIMAL(18,2) NOT NULL,
    "Description" TEXT,
    "RequestedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Pending',
    "AuthorizedBy" VARCHAR(100),
    "AuthorizedAt" TIMESTAMP WITH TIME ZONE,
    "Reason" TEXT
);

CREATE TABLE IF NOT EXISTS "CashPositions" (
    "Id" SERIAL PRIMARY KEY,
    "VaultCash" DECIMAL(18,2) NOT NULL,
    "TellerCash" DECIMAL(18,2) NOT NULL,
    "ATMCash" DECIMAL(18,2) NOT NULL,
    "LastUpdated" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedBy" VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS "RiskAlerts" (
    "Id" SERIAL PRIMARY KEY,
    "AlertType" VARCHAR(50) NOT NULL,
    "AccountNumber" VARCHAR(20) NOT NULL,
    "CustomerName" VARCHAR(200),
    "Description" TEXT NOT NULL,
    "RiskLevel" VARCHAR(20) NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Open',
    "AssignedTo" VARCHAR(100),
    "Resolution" TEXT,
    "ResolvedAt" TIMESTAMP WITH TIME ZONE
);

CREATE TABLE IF NOT EXISTS "BranchReports" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Type" VARCHAR(50) NOT NULL,
    "FilePath" VARCHAR(500),
    "GeneratedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "GeneratedBy" VARCHAR(100) NOT NULL,
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Ready',
    "FileSize" BIGINT NOT NULL DEFAULT 0
);

-- Insert sample data
INSERT INTO "Authorizations" ("TransactionId", "Type", "TellerId", "CustomerAccount", "CustomerName", "Amount", "Description", "Status") VALUES
('TXN20260123001', 'CashWithdrawal', 'teller1', '1001234567', 'Sarah Wilson', 15000.00, 'High-value cash withdrawal', 'Pending'),
('TXN20260123002', 'AccountOpening', 'teller1', '1001234568', 'Mike Johnson', 5000.00, 'New savings account opening', 'Pending'),
('TXN20260123003', 'Override', 'teller1', '1001234569', 'David Brown', 2500.00, 'Insufficient balance override', 'Pending');

INSERT INTO "CashPositions" ("VaultCash", "TellerCash", "ATMCash", "UpdatedBy") VALUES
(125000.00, 45000.00, 35000.00, 'System');

INSERT INTO "RiskAlerts" ("AlertType", "AccountNumber", "CustomerName", "Description", "RiskLevel", "Status") VALUES
('AML', '1001234567', 'John Smith', 'Large cash transaction - $25,000 cash deposit', 'Critical', 'Open'),
('SuspiciousActivity', '1001234568', 'Jane Doe', 'Multiple withdrawals in short period - 5 withdrawals today', 'Medium', 'Open'),
('KYC', '1001234569', 'Mike Johnson', 'Customer ID document expired', 'Low', 'Open'),
('Dormant', '1001234570', 'Sarah Wilson', 'No activity for 6 months', 'Low', 'Open');

INSERT INTO "BranchReports" ("Name", "Type", "GeneratedBy", "Status") VALUES
('Daily Report - 2026-01-23', 'daily', 'branchmanager1', 'Ready'),
('Weekly Report - 2026-01-20', 'weekly', 'branchmanager1', 'Ready'),
('Monthly Report - 2026-01-01', 'monthly', 'branchmanager1', 'Ready');