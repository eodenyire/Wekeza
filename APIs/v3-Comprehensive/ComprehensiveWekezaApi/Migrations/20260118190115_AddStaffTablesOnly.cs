using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComprehensiveWekezaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffTablesOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only create Staff tables if they don't exist
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""Staff"" (
                    ""Id"" uuid NOT NULL,
                    ""EmployeeId"" character varying(50) NOT NULL,
                    ""FirstName"" character varying(100) NOT NULL,
                    ""LastName"" character varying(100) NOT NULL,
                    ""Email"" character varying(200) NOT NULL,
                    ""Phone"" character varying(20) NOT NULL,
                    ""Role"" character varying(50) NOT NULL,
                    ""BranchId"" integer NOT NULL,
                    ""BranchName"" character varying(200) NOT NULL,
                    ""DepartmentId"" integer NOT NULL,
                    ""DepartmentName"" character varying(200) NOT NULL,
                    ""Status"" character varying(20) NOT NULL,
                    ""CreatedAt"" timestamp with time zone NOT NULL,
                    ""CreatedBy"" character varying(100),
                    ""UpdatedAt"" timestamp with time zone,
                    ""UpdatedBy"" character varying(100),
                    ""LastLogin"" timestamp with time zone,
                    CONSTRAINT ""PK_Staff"" PRIMARY KEY (""Id"")
                );
                
                CREATE TABLE IF NOT EXISTS ""StaffLogins"" (
                    ""Id"" uuid NOT NULL,
                    ""StaffId"" uuid NOT NULL,
                    ""LoginTime"" timestamp with time zone NOT NULL,
                    ""IpAddress"" character varying(50),
                    ""UserAgent"" text,
                    CONSTRAINT ""PK_StaffLogins"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_StaffLogins_Staff_StaffId"" FOREIGN KEY (""StaffId"") REFERENCES ""Staff"" (""Id"") ON DELETE CASCADE
                );
                
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Staff_Email"" ON ""Staff"" (""Email"");
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Staff_EmployeeId"" ON ""Staff"" (""EmployeeId"");
                CREATE INDEX IF NOT EXISTS ""IX_StaffLogins_StaffId"" ON ""StaffLogins"" (""StaffId"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS ""StaffLogins"";
                DROP TABLE IF EXISTS ""Staff"";
            ");
        }
    }
}