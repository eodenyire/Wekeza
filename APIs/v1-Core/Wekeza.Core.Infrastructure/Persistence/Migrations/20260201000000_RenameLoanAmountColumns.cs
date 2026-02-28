using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wekeza.Core.Infrastructure.Persistence.Migrations;

public partial class RenameLoanAmountColumns : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PastDueAmountAmount",
            table: "Loans",
            newName: "PastDueAmount");

        migrationBuilder.RenameColumn(
            name: "ProvisionAmountAmount",
            table: "Loans",
            newName: "ProvisionAmount");
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
