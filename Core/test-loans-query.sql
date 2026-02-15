SELECT 
    SUM("OutstandingBalance") as TotalOutstanding,
    SUM(CASE WHEN "LoanType" = 'GOVERNMENT' THEN "OutstandingBalance" ELSE 0 END) as NationalGovernment,
    COUNT(*) as TotalLoans
FROM "Loans"
WHERE "Status" IN ('Disbursed', 'Active');
