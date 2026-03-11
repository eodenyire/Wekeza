-- =============================================================================
-- Wekeza Bank — Complete Database Schema (DDL)
-- Generated: 2026-03-11
-- Database:  PostgreSQL 15+
-- Schema:    public (operational), audit (change-tracking)
--
-- USAGE:
--   This file creates all tables from scratch.
--   It is safe to run on an empty database.
--   For a running system use the init scripts instead:
--     1. scripts/init-db.sql                    (extensions + audit schema)
--     2. scripts/seed-banking-data.sql           (Users, Customers, Accounts, Transactions)
--     3. scripts/seed-portal-users.sql           (12 portal users with BCrypt hashes)
--     4. scripts/seed-comprehensive-data.sql     (full demo dataset — 1000 rows)
-- =============================================================================

-- Extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Schemas
CREATE SCHEMA IF NOT EXISTS audit;
CREATE SCHEMA IF NOT EXISTS reporting;

GRANT ALL ON SCHEMA public    TO wekeza_app;
GRANT ALL ON SCHEMA audit     TO wekeza_app;
GRANT ALL ON SCHEMA reporting TO wekeza_app;

-- =============================================================================
-- AUDIT
-- =============================================================================

CREATE TABLE IF NOT EXISTS audit.audit_log (
    id          UUID        PRIMARY KEY DEFAULT uuid_generate_v4(),
    table_name  VARCHAR(100) NOT NULL,
    operation   VARCHAR(10)  NOT NULL,   -- INSERT | UPDATE | DELETE | INIT
    old_data    JSONB,
    new_data    JSONB,
    changed_by  VARCHAR(100),
    changed_at  TIMESTAMP    DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_audit_log_table_name ON audit.audit_log(table_name);
CREATE INDEX IF NOT EXISTS idx_audit_log_changed_at  ON audit.audit_log(changed_at);

-- =============================================================================
-- USERS  (staff + portal accounts)
-- =============================================================================

CREATE TABLE IF NOT EXISTS "Users" (
    "Id"               UUID         PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Username"         VARCHAR(100) UNIQUE NOT NULL,
    "Email"            VARCHAR(200) UNIQUE NOT NULL,
    "PasswordHash"     VARCHAR(500) NOT NULL,
    "FullName"         VARCHAR(200) NOT NULL,
    "Role"             VARCHAR(50)  NOT NULL,
    "IsActive"         BOOLEAN      DEFAULT true,
    "CreatedAt"        TIMESTAMP    DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"        TIMESTAMP    DEFAULT CURRENT_TIMESTAMP,
    "LastLoginAt"      TIMESTAMP,
    "IsEmailConfirmed" BOOLEAN      DEFAULT false,
    "PhoneNumber"      VARCHAR(20),
    "Department"       VARCHAR(100),
    "Branch"           VARCHAR(100)
);

CREATE INDEX IF NOT EXISTS idx_users_username ON "Users"("Username");
CREATE INDEX IF NOT EXISTS idx_users_email    ON "Users"("Email");
CREATE INDEX IF NOT EXISTS idx_users_role     ON "Users"("Role");

-- =============================================================================
-- CUSTOMERS
-- =============================================================================

CREATE TABLE IF NOT EXISTS "Customers" (
    "Id"                   UUID         PRIMARY KEY DEFAULT uuid_generate_v4(),
    "FirstName"            VARCHAR(100) NOT NULL,
    "LastName"             VARCHAR(100) NOT NULL,
    "Email"                VARCHAR(200) UNIQUE NOT NULL,
    "PhoneNumber"          VARCHAR(20)  NOT NULL,
    "IdentificationNumber" VARCHAR(50)  UNIQUE NOT NULL,
    "RiskRating"           INTEGER      DEFAULT 0,
    "IsActive"             BOOLEAN      DEFAULT true,
    "UserId"               UUID,        -- link to portal user account
    "CreatedAt"            TIMESTAMP    DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"            TIMESTAMP    DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_customers_email  ON "Customers"("Email");
CREATE INDEX IF NOT EXISTS idx_customers_phone  ON "Customers"("PhoneNumber");
CREATE INDEX IF NOT EXISTS idx_customers_idno   ON "Customers"("IdentificationNumber");
CREATE INDEX IF NOT EXISTS idx_customers_userid ON "Customers"("UserId");

-- =============================================================================
-- BRANCHES
-- =============================================================================

CREATE TABLE IF NOT EXISTS "Branches" (
    "Id"                         UUID          PRIMARY KEY DEFAULT uuid_generate_v4(),
    "BranchCode"                 VARCHAR(20)   NOT NULL UNIQUE,
    "BranchName"                 VARCHAR(200)  NOT NULL,
    "Address"                    VARCHAR(500)  NOT NULL,
    "City"                       VARCHAR(100)  NOT NULL,
    "Country"                    VARCHAR(100)  NOT NULL DEFAULT 'Kenya',
    "PhoneNumber"                VARCHAR(20)   NOT NULL,
    "Email"                      VARCHAR(100)  NOT NULL,
    "BranchType"                 INTEGER       NOT NULL DEFAULT 0,   -- 0=Full, 1=Mini, 2=Agency
    "Status"                     INTEGER       NOT NULL DEFAULT 0,   -- 0=Active, 1=Closed
    "OpeningDate"                TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
    "ClosingDate"                TIMESTAMPTZ,
    "TimeZone"                   VARCHAR(50)   NOT NULL DEFAULT 'Africa/Nairobi',
    "BusinessHoursStart"         INTERVAL      NOT NULL DEFAULT INTERVAL '08:00:00',
    "BusinessHoursEnd"           INTERVAL      NOT NULL DEFAULT INTERVAL '17:00:00',
    "ManagerId"                  VARCHAR(100)  NOT NULL,
    "DeputyManagerId"            VARCHAR(100),
    "CashLimit"                  NUMERIC(18,2) NOT NULL DEFAULT 50000000.00,
    "CashLimitCurrency"          VARCHAR(3)    NOT NULL DEFAULT 'KES',
    "TransactionLimit"           NUMERIC(18,2) NOT NULL DEFAULT 10000000.00,
    "TransactionLimitCurrency"   VARCHAR(3)    NOT NULL DEFAULT 'KES',
    "IsEODCompleted"             BOOLEAN       NOT NULL DEFAULT false,
    "LastEODDate"                TIMESTAMPTZ,
    "IsBODCompleted"             BOOLEAN       NOT NULL DEFAULT false,
    "LastBODDate"                TIMESTAMPTZ,
    "CreatedBy"                  VARCHAR(100)  NOT NULL DEFAULT 'system',
    "CreatedAt"                  TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
    "ModifiedBy"                 VARCHAR(100),
    "ModifiedAt"                 TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_branches_code   ON "Branches"("BranchCode");
CREATE INDEX IF NOT EXISTS idx_branches_status ON "Branches"("Status");
CREATE INDEX IF NOT EXISTS idx_branches_city   ON "Branches"("City");

-- =============================================================================
-- ACCOUNTS
-- =============================================================================

CREATE TABLE IF NOT EXISTS "Accounts" (
    "Id"               UUID          PRIMARY KEY DEFAULT uuid_generate_v4(),
    "AccountNumber"    VARCHAR(20)   UNIQUE NOT NULL,
    "CustomerId"       UUID          NOT NULL REFERENCES "Customers"("Id") ON DELETE RESTRICT,
    "AccountType"      VARCHAR(50)   NOT NULL DEFAULT 'Savings',
    "Currency"         VARCHAR(3)    NOT NULL DEFAULT 'KES',
    "Balance"          NUMERIC(18,2) NOT NULL DEFAULT 0.00,
    "AvailableBalance" NUMERIC(18,2) NOT NULL DEFAULT 0.00,
    "Status"           VARCHAR(20)   NOT NULL DEFAULT 'Active',
    "OpenedDate"       TIMESTAMPTZ   DEFAULT NOW(),
    "ClosedDate"       TIMESTAMPTZ,
    "CreatedAt"        TIMESTAMPTZ   DEFAULT NOW(),
    "UpdatedAt"        TIMESTAMPTZ   DEFAULT NOW(),
    "BranchCode"       VARCHAR(20)
);

CREATE INDEX IF NOT EXISTS idx_accounts_customer   ON "Accounts"("CustomerId");
CREATE INDEX IF NOT EXISTS idx_accounts_number     ON "Accounts"("AccountNumber");
CREATE INDEX IF NOT EXISTS idx_accounts_status     ON "Accounts"("Status");
CREATE INDEX IF NOT EXISTS idx_accounts_branchcode ON "Accounts"("BranchCode");

-- =============================================================================
-- BALANCES  (snapshot table — 1 row per account)
-- =============================================================================

CREATE TABLE IF NOT EXISTS "Balances" (
    "Id"               UUID          PRIMARY KEY DEFAULT uuid_generate_v4(),
    "AccountId"        UUID          NOT NULL UNIQUE REFERENCES "Accounts"("Id") ON DELETE CASCADE,
    "CurrentBalance"   NUMERIC(18,2) NOT NULL DEFAULT 0.00,
    "AvailableBalance" NUMERIC(18,2) NOT NULL DEFAULT 0.00,
    "LedgerBalance"    NUMERIC(18,2) NOT NULL DEFAULT 0.00,
    "Currency"         VARCHAR(3)    NOT NULL DEFAULT 'KES',
    "AsOfDate"         TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
    "CreatedAt"        TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
    "UpdatedAt"        TIMESTAMPTZ   NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_balances_account ON "Balances"("AccountId");
CREATE INDEX IF NOT EXISTS idx_balances_asofdate ON "Balances"("AsOfDate");

-- =============================================================================
-- TRANSACTIONS
-- =============================================================================

CREATE TABLE IF NOT EXISTS "Transactions" (
    "Id"                   UUID          PRIMARY KEY DEFAULT uuid_generate_v4(),
    "TransactionReference" VARCHAR(100)  UNIQUE NOT NULL,
    "AccountId"            UUID          REFERENCES "Accounts"("Id") ON DELETE RESTRICT,
    "TransactionType"      VARCHAR(50)   NOT NULL,   -- Deposit | Withdrawal | Transfer | Payment
    "Amount"               NUMERIC(18,2) NOT NULL,
    "Currency"             VARCHAR(3)    NOT NULL DEFAULT 'KES',
    "Status"               VARCHAR(20)   DEFAULT 'Pending',  -- Pending | Completed | Failed
    "Description"          VARCHAR(500),
    "CreatedAt"            TIMESTAMP     DEFAULT CURRENT_TIMESTAMP,
    "ProcessedAt"          TIMESTAMP,
    "BalanceAfter"         NUMERIC(18,2)
);

CREATE INDEX IF NOT EXISTS idx_transactions_account   ON "Transactions"("AccountId");
CREATE INDEX IF NOT EXISTS idx_transactions_reference ON "Transactions"("TransactionReference");
CREATE INDEX IF NOT EXISTS idx_transactions_date      ON "Transactions"("CreatedAt");
CREATE INDEX IF NOT EXISTS idx_transactions_status    ON "Transactions"("Status");
CREATE INDEX IF NOT EXISTS idx_transactions_type      ON "Transactions"("TransactionType");

-- =============================================================================
-- PORTAL / SESSION (used by teller, supervisor portals)
-- =============================================================================

CREATE TABLE IF NOT EXISTS "TellerSessions" (
    "Id"            UUID         PRIMARY KEY DEFAULT uuid_generate_v4(),
    "UserId"        UUID         NOT NULL REFERENCES "Users"("Id"),
    "BranchCode"    VARCHAR(20),
    "OpenedAt"      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ClosedAt"      TIMESTAMP,
    "DrawerBalance" NUMERIC(18,2) NOT NULL DEFAULT 0.00,
    "Status"        VARCHAR(20)   NOT NULL DEFAULT 'Open',  -- Open | Closed
    "CreatedAt"     TIMESTAMP     NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_teller_sessions_user   ON "TellerSessions"("UserId");
CREATE INDEX IF NOT EXISTS idx_teller_sessions_branch ON "TellerSessions"("BranchCode");
CREATE INDEX IF NOT EXISTS idx_teller_sessions_status ON "TellerSessions"("Status");

-- =============================================================================
-- GL / CHART OF ACCOUNTS (Product & GL portal)
-- =============================================================================

CREATE TABLE IF NOT EXISTS "GLAccounts" (
    "Id"          UUID         PRIMARY KEY DEFAULT uuid_generate_v4(),
    "AccountCode" VARCHAR(20)  UNIQUE NOT NULL,
    "AccountName" VARCHAR(200) NOT NULL,
    "AccountType" VARCHAR(50)  NOT NULL,   -- Asset | Liability | Equity | Income | Expense
    "ParentCode"  VARCHAR(20),
    "Currency"    VARCHAR(3)   NOT NULL DEFAULT 'KES',
    "Balance"     NUMERIC(18,2) NOT NULL DEFAULT 0.00,
    "IsActive"    BOOLEAN      NOT NULL DEFAULT true,
    "CreatedAt"   TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"   TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_gl_accounts_code   ON "GLAccounts"("AccountCode");
CREATE INDEX IF NOT EXISTS idx_gl_accounts_type   ON "GLAccounts"("AccountType");
CREATE INDEX IF NOT EXISTS idx_gl_accounts_parent ON "GLAccounts"("ParentCode");

-- =============================================================================
-- LOANS
-- =============================================================================

CREATE TABLE IF NOT EXISTS "Loans" (
    "Id"              UUID          PRIMARY KEY DEFAULT uuid_generate_v4(),
    "LoanNumber"      VARCHAR(30)   UNIQUE NOT NULL,
    "CustomerId"      UUID          NOT NULL REFERENCES "Customers"("Id"),
    "PrincipalAmount" NUMERIC(18,2) NOT NULL,
    "OutstandingBalance" NUMERIC(18,2) NOT NULL,
    "InterestRate"    NUMERIC(5,2)  NOT NULL,
    "LoanType"        VARCHAR(50)   NOT NULL,  -- Personal | Business | Mortgage | Vehicle
    "Status"          VARCHAR(20)   NOT NULL DEFAULT 'Active',
    "DisbursedDate"   TIMESTAMP,
    "MaturityDate"    TIMESTAMP,
    "CreatedAt"       TIMESTAMP     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"       TIMESTAMP     NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_loans_customer ON "Loans"("CustomerId");
CREATE INDEX IF NOT EXISTS idx_loans_status   ON "Loans"("Status");
CREATE INDEX IF NOT EXISTS idx_loans_number   ON "Loans"("LoanNumber");

-- =============================================================================
-- CARDS
-- =============================================================================

CREATE TABLE IF NOT EXISTS "Cards" (
    "Id"           UUID        PRIMARY KEY DEFAULT uuid_generate_v4(),
    "CardNumber"   VARCHAR(20) UNIQUE NOT NULL,
    "AccountId"    UUID        NOT NULL REFERENCES "Accounts"("Id"),
    "CardType"     VARCHAR(20) NOT NULL,  -- Debit | Credit | Virtual
    "Status"       VARCHAR(20) NOT NULL DEFAULT 'Active',
    "ExpiryDate"   TIMESTAMP   NOT NULL,
    "IsVirtual"    BOOLEAN     NOT NULL DEFAULT false,
    "CreatedAt"    TIMESTAMP   NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"    TIMESTAMP   NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_cards_account ON "Cards"("AccountId");
CREATE INDEX IF NOT EXISTS idx_cards_status  ON "Cards"("Status");

-- =============================================================================
-- APPROVAL WORKFLOW (Supervisor / Workflow portals)
-- =============================================================================

CREATE TABLE IF NOT EXISTS "ApprovalRequests" (
    "Id"           UUID         PRIMARY KEY DEFAULT uuid_generate_v4(),
    "RequestType"  VARCHAR(100) NOT NULL,
    "RequestedBy"  VARCHAR(100) NOT NULL,
    "ApprovedBy"   VARCHAR(100),
    "Status"       VARCHAR(20)  NOT NULL DEFAULT 'Pending',  -- Pending | Approved | Rejected
    "RequestData"  JSONB,
    "Notes"        TEXT,
    "RequestedAt"  TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ResolvedAt"   TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_approval_status      ON "ApprovalRequests"("Status");
CREATE INDEX IF NOT EXISTS idx_approval_requestedby ON "ApprovalRequests"("RequestedBy");

-- =============================================================================
-- SYSTEM CONFIG
-- =============================================================================

CREATE TABLE IF NOT EXISTS "SystemConfigurations" (
    "Id"          UUID         PRIMARY KEY DEFAULT uuid_generate_v4(),
    "ConfigKey"   VARCHAR(200) UNIQUE NOT NULL,
    "ConfigValue" TEXT         NOT NULL,
    "Category"    VARCHAR(100),
    "Description" VARCHAR(500),
    "IsEncrypted" BOOLEAN      NOT NULL DEFAULT false,
    "CreatedAt"   TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"   TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- =============================================================================
-- Log the DDL run
-- =============================================================================

INSERT INTO audit.audit_log (table_name, operation, new_data, changed_by)
VALUES (
    'system', 'INIT',
    '{"message": "Full schema DDL applied — all tables created"}',
    'full-schema-ddl.sql'
);
