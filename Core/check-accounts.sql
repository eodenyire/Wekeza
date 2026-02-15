-- Check existing accounts
SELECT "Id", "AccountNumber", "AccountName", "BalanceAmount", "Status" 
FROM "Accounts" 
WHERE "Status" = 'Active'
LIMIT 5;

-- Check existing customers
SELECT "Id", "Name", "Email"
FROM "Customers"
LIMIT 5;
