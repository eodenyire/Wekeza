using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Wekeza.MVP4._0.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerCareEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0ea90821-b397-4a35-8f8b-7a272dfa1e63"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0fa71a27-3805-4e49-b0af-3c17b08a6a50"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("13d46e19-b0f8-4b2e-a743-26475f432caf"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1cb2eee2-859b-4d6a-b79e-0d29552bd021"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("30a97946-9e11-462d-b4a7-0fbeb014267e"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("94aa038e-2d46-4213-bfec-eeae1e9921e3"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a9177969-5cb4-43d7-a269-c69b88496f22"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b493e78c-acbd-48d4-a3ef-166747e2b6d7"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c99f5249-d3da-4c27-95c8-30df9f8559d8"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("df9f9316-3300-4ba5-855e-29ec444b0acc"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f17ec715-1b1a-4810-95ff-869cf69717d4"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f3b81372-3dfa-45a9-a670-845fa1551ee2"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("fb58333f-be86-4452-a9df-66abc4bf9d77"));

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "RiskAlerts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "BranchReports",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Authorizations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateTable(
                name: "AccountStatusRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AccountNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequestedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProcessedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatusRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AccountNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CardNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequestedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProcessedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    IdNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IdType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Occupation = table.Column<string>(type: "text", nullable: false),
                    EmployerName = table.Column<string>(type: "text", nullable: false),
                    EmployerAddress = table.Column<string>(type: "text", nullable: false),
                    KycStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    KycExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CustomerStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AccountType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AvailableBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OpenedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastTransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDormant = table.Column<bool>(type: "boolean", nullable: false),
                    IsFrozen = table.Column<bool>(type: "boolean", nullable: false),
                    BranchCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerComplaints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ComplaintNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AssignedTo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Resolution = table.Column<string>(type: "text", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerComplaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerComplaints_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UploadedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerDocuments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StandingInstructions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FromAccount = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ToAccount = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BeneficiaryName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Frequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextExecutionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandingInstructions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandingInstructions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AccountNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TransactionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RunningBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Channel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InitiatedBy = table.Column<string>(type: "text", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComplaintDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UploadedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ComplaintId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplaintDocuments_CustomerComplaints_ComplaintId",
                        column: x => x.ComplaintId,
                        principalTable: "CustomerComplaints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComplaintUpdates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdateText = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ComplaintId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplaintUpdates_CustomerComplaints_ComplaintId",
                        column: x => x.ComplaintId,
                        principalTable: "CustomerComplaints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "CreatedAt", "Email", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { new Guid("0b939281-1524-49ea-a1cc-6a4304d7f7c0"), null, new DateTime(2026, 1, 23, 20, 53, 17, 460, DateTimeKind.Utc).AddTicks(1183), "cashofficer1@wekeza.com", "Sarah Cash", true, null, "$2a$11$PpcBAtD4lpLPBoHYo1Bh4e5wv02Omnm5r6hsrfyCszSgQu1kw.Yey", "CashOfficer", "cashofficer1" },
                    { new Guid("1577a596-07ca-47a5-86b8-43c8394edad5"), null, new DateTime(2026, 1, 23, 20, 53, 17, 64, DateTimeKind.Utc).AddTicks(9239), "supervisor1@wekeza.com", "Jane Supervisor", true, null, "$2a$11$3nJrflLrFC5RH6p3yQ/vTO1oPEHX8xfA3tb0mBOMJAIfd980D8nzO", "Supervisor", "supervisor1" },
                    { new Guid("2e4d002b-e2a2-4dbd-9f6a-22b0409b01a8"), null, new DateTime(2026, 1, 23, 20, 53, 18, 883, DateTimeKind.Utc).AddTicks(274), "auditor1@wekeza.com", "William Auditor", true, null, "$2a$11$aj3PVxvfMzx52G/nvdjX0ObR2TjdIGYPayjWCaMjW3OI7G3pS6iRG", "Auditor", "auditor1" },
                    { new Guid("59979aa7-3bc0-4aa3-b422-b7457f3cbd02"), null, new DateTime(2026, 1, 23, 20, 53, 17, 930, DateTimeKind.Utc).AddTicks(9595), "customercare1@wekeza.com", "Emily Care", true, null, "$2a$11$OsIKFsDBRy3yaAX7YkO7du.QsjVgnyLoLuG6hzapzlPtLQn1/4U9C", "CustomerCareOfficer", "customercare1" },
                    { new Guid("5ba9885d-e5f2-4863-95d7-3d0c43624201"), null, new DateTime(2026, 1, 23, 20, 53, 16, 877, DateTimeKind.Utc).AddTicks(8514), "teller1@wekeza.com", "John Teller", true, null, "$2a$11$uitNd9F6R2EYmYkIX.bFY.a1bFcvh6hECzacIAvUav2PO0QzCbwxa", "Teller", "teller1" },
                    { new Guid("6ac4e0a1-a87e-44d5-9b5c-a1e5dece9fbb"), null, new DateTime(2026, 1, 23, 20, 53, 18, 350, DateTimeKind.Utc).AddTicks(1772), "compliance1@wekeza.com", "Linda Compliance", true, null, "$2a$11$/boLRhIC3fsGPMPEQZ2n3eTrgnesxZKbwksOfkJM9ZfIXvcfhH/4q", "ComplianceOfficer", "compliance1" },
                    { new Guid("81833d8a-258b-46ae-8fe4-c2bfb5fa503c"), null, new DateTime(2026, 1, 23, 20, 53, 18, 163, DateTimeKind.Utc).AddTicks(3208), "bancassurance1@wekeza.com", "Robert Insurance", true, null, "$2a$11$TNc3ApmE4ZcfSTv8QkHWFusNLONJQHKVVOqYs5JYIHsAGabsfMdI6", "BancassuranceAgent", "bancassurance1" },
                    { new Guid("8e21aab6-7803-4a33-a5d6-917b5b0e284f"), null, new DateTime(2026, 1, 23, 20, 53, 18, 700, DateTimeKind.Utc).AddTicks(9807), "loanofficer1@wekeza.com", "Patricia Loan", true, null, "$2a$11$FgalT2SSGD08Je51wY4Ucus4bsF4/yoo1YpdCY9M.JGu1DQ5zqspK", "LoanOfficer", "loanofficer1" },
                    { new Guid("97fda817-e316-44ce-91b3-a02f2ca4f6f3"), null, new DateTime(2026, 1, 23, 20, 53, 18, 530, DateTimeKind.Utc).AddTicks(5320), "risk1@wekeza.com", "James Risk", true, null, "$2a$11$EowZEvLGrR6WXUxmlgJtc.sJEg/J5/R1UA2SMxlzOp8Rki7Zk/gHq", "RiskOfficer", "risk1" },
                    { new Guid("a901f763-e8ec-4f44-8084-6285b404170b"), null, new DateTime(2026, 1, 23, 20, 53, 19, 48, DateTimeKind.Utc).AddTicks(1587), "itadmin1@wekeza.com", "Thomas IT", true, null, "$2a$11$FY2sBscQd.bD1J93v9QxkOvxH.R.6lX0LC8WTXG2bWkmlkFRac1Ka", "ITAdministrator", "itadmin1" },
                    { new Guid("b201979e-8265-4cb7-8d9a-e20ab4a23b9b"), null, new DateTime(2026, 1, 23, 20, 53, 17, 276, DateTimeKind.Utc).AddTicks(6557), "branchmanager1@wekeza.com", "Michael Manager", true, null, "$2a$11$pye5w4muIWZIZrjFdGjcnOkWqkCV6ASXBwKvL5sbtVaGZPDNurg3C", "BranchManager", "branchmanager1" },
                    { new Guid("b914b2c2-cad1-4358-af3b-12138789db32"), null, new DateTime(2026, 1, 23, 20, 53, 16, 657, DateTimeKind.Utc).AddTicks(7018), "admin@wekeza.com", "System Administrator", true, null, "$2a$11$rt.yfTP8q7FBnp8oyO1yYemsrr85PxSmSsSn2TZOLnzV.GehtSnC6", "Administrator", "admin" },
                    { new Guid("d87f87ca-6376-44cd-b41f-ff0bf4387c97"), null, new DateTime(2026, 1, 23, 20, 53, 17, 695, DateTimeKind.Utc).AddTicks(7603), "backoffice1@wekeza.com", "David BackOffice", true, null, "$2a$11$OHZIPsDopwbSMDswZ208K.rhUC9XdJaAhg3u7dqgDhAO9L2Q8bqc2", "BackOfficeStaff", "backoffice1" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountNumber",
                table: "Accounts",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CustomerId",
                table: "Accounts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintDocuments_ComplaintId",
                table: "ComplaintDocuments",
                column: "ComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintUpdates_ComplaintId",
                table: "ComplaintUpdates",
                column: "ComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerComplaints_CustomerId",
                table: "CustomerComplaints",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDocuments_CustomerId",
                table: "CustomerDocuments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerNumber",
                table: "Customers",
                column: "CustomerNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IdNumber",
                table: "Customers",
                column: "IdNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StandingInstructions_AccountId",
                table: "StandingInstructions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountStatusRequests");

            migrationBuilder.DropTable(
                name: "CardRequests");

            migrationBuilder.DropTable(
                name: "ComplaintDocuments");

            migrationBuilder.DropTable(
                name: "ComplaintUpdates");

            migrationBuilder.DropTable(
                name: "CustomerDocuments");

            migrationBuilder.DropTable(
                name: "StandingInstructions");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "CustomerComplaints");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0b939281-1524-49ea-a1cc-6a4304d7f7c0"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1577a596-07ca-47a5-86b8-43c8394edad5"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2e4d002b-e2a2-4dbd-9f6a-22b0409b01a8"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("59979aa7-3bc0-4aa3-b422-b7457f3cbd02"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5ba9885d-e5f2-4863-95d7-3d0c43624201"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6ac4e0a1-a87e-44d5-9b5c-a1e5dece9fbb"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("81833d8a-258b-46ae-8fe4-c2bfb5fa503c"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8e21aab6-7803-4a33-a5d6-917b5b0e284f"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("97fda817-e316-44ce-91b3-a02f2ca4f6f3"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a901f763-e8ec-4f44-8084-6285b404170b"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b201979e-8265-4cb7-8d9a-e20ab4a23b9b"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b914b2c2-cad1-4358-af3b-12138789db32"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87f87ca-6376-44cd-b41f-ff0bf4387c97"));

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "RiskAlerts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "BranchReports",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Authorizations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "CreatedAt", "Email", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { new Guid("0ea90821-b397-4a35-8f8b-7a272dfa1e63"), null, new DateTime(2026, 1, 23, 16, 43, 33, 961, DateTimeKind.Utc).AddTicks(5135), "itadmin1@wekeza.com", "Thomas IT", true, null, "$2a$11$/ljmO6j2oPVimHrmHyLlT.i7ZoKdVJguw93o/BVjUVq8KBOxHCi/q", "ITAdministrator", "itadmin1" },
                    { new Guid("0fa71a27-3805-4e49-b0af-3c17b08a6a50"), null, new DateTime(2026, 1, 23, 16, 43, 32, 852, DateTimeKind.Utc).AddTicks(8191), "compliance1@wekeza.com", "Linda Compliance", true, null, "$2a$11$4Jsur8Iyh46BJPrhrc31BOKlziV4DtDNM.M6UBiBvzo3LaFgBIrHK", "ComplianceOfficer", "compliance1" },
                    { new Guid("13d46e19-b0f8-4b2e-a743-26475f432caf"), null, new DateTime(2026, 1, 23, 16, 43, 33, 627, DateTimeKind.Utc).AddTicks(3719), "auditor1@wekeza.com", "William Auditor", true, null, "$2a$11$HY9KjHFnpKe92XM6LLXunOLh1RLkJUwzQL3dsKLSMEByZNlzpveUa", "Auditor", "auditor1" },
                    { new Guid("1cb2eee2-859b-4d6a-b79e-0d29552bd021"), null, new DateTime(2026, 1, 23, 16, 43, 32, 250, DateTimeKind.Utc).AddTicks(4754), "backoffice1@wekeza.com", "David BackOffice", true, null, "$2a$11$Ea0eeSpeRW6NGOg134oc1OOb/H6wj7wckcgWpe3GaIip5sALE8k42", "BackOfficeStaff", "backoffice1" },
                    { new Guid("30a97946-9e11-462d-b4a7-0fbeb014267e"), null, new DateTime(2026, 1, 23, 16, 43, 30, 984, DateTimeKind.Utc).AddTicks(1925), "branchmanager1@wekeza.com", "Michael Manager", true, null, "$2a$11$9515R5dh5PU067BrT02NiulLWPYVI0XsEpwybSnWwOlXHisKcx.zq", "BranchManager", "branchmanager1" },
                    { new Guid("94aa038e-2d46-4213-bfec-eeae1e9921e3"), null, new DateTime(2026, 1, 23, 16, 43, 30, 641, DateTimeKind.Utc).AddTicks(7796), "teller1@wekeza.com", "John Teller", true, null, "$2a$11$Oc8kxnexAW4rRmS/AwkOS.wwwL/HSimZbkDim63Zq5UfevVq/3aD2", "Teller", "teller1" },
                    { new Guid("a9177969-5cb4-43d7-a269-c69b88496f22"), null, new DateTime(2026, 1, 23, 16, 43, 33, 11, DateTimeKind.Utc).AddTicks(4211), "risk1@wekeza.com", "James Risk", true, null, "$2a$11$C8URQbn9.WknJCGALhrG6OY5WV3lRT5R1WyLG.ZZKT8SQ2hPuOZ6u", "RiskOfficer", "risk1" },
                    { new Guid("b493e78c-acbd-48d4-a3ef-166747e2b6d7"), null, new DateTime(2026, 1, 23, 16, 43, 32, 500, DateTimeKind.Utc).AddTicks(989), "customercare1@wekeza.com", "Emily Care", true, null, "$2a$11$E2CbEfjuYv64kzmbi3x2LOornEKrK.AZRMYObrBc7SChkS8mCEwzu", "CustomerCareOfficer", "customercare1" },
                    { new Guid("c99f5249-d3da-4c27-95c8-30df9f8559d8"), null, new DateTime(2026, 1, 23, 16, 43, 29, 367, DateTimeKind.Utc).AddTicks(199), "admin@wekeza.com", "System Administrator", true, null, "$2a$11$N4S.3gzkdjm.gbDZRilLDeouiytDNRpYLpYa4YpeF4ltDHOpkpnv2", "Administrator", "admin" },
                    { new Guid("df9f9316-3300-4ba5-855e-29ec444b0acc"), null, new DateTime(2026, 1, 23, 16, 43, 30, 823, DateTimeKind.Utc).AddTicks(269), "supervisor1@wekeza.com", "Jane Supervisor", true, null, "$2a$11$NpLfNUqSAZOz1prdkjIk/.EOnQvLrN1Orv75GNvbJjYuP/Wo8/YAW", "Supervisor", "supervisor1" },
                    { new Guid("f17ec715-1b1a-4810-95ff-869cf69717d4"), null, new DateTime(2026, 1, 23, 16, 43, 31, 820, DateTimeKind.Utc).AddTicks(1506), "cashofficer1@wekeza.com", "Sarah Cash", true, null, "$2a$11$tRGlEtF3xqThFTRqf/FiJOrvEMhzltPA20MyTF9KBRcXPYqJWYCRi", "CashOfficer", "cashofficer1" },
                    { new Guid("f3b81372-3dfa-45a9-a670-845fa1551ee2"), null, new DateTime(2026, 1, 23, 16, 43, 33, 383, DateTimeKind.Utc).AddTicks(7716), "loanofficer1@wekeza.com", "Patricia Loan", true, null, "$2a$11$XCcati.jZZo.KgDonprC1utrgbeHE6gmkP2kQfpdwsQfX25WZTXVu", "LoanOfficer", "loanofficer1" },
                    { new Guid("fb58333f-be86-4452-a9df-66abc4bf9d77"), null, new DateTime(2026, 1, 23, 16, 43, 32, 687, DateTimeKind.Utc).AddTicks(6826), "bancassurance1@wekeza.com", "Robert Insurance", true, null, "$2a$11$fpqxF7iO3MyFX/SJzrhmAeHgF3iEL7v4fYhfZ8bglhzG2azevsRT.", "BancassuranceAgent", "bancassurance1" }
                });
        }
    }
}
