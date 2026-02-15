using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Wekeza.MVP4._0.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchManagerModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
