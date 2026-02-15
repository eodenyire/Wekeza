-- Fix null CustomerName values in Authorizations
UPDATE "Authorizations" SET "CustomerName" = 'Unknown Customer' WHERE "CustomerName" IS NULL;

-- Fix null CustomerName values in RiskAlerts  
UPDATE "RiskAlerts" SET "CustomerName" = 'Unknown Customer' WHERE "CustomerName" IS NULL;