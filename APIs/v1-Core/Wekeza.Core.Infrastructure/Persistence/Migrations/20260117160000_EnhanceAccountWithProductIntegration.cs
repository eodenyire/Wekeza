using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Wekeza.Core.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
[DbContext(typeof(ApplicationDbContext))]
[Migration("20260117160000_EnhanceAccountWithProductIntegration")]
public partial class EnhanceAccountWithProductIntegration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            @"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'ProductId') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""ProductId"" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'Status') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""Status"" integer NOT NULL DEFAULT 0;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'OpenedDate') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""OpenedDate"" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'ClosedDate') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""ClosedDate"" timestamp with time zone NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'OpenedBy') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""OpenedBy"" character varying(100) NOT NULL DEFAULT 'SYSTEM';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'ClosedBy') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""ClosedBy"" character varying(100) NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'InterestRate') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""InterestRate"" numeric(5,4) NOT NULL DEFAULT 0;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'LastInterestCalculationDate') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""LastInterestCalculationDate"" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'AccruedInterest_Amount') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""AccruedInterest_Amount"" numeric(18,2) NOT NULL DEFAULT 0;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'AccruedInterest_Currency_Code') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""AccruedInterest_Currency_Code"" character varying(3) NOT NULL DEFAULT 'KES';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'DailyTransactionLimit_Amount') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""DailyTransactionLimit_Amount"" numeric(18,2) NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'DailyTransactionLimit_Currency_Code') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""DailyTransactionLimit_Currency_Code"" character varying(3) NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'MonthlyTransactionLimit_Amount') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""MonthlyTransactionLimit_Amount"" numeric(18,2) NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'MonthlyTransactionLimit_Currency_Code') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""MonthlyTransactionLimit_Currency_Code"" character varying(3) NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'MinimumBalance_Amount') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""MinimumBalance_Amount"" numeric(18,2) NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'MinimumBalance_Currency_Code') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""MinimumBalance_Currency_Code"" character varying(3) NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'CustomerGLCode') THEN
        ALTER TABLE ""Accounts"" ADD COLUMN ""CustomerGLCode"" character varying(20) NOT NULL DEFAULT '';
    END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Accounts' AND column_name = 'IsFrozen') THEN
        ALTER TABLE ""Accounts"" DROP COLUMN ""IsFrozen"";
    END IF;
END $$;
");

        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS \"IX_Accounts_ProductId\" ON \"Accounts\" (\"ProductId\");");
        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS \"IX_Accounts_CustomerGLCode\" ON \"Accounts\" (\"CustomerGLCode\");");
        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS \"IX_Accounts_Status\" ON \"Accounts\" (\"Status\");");

        migrationBuilder.Sql(
            @"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Accounts_Products_ProductId')
             AND NOT EXISTS (
                     SELECT 1
                     FROM ""Accounts"" a
                     LEFT JOIN ""Products"" p ON p.""Id"" = a.""ProductId""
                     WHERE a.""ProductId"" IS NOT NULL
                         AND p.""Id"" IS NULL
             ) THEN
        ALTER TABLE ""Accounts""
            ADD CONSTRAINT ""FK_Accounts_Products_ProductId""
            FOREIGN KEY (""ProductId"") REFERENCES ""Products"" (""Id"") ON DELETE RESTRICT;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Accounts_GLAccounts_CustomerGLCode')
             AND NOT EXISTS (
                     SELECT 1
                     FROM ""Accounts"" a
                     LEFT JOIN ""GLAccounts"" g ON g.""GLCode"" = a.""CustomerGLCode""
                     WHERE a.""CustomerGLCode"" IS NOT NULL
                         AND g.""GLCode"" IS NULL
             ) THEN
        ALTER TABLE ""Accounts""
            ADD CONSTRAINT ""FK_Accounts_GLAccounts_CustomerGLCode""
            FOREIGN KEY (""CustomerGLCode"") REFERENCES ""GLAccounts"" (""GLCode"") ON DELETE RESTRICT;
    END IF;
END $$;
");
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