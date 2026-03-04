-- Verify core starter dataset and show whether each target reached 1000
WITH counts AS (
    SELECT 'Branches'::text AS table_name, COUNT(*)::bigint AS total_records FROM "Branches"
    UNION ALL SELECT 'Users (Staff)', COUNT(*) FROM "Users"
    UNION ALL SELECT 'Customers', COUNT(*) FROM "Customers"
    UNION ALL SELECT 'Accounts', COUNT(*) FROM "Accounts"
    UNION ALL SELECT 'Balances', COUNT(*) FROM "Balances"
    UNION ALL SELECT 'Transactions', COUNT(*) FROM "Transactions"
)
SELECT
    table_name,
    total_records,
    CASE WHEN total_records >= 1000 THEN 'PASS' ELSE 'FAIL' END AS threshold_1000
FROM counts
ORDER BY table_name;

SELECT table_name
FROM information_schema.tables
WHERE table_schema='public'
ORDER BY table_name;
