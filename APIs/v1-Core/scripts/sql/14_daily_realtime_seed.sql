-- Daily realtime seed: appends fresh branch/staff/customer/account/transaction activity

BEGIN;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Ensure baseline heavy setup is already available for core tables
-- (Balances/Branches/Accounts columns expected)

DO $$
DECLARE
  v_day text := to_char(CURRENT_DATE, 'YYYYMMDD');
  v_branch_code text := 'BRDAY' || to_char(CURRENT_DATE, 'YYYYMMDD');
BEGIN
  INSERT INTO "Branches" (
      "Id", "BranchCode", "BranchName", "Address", "City", "Country", "PhoneNumber", "Email",
      "BranchType", "Status", "OpeningDate", "TimeZone", "BusinessHoursStart", "BusinessHoursEnd",
      "ManagerId", "DeputyManagerId", "CashLimit", "CashLimitCurrency",
      "TransactionLimit", "TransactionLimitCurrency", "IsEODCompleted", "IsBODCompleted",
      "CreatedBy", "CreatedAt"
  )
  VALUES (
      uuid_generate_v4(),
      v_branch_code,
      'Realtime Branch ' || v_day,
      'Realtime Seed Street, Nairobi',
      'Nairobi',
      'Kenya',
      '+254799000000',
      'realtime-' || lower(v_day) || '@wekeza.com',
      0,
      0,
      NOW(),
      'Africa/Nairobi',
      INTERVAL '08:00:00',
      INTERVAL '17:00:00',
      'MGR-RT-' || v_day,
      'DMG-RT-' || v_day,
      75000000,
      'KES',
      15000000,
      'KES',
      false,
      false,
      'daily-seed',
      NOW()
  )
  ON CONFLICT ("BranchCode") DO NOTHING;
END $$;

-- Daily staff additions (20)
INSERT INTO "Users" (
  "Id", "Username", "Email", "PasswordHash", "FullName", "Role", "IsActive",
  "CreatedAt", "UpdatedAt", "Department", "Branch", "PhoneNumber"
)
SELECT
  uuid_generate_v4(),
  'daily_staff_' || to_char(CURRENT_DATE, 'YYYYMMDD') || '_' || LPAD(gs::text, 3, '0'),
  'daily.staff.' || to_char(CURRENT_DATE, 'YYYYMMDD') || '.' || LPAD(gs::text, 3, '0') || '@wekeza.com',
  '$2a$06$6xibsCYusSBfCYv1eriL5OE/bPLH8ue5gk2otpxOjNFAVgy42/SwC',
  'Daily Staff ' || gs,
  CASE (gs % 5)
    WHEN 0 THEN 'Teller'
    WHEN 1 THEN 'Supervisor'
    WHEN 2 THEN 'BranchManager'
    WHEN 3 THEN 'CustomerService'
    ELSE 'BackOfficeStaff'
  END,
  true,
  NOW(),
  NOW(),
  'Realtime Operations',
  'BRDAY' || to_char(CURRENT_DATE, 'YYYYMMDD'),
  '+254733' || LPAD(gs::text, 6, '0')
FROM generate_series(1, 20) gs
ON CONFLICT ("Username") DO NOTHING;

-- Daily customer additions (100)
INSERT INTO "Customers" (
  "Id", "FirstName", "LastName", "Email", "PhoneNumber", "IdentificationNumber",
  "RiskRating", "IsActive", "CreatedAt", "UpdatedAt"
)
SELECT
  uuid_generate_v4(),
  'Realtime',
  'Customer ' || gs,
  'realtime.customer.' || to_char(CURRENT_DATE, 'YYYYMMDD') || '.' || LPAD(gs::text, 4, '0') || '@example.com',
  '+254744' || LPAD(gs::text, 6, '0'),
  'RTID' || to_char(CURRENT_DATE, 'YYYYMMDD') || LPAD(gs::text, 4, '0'),
  gs % 4,
  true,
  NOW(),
  NOW()
FROM generate_series(1, 100) gs
ON CONFLICT ("Email") DO NOTHING;

