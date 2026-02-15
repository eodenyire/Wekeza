-- Check pending authorizations
SELECT COUNT(*) as pending_auths FROM "Authorizations" WHERE "Status" = 'Pending';

-- Check risk alerts
SELECT COUNT(*) as active_alerts FROM "RiskAlerts" WHERE "Status" != 'Resolved';

-- Check cash position
SELECT "VaultCash", "TellerCash", "ATMCash", ("VaultCash" + "TellerCash" + "ATMCash") as total FROM "CashPositions" ORDER BY "LastUpdated" DESC LIMIT 1;

-- Check all authorizations
SELECT "Id", "Type", "Status", "Amount", "CustomerName" FROM "Authorizations";

-- Check all risk alerts
SELECT "Id", "AlertType", "Status", "RiskLevel", "AccountNumber" FROM "RiskAlerts";