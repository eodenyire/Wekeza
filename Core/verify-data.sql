-- Verify revenue transactions by month
SELECT 
    TO_CHAR("ValueDate", 'YYYY-MM') as month, 
    COUNT(*) as transactions, 
    SUM("Amount") as total_revenue 
FROM "Transactions" 
WHERE "TransactionType" = 'DEPOSIT' 
GROUP BY TO_CHAR("ValueDate", 'YYYY-MM') 
ORDER BY month;

-- Verify grant disbursements by month
SELECT 
    TO_CHAR("DisbursementDate", 'YYYY-MM') as month, 
    COUNT(*) as grants, 
    SUM("Amount") as total_disbursed 
FROM "Grants" 
WHERE "DisbursementDate" IS NOT NULL 
GROUP BY TO_CHAR("DisbursementDate", 'YYYY-MM') 
ORDER BY month;
