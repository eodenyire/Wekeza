using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wekeza.Core.Infrastructure.Persistence.Migrations;

/// <summary>
/// Enhanced Loan Management Migration - Week 6 Implementation
/// Adds comprehensive loan management capabilities with credit scoring,
/// repayment schedules, collaterals, guarantors, and GL integration
/// </summary>
public partial class EnhanceLoanManagement : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Drop existing Loans table if it exists (for clean migration)
        migrationBuilder.DropTable(
            name: "Loans",
            schema: null);

        // Create enhanced Loans table
        migrationBuilder.CreateTable(
            name: "Loans",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                LoanNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                DisbursementAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                
                // Principal amount (Money value object)
                PrincipalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                PrincipalCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                
                // Outstanding principal (Money value object)
                OutstandingPrincipalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                OutstandingPrincipalCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                
                InterestRate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                TermInMonths = table.Column<int>(type: "integer", nullable: false),
                FirstPaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                MaturityDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                
                // Loan status and lifecycle
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                SubStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                ApplicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ApprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                DisbursementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ClosureDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                
                // Credit assessment
                CreditScore = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                RiskGrade = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                RiskPremium = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                
                // Accrued interest (Money value object)
                AccruedInterestAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                AccruedInterestCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                
                // Total interest paid (Money value object)
                TotalInterestPaidAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                TotalInterestPaidCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                
                // Total fees paid (Money value object)
                TotalFeesPaidAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                TotalFeesPaidCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                
                LastInterestCalculationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                
                // Total amount paid (Money value object)
                TotalAmountPaidAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                TotalAmountPaidCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                
                LastPaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                DaysPastDue = table.Column<int>(type: "integer", nullable: false),
                
                // Past due amount (Money value object)
                PastDueAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                PastDueAmountCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                
                // GL Integration
                LoanGLCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                InterestReceivableGLCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                
                // Provisioning
                ProvisionRate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                ProvisionAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                ProvisionAmountCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                LastProvisionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                
                // Audit trail
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ApprovedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                DisbursedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                
                // JSON columns for complex data
                RepaymentSchedule = table.Column<string>(type: "jsonb", nullable: true),
                Collaterals = table.Column<string>(type: "jsonb", nullable: true),
                Guarantors = table.Column<string>(type: "jsonb", nullable: true),
                Conditions = table.Column<string>(type: "jsonb", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Loans", x => x.Id);
                table.ForeignKey(
                    name: "FK_Loans_Accounts_DisbursementAccountId",
                    column: x => x.DisbursementAccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Loans_Parties_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Parties",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Loans_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        // Create indexes for performance
        migrationBuilder.CreateIndex(
            name: "IX_Loans_LoanNumber",
            table: "Loans",
            column: "LoanNumber",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Loans_CustomerId",
            table: "Loans",
            column: "CustomerId");

        migrationBuilder.CreateIndex(
            name: "IX_Loans_ProductId",
            table: "Loans",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_Loans_DisbursementAccountId",
            table: "Loans",
            column: "DisbursementAccountId");

        migrationBuilder.CreateIndex(
            name: "IX_Loans_Status",
            table: "Loans",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_Loans_SubStatus",
            table: "Loans",
            column: "SubStatus");

        migrationBuilder.CreateIndex(
            name: "IX_Loans_RiskGrade",
            table: "Loans",
            column: "RiskGrade");

        migrationBuilder.CreateIndex(
            name: "IX_Loans_ApplicationDate",
            table: "Loans",
            column: "ApplicationDate");

        migrationBuilder.CreateIndex(
            name: "IX_Loans_DisbursementDate",
            table: "Loans",
            column: "DisbursementDate");

        migrationBuilder.CreateIndex(
            name: "IX_Loans_MaturityDate",
            table: "Loans",
            column: "MaturityDate");

        migrationBuilder.CreateIndex(
            name: "IX_Loans_DaysPastDue",
            table: "Loans",
            column: "DaysPastDue");

        // Create composite indexes for common queries
        migrationBuilder.CreateIndex(
            name: "IX_Loans_Status_SubStatus",
            table: "Loans",
            columns: new[] { "Status", "SubStatus" });

        migrationBuilder.CreateIndex(
            name: "IX_Loans_CustomerId_Status",
            table: "Loans",
            columns: new[] { "CustomerId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_Loans_ProductId_Status",
            table: "Loans",
            columns: new[] { "ProductId", "Status" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Loans");
    }
}
