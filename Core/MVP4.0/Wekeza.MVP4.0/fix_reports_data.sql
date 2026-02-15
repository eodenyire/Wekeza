-- Fix null FilePath values in BranchReports
UPDATE "BranchReports" SET "FilePath" = 'Reports/' || "Type" || '_' || to_char("GeneratedAt", 'YYYYMMDD_HH24MISS') || '.csv' WHERE "FilePath" IS NULL;

-- Check the updated data
SELECT "Id", "Name", "Type", "FilePath", "Status" FROM "BranchReports";