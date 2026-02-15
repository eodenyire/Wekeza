-- Create Customer Care tables manually

-- Create AccountStatusRequests table
CREATE TABLE "AccountStatusRequests" (
    "Id" uuid NOT NULL,
    "RequestNumber" character varying(50) NOT NULL,
    "AccountNumber" character varying(20) NOT NULL,
    "RequestType" character varying(50) NOT NULL,
    "Reason" text NOT NULL,
    "Status" character varying(20) NOT NULL,
    "RequestedAt" timestamp with time zone NOT NULL,
    "ProcessedAt" timestamp with time zone,
    "RequestedBy" character varying(100) NOT NULL,
    "ProcessedBy" character varying(100),
    "Comments" text NOT NULL,
    CONSTRAINT "PK_AccountStatusRequests" PRIMARY KEY ("Id")
);

-- Create CardRequests table
CREATE TABLE "CardRequests" (
    "Id" uuid NOT NULL,
    "RequestNumber" character varying(50) NOT NULL,
    "AccountNumber" character varying(20) NOT NULL,
    "CardNumber" character varying(20),
    "RequestType" character varying(50) NOT NULL,
    "Reason" text NOT NULL,
    "Status" character varying(20) NOT NULL,
    "RequestedAt" timestamp with time zone NOT NULL,
    "ProcessedAt" timestamp with time zone,
    "RequestedBy" character varying(100) NOT NULL,
    "ProcessedBy" character varying(100),
    "Comments" text NOT NULL,
    CONSTRAINT "PK_CardRequests" PRIMARY KEY ("Id")
);

-- Create Customers table
CREATE TABLE "Customers" (
    "Id" uuid NOT NULL,
    "CustomerNumber" character varying(20) NOT NULL,
    "FullName" character varying(200) NOT NULL,
    "Email" character varying(100) NOT NULL,
    "PhoneNumber" character varying(20) NOT NULL,
    "Address" text NOT NULL,
    "IdNumber" character varying(50) NOT NULL,
    "IdType" character varying(20) NOT NULL,
    "DateOfBirth" timestamp with time zone NOT NULL,
    "Gender" text NOT NULL,
    "Occupation" text NOT NULL,
    "EmployerName" text NOT NULL,
    "EmployerAddress" text NOT NULL,
    "KycStatus" character varying(20) NOT NULL,
    "KycExpiryDate" timestamp with time zone NOT NULL,
    "CustomerStatus" character varying(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text NOT NULL,
    "UpdatedBy" text NOT NULL,
    CONSTRAINT "PK_Customers" PRIMARY KEY ("Id")
);

-- Create Accounts table
CREATE TABLE "Accounts" (
    "Id" uuid NOT NULL,
    "AccountNumber" character varying(20) NOT NULL,
    "AccountName" character varying(200) NOT NULL,
    "AccountType" character varying(50) NOT NULL,
    "Balance" numeric(18,2) NOT NULL,
    "AvailableBalance" numeric(18,2) NOT NULL,
    "Currency" character varying(3) NOT NULL,
    "Status" character varying(20) NOT NULL,
    "OpenedDate" timestamp with time zone NOT NULL,
    "LastTransactionDate" timestamp with time zone NOT NULL,
    "IsDormant" boolean NOT NULL,
    "IsFrozen" boolean NOT NULL,
    "BranchCode" character varying(10) NOT NULL,
    "CustomerId" uuid NOT NULL,
    CONSTRAINT "PK_Accounts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Accounts_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE
);

-- Create CustomerComplaints table
CREATE TABLE "CustomerComplaints" (
    "Id" uuid NOT NULL,
    "ComplaintNumber" character varying(50) NOT NULL,
    "Subject" character varying(200) NOT NULL,
    "Description" text NOT NULL,
    "Category" character varying(50) NOT NULL,
    "Priority" character varying(20) NOT NULL,
    "Status" character varying(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "ResolvedAt" timestamp with time zone,
    "CreatedBy" character varying(100) NOT NULL,
    "AssignedTo" character varying(100) NOT NULL,
    "Resolution" text NOT NULL,
    "CustomerId" uuid NOT NULL,
    CONSTRAINT "PK_CustomerComplaints" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CustomerComplaints_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE
);

-- Create CustomerDocuments table
CREATE TABLE "CustomerDocuments" (
    "Id" uuid NOT NULL,
    "DocumentType" character varying(50) NOT NULL,
    "FileName" character varying(200) NOT NULL,
    "FilePath" character varying(500) NOT NULL,
    "UploadedAt" timestamp with time zone NOT NULL,
    "ExpiryDate" timestamp with time zone,
    "Status" character varying(20) NOT NULL,
    "UploadedBy" character varying(100) NOT NULL,
    "CustomerId" uuid NOT NULL,
    CONSTRAINT "PK_CustomerDocuments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CustomerDocuments_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE
);

-- Create StandingInstructions table
CREATE TABLE "StandingInstructions" (
    "Id" uuid NOT NULL,
    "InstructionId" character varying(50) NOT NULL,
    "FromAccount" character varying(20) NOT NULL,
    "ToAccount" character varying(20) NOT NULL,
    "BeneficiaryName" character varying(200) NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "Frequency" character varying(20) NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone,
    "NextExecutionDate" timestamp with time zone NOT NULL,
    "Status" character varying(20) NOT NULL,
    "Description" text NOT NULL,
    "AccountId" uuid NOT NULL,
    CONSTRAINT "PK_StandingInstructions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StandingInstructions_Accounts_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Accounts" ("Id") ON DELETE CASCADE
);

-- Create Transactions table
CREATE TABLE "Transactions" (
    "Id" uuid NOT NULL,
    "TransactionId" character varying(50) NOT NULL,
    "AccountNumber" character varying(20) NOT NULL,
    "TransactionType" character varying(50) NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "Currency" character varying(3) NOT NULL,
    "Description" text NOT NULL,
    "Reference" text NOT NULL,
    "TransactionDate" timestamp with time zone NOT NULL,
    "ValueDate" timestamp with time zone NOT NULL,
    "Status" character varying(20) NOT NULL,
    "RunningBalance" numeric(18,2) NOT NULL,
    "Channel" character varying(20) NOT NULL,
    "InitiatedBy" text NOT NULL,
    "AccountId" uuid NOT NULL,
    CONSTRAINT "PK_Transactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Transactions_Accounts_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Accounts" ("Id") ON DELETE CASCADE
);

-- Create ComplaintDocuments table
CREATE TABLE "ComplaintDocuments" (
    "Id" uuid NOT NULL,
    "FileName" character varying(200) NOT NULL,
    "FilePath" character varying(500) NOT NULL,
    "FileType" character varying(50) NOT NULL,
    "FileSize" bigint NOT NULL,
    "UploadedAt" timestamp with time zone NOT NULL,
    "UploadedBy" character varying(100) NOT NULL,
    "ComplaintId" uuid NOT NULL,
    CONSTRAINT "PK_ComplaintDocuments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ComplaintDocuments_CustomerComplaints_ComplaintId" FOREIGN KEY ("ComplaintId") REFERENCES "CustomerComplaints" ("Id") ON DELETE CASCADE
);

-- Create ComplaintUpdates table
CREATE TABLE "ComplaintUpdates" (
    "Id" uuid NOT NULL,
    "UpdateText" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(100) NOT NULL,
    "ComplaintId" uuid NOT NULL,
    CONSTRAINT "PK_ComplaintUpdates" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ComplaintUpdates_CustomerComplaints_ComplaintId" FOREIGN KEY ("ComplaintId") REFERENCES "CustomerComplaints" ("Id") ON DELETE CASCADE
);

-- Create indexes
CREATE UNIQUE INDEX "IX_Accounts_AccountNumber" ON "Accounts" ("AccountNumber");
CREATE INDEX "IX_Accounts_CustomerId" ON "Accounts" ("CustomerId");
CREATE INDEX "IX_ComplaintDocuments_ComplaintId" ON "ComplaintDocuments" ("ComplaintId");
CREATE INDEX "IX_ComplaintUpdates_ComplaintId" ON "ComplaintUpdates" ("ComplaintId");
CREATE INDEX "IX_CustomerComplaints_CustomerId" ON "CustomerComplaints" ("CustomerId");
CREATE INDEX "IX_CustomerDocuments_CustomerId" ON "CustomerDocuments" ("CustomerId");
CREATE UNIQUE INDEX "IX_Customers_CustomerNumber" ON "Customers" ("CustomerNumber");
CREATE UNIQUE INDEX "IX_Customers_IdNumber" ON "Customers" ("IdNumber");
CREATE INDEX "IX_StandingInstructions_AccountId" ON "StandingInstructions" ("AccountId");
CREATE INDEX "IX_Transactions_AccountId" ON "Transactions" ("AccountId");