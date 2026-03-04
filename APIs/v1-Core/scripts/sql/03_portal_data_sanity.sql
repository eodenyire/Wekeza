-- Basic data sanity checks used while validating portal endpoints
SELECT COUNT(*) AS total_users FROM "Users";
SELECT COUNT(*) AS total_customers FROM "Customers";
SELECT COUNT(*) AS total_accounts FROM "Accounts";
SELECT COUNT(*) AS total_transactions FROM "Transactions";
