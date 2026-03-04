-- =============================================================
-- Wekeza v1-Core Heavy Database Bootstrap + 1000-row Seed
-- Purpose:
--   1) Ensure core operational tables exist
--   2) Seed at least 1000 records for key starter entities:
--      - Branches
--      - Staff (Users)
--      - Customers
--      - Accounts
--      - Balances
--      - Transactions
--   3) Print table inventory + core counts
--
-- Safe to re-run: script is idempotent and tops up to 1000 records.
-- =============================================================

BEGIN;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- -------------------------------------------------------------
-- Core starter tables (created if missing)
-- -------------------------------------------------------------

CREATE TABLE IF NOT EXISTS "Branches" (
    "Id" uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    "BranchCode" varchar(20) NOT NULL UNIQUE,
    "BranchName" varchar(200) NOT NULL,
    "Address" varchar(500) NOT NULL,
    "City" varchar(100) NOT NULL,
    "Country" varchar(100) NOT NULL,
    "PhoneNumber" varchar(20) NOT NULL,
    "Email" varchar(100) NOT NULL,
    "BranchType" integer NOT NULL DEFAULT 0,
    "Status" integer NOT NULL DEFAULT 0,
    "OpeningDate" timestamptz NOT NULL DEFAULT NOW(),
    "ClosingDate" timestamptz NULL,
    "TimeZone" varchar(50) NOT NULL DEFAULT 'Africa/Nairobi',
    "BusinessHoursStart" interval NOT NULL DEFAULT INTERVAL '08:00:00',
    "BusinessHoursEnd" interval NOT NULL DEFAULT INTERVAL '17:00:00',
    "ManagerId" varchar(100) NOT NULL,
    "DeputyManagerId" varchar(100) NULL,
    "CashLimit" numeric(18,2) NOT NULL DEFAULT 50000000.00,
    "CashLimitCurrency" varchar(3) NOT NULL DEFAULT 'KES',
    "TransactionLimit" numeric(18,2) NOT NULL DEFAULT 10000000.00,
    "TransactionLimitCurrency" varchar(3) NOT NULL DEFAULT 'KES',
    "IsEODCompleted" boolean NOT NULL DEFAULT false,
    "LastEODDate" timestamptz NULL,
    "IsBODCompleted" boolean NOT NULL DEFAULT false,
    "LastBODDate" timestamptz NULL,
    "CreatedBy" varchar(100) NOT NULL DEFAULT 'system',
    "CreatedAt" timestamptz NOT NULL DEFAULT NOW(),
    "ModifiedBy" varchar(100) NULL,
    "ModifiedAt" timestamptz NULL
);

CREATE TABLE IF NOT EXISTS "Balances" (
    "Id" uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    "AccountId" uuid NOT NULL UNIQUE,
    "CurrentBalance" numeric(18,2) NOT NULL DEFAULT 0.00,
    "AvailableBalance" numeric(18,2) NOT NULL DEFAULT 0.00,
    "LedgerBalance" numeric(18,2) NOT NULL DEFAULT 0.00,
    "Currency" varchar(3) NOT NULL DEFAULT 'KES',
    "AsOfDate" timestamptz NOT NULL DEFAULT NOW(),
    "CreatedAt" timestamptz NOT NULL DEFAULT NOW(),
    "UpdatedAt" timestamptz NOT NULL DEFAULT NOW(),
    CONSTRAINT "FK_Balances_Accounts_AccountId"
      FOREIGN KEY ("AccountId") REFERENCES "Accounts"("Id") ON DELETE CASCADE
);

-- Helpful relationship column for branch-level analytics (non-breaking)
ALTER TABLE "Accounts" ADD COLUMN IF NOT EXISTS "BranchCode" varchar(20);

CREATE INDEX IF NOT EXISTS idx_branches_code ON "Branches"("BranchCode");
CREATE INDEX IF NOT EXISTS idx_branches_status ON "Branches"("Status");
CREATE INDEX IF NOT EXISTS idx_balances_account ON "Balances"("AccountId");
CREATE INDEX IF NOT EXISTS idx_accounts_branchcode ON "Accounts"("BranchCode");

-- -------------------------------------------------------------
-- Ensure baseline test users used by smoke tests
-- -------------------------------------------------------------
-- Hashes correspond to working environment credentials:
-- admin / Admin@123
-- manager1 / Manager@123
-- teller1 / Teller@123

INSERT INTO "Users" (
    "Id", "Username", "Email", "PasswordHash", "FullName", "Role", "IsActive",
    "Department", "Branch", "CreatedAt", "UpdatedAt"
)
VALUES
(
    uuid_generate_v4(), 'admin', 'admin@wekeza.com',
    '$2a$06$6xibsCYusSBfCYv1eriL5OE/bPLH8ue5gk2otpxOjNFAVgy42/SwC',
    'System Admin', 'Administrator', true,
    'IT Administration', 'HQ', NOW(), NOW()
),
(
    uuid_generate_v4(), 'manager1', 'manager1@wekeza.com',
    '$2a$06$lHNDaP0w3QcgdQK31p/jtODJBOpjcZ2sQ85o1zrCCiCzElUTR2Op6',
    'Branch Manager One', 'Manager', true,
    'Branch Operations', 'BR100001', NOW(), NOW()
),
(
    uuid_generate_v4(), 'teller1', 'teller1@wekeza.com',
    '$2a$06$e7iQaE/Wj.x6N.mIR5uMTuyBEVQrDPzdMLnKRwAQozDxvC4kWb7V.',
    'Teller One', 'Teller', true,
    'Retail Banking', 'BR100001', NOW(), NOW()
)
ON CONFLICT ("Username") DO UPDATE SET
    "Email" = EXCLUDED."Email",
    "Role" = EXCLUDED."Role",
    "IsActive" = true,
    "UpdatedAt" = NOW();

-- -------------------------------------------------------------
-- Top up Branches to 1000 rows
-- -------------------------------------------------------------
DO $$
DECLARE
    v_existing integer;
    v_missing integer;
BEGIN
    SELECT COUNT(*) INTO v_existing FROM "Branches";
    v_missing := GREATEST(0, 1000 - v_existing);

    IF v_missing > 0 THEN
        INSERT INTO "Branches" (
            "Id", "BranchCode", "BranchName", "Address", "City", "Country", "PhoneNumber", "Email",
            "BranchType", "Status", "OpeningDate", "TimeZone", "BusinessHoursStart", "BusinessHoursEnd",
            "ManagerId", "DeputyManagerId", "CashLimit", "CashLimitCurrency",
            "TransactionLimit", "TransactionLimitCurrency", "IsEODCompleted", "IsBODCompleted",
            "CreatedBy", "CreatedAt"
        )
        SELECT
            uuid_generate_v4(),
            'BR' || LPAD((100000 + v_existing + s.i)::text, 6, '0'),
            'Branch ' || (v_existing + s.i)::text,
            'Street ' || (v_existing + s.i)::text || ', Nairobi',
            CASE WHEN (s.i % 4) = 0 THEN 'Nairobi'
                 WHEN (s.i % 4) = 1 THEN 'Mombasa'
                 WHEN (s.i % 4) = 2 THEN 'Kisumu'
                 ELSE 'Nakuru' END,
            'Kenya',
            '+254700' || LPAD((v_existing + s.i)::text, 6, '0'),
            'branch' || (v_existing + s.i)::text || '@wekeza.com',
            0,
            0,
            NOW() - ((s.i % 3650) || ' days')::interval,
            'Africa/Nairobi',
            INTERVAL '08:00:00',
            INTERVAL '17:00:00',
            'MGR' || LPAD((v_existing + s.i)::text, 6, '0'),
            'DMG' || LPAD((v_existing + s.i)::text, 6, '0'),
            50000000 + ((s.i % 100) * 100000),
            'KES',
            10000000 + ((s.i % 50) * 50000),
            'KES',
            false,
            false,
            'seed-script',
            NOW()
        FROM generate_series(1, v_missing) AS s(i)
        ON CONFLICT ("BranchCode") DO NOTHING;
    END IF;

    RAISE NOTICE 'Branches existing before: %, inserted: %', v_existing, v_missing;
END $$;

-- -------------------------------------------------------------
-- Top up Users (staff) to 1000 rows
-- -------------------------------------------------------------
DO $$
DECLARE
    v_existing integer;
    v_missing integer;
