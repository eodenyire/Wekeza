-- Create Staff table
CREATE TABLE IF NOT EXISTS "Staff" (
    "Id" uuid NOT NULL,
    "EmployeeId" character varying(50) NOT NULL,
    "FirstName" character varying(100) NOT NULL,
    "LastName" character varying(100) NOT NULL,
    "Email" character varying(200) NOT NULL,
    "Phone" character varying(20) NOT NULL,
    "Role" character varying(50) NOT NULL,
    "BranchId" integer NOT NULL,
    "BranchName" character varying(200) NOT NULL,
    "DepartmentId" integer NOT NULL,
    "DepartmentName" character varying(200) NOT NULL,
    "Status" character varying(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(100),
    "UpdatedAt" timestamp with time zone,
    "UpdatedBy" character varying(100),
    "LastLogin" timestamp with time zone,
    CONSTRAINT "PK_Staff" PRIMARY KEY ("Id")
);

-- Create StaffLogins table
CREATE TABLE IF NOT EXISTS "StaffLogins" (
    "Id" uuid NOT NULL,
    "StaffId" uuid NOT NULL,
    "LoginTime" timestamp with time zone NOT NULL,
    "IpAddress" character varying(50),
    "UserAgent" text,
    CONSTRAINT "PK_StaffLogins" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StaffLogins_Staff_StaffId" FOREIGN KEY ("StaffId") REFERENCES "Staff" ("Id") ON DELETE CASCADE
);

-- Create indexes
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Staff_Email" ON "Staff" ("Email");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Staff_EmployeeId" ON "Staff" ("EmployeeId");
CREATE INDEX IF NOT EXISTS "IX_StaffLogins_StaffId" ON "StaffLogins" ("StaffId");

-- Insert sample data
INSERT INTO "Staff" ("Id", "EmployeeId", "FirstName", "LastName", "Email", "Phone", "Role", "BranchId", "BranchName", "DepartmentId", "DepartmentName", "Status", "CreatedAt", "CreatedBy")
VALUES 
    ('11111111-1111-1111-1111-111111111111', 'ADM001', 'System', 'Administrator', 'admin@wekeza.com', '+254700000000', 'Administrator', 1, 'Main Branch', 9, 'IT & Systems', 'Active', NOW(), 'system'),
    ('22222222-2222-2222-2222-222222222222', 'TEL001', 'Jane', 'Smith', 'jane.smith@wekeza.com', '+254700000001', 'Teller', 1, 'Main Branch', 2, 'Teller Operations', 'Active', NOW(), 'system')
ON CONFLICT ("Id") DO NOTHING;

SELECT 'Staff tables created successfully!' as result;