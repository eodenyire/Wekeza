using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Wekeza.MVP4._0.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "CreatedAt", "Email", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { new Guid("21e4193f-9b28-4f36-bf8e-a2ed92fbd7a7"), null, new DateTime(2026, 1, 22, 17, 13, 5, 201, DateTimeKind.Utc).AddTicks(6359), "customercare1@wekeza.com", "Emily Care", true, null, "$2a$11$IbAZFkYu7hSvLt/PVW8OTer3VUpPjWl0NWgTrcxwLFXoDQfevnN2y", "CustomerCareOfficer", "customercare1" },
                    { new Guid("49beb167-ab61-414a-9876-0e7497e1c476"), null, new DateTime(2026, 1, 22, 17, 13, 5, 7, DateTimeKind.Utc).AddTicks(3869), "backoffice1@wekeza.com", "David BackOffice", true, null, "$2a$11$RoVTtXDb850n4nEG2ynpduCEbrsMxLAFhLpePXHgOg36MlhoBa72C", "BackOfficeStaff", "backoffice1" },
                    { new Guid("54102691-16fd-4760-afdc-c5fc079d03ea"), null, new DateTime(2026, 1, 22, 17, 13, 5, 577, DateTimeKind.Utc).AddTicks(9753), "compliance1@wekeza.com", "Linda Compliance", true, null, "$2a$11$gvrjymwkyY8R7g7y3O2xUObFDYgWva3QyC8Q.tti2xQ1J3RwQzMUy", "ComplianceOfficer", "compliance1" },
                    { new Guid("58eb5696-1895-4f7d-a029-66e64f086546"), null, new DateTime(2026, 1, 22, 17, 13, 5, 382, DateTimeKind.Utc).AddTicks(5406), "bancassurance1@wekeza.com", "Robert Insurance", true, null, "$2a$11$/CArNoz2LiNQPzSWpgv.e.P8ZfkZKAoZ4FDjvLrf/o./DEo9JSgya", "BancassuranceAgent", "bancassurance1" },
                    { new Guid("648471ac-7855-4359-93b8-76e343028b3e"), null, new DateTime(2026, 1, 22, 17, 13, 4, 258, DateTimeKind.Utc).AddTicks(423), "teller1@wekeza.com", "John Teller", true, null, "$2a$11$/EHeyE23ziD5D1jC0jFaouY7EUCaARPtd54ip8eaMp1X1RZ/5vPKG", "Teller", "teller1" },
                    { new Guid("86333acd-6b6e-414d-b6fe-6e6b7729c0dd"), null, new DateTime(2026, 1, 22, 17, 13, 4, 656, DateTimeKind.Utc).AddTicks(8587), "branchmanager1@wekeza.com", "Michael Manager", true, null, "$2a$11$G64bzyuDoV/5j.cI1YfYnOH/6N2TThgwldLMaKRrDPActXFbRz4sS", "BranchManager", "branchmanager1" },
                    { new Guid("875825f3-4698-4058-85c9-60f1aab8de94"), null, new DateTime(2026, 1, 22, 17, 13, 5, 773, DateTimeKind.Utc).AddTicks(2273), "risk1@wekeza.com", "James Risk", true, null, "$2a$11$rjfz3nfY82p05g.ebLGwi.BxsakwBb4osmS3X9D5aplcPHLMmCGKm", "RiskOfficer", "risk1" },
                    { new Guid("a1f46370-97af-4b3d-8e57-4c01b5b069ab"), null, new DateTime(2026, 1, 22, 17, 13, 4, 480, DateTimeKind.Utc).AddTicks(1047), "supervisor1@wekeza.com", "Jane Supervisor", true, null, "$2a$11$qfXv3hFWLfUyVl4HnBpyTecFDc9JAYANS6Xrk.EZ0r7aHFKcoZRmy", "Supervisor", "supervisor1" },
                    { new Guid("a94edf1f-690a-4f6b-84f6-148ceb9e0d5e"), null, new DateTime(2026, 1, 22, 17, 13, 6, 125, DateTimeKind.Utc).AddTicks(5393), "auditor1@wekeza.com", "William Auditor", true, null, "$2a$11$W7MlVOo/He/RmC09/AGBCeNSveLNf270g6GaIYJ86/vieFLWSv5Te", "Auditor", "auditor1" },
                    { new Guid("cde1a763-93a2-471f-94b4-d29aba96a2bf"), null, new DateTime(2026, 1, 22, 17, 13, 4, 31, DateTimeKind.Utc).AddTicks(7232), "admin@wekeza.com", "System Administrator", true, null, "$2a$11$Wmv9267o7ZiiIxH8aPsKG.XTu/jttPheqj57XTZbrZRESoY398QFK", "Administrator", "admin" },
                    { new Guid("d904aebf-52ce-45cc-9d12-82cdc39382ed"), null, new DateTime(2026, 1, 22, 17, 13, 6, 303, DateTimeKind.Utc).AddTicks(2635), "itadmin1@wekeza.com", "Thomas IT", true, null, "$2a$11$Zca83ZFoTiKG2aiDue5rW.k1ZVrmNlQP0ft7qez7rFgAtZYbXf7am", "ITAdministrator", "itadmin1" },
                    { new Guid("f5d7f2e3-8f07-4ba3-983c-d7a87c1ca29e"), null, new DateTime(2026, 1, 22, 17, 13, 5, 953, DateTimeKind.Utc).AddTicks(6118), "loanofficer1@wekeza.com", "Patricia Loan", true, null, "$2a$11$MJ9EoY0nb7wN/oCU/bsjDeiTppS849j3kZlR55PvsGsBfQ/fBqIJO", "LoanOfficer", "loanofficer1" },
                    { new Guid("fb27197c-3c22-44ab-9bee-62c0ee7324f4"), null, new DateTime(2026, 1, 22, 17, 13, 4, 831, DateTimeKind.Utc).AddTicks(5444), "cashofficer1@wekeza.com", "Sarah Cash", true, null, "$2a$11$C6NBz3Wbhw3.SN1dpqRiUONbNdFQi4TA5DA034bwwPS9SnFlDxk6C", "CashOfficer", "cashofficer1" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
