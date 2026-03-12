-- =============================================================
-- Portal test users with verified BCrypt hashes
-- Generated: each hash verified against its password
-- =============================================================

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

INSERT INTO "Users" (
  "Id", "Username", "Email", "PasswordHash", "FullName", "Role", "IsActive", "Department", "CreatedAt", "UpdatedAt"
) VALUES
  (uuid_generate_v4(), 'admin', 'admin@wekeza.com',
   '$2b$11$Y.IAhH3umZRuSffbzBM4JuSXnCnn1gRu5Dez2ib5eBO0ylQriijE6',
   'System Administrator', 'Administrator', true, 'IT Operations', NOW(), NOW()),
  (uuid_generate_v4(), 'manager1', 'manager1@wekeza.com',
   '$2b$11$0Ib5nAh1B5gvp79ePRbV9OJX8fkkrkj3ipiIVtQUYZpt.2x2VIs.O',
   'Branch Manager One', 'Manager', true, 'Branch Management', NOW(), NOW()),
  (uuid_generate_v4(), 'teller1', 'teller1@wekeza.com',
   '$2b$11$gW04w4OwoOSPE0EPdc0KEuVnhCsJqhQfedyKDoyY0KIWCyzUyHawy',
   'Teller One', 'Teller', true, 'Teller Operations', NOW(), NOW()),
  (uuid_generate_v4(), 'supervisor1', 'supervisor1@wekeza.com',
   '$2b$11$VQLR8p77wiLcVVj6vN2JeO3uKFt7yNgphtNYbelUpLCkTkuji1qpO',
   'Supervisor One', 'Supervisor', true, 'Operations', NOW(), NOW()),
  (uuid_generate_v4(), 'compliance1', 'compliance1@wekeza.com',
   '$2b$11$/lY1hd2V910GPmZscYWpreTvX5Y2DS48t7vVie/pqeKuCQOz2jS9e',
   'Compliance Officer One', 'ComplianceOfficer', true, 'Compliance', NOW(), NOW()),
  (uuid_generate_v4(), 'treasury1', 'treasury1@wekeza.com',
   '$2b$11$dVsRYlzW00dKeK.PGaWC/O2y4j/LHr0FJKd43ynTV5/.Ifhv2TmFK',
   'Treasury Dealer One', 'TreasuryDealer', true, 'Treasury', NOW(), NOW()),
  (uuid_generate_v4(), 'tradeFinance1', 'tradefinance1@wekeza.com',
   '$2b$11$RIDhWLY/Z.pR1Av4.WnF8uaZ5SP5L5M.fNT7v8EBA9C5p1aPMPzlq',
   'Trade Finance Officer One', 'TradeFinanceOfficer', true, 'Trade Finance', NOW(), NOW()),
  (uuid_generate_v4(), 'payments1', 'payments1@wekeza.com',
   '$2b$11$nrYcMFVC/wYieRyQBW8S3.8iJB8eR7V.g/lnuH8leZrWbwVZxdq76',
   'Payments Officer One', 'PaymentsOfficer', true, 'Payments', NOW(), NOW()),
  (uuid_generate_v4(), 'productGL1', 'productgl1@wekeza.com',
   '$2b$11$aJv9AO/BFnI7bCK.dfnwVe3nlv6gpC7XCn75LZSFgxKUDPC67smBa',
   'Product GL Manager One', 'ProductManager', true, 'Product Management', NOW(), NOW()),
  (uuid_generate_v4(), 'customer1', 'customer1@wekeza.com',
   '$2b$11$t9UQoNQjb0jFzvHyej5aee/DUo6r.jD6jWhPqCdk2R8xHSrix1vzS',
   'Customer One', 'Customer', true, 'Customer Service', NOW(), NOW()),
  (uuid_generate_v4(), 'vaultOfficer1', 'vaultofficer1@wekeza.com',
   '$2b$11$QaAMcNQiwcY3HZ4V948.4OddpGb.NAssnyZbFFMRE3pOeROA3ZzKC',
   'Vault Officer One', 'VaultOfficer', true, 'Vault Operations', NOW(), NOW()),
  (uuid_generate_v4(), 'executive1', 'executive1@wekeza.com',
   '$2b$11$J2uC1zN9d78NLd0dIyYVeOTng2RiY96x6NmWV5V/deFENWQqrqT5i',
   'Chief Executive Officer', 'CEO', true, 'Executive', NOW(), NOW())
ON CONFLICT ("Username") DO UPDATE SET
  "PasswordHash" = EXCLUDED."PasswordHash",
  "Role" = EXCLUDED."Role",
  "FullName" = EXCLUDED."FullName",
  "IsActive" = true,
  "UpdatedAt" = NOW();

SELECT 'Portal test users seeded' as status, COUNT(*) as total FROM "Users";