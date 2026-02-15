using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Wekeza.MVP4._0.Migrations
{
    /// <inheritdoc />
    public partial class AddBankingWorkflowRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "BankingAuditLogs",
                columns: table => new
                {
                    AuditId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    IPAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankingAuditLogs", x => x.AuditId);
                    table.ForeignKey(
                        name: "FK_BankingAuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsurancePolicies",
                columns: table => new
                {
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    PolicyNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    PolicyType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CoverageAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PremiumAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PremiumFrequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LinkedAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BancassuranceOfficerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePolicies", x => x.PolicyId);
                    table.ForeignKey(
                        name: "FK_InsurancePolicies_Accounts_LinkedAccountId",
                        column: x => x.LinkedAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InsurancePolicies_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsurancePolicies_Users_BancassuranceOfficerId",
                        column: x => x.BancassuranceOfficerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KYCData",
                columns: table => new
                {
                    KYCId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RiskRating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LastReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ComplianceNotes = table.Column<string>(type: "text", nullable: false),
                    AMLFlagged = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KYCData", x => x.KYCId);
                    table.ForeignKey(
                        name: "FK_KYCData_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    LoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    Tenure = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DisbursementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaturityDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LoanOfficerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Purpose = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OutstandingBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.LoanId);
                    table.ForeignKey(
                        name: "FK_Loans_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Loans_Users_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_Users_LoanOfficerId",
                        column: x => x.LoanOfficerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Resource = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Conditions = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.PermissionId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ApprovalLimit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "SignatoryRules",
                columns: table => new
                {
                    RuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SignatoryType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MinimumSignatures = table.Column<int>(type: "integer", nullable: false),
                    MaximumAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignatoryRules", x => x.RuleId);
                    table.ForeignKey(
                        name: "FK_SignatoryRules_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignatoryRules_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstances",
                columns: table => new
                {
                    WorkflowId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InitiatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    InitiatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Data = table.Column<string>(type: "text", nullable: true),
                    BusinessJustification = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstances", x => x.WorkflowId);
                    table.ForeignKey(
                        name: "FK_WorkflowInstances_Users_InitiatedBy",
                        column: x => x.InitiatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceClaims",
                columns: table => new
                {
                    ClaimId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ClaimAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IncidentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClaimDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ProcessedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProcessingNotes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceClaims", x => x.ClaimId);
                    table.ForeignKey(
                        name: "FK_InsuranceClaims_InsurancePolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "InsurancePolicies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsuranceClaims_Users_ProcessedBy",
                        column: x => x.ProcessedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PolicyBeneficiaries",
                columns: table => new
                {
                    BeneficiaryId = table.Column<Guid>(type: "uuid", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BeneficiaryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Relationship = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BenefitPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyBeneficiaries", x => x.BeneficiaryId);
                    table.ForeignKey(
                        name: "FK_PolicyBeneficiaries_InsurancePolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "InsurancePolicies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PremiumPayments",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScheduledAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiumPayments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_PremiumPayments_InsurancePolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "InsurancePolicies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PremiumPayments_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RiskIndicators",
                columns: table => new
                {
                    IndicatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    KYCId = table.Column<Guid>(type: "uuid", nullable: false),
                    IndicatorType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IdentifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdentifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskIndicators", x => x.IndicatorId);
                    table.ForeignKey(
                        name: "FK_RiskIndicators_KYCData_KYCId",
                        column: x => x.KYCId,
                        principalTable: "KYCData",
                        principalColumn: "KYCId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RiskIndicators_Users_IdentifiedBy",
                        column: x => x.IdentifiedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Collaterals",
                columns: table => new
                {
                    CollateralId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    CollateralType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EstimatedValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValuationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ValuationReference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collaterals", x => x.CollateralId);
                    table.ForeignKey(
                        name: "FK_Collaterals_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditAssessments",
                columns: table => new
                {
                    AssessmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditScore = table.Column<int>(type: "integer", nullable: false),
                    RiskGrade = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DebtToIncomeRatio = table.Column<decimal>(type: "numeric(5,4)", nullable: true),
                    CollateralValue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    AssessmentNotes = table.Column<string>(type: "text", nullable: true),
                    AssessmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssessedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditAssessments", x => x.AssessmentId);
                    table.ForeignKey(
                        name: "FK_CreditAssessments_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditAssessments_Users_AssessedBy",
                        column: x => x.AssessedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanDocuments",
                columns: table => new
                {
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanDocuments", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_LoanDocuments_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanDocuments_Users_UploadedBy",
                        column: x => x.UploadedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanRepayments",
                columns: table => new
                {
                    RepaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScheduledAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PaymentReference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRepayments", x => x.RepaymentId);
                    table.ForeignKey(
                        name: "FK_LoanRepayments_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RolePermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.RolePermissionId);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleAssignments",
                columns: table => new
                {
                    UserRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleAssignments", x => x.UserRoleId);
                    table.ForeignKey(
                        name: "FK_UserRoleAssignments_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleAssignments_Users_AssignedBy",
                        column: x => x.AssignedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoleAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountSignatories",
                columns: table => new
                {
                    SignatoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SignatoryRole = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AddedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    SignatoryRuleRuleId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSignatories", x => x.SignatoryId);
                    table.ForeignKey(
                        name: "FK_AccountSignatories_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountSignatories_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountSignatories_SignatoryRules_SignatoryRuleRuleId",
                        column: x => x.SignatoryRuleRuleId,
                        principalTable: "SignatoryRules",
                        principalColumn: "RuleId");
                    table.ForeignKey(
                        name: "FK_AccountSignatories_Users_AddedBy",
                        column: x => x.AddedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalSteps",
                columns: table => new
                {
                    StepId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uuid", nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    ApproverRole = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AssignedTo = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comments = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalSteps", x => x.StepId);
                    table.ForeignKey(
                        name: "FK_ApprovalSteps_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalSteps_WorkflowInstances_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "WorkflowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClaimDocuments",
                columns: table => new
                {
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimDocuments", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_ClaimDocuments_InsuranceClaims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "InsuranceClaims",
                        principalColumn: "ClaimId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimDocuments_Users_UploadedBy",
                        column: x => x.UploadedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "Action", "Conditions", "CreatedAt", "Resource" },
                values: new object[,]
                {
                    { new Guid("07e1ecc8-7b1c-453b-bf7a-a7dd368efb77"), "Override", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(782), "Policy" },
                    { new Guid("14070ea3-b6a5-49bd-8d33-6747c5973191"), "Process", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(779), "Claim" },
                    { new Guid("31b3b817-03da-43ba-a3cf-3e5a9245442b"), "Service", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(777), "Policy" },
                    { new Guid("44e31322-8715-4eaf-b170-06f1f6ba2d98"), "Create", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(774), "Policy" },
                    { new Guid("45f5c88e-f606-448a-b461-7fed7f7ad5e3"), "Disburse", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(770), "Loan" },
                    { new Guid("4dd32566-4c1c-4c95-af99-d1e72c135b32"), "Create", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(767), "Loan" },
                    { new Guid("6ea48b68-3085-479f-89d2-13eae2c95c1e"), "Assess", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(768), "Loan" },
                    { new Guid("6faae9c1-4d37-4e9d-a1f5-d7f83b37f969"), "Process", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(765), "Transaction" },
                    { new Guid("87e95fae-804b-4d35-93df-f5110443fa89"), "Close", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(761), "Account" },
                    { new Guid("8ea4e453-7873-4fcb-ac90-6f9f83f30c0b"), "Service", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(772), "Loan" },
                    { new Guid("a4304a0e-ad64-45a7-b866-a8b23e34261f"), "Update", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(749), "Account" },
                    { new Guid("af59526f-9151-48ed-9bc7-e93451c07e33"), "Create", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(745), "Account" },
                    { new Guid("de2d627d-15eb-4308-b7c7-e520a5b44f47"), "Approve", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(780), "All" },
                    { new Guid("f50053e5-04cd-43c6-8325-0134b61f90e2"), "Process", null, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(763), "KYC" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "ApprovalLimit", "CreatedAt", "Description", "IsActive", "RoleName" },
                values: new object[,]
                {
                    { new Guid("2af2ad61-c154-4102-ac0f-beff8c18bbca"), 50000m, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(595), "Manages complete credit lifecycle within approval limits", true, "LoanOfficer" },
                    { new Guid("4e0a5944-984e-4a04-9809-d48c85a7c143"), 10000m, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(557), "Handles account operations, KYC processing, and non-cash transactions", true, "BackOfficeOfficer" },
                    { new Guid("87d762f8-ef92-4636-96ec-a0150d9a20de"), 500000m, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(599), "Approval authority for high-risk actions and policy overrides", true, "BranchManager" },
                    { new Guid("d7cffe60-ef05-4e91-9c87-52cce0ba42e0"), 25000m, new DateTime(2026, 1, 24, 17, 58, 19, 387, DateTimeKind.Utc).AddTicks(597), "Manages insurance products within banking system", true, "BancassuranceOfficer" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "CreatedAt", "Email", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { new Guid("0100b114-c7ce-441a-b207-cb27cd964172"), null, new DateTime(2026, 1, 24, 17, 58, 18, 14, DateTimeKind.Utc).AddTicks(3662), "branchmanager1@wekeza.com", "Michael Manager", true, null, "$2a$11$wt1KWdLy1ddiR2BgAIKAyeVff1.QpJ9NMZ7hDjB.QN6tq1ypKi3X2", "BranchManager", "branchmanager1" },
                    { new Guid("43f8750d-2ffd-4c3b-ade4-27670f1baf30"), null, new DateTime(2026, 1, 24, 17, 58, 18, 479, DateTimeKind.Utc).AddTicks(5931), "customercare1@wekeza.com", "Emily Care", true, null, "$2a$11$1Nqzzm1QQrV2wgYAbY9P1ezer56gkvBvm/5Le8VSNxqGDPAdH5Gsu", "CustomerCareOfficer", "customercare1" },
                    { new Guid("477b1c6b-3a4b-4643-9a82-cc9b2cc5d193"), null, new DateTime(2026, 1, 24, 17, 58, 17, 538, DateTimeKind.Utc).AddTicks(9128), "admin@wekeza.com", "System Administrator", true, null, "$2a$11$2u7DpGjRz9M1e8CO/O1C/erwEypP9ItJ1upR9b.pPsNiPG9p.4LwS", "Administrator", "admin" },
                    { new Guid("556a9a1c-a717-4a42-9148-694d97bc1c34"), null, new DateTime(2026, 1, 24, 17, 58, 19, 386, DateTimeKind.Utc).AddTicks(9706), "itadmin1@wekeza.com", "Thomas IT", true, null, "$2a$11$X4fewXKRIL.EXnozAJ71HO5XlfM4qKgCTBBdA4DAfoYm0GGShKQDy", "ITAdministrator", "itadmin1" },
                    { new Guid("570fc979-c253-49c4-bf61-c19b629751f5"), null, new DateTime(2026, 1, 24, 17, 58, 19, 85, DateTimeKind.Utc).AddTicks(7657), "loanofficer1@wekeza.com", "Patricia Loan", true, null, "$2a$11$VDZ2GcvOyWHh/rq716x7ve2koKa5mJ7a63UnJTi/aEgcYCL965f5.", "LoanOfficer", "loanofficer1" },
                    { new Guid("a31aadf3-40ea-46c2-aa50-c37e92ea7b8f"), null, new DateTime(2026, 1, 24, 17, 58, 17, 854, DateTimeKind.Utc).AddTicks(3854), "supervisor1@wekeza.com", "Jane Supervisor", true, null, "$2a$11$LXEeUjUBs/SE46G5VP0WC.LtIOHIobGVDrC1smachVTd.CWQYqY/e", "Supervisor", "supervisor1" },
                    { new Guid("a6d5b107-4fbb-4d94-863c-c4f89befbbd0"), null, new DateTime(2026, 1, 24, 17, 58, 18, 788, DateTimeKind.Utc).AddTicks(6256), "compliance1@wekeza.com", "Linda Compliance", true, null, "$2a$11$RaDaY0/RuhYdalz0ZfjB5unnvrJKwLJgK91sUUdCdmqiw/jo1.U1a", "ComplianceOfficer", "compliance1" },
                    { new Guid("ba509ee4-aa92-4a7a-93f5-57c851b0b40a"), null, new DateTime(2026, 1, 24, 17, 58, 18, 629, DateTimeKind.Utc).AddTicks(5564), "bancassurance1@wekeza.com", "Robert Insurance", true, null, "$2a$11$XLRZsCsNsCH7oDFrN2exOumMMv5jen59WuVBU1EgVl4TrnFS2VeNO", "BancassuranceAgent", "bancassurance1" },
                    { new Guid("c3d53fde-7098-4c1b-9197-a33c7dca80cf"), null, new DateTime(2026, 1, 24, 17, 58, 18, 331, DateTimeKind.Utc).AddTicks(573), "backoffice1@wekeza.com", "David BackOffice", true, null, "$2a$11$m8WKqtCDHEWa5KwWRZtshOFltxz8nhlN6V0KR8OTuc.uvrC0gZA52", "BackOfficeStaff", "backoffice1" },
                    { new Guid("c7ee665d-68a4-42ed-a07b-767f216941f7"), null, new DateTime(2026, 1, 24, 17, 58, 17, 704, DateTimeKind.Utc).AddTicks(6824), "teller1@wekeza.com", "John Teller", true, null, "$2a$11$mWExedbQMk7FDCg10lwxT.N/R25FuMpUyNGQe600YQUQvw.Q.3qpu", "Teller", "teller1" },
                    { new Guid("c84f5c6a-908d-4acc-a976-9daedd65572a"), null, new DateTime(2026, 1, 24, 17, 58, 19, 233, DateTimeKind.Utc).AddTicks(8136), "auditor1@wekeza.com", "William Auditor", true, null, "$2a$11$cA8wS.LpC.vuS1oncQzICuujTN5F.c3rwq.B3Fo/nOOcRtrjdCtDy", "Auditor", "auditor1" },
                    { new Guid("c9ad1a65-d799-44a5-b700-f3f1b560ad46"), null, new DateTime(2026, 1, 24, 17, 58, 18, 180, DateTimeKind.Utc).AddTicks(8505), "cashofficer1@wekeza.com", "Sarah Cash", true, null, "$2a$11$I7.FQFZhIoIn.t.1A5Uya.dvg5a/.GMx8J0vkUUQ1mhpTy.HIVlOO", "CashOfficer", "cashofficer1" },
                    { new Guid("ea71b988-9d73-4282-a6a4-60c7e64fcf66"), null, new DateTime(2026, 1, 24, 17, 58, 18, 935, DateTimeKind.Utc).AddTicks(9813), "risk1@wekeza.com", "James Risk", true, null, "$2a$11$6J6LYGBack3GNAp5PnzEu.TTpD6RR0L1f9IsLVa/iB2SUenRlINki", "RiskOfficer", "risk1" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountSignatories_AccountId",
                table: "AccountSignatories",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSignatories_AddedBy",
                table: "AccountSignatories",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSignatories_CustomerId",
                table: "AccountSignatories",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSignatories_SignatoryRuleRuleId",
                table: "AccountSignatories",
                column: "SignatoryRuleRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalSteps_AssignedTo",
                table: "ApprovalSteps",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalSteps_WorkflowId",
                table: "ApprovalSteps",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_BankingAuditLogs_UserId",
                table: "BankingAuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDocuments_ClaimId",
                table: "ClaimDocuments",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDocuments_UploadedBy",
                table: "ClaimDocuments",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Collaterals_LoanId",
                table: "Collaterals",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditAssessments_AssessedBy",
                table: "CreditAssessments",
                column: "AssessedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CreditAssessments_LoanId",
                table: "CreditAssessments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_ClaimNumber",
                table: "InsuranceClaims",
                column: "ClaimNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_PolicyId",
                table: "InsuranceClaims",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_ProcessedBy",
                table: "InsuranceClaims",
                column: "ProcessedBy");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_BancassuranceOfficerId",
                table: "InsurancePolicies",
                column: "BancassuranceOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_CustomerId",
                table: "InsurancePolicies",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_LinkedAccountId",
                table: "InsurancePolicies",
                column: "LinkedAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_PolicyNumber",
                table: "InsurancePolicies",
                column: "PolicyNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KYCData_CustomerId",
                table: "KYCData",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDocuments_LoanId",
                table: "LoanDocuments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDocuments_UploadedBy",
                table: "LoanDocuments",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepayments_LoanId",
                table: "LoanRepayments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ApprovedBy",
                table: "Loans",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_CustomerId",
                table: "Loans",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_LoanNumber",
                table: "Loans",
                column: "LoanNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_LoanOfficerId",
                table: "Loans",
                column: "LoanOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Resource_Action",
                table: "Permissions",
                columns: new[] { "Resource", "Action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyBeneficiaries_PolicyId",
                table: "PolicyBeneficiaries",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_PremiumPayments_PolicyId",
                table: "PremiumPayments",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_PremiumPayments_TransactionId",
                table: "PremiumPayments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIndicators_IdentifiedBy",
                table: "RiskIndicators",
                column: "IdentifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIndicators_KYCId",
                table: "RiskIndicators",
                column: "KYCId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SignatoryRules_AccountId",
                table: "SignatoryRules",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SignatoryRules_CreatedBy",
                table: "SignatoryRules",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_AssignedBy",
                table: "UserRoleAssignments",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_RoleId",
                table: "UserRoleAssignments",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_UserId",
                table: "UserRoleAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_InitiatedBy",
                table: "WorkflowInstances",
                column: "InitiatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountSignatories");

            migrationBuilder.DropTable(
                name: "ApprovalSteps");

            migrationBuilder.DropTable(
                name: "BankingAuditLogs");

            migrationBuilder.DropTable(
                name: "ClaimDocuments");

            migrationBuilder.DropTable(
                name: "Collaterals");

            migrationBuilder.DropTable(
                name: "CreditAssessments");

            migrationBuilder.DropTable(
                name: "LoanDocuments");

            migrationBuilder.DropTable(
                name: "LoanRepayments");

            migrationBuilder.DropTable(
                name: "PolicyBeneficiaries");

            migrationBuilder.DropTable(
                name: "PremiumPayments");

            migrationBuilder.DropTable(
                name: "RiskIndicators");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoleAssignments");

            migrationBuilder.DropTable(
                name: "SignatoryRules");

            migrationBuilder.DropTable(
                name: "WorkflowInstances");

            migrationBuilder.DropTable(
                name: "InsuranceClaims");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "KYCData");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "InsurancePolicies");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0100b114-c7ce-441a-b207-cb27cd964172"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("43f8750d-2ffd-4c3b-ade4-27670f1baf30"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("477b1c6b-3a4b-4643-9a82-cc9b2cc5d193"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("556a9a1c-a717-4a42-9148-694d97bc1c34"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("570fc979-c253-49c4-bf61-c19b629751f5"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a31aadf3-40ea-46c2-aa50-c37e92ea7b8f"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a6d5b107-4fbb-4d94-863c-c4f89befbbd0"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ba509ee4-aa92-4a7a-93f5-57c851b0b40a"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c3d53fde-7098-4c1b-9197-a33c7dca80cf"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c7ee665d-68a4-42ed-a07b-767f216941f7"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c84f5c6a-908d-4acc-a976-9daedd65572a"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c9ad1a65-d799-44a5-b700-f3f1b560ad46"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ea71b988-9d73-4282-a6a4-60c7e64fcf66"));

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
        }
    }
}
