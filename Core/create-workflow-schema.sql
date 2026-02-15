-- Maker-Checker-Approver Workflow Schema
-- Core tables for payment approval workflow

-- Payment Requests table (initiated by Maker)
CREATE TABLE IF NOT EXISTS "PaymentRequests" (
    "Id" UUID PRIMARY KEY,
    "RequestNumber" VARCHAR(50) NOT NULL UNIQUE,
    "InitiatorId" UUID NOT NULL REFERENCES "Users"("Id"),
    "CustomerId" UUID NOT NULL REFERENCES "Customers"("Id"),
    "AccountId" UUID NOT NULL REFERENCES "Accounts"("Id"),
    "PaymentType" VARCHAR(50) NOT NULL, -- SINGLE, BULK, PAYROLL, SUPPLIER
    "Amount" DECIMAL(18,2) NOT NULL,
    "Currency" VARCHAR(3) NOT NULL DEFAULT 'KES',
    "BeneficiaryName" VARCHAR(200) NOT NULL,
    "BeneficiaryAccount" VARCHAR(50) NOT NULL,
    "BeneficiaryBank" VARCHAR(100),
    "Purpose" VARCHAR(500) NOT NULL,
    "Reference" VARCHAR(100),
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Approved, Rejected, Executed, Failed
    "CurrentApprovalLevel" INT NOT NULL DEFAULT 1,
    "RequiredApprovalLevels" INT NOT NULL DEFAULT 2,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ExecutedAt" TIMESTAMP,
    "RejectionReason" TEXT
);

