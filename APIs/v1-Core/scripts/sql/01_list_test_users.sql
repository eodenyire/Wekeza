-- List users used in portal smoke testing
SELECT
  "Username",
  "Role",
  "IsActive",
  "LastLoginAt"
FROM "Users"
WHERE lower("Username") IN ('admin', 'manager1', 'teller1')
ORDER BY "Username";