-- One account per today's customers where account does not yet exist
WITH todays_customers AS (
  SELECT c."Id"
  FROM "Customers" c
  WHERE c."CreatedAt"::date = CURRENT_DATE
), missing_accounts AS (
  SELECT tc."Id" AS customer_id, ROW_NUMBER() OVER (ORDER BY tc."Id") rn
  FROM todays_customers tc
  LEFT JOIN "Accounts" a ON a."CustomerId" = tc."Id"
  WHERE a."Id" IS NULL
)
INSERT INTO "Accounts" (
  "Id", "AccountNumber", "CustomerId", "AccountType", "Currency", "Balance", "AvailableBalance",
  "Status", "OpenedDate", "CreatedAt", "UpdatedAt", "BranchCode"
)
SELECT
  uuid_generate_v4(),
  'RTACC' || to_char(CURRENT_DATE, 'YYYYMMDD') || LPAD(ma.rn::text, 5, '0'),
  ma.customer_id,
  CASE (ma.rn % 3)
    WHEN 0 THEN 'Savings'
    WHEN 1 THEN 'Current'
    ELSE 'Business'
  END,
  'KES',
  (1000 + (ma.rn * 50))::numeric(18,2),
  (1000 + (ma.rn * 50))::numeric(18,2),
  'Active',
  NOW(),
  NOW(),
  NOW(),
  'BRDAY' || to_char(CURRENT_DATE, 'YYYYMMDD')
FROM missing_accounts ma
ON CONFLICT ("AccountNumber") DO NOTHING;

-- Ensure balances for new accounts
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
ON CONFLICT ("AccountId") DO NOTHING;

-- Create realtime transaction traffic (2000/day)
DROP TABLE IF EXISTS tmp_daily_tx;
CREATE TEMP TABLE tmp_daily_tx
(
  tx_id uuid NOT NULL,
  tx_ref varchar(80) NOT NULL,
  account_id uuid NOT NULL,
  tx_type varchar(30) NOT NULL,
  amount numeric(18,2) NOT NULL,
  signed_delta numeric(18,2) NOT NULL,
  created_at timestamp without time zone NOT NULL
);

INSERT INTO tmp_daily_tx (tx_id, tx_ref, account_id, tx_type, amount, signed_delta, created_at)
SELECT
  uuid_generate_v4(),
  'RTX-' || to_char(CURRENT_TIMESTAMP, 'YYYYMMDDHH24MISSMS') || '-' || substr(md5(random()::text || gs::text), 1, 12),
  a."Id",
  CASE WHEN gs % 5 = 0 THEN 'Withdrawal' ELSE 'Deposit' END,
  round((100 + random() * 10000)::numeric, 2),
  CASE WHEN gs % 5 = 0 THEN -round((100 + random() * 10000)::numeric, 2)
       ELSE round((100 + random() * 10000)::numeric, 2)
  END,
  NOW() - ((gs % 86400) || ' seconds')::interval
FROM generate_series(1, 2000) gs
CROSS JOIN LATERAL (
  SELECT "Id"
  FROM "Accounts"
  ORDER BY random()
  LIMIT 1
) a;

INSERT INTO "Transactions" (
  "Id", "TransactionReference", "AccountId", "TransactionType", "Amount", "Currency",
  "Status", "Description", "CreatedAt", "ProcessedAt", "BalanceAfter"
)
SELECT
  tx_id,
  tx_ref,
  account_id,
  tx_type,
  ABS(amount),
  'KES',
  'Completed',
  'Daily realtime seed transaction',
  created_at,
  created_at,
  0
FROM tmp_daily_tx
ON CONFLICT ("TransactionReference") DO NOTHING;

-- Apply transaction deltas to account balances
WITH delta AS (
  SELECT account_id, SUM(signed_delta) AS total_delta
  FROM tmp_daily_tx
  GROUP BY account_id
)
UPDATE "Accounts" a
SET
  "Balance" = COALESCE(a."Balance", 0) + d.total_delta,
  "AvailableBalance" = COALESCE(a."AvailableBalance", COALESCE(a."Balance", 0)) + d.total_delta,
  "UpdatedAt" = NOW()
FROM delta d
WHERE a."Id" = d.account_id;

-- Sync balances table from account balances
UPDATE "Balances" b
SET
  "CurrentBalance" = COALESCE(a."Balance", 0),
  "AvailableBalance" = COALESCE(a."AvailableBalance", COALESCE(a."Balance", 0)),
  "LedgerBalance" = COALESCE(a."Balance", 0),
  "AsOfDate" = NOW(),
  "UpdatedAt" = NOW()
FROM "Accounts" a
WHERE b."AccountId" = a."Id";

-- Set realistic balance-after for today's seeded tx
UPDATE "Transactions" t
SET "BalanceAfter" = COALESCE(a."Balance", 0)
FROM "Accounts" a
WHERE t."AccountId" = a."Id"
  AND t."Description" = 'Daily realtime seed transaction'
  AND t."CreatedAt"::date = CURRENT_DATE;

COMMIT;

SELECT 'Daily realtime seed completed' AS message, NOW() AS executed_at;
