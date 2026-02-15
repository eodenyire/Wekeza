-- Create Users table
CREATE TABLE "Users" (
    "Id" uuid PRIMARY KEY,
    "Username" varchar(255) UNIQUE NOT NULL,
    "Email" varchar(255) UNIQUE NOT NULL,
    "PasswordHash" text NOT NULL,
    "FullName" varchar(255) NOT NULL,
    "Role" varchar(50) NOT NULL,
    "BranchId" uuid,
    "IsActive" boolean NOT NULL DEFAULT true,
    "CreatedAt" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "LastLoginAt" timestamp
);

-- Insert test users with real BCrypt hashed passwords
INSERT INTO "Users" ("Id", "Username", "Email", "PasswordHash", "FullName", "Role", "IsActive", "CreatedAt") VALUES
('11111111-1111-1111-1111-111111111111', 'admin', 'admin@wekeza.com', '$2a$11$TSkedyuEAByQjyd0k0xDAegq41stZ/9KBpZbFPCiunAfoxkHt7SPy', 'System Administrator', 'Administrator', true, CURRENT_TIMESTAMP),
('22222222-2222-2222-2222-222222222222', 'teller1', 'teller1@wekeza.com', '$2a$11$UpwJ1l73x.L2FrpmNo5Ptuay8TqZ8gygDiif1XgfWwRYa3B0HmMma', 'John Teller', 'Teller', true, CURRENT_TIMESTAMP),
('33333333-3333-3333-3333-333333333333', 'supervisor1', 'supervisor1@wekeza.com', '$2a$11$EB/nvx0yX3SkaZpfVmRKAecoqM9SBLdQu80WdgEsKWy6Rm4CHiGFK', 'Jane Supervisor', 'Supervisor', true, CURRENT_TIMESTAMP),
('44444444-4444-4444-4444-444444444444', 'branchmanager1', 'branchmanager1@wekeza.com', '$2a$11$zDQIt0zRPORVLAyNXvRVkev.IAaJo4O8Q4WZARAw9yIAQHsUCoNke', 'Michael Manager', 'BranchManager', true, CURRENT_TIMESTAMP),
('55555555-5555-5555-5555-555555555555', 'cashofficer1', 'cashofficer1@wekeza.com', '$2a$11$rWt53Y0eHsZ7xtjrFX826OrruAgjM/2QgP7CUqgWCQNjomNTrM9RG', 'Sarah Cash', 'CashOfficer', true, CURRENT_TIMESTAMP),
('66666666-6666-6666-6666-666666666666', 'auditor1', 'auditor1@wekeza.com', '$2a$11$YmQG5EXPsMbr317/YapJve69SdyVma3k.qdN5c.4fycwAg7Zm8F5y', 'William Auditor', 'Auditor', true, CURRENT_TIMESTAMP);