using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wekeza.Core.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class EnhanceAccountWithProductIntegration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Add new columns to Accounts table for Product Factory integration
        migrationBuilder.AddColumn<Guid>(
            name: "ProductId",
            table: "Accounts",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<int>(
            name: "Status",
            table: "Accounts",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "OpenedDate",
            table: "Accounts",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "CURRENT_TIMESTAMP");

        migrationBuilder.AddColumn<DateTime>(
            name: "ClosedDate",
            table: "Accounts",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "OpenedBy",
            table: "Accounts",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "SYSTEM");

        migrationBuilder.AddColumn<string>(
            name: "ClosedBy",
            table: "Accounts",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "InterestRate",
            table: "Accounts",
            type: "numeric(5,4)",
            precision: 5,
            scale: 4,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<DateTime>(
            name: "LastInterestCalculationDate",
            table: "Accounts",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "CURRENT_TIMESTAMP");

        migrationBuilder.AddColumn<decimal>(
            name: "AccruedInterest_Amount",
            table: "Accounts",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<string>(
            name: "AccruedInterest_Currency_Code",
            table: "Accounts",
            type: "character varying(3)",
            maxLength: 3,
            nullable: false,
            defaultValue: "KES");

        migrationBuilder.AddColumn<decimal>(
            name: "DailyTransactionLimit_Amount",
            table: "Accounts",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DailyTransactionLimit_Currency_Code",
            table: "Accounts",
            type: "character varying(3)",
            maxLength: 3,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "MonthlyTransactionLimit_Amount",
            table: "Accounts",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "MonthlyTransactionLimit_Currency_Code",
            table: "Accounts",
            type: "character varying(3)",
            maxLength: 3,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "MinimumBalance_Amount",
            table: "Accounts",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "MinimumBalance_Currency_Code",
            table: "Accounts",
            type: "character varying(3)",
            maxLength: 3,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "CustomerGLCode",
            table: "Accounts",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");

        // Remove the old IsFrozen column as it's replaced by Status
        migrationBuilder.DropColumn(
            name: "IsFrozen",
            table: "Accounts");

        // Create foreign key relationship with Products
        migrationBuilder.CreateIndex(
            name: "IX_Accounts_ProductId",
            table: "Accounts",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_Accounts_CustomerGLCode",
            table: "Accounts",
            column: "CustomerGLCode");

        migrationBuilder.CreateIndex(
            name: "IX_Accounts_Status",
            table: "Accounts",
            column: "Status");

        migrationBuilder.AddForeignKey(
            name: "FK_Accounts_Products_ProductId",
            table: "Accounts",
            column: "ProductId",
            principalTable: "Products",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Accounts_GLAccounts_CustomerGLCode",
            table: "Accounts",
            column: "CustomerGLCode",
            principalTable: "GLAccounts",
            principalColumn: "GLCode",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Drop foreign keys
        migrationBuilder.DropForeignKey(
            name: "FK_Accounts_Products_ProductId",
            table: "Accounts");

        migrationBuilder.DropForeignKey(
            name: "FK_Accounts_GLAccounts_CustomerGLCode",
            table: "Accounts");

        // Drop indexes
        migrationBuilder.DropIndex(
            name: "IX_Accounts_ProductId",
            table: "Accounts");

        migrationBuilder.DropIndex(
            name: "IX_Accounts_CustomerGLCode",
            table: "Accounts");

        migrationBuilder.DropIndex(
            name: "IX_Accounts_Status",
            table: "Accounts");

        // Remove new columns
        migrationBuilder.DropColumn(name: "ProductId", table: "Accounts");
        migrationBuilder.DropColumn(name: "Status", table: "Accounts");
        migrationBuilder.DropColumn(name: "OpenedDate", table: "Accounts");
        migrationBuilder.DropColumn(name: "ClosedDate", table: "Accounts");
        migrationBuilder.DropColumn(name: "OpenedBy", table: "Accounts");
        migrationBuilder.DropColumn(name: "ClosedBy", table: "Accounts");
        migrationBuilder.DropColumn(name: "InterestRate", table: "Accounts");
        migrationBuilder.DropColumn(name: "LastInterestCalculationDate", table: "Accounts");
        migrationBuilder.DropColumn(name: "AccruedInterest_Amount", table: "Accounts");
        migrationBuilder.DropColumn(name: "AccruedInterest_Currency_Code", table: "Accounts");
        migrationBuilder.DropColumn(name: "DailyTransactionLimit_Amount", table: "Accounts");
        migrationBuilder.DropColumn(name: "DailyTransactionLimit_Currency_Code", table: "Accounts");
        migrationBuilder.DropColumn(name: "MonthlyTransactionLimit_Amount", table: "Accounts");
        migrationBuilder.DropColumn(name: "MonthlyTransactionLimit_Currency_Code", table: "Accounts");
        migrationBuilder.DropColumn(name: "MinimumBalance_Amount", table: "Accounts");
        migrationBuilder.DropColumn(name: "MinimumBalance_Currency_Code", table: "Accounts");
        migrationBuilder.DropColumn(name: "CustomerGLCode", table: "Accounts");

        // Restore old IsFrozen column
        migrationBuilder.AddColumn<bool>(
            name: "IsFrozen",
            table: "Accounts",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }
}