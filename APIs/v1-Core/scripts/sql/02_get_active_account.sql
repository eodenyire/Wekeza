-- Pick one active account for teller transaction smoke tests
SELECT "AccountNumber"
FROM "Accounts"
WHERE "Status" = 'Active'
ORDER BY "CreatedAt" DESC
LIMIT 1;