-- Payment Approvals table (approval history)
CREATE TABLE IF NOT EXISTS "PaymentApprovals" (
    "Id" UUID PRIMARY KEY,
    "PaymentRequestId" UUID NOT NULL REFERENCES "PaymentRequests"("Id"),
    "ApproverId" UUID NOT NULL REFERENCES "Users"("Id"),
    "ApprovalLevel" INT NOT NULL,
    "Action" VARCHAR(20) NOT NULL, -- APPROVED, REJECTED
    "Comments" TEXT,
    "ApprovedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Approval Limits table (role-based limits)
CREATE TABLE IF NOT EXISTS "ApprovalLimits" (
    "Id" UUID PRIMARY KEY,
    "RoleName" VARCHAR(50) NOT NULL,
    "ApprovalLevel" INT NOT NULL,
    "MinAmount" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "MaxAmount" DECIMAL(18,2) NOT NULL,
    "Currency" VARCHAR(3) NOT NULL DEFAULT 'KES',
    "TransactionType" VARCHAR(50) NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Bulk Payment Batches table
CREATE TABLE IF NOT EXISTS "BulkPaymentBatches" (
    "Id" UUID PRIMARY KEY,
    "BatchNumber" VARCHAR(50) NOT NULL UNIQUE,
    "UploadedBy" UUID NOT NULL REFERENCES "Users"("Id"),
    "FileName" VARCHAR(255) NOT NULL,
    "TotalRecords" INT NOT NULL,
    "TotalAmount" DECIMAL(18,2) NOT NULL,
    "SuccessfulRecords" INT NOT NULL DEFAULT 0,
    "FailedRecords" INT NOT NULL DEFAULT 0,
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Uploaded', -- Uploaded, Validated, Approved, Processing, Completed, Failed
    "UploadedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ProcessedAt" TIMESTAMP
);

-- Bulk Payment Items table
CREATE TABLE IF NOT EXISTS "BulkPaymentItems" (
    "Id" UUID PRIMARY KEY,
    "BatchId" UUID NOT NULL REFERENCES "BulkPaymentBatches"("Id"),
    "LineNumber" INT NOT NULL,
    "BeneficiaryName" VARCHAR(200) NOT NULL,
    "BeneficiaryAccount" VARCHAR(50) NOT NULL,
    "BeneficiaryBank" VARCHAR(100),
    "Amount" DECIMAL(18,2) NOT NULL,
    "Currency" VARCHAR(3) NOT NULL DEFAULT 'KES',
    "Purpose" VARCHAR(500),
    "Reference" VARCHAR(100),
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Validated, Executed, Failed
    "ValidationErrors" TEXT,
    "ExecutionResult" TEXT,
    "ExecutedAt" TIMESTAMP
);

-- Budget Allocations table
CREATE TABLE IF NOT EXISTS "BudgetAllocations" (
    "Id" UUID PRIMARY KEY,
    "DepartmentId" UUID NOT NULL REFERENCES "Customers"("Id"),
    "FiscalYear" INT NOT NULL,
    "Category" VARCHAR(100) NOT NULL, -- OPERATIONS, CAPITAL, PERSONNEL, etc.
    "SubCategory" VARCHAR(100),
    "AllocatedAmount" DECIMAL(18,2) NOT NULL,
    "SpentAmount" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "CommittedAmount" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "AvailableAmount" DECIMAL(18,2) NOT NULL,
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Active', -- Active, Frozen, Closed
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Budget Commitments table
CREATE TABLE IF NOT EXISTS "BudgetCommitments" (
    "Id" UUID PRIMARY KEY,
    "AllocationId" UUID NOT NULL REFERENCES "BudgetAllocations"("Id"),
    "Reference" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(500) NOT NULL,
    "Amount" DECIMAL(18,2) NOT NULL,
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Active', -- Active, Released, Utilized
    "CreatedBy" UUID NOT NULL REFERENCES "Users"("Id"),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ReleasedAt" TIMESTAMP,
    "UtilizedAt" TIMESTAMP
);

-- Audit Trail table (comprehensive logging)
CREATE TABLE IF NOT EXISTS "AuditTrail" (
    "Id" UUID PRIMARY KEY,
    "UserId" UUID REFERENCES "Users"("Id"),
    "Action" VARCHAR(100) NOT NULL,
    "EntityType" VARCHAR(50) NOT NULL,
    "EntityId" UUID,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "IpAddress" VARCHAR(50),
    "UserAgent" TEXT,
    "Timestamp" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS "IX_PaymentRequests_Status" ON "PaymentRequests"("Status");
CREATE INDEX IF NOT EXISTS "IX_PaymentRequests_InitiatorId" ON "PaymentRequests"("InitiatorId");
CREATE INDEX IF NOT EXISTS "IX_PaymentRequests_CustomerId" ON "PaymentRequests"("CustomerId");
CREATE INDEX IF NOT EXISTS "IX_PaymentApprovals_PaymentRequestId" ON "PaymentApprovals"("PaymentRequestId");
CREATE INDEX IF NOT EXISTS "IX_PaymentApprovals_ApproverId" ON "PaymentApprovals"("ApproverId");
CREATE INDEX IF NOT EXISTS "IX_BulkPaymentBatches_Status" ON "BulkPaymentBatches"("Status");
CREATE INDEX IF NOT EXISTS "IX_BulkPaymentItems_BatchId" ON "BulkPaymentItems"("BatchId");
CREATE INDEX IF NOT EXISTS "IX_BudgetAllocations_DepartmentId" ON "BudgetAllocations"("DepartmentId");
CREATE INDEX IF NOT EXISTS "IX_BudgetAllocations_FiscalYear" ON "BudgetAllocations"("FiscalYear");
CREATE INDEX IF NOT EXISTS "IX_AuditTrail_UserId" ON "AuditTrail"("UserId");
CREATE INDEX IF NOT EXISTS "IX_AuditTrail_EntityType" ON "AuditTrail"("EntityType");
CREATE INDEX IF NOT EXISTS "IX_AuditTrail_Timestamp" ON "AuditTrail"("Timestamp");

-- Insert default approval limits
INSERT INTO "ApprovalLimits" ("Id", "RoleName", "ApprovalLevel", "MinAmount", "MaxAmount", "Currency", "TransactionType")
VALUES 
('a0000000-0000-0000-0000-000000000001', 'Maker', 0, 0, 999999999999.99, 'KES', 'ALL'),
('a0000000-0000-0000-0000-000000000002', 'Checker', 1, 0, 10000000.00, 'KES', 'ALL'),
('a0000000-0000-0000-0000-000000000003', 'Approver', 2, 10000000.01, 100000000.00, 'KES', 'ALL'),
('a0000000-0000-0000-0000-000000000004', 'Senior Approver', 3, 100000000.01, 999999999999.99, 'KES', 'ALL')
ON CONFLICT ("Id") DO NOTHING;

-- Insert sample budget allocations for 2026
INSERT INTO "BudgetAllocations" ("Id", "DepartmentId", "FiscalYear", "Category", "AllocatedAmount", "SpentAmount", "CommittedAmount", "AvailableAmount", "Status")
VALUES 
('b0000000-0000-0000-0000-000000000001', '22222222-2222-2222-2222-222222222222', 2026, 'OPERATIONS', 50000000000.00, 15000000000.00, 5000000000.00, 30000000000.00, 'Active'),
('b0000000-0000-0000-0000-000000000002', '22222222-2222-2222-2222-222222222222', 2026, 'CAPITAL', 100000000000.00, 25000000000.00, 10000000000.00, 65000000000.00, 'Active'),
('b0000000-0000-0000-0000-000000000003', '33333333-3333-3333-3333-333333333333', 2026, 'OPERATIONS', 15000000000.00, 5000000000.00, 2000000000.00, 8000000000.00, 'Active'),
('b0000000-0000-0000-0000-000000000004', '44444444-4444-4444-4444-444444444444', 2026, 'OPERATIONS', 8000000000.00, 3000000000.00, 1000000000.00, 4000000000.00, 'Active')
ON CONFLICT ("Id") DO NOTHING;
