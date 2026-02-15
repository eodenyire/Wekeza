-- Public Sector Portal Database Schema
-- Essential tables for end-to-end integration

-- Users table for authentication
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" UUID PRIMARY KEY,
    "Username" VARCHAR(100) NOT NULL UNIQUE,
    "Email" VARCHAR(255) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(500) NOT NULL,
    "FirstName" VARCHAR(100),
    "LastName" VARCHAR(100),
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Active',
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Customers table
CREATE TABLE IF NOT EXISTS "Customers" (
    "Id" UUID PRIMARY KEY,
    "CustomerNumber" VARCHAR(50) NOT NULL UNIQUE,
    "CustomerType" VARCHAR(20) NOT NULL,
    "Name" VARCHAR(200) NOT NULL,
    "Email" VARCHAR(255),
    "PhoneNumber" VARCHAR(20),
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Active',
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Accounts table
CREATE TABLE IF NOT EXISTS "Accounts" (
    "Id" UUID PRIMARY KEY,
    "AccountNumber" VARCHAR(20) NOT NULL UNIQUE,
    "CustomerId" UUID NOT NULL REFERENCES "Customers"("Id"),
    "AccountType" VARCHAR(50) NOT NULL,
    "BalanceAmount" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "CurrencyCode" VARCHAR(3) NOT NULL DEFAULT 'KES',
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Active',
    "OpenedDate" DATE NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Securities table (T-Bills, Bonds, Stocks)
CREATE TABLE IF NOT EXISTS "Securities" (
    "Id" UUID PRIMARY KEY,
    "SecurityCode" VARCHAR(50) NOT NULL UNIQUE,
    "SecurityType" VARCHAR(20) NOT NULL,
    "Name" VARCHAR(200) NOT NULL,
    "IssueDate" DATE NOT NULL,
    "MaturityDate" DATE,
    "CouponRate" DECIMAL(5,2),
    "FaceValue" DECIMAL(18,2) NOT NULL,
    "CurrentPrice" DECIMAL(18,2) NOT NULL,
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Active',
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Security Orders table
CREATE TABLE IF NOT EXISTS "SecurityOrders" (
    "Id" UUID PRIMARY KEY,
    "OrderNumber" VARCHAR(50) NOT NULL UNIQUE,
    "CustomerId" UUID NOT NULL REFERENCES "Customers"("Id"),
    "SecurityId" UUID NOT NULL REFERENCES "Securities"("Id"),
    "OrderType" VARCHAR(10) NOT NULL,
    "Quantity" INT NOT NULL,
    "Price" DECIMAL(18,2) NOT NULL,
    "TotalAmount" DECIMAL(18,2) NOT NULL,
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Pending',
    "OrderDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ExecutionDate" TIMESTAMP
);

-- Loans table
CREATE TABLE IF NOT EXISTS "Loans" (
    "Id" UUID PRIMARY KEY,
    "LoanNumber" VARCHAR(50) NOT NULL UNIQUE,
    "CustomerId" UUID NOT NULL REFERENCES "Customers"("Id"),
    "LoanType" VARCHAR(50) NOT NULL,
    "PrincipalAmount" DECIMAL(18,2) NOT NULL,
    "InterestRate" DECIMAL(5,2) NOT NULL,
    "TenorMonths" INT NOT NULL,
    "OutstandingBalance" DECIMAL(18,2) NOT NULL,
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Pending',
    "ApplicationDate" DATE NOT NULL,
    "ApprovalDate" DATE,
    "DisbursementDate" DATE,
    "MaturityDate" DATE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Transactions table
CREATE TABLE IF NOT EXISTS "Transactions" (
    "Id" UUID PRIMARY KEY,
    "TransactionReference" VARCHAR(100) NOT NULL UNIQUE,
    "AccountId" UUID NOT NULL REFERENCES "Accounts"("Id"),
    "TransactionType" VARCHAR(50) NOT NULL,
    "Amount" DECIMAL(18,2) NOT NULL,
    "CurrencyCode" VARCHAR(3) NOT NULL DEFAULT 'KES',
    "BalanceAfter" DECIMAL(18,2),
    "Description" VARCHAR(500),
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Posted',
    "ValueDate" DATE NOT NULL,
    "PostedDate" TIMESTAMP,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Grants table (for public sector)
CREATE TABLE IF NOT EXISTS "Grants" (
    "Id" UUID PRIMARY KEY,
    "GrantNumber" VARCHAR(50) NOT NULL UNIQUE,
    "CustomerId" UUID NOT NULL REFERENCES "Customers"("Id"),
    "GrantType" VARCHAR(50) NOT NULL,
    "Amount" DECIMAL(18,2) NOT NULL,
    "Purpose" VARCHAR(500),
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Pending',
    "ApplicationDate" DATE NOT NULL,
    "ApprovalDate" DATE,
    "DisbursementDate" DATE,
    "ComplianceRate" DECIMAL(5,2),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS "IX_Accounts_CustomerId" ON "Accounts"("CustomerId");
CREATE INDEX IF NOT EXISTS "IX_Accounts_AccountNumber" ON "Accounts"("AccountNumber");
CREATE INDEX IF NOT EXISTS "IX_SecurityOrders_CustomerId" ON "SecurityOrders"("CustomerId");
CREATE INDEX IF NOT EXISTS "IX_SecurityOrders_SecurityId" ON "SecurityOrders"("SecurityId");
CREATE INDEX IF NOT EXISTS "IX_Loans_CustomerId" ON "Loans"("CustomerId");
CREATE INDEX IF NOT EXISTS "IX_Loans_Status" ON "Loans"("Status");
CREATE INDEX IF NOT EXISTS "IX_Transactions_AccountId" ON "Transactions"("AccountId");
CREATE INDEX IF NOT EXISTS "IX_Transactions_ValueDate" ON "Transactions"("ValueDate");
CREATE INDEX IF NOT EXISTS "IX_Grants_CustomerId" ON "Grants"("CustomerId");
CREATE INDEX IF NOT EXISTS "IX_Grants_Status" ON "Grants"("Status");
