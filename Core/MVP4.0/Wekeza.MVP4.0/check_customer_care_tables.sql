-- Check if Customer Care tables exist
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
AND table_name IN ('Customers', 'Accounts', 'Transactions', 'CustomerComplaints', 'AccountStatusRequests', 'CardRequests')
ORDER BY table_name;