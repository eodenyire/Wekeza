using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Wekeza.MVP4._0.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchManagerTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("21e4193f-9b28-4f36-bf8e-a2ed92fbd7a7"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("49beb167-ab61-414a-9876-0e7497e1c476"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("54102691-16fd-4760-afdc-c5fc079d03ea"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("58eb5696-1895-4f7d-a029-66e64f086546"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("648471ac-7855-4359-93b8-76e343028b3e"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("86333acd-6b6e-414d-b6fe-6e6b7729c0dd"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("875825f3-4698-4058-85c9-60f1aab8de94"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1f46370-97af-4b3d-8e57-4c01b5b069ab"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a94edf1f-690a-4f6b-84f6-148ceb9e0d5e"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cde1a763-93a2-471f-94b4-d29aba96a2bf"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d904aebf-52ce-45cc-9d12-82cdc39382ed"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f5d7f2e3-8f07-4ba3-983c-d7a87c1ca29e"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("fb27197c-3c22-44ab-9bee-62c0ee7324f4"));

            migrationBuilder.CreateTable(
                name: "Authorizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TellerId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerAccount = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AuthorizedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AuthorizedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BranchReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeneratedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VaultCash = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TellerCash = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ATMCash = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiskAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlertType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AccountNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RiskLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AssignedTo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Resolution = table.Column<string>(type: "text", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskAlerts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "CreatedAt", "Email", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { new Guid("0e762cd9-342c-4619-839a-d7afde1aad37"), null, new DateTime(2026, 1, 22, 20, 57, 14, 440, DateTimeKind.Utc).AddTicks(9201), "branchmanager1@wekeza.com", "Michael Manager", true, null, "$2a$11$6rtF8dCFIAHSC5eQ0qjoxuZYgmXtL4EClV0COWH0VGu4RGC9zPDSq", "BranchManager", "branchmanager1" },
                    { new Guid("0f1951ce-6e32-4d14-be17-f19c9d7003f8"), null, new DateTime(2026, 1, 22, 20, 57, 13, 81, DateTimeKind.Utc).AddTicks(3589), "admin@wekeza.com", "System Administrator", true, null, "$2a$11$QmXFdmPc0JjB.ejq2WuTYOJXFxkSuktNsv0XRytSt.XWY0O18mL3W", "Administrator", "admin" },
                    { new Guid("32668084-5dfd-4e94-9421-0e78a226e2ca"), null, new DateTime(2026, 1, 22, 20, 57, 17, 920, DateTimeKind.Utc).AddTicks(825), "loanofficer1@wekeza.com", "Patricia Loan", true, null, "$2a$11$OCWLlHvpwGu3Hhp4eO72uei8aPGnkaLfdV3598Dg8MJft2apavCo6", "LoanOfficer", "loanofficer1" },
                    { new Guid("432fec2e-84c8-4b17-b53c-08807783892d"), null, new DateTime(2026, 1, 22, 20, 57, 14, 75, DateTimeKind.Utc).AddTicks(3617), "supervisor1@wekeza.com", "Jane Supervisor", true, null, "$2a$11$O2QS42jZzJgjpjs9eDtoKubXWHXv9lnltPJDQwnWMGXS6yO1ExtnC", "Supervisor", "supervisor1" },
                    { new Guid("5628d9b4-f7a5-4084-9105-75ca2a89b4ff"), null, new DateTime(2026, 1, 22, 20, 57, 17, 630, DateTimeKind.Utc).AddTicks(4202), "risk1@wekeza.com", "James Risk", true, null, "$2a$11$gIHjToCnqQbIiD3ow5NH9.xMrKYybH9Al5TtagxCwWUzzSlFFoT7m", "RiskOfficer", "risk1" },
                    { new Guid("ad02b494-4ec5-4e14-8c82-43d2b1596da0"), null, new DateTime(2026, 1, 22, 20, 57, 18, 576, DateTimeKind.Utc).AddTicks(8681), "itadmin1@wekeza.com", "Thomas IT", true, null, "$2a$11$ba.UQNvSs/8RquAdPJqPm.TAWilnLmksUTlC/4F91XuxC8sXeN4fu", "ITAdministrator", "itadmin1" },
                    { new Guid("b126528b-2dcf-49bc-8bc8-ccffac650a12"), null, new DateTime(2026, 1, 22, 20, 57, 18, 295, DateTimeKind.Utc).AddTicks(234), "auditor1@wekeza.com", "William Auditor", true, null, "$2a$11$.KG2n78NWl5QHDqPhYO2Fuer59Nyx52jxdMzmOa4wsOan1iqk05fe", "Auditor", "auditor1" },
                    { new Guid("c21c2f13-4012-4a0b-a7e6-0f7cf919bcdc"), null, new DateTime(2026, 1, 22, 20, 57, 15, 501, DateTimeKind.Utc).AddTicks(7324), "customercare1@wekeza.com", "Emily Care", true, null, "$2a$11$RG8TDprLFn7gYkk8.OjKw.7v/c1LrHbXjgKkEhBpRZtiql58tElMi", "CustomerCareOfficer", "customercare1" },
                    { new Guid("c6a1a5ab-756a-4f1a-8070-9182210dff84"), null, new DateTime(2026, 1, 22, 20, 57, 13, 626, DateTimeKind.Utc).AddTicks(940), "teller1@wekeza.com", "John Teller", true, null, "$2a$11$9kDsE1OOlsNE.aSBBar1ceHq6N/Hl0qltrwNkEF6RM5rQw67LOjc2", "Teller", "teller1" },
                    { new Guid("c8698b89-4851-4309-b425-e16382bd2114"), null, new DateTime(2026, 1, 22, 20, 57, 15, 177, DateTimeKind.Utc).AddTicks(6305), "backoffice1@wekeza.com", "David BackOffice", true, null, "$2a$11$PG1yyMKSkHgNNBvZ.5cR4OWETI/4TEaMYyQOndzjeU3Ed7zPo6ls.", "BackOfficeStaff", "backoffice1" },
                    { new Guid("db7ca284-5c19-4fb7-8697-315296830690"), null, new DateTime(2026, 1, 22, 20, 57, 15, 911, DateTimeKind.Utc).AddTicks(4191), "bancassurance1@wekeza.com", "Robert Insurance", true, null, "$2a$11$3zGqZgvHF.NwKPDjYf/y5OLMKoo9rSh8ueaVyL.uSatwnR7wrrsCu", "BancassuranceAgent", "bancassurance1" },
                    { new Guid("ebcab2f2-1e93-4c0b-9e21-70158c6ccc26"), null, new DateTime(2026, 1, 22, 20, 57, 14, 910, DateTimeKind.Utc).AddTicks(4565), "cashofficer1@wekeza.com", "Sarah Cash", true, null, "$2a$11$PfR0gfKD5wAbUfX/EH0kI.FWbTyunxMeYAaT8TXl6EFlvPRuTYSI6", "CashOfficer", "cashofficer1" },
                    { new Guid("f536035d-a603-4f2f-8140-f64cf7599396"), null, new DateTime(2026, 1, 22, 20, 57, 16, 589, DateTimeKind.Utc).AddTicks(8622), "compliance1@wekeza.com", "Linda Compliance", true, null, "$2a$11$TCiKdHuwRNpW/d3FikLN/edi2mtXjcEV4NB0.mmuvICozFBPJKYSS", "ComplianceOfficer", "compliance1" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authorizations");

            migrationBuilder.DropTable(
                name: "BranchReports");

            migrationBuilder.DropTable(
                name: "CashPositions");

            migrationBuilder.DropTable(
                name: "RiskAlerts");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0e762cd9-342c-4619-839a-d7afde1aad37"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0f1951ce-6e32-4d14-be17-f19c9d7003f8"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("32668084-5dfd-4e94-9421-0e78a226e2ca"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("432fec2e-84c8-4b17-b53c-08807783892d"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5628d9b4-f7a5-4084-9105-75ca2a89b4ff"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ad02b494-4ec5-4e14-8c82-43d2b1596da0"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b126528b-2dcf-49bc-8bc8-ccffac650a12"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c21c2f13-4012-4a0b-a7e6-0f7cf919bcdc"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c6a1a5ab-756a-4f1a-8070-9182210dff84"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c8698b89-4851-4309-b425-e16382bd2114"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("db7ca284-5c19-4fb7-8697-315296830690"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ebcab2f2-1e93-4c0b-9e21-70158c6ccc26"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f536035d-a603-4f2f-8140-f64cf7599396"));

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
        }
    }
}
