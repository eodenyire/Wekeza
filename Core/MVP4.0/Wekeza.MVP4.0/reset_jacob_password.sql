-- Reset jacobodenyire password to 'admin123' (same as admin user)
UPDATE "Users" 
SET "PasswordHash" = '$2a$11$TSkedyuEAByQjyd0k0xDAegq41stZ/9KBpZbFPCiunAfoxkHt7SPy'
WHERE "Username" = 'jacobodenyire';