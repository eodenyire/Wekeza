using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Wekeza.Core.Infrastructure.Persistence.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260201000000_RenameLoanAmountColumns")]
public partial class RenameLoanAmountColumns : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            @"
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Loans' AND column_name = 'PastDueAmountAmount')
       AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Loans' AND column_name = 'PastDueAmount') THEN
        ALTER TABLE ""Loans"" RENAME COLUMN ""PastDueAmountAmount"" TO ""PastDueAmount"";
    END IF;

    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Loans' AND column_name = 'ProvisionAmountAmount')
       AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Loans' AND column_name = 'ProvisionAmount') THEN
        ALTER TABLE ""Loans"" RENAME COLUMN ""ProvisionAmountAmount"" TO ""ProvisionAmount"";
    END IF;
END $$;
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PastDueAmount",
            table: "Loans",
            newName: "PastDueAmountAmount");

        migrationBuilder.RenameColumn(
            name: "ProvisionAmount",
            table: "Loans",
            newName: "ProvisionAmountAmount");
    }
}
