-- Create Users table for authentication
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Username" VARCHAR(100) UNIQUE NOT NULL,
    "Email" VARCHAR(200) UNIQUE NOT NULL,
    "PasswordHash" VARCHAR(500) NOT NULL,
    "FullName" VARCHAR(200) NOT NULL,
    "Role" VARCHAR(50) NOT NULL,
    "IsActive" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "LastLoginAt" TIMESTAMP,
    "IsEmailConfirmed" BOOLEAN DEFAULT false,
    "PhoneNumber" VARCHAR(20),
    "Department" VARCHAR(100),
    "Branch" VARCHAR(100)
);

-- Create index for faster lookups
CREATE INDEX IF NOT EXISTS idx_users_username ON "Users"("Username");
CREATE INDEX IF NOT EXISTS idx_users_email ON "Users"("Email");
CREATE INDEX IF NOT EXISTS idx_users_role ON "Users"("Role");

-- Insert default admin user
-- Password: Admin@123 (you should change this immediately)
-- Password hash for Admin@123 using BCrypt
INSERT INTO "Users" ("Id", "Username", "Email", "PasswordHash", "FullName", "Role", "IsActive", "IsEmailConfirmed", "Department")
VALUES 
(uuid_generate_v4(), 'admin', 'admin@wekeza.com', '$2a$11$vEj9qRXHQZVp7TbS.xvCxO9DjGLzJ6Y3iQqEqZVc4BGKmNL4VJ5Zm', 'System Administrator', 'Administrator', true, true, 'IT Operations'),
(uuid_generate_v4(), 'teller1', 'teller1@wekeza.com', '$2a$11$vEj9qRXHQZVp7TbS.xvCxO9DjGLzJ6Y3iQqEqZVc4BGKmNL4VJ5Zm', 'John Doe', 'Teller', true, true, 'Teller Operations'),
(uuid_generate_v4(), 'manager1', 'manager1@wekeza.com', '$2a$11$vEj9qRXHQZVp7TbS.xvCxO9DjGLzJ6Y3iQqEqZVc4BGKmNL4VJ5Zm', 'Jane Smith', 'Manager', true, true, 'Branch Management')
ON CONFLICT ("Username") DO NOTHING;

-- Create Customers table
CREATE TABLE IF NOT EXISTS "Customers" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "FirstName" VARCHAR(100) NOT NULL,
    "LastName" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(200) UNIQUE NOT NULL,
    "PhoneNumber" VARCHAR(20) NOT NULL,
    "IdentificationNumber" VARCHAR(50) UNIQUE NOT NULL,
    "RiskRating" INTEGER DEFAULT 0,
    "IsActive" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_customers_email ON "Customers"("Email");
CREATE INDEX IF NOT EXISTS idx_customers_phone ON "Customers"("PhoneNumber");

-- Insert sample customers
INSERT INTO "Customers" ("Id", "FirstName", "LastName", "Email", "PhoneNumber", "IdentificationNumber", "RiskRating", "IsActive")
VALUES 
(uuid_generate_v4(), 'Alice', 'Johnson', 'alice.johnson@example.com', '+254712345678', 'ID12345678', 1, true),
(uuid_generate_v4(), 'Bob', 'Williams', 'bob.williams@example.com', '+254723456789', 'ID23456789', 0, true),
(uuid_generate_v4(), 'Carol', 'Brown', 'carol.brown@example.com', '+254734567890', 'ID34567890', 1, true)
ON CONFLICT ("Email") DO NOTHING;

-- Create Accounts table
CREATE TABLE IF NOT EXISTS "Accounts" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "AccountNumber" VARCHAR(50) UNIQUE NOT NULL,
    "CustomerId" UUID REFERENCES "Customers"("Id"),
    "AccountType" VARCHAR(50) NOT NULL,
    "Currency" VARCHAR(3) DEFAULT 'KES',
    "Balance" DECIMAL(18, 2) DEFAULT 0.00,
    "AvailableBalance" DECIMAL(18, 2) DEFAULT 0.00,
    "Status" VARCHAR(20) DEFAULT 'Active',
    "OpenedDate" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_accounts_customer ON "Accounts"("CustomerId");
CREATE INDEX IF NOT EXISTS idx_accounts_number ON "Accounts"("AccountNumber");

-- Insert sample accounts
INSERT INTO "Accounts" ("Id", "AccountNumber", "CustomerId", "AccountType", "Balance", "AvailableBalance", "Status")
SELECT 
    uuid_generate_v4(),
    'ACC' || LPAD(FLOOR(RANDOM() * 10000000)::TEXT, 10, '0'),
    c."Id",
    'Savings',
    FLOOR(RANDOM() * 100000 + 10000)::DECIMAL(18,2),
    FLOOR(RANDOM() * 100000 + 10000)::DECIMAL(18,2),
    'Active'
FROM "Customers" c
ON CONFLICT ("AccountNumber") DO NOTHING;

-- Create Transactions table
CREATE TABLE IF NOT EXISTS "Transactions" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "TransactionReference" VARCHAR(100) UNIQUE NOT NULL,
    "AccountId" UUID REFERENCES "Accounts"("Id"),
    "TransactionType" VARCHAR(50) NOT NULL,
    "Amount" DECIMAL(18, 2) NOT NULL,
    "Currency" VARCHAR(3) DEFAULT 'KES',
    "Status" VARCHAR(20) DEFAULT 'Pending',
    "Description" VARCHAR(500),
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "ProcessedAt" TIMESTAMP,
    "BalanceAfter" DECIMAL(18, 2)
);

CREATE INDEX IF NOT EXISTS idx_transactions_account ON "Transactions"("AccountId");
CREATE INDEX IF NOT EXISTS idx_transactions_reference ON "Transactions"("TransactionReference");
CREATE INDEX IF NOT EXISTS idx_transactions_date ON "Transactions"("CreatedAt");

-- Log initialization
INSERT INTO audit.audit_log (table_name, operation, new_data, changed_by)
VALUES ('system', 'SEED', '{"message": "Basic tables and sample data created"}', 'system');