BEGIN
    SELECT COUNT(*) INTO v_existing FROM "Users";
    v_missing := GREATEST(0, 1000 - v_existing);

    IF v_missing > 0 THEN
        INSERT INTO "Users" (
            "Id", "Username", "Email", "PasswordHash", "FullName", "Role", "IsActive",
            "CreatedAt", "UpdatedAt", "Department", "Branch", "PhoneNumber"
        )
        SELECT
            uuid_generate_v4(),
            'staff' || LPAD((v_existing + s.i)::text, 6, '0'),
            'staff' || LPAD((v_existing + s.i)::text, 6, '0') || '@wekeza.com',
            '$2a$06$6xibsCYusSBfCYv1eriL5OE/bPLH8ue5gk2otpxOjNFAVgy42/SwC',
            'Staff ' || (v_existing + s.i)::text,
            CASE (s.i % 6)
                WHEN 0 THEN 'Teller'
                WHEN 1 THEN 'Supervisor'
                WHEN 2 THEN 'BranchManager'
                WHEN 3 THEN 'LoanOfficer'
                WHEN 4 THEN 'CustomerService'
                ELSE 'BackOfficeStaff'
            END,
            true,
            NOW() - ((s.i % 1000) || ' days')::interval,
            NOW(),
            CASE (s.i % 4)
                WHEN 0 THEN 'Retail Banking'
                WHEN 1 THEN 'Credit'
                WHEN 2 THEN 'Operations'
                ELSE 'Customer Service'
            END,
            'BR' || LPAD((100000 + ((s.i % 1000) + 1))::text, 6, '0'),
            '+254711' || LPAD((v_existing + s.i)::text, 6, '0')
        FROM generate_series(1, v_missing) AS s(i)
        ON CONFLICT ("Username") DO NOTHING;
    END IF;

    RAISE NOTICE 'Users existing before: %, inserted: %', v_existing, v_missing;
END $$;

-- -------------------------------------------------------------
-- Top up Customers to 1000 rows
-- -------------------------------------------------------------
DO $$
DECLARE
    v_existing integer;
    v_missing integer;
BEGIN
    SELECT COUNT(*) INTO v_existing FROM "Customers";
    v_missing := GREATEST(0, 1000 - v_existing);

    IF v_missing > 0 THEN
        INSERT INTO "Customers" (
            "Id", "FirstName", "LastName", "Email", "PhoneNumber", "IdentificationNumber",
            "RiskRating", "IsActive", "CreatedAt", "UpdatedAt"
        )
        SELECT
            uuid_generate_v4(),
            'Customer',
            (v_existing + s.i)::text,
            'customer' || (v_existing + s.i)::text || '@example.com',
            '+254712' || LPAD((v_existing + s.i)::text, 6, '0'),
            'ID' || LPAD((1000000 + v_existing + s.i)::text, 8, '0'),
            (s.i % 4),
            true,
            NOW() - ((s.i % 1000) || ' days')::interval,
            NOW()
        FROM generate_series(1, v_missing) AS s(i)
        ON CONFLICT ("Email") DO NOTHING;
    END IF;

    RAISE NOTICE 'Customers existing before: %, inserted: %', v_existing, v_missing;
END $$;

-- -------------------------------------------------------------
-- Top up Accounts to 1000 rows
-- -------------------------------------------------------------
DO $$
DECLARE
    v_existing integer;
    v_missing integer;
    v_customer_count integer;
    v_branch_count integer;
BEGIN
    SELECT COUNT(*) INTO v_existing FROM "Accounts";
    SELECT COUNT(*) INTO v_customer_count FROM "Customers";
    SELECT COUNT(*) INTO v_branch_count FROM "Branches";
    v_missing := GREATEST(0, 1000 - v_existing);

    IF v_customer_count = 0 THEN
        RAISE EXCEPTION 'Cannot seed Accounts because Customers count is 0';
    END IF;

    IF v_missing > 0 THEN
        INSERT INTO "Accounts" (
            "Id", "AccountNumber", "CustomerId", "AccountType", "Currency", "Balance", "AvailableBalance",
            "Status", "OpenedDate", "CreatedAt", "UpdatedAt", "BranchCode"
        )
        SELECT
            uuid_generate_v4(),
            'ACC' || LPAD((1000000000 + v_existing + s.i)::text, 10, '0'),
            c."Id",
            CASE (s.i % 4)
                WHEN 0 THEN 'Savings'
                WHEN 1 THEN 'Current'
                WHEN 2 THEN 'Business'
                ELSE 'FixedDeposit'
            END,
            'KES',
            (1000 + ((s.i % 2000) * 125))::numeric(18,2),
            (1000 + ((s.i % 2000) * 125))::numeric(18,2),
            'Active',
            NOW() - ((s.i % 1200) || ' days')::interval,
            NOW(),
            NOW(),
            b."BranchCode"
        FROM generate_series(1, v_missing) AS s(i)
        CROSS JOIN LATERAL (
            SELECT "Id"
            FROM "Customers"
            ORDER BY "CreatedAt", "Id"
            OFFSET ((s.i - 1) % v_customer_count)
            LIMIT 1
        ) c
        CROSS JOIN LATERAL (
            SELECT "BranchCode"
            FROM "Branches"
            ORDER BY "BranchCode"
            OFFSET ((s.i - 1) % GREATEST(v_branch_count, 1))
            LIMIT 1
        ) b
        ON CONFLICT ("AccountNumber") DO NOTHING;
    END IF;

    RAISE NOTICE 'Accounts existing before: %, inserted: %', v_existing, v_missing;
END $$;

-- -------------------------------------------------------------
-- Top up Balances to 1000 rows (1 per account where possible)
-- -------------------------------------------------------------
DO $$
DECLARE
    v_existing integer;
    v_missing integer;
BEGIN
    SELECT COUNT(*) INTO v_existing FROM "Balances";
    v_missing := GREATEST(0, 1000 - v_existing);

    IF v_missing > 0 THEN
        INSERT INTO "Balances" (
            "Id", "AccountId", "CurrentBalance", "AvailableBalance", "LedgerBalance", "Currency", "AsOfDate", "CreatedAt", "UpdatedAt"
        )
        SELECT
            uuid_generate_v4(),
            a."Id",
            COALESCE(a."Balance", 0),
            COALESCE(a."AvailableBalance", COALESCE(a."Balance", 0)),
            COALESCE(a."Balance", 0),
            COALESCE(a."Currency", 'KES'),
            NOW(),
            NOW(),
            NOW()
        FROM "Accounts" a
        LEFT JOIN "Balances" b ON b."AccountId" = a."Id"
        WHERE b."AccountId" IS NULL
        ORDER BY a."CreatedAt", a."Id"
        LIMIT v_missing
        ON CONFLICT ("AccountId") DO NOTHING;
    END IF;

    RAISE NOTICE 'Balances existing before: %, inserted (attempted): %', v_existing, v_missing;
END $$;

-- -------------------------------------------------------------
-- Top up Transactions to 1000 rows
-- -------------------------------------------------------------
DO $$
DECLARE
    v_existing integer;
    v_missing integer;
    v_account_count integer;
BEGIN
    SELECT COUNT(*) INTO v_existing FROM "Transactions";
    SELECT COUNT(*) INTO v_account_count FROM "Accounts";
    v_missing := GREATEST(0, 1000 - v_existing);

    IF v_account_count = 0 THEN
        RAISE EXCEPTION 'Cannot seed Transactions because Accounts count is 0';
    END IF;

    IF v_missing > 0 THEN
        INSERT INTO "Transactions" (
            "Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "Currency",
            "Status", "Description", "CreatedAt", "ProcessedAt", "BalanceAfter"
        )
        SELECT
            uuid_generate_v4(),
            'TX-' || TO_CHAR(NOW(), 'YYYYMMDD') || '-' || LPAD((v_existing + s.i)::text, 8, '0'),
            a."Id",
            CASE (s.i % 4)
                WHEN 0 THEN 'Deposit'
                WHEN 1 THEN 'Withdrawal'
                WHEN 2 THEN 'Transfer'
                ELSE 'Payment'
            END,
            (100 + ((s.i % 5000) * 10))::numeric(18,2),
            'KES',
            CASE WHEN (s.i % 20) = 0 THEN 'Failed' ELSE 'Completed' END,
            'Seed transaction ' || (v_existing + s.i)::text,
            NOW() - ((s.i % 365) || ' days')::interval,
            NOW() - ((s.i % 365) || ' days')::interval,
            COALESCE(a."Balance", 0)
        FROM generate_series(1, v_missing) AS s(i)
        CROSS JOIN LATERAL (
            SELECT "Id", "Balance"
            FROM "Accounts"
            ORDER BY "CreatedAt", "Id"
            OFFSET ((s.i - 1) % v_account_count)
            LIMIT 1
        ) a
        ON CONFLICT ("TransactionReference") DO NOTHING;
    END IF;

    RAISE NOTICE 'Transactions existing before: %, inserted: %', v_existing, v_missing;
END $$;

COMMIT;

-- -------------------------------------------------------------
-- Output: all tables currently in database
-- -------------------------------------------------------------
SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public'
ORDER BY table_name;

-- -------------------------------------------------------------
-- Output: mandatory starter counts (target >= 1000)
-- -------------------------------------------------------------
SELECT 'Branches' AS table_name, COUNT(*) AS total_records FROM "Branches"
UNION ALL
SELECT 'Users (Staff)', COUNT(*) FROM "Users"
UNION ALL
SELECT 'Customers', COUNT(*) FROM "Customers"
UNION ALL
SELECT 'Accounts', COUNT(*) FROM "Accounts"
UNION ALL
SELECT 'Balances', COUNT(*) FROM "Balances"
UNION ALL
SELECT 'Transactions', COUNT(*) FROM "Transactions"
ORDER BY table_name;
