using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wekeza.Core.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGLTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // GLAccounts table
            migrationBuilder.CreateTable(
                name: "GLAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GLCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    GLName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AccountType = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ParentGLCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    IsLeaf = table.Column<bool>(type: "boolean", nullable: false),
                    DebitBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreditBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    IsMultiCurrency = table.Column<bool>(type: "boolean", nullable: false),
                    AllowManualPosting = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresCostCenter = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresProfitCenter = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastPostingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLAccounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GLAccounts_GLCode",
                table: "GLAccounts",
                column: "GLCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GLAccounts_AccountType",
                table: "GLAccounts",
                column: "AccountType");

            migrationBuilder.CreateIndex(
                name: "IX_GLAccounts_Category",
                table: "GLAccounts",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_GLAccounts_Status",
                table: "GLAccounts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_GLAccounts_ParentGLCode",
                table: "GLAccounts",
                column: "ParentGLCode");

            migrationBuilder.CreateIndex(
                name: "IX_GLAccounts_IsLeaf",
                table: "GLAccounts",
                column: "IsLeaf");

            migrationBuilder.CreateIndex(
                name: "IX_GLAccounts_AccountType_Status",
                table: "GLAccounts",
                columns: new[] { "AccountType", "Status" });

            // JournalEntries table
            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JournalNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PostingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceReference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Lines = table.Column<string>(type: "jsonb", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PostedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PostedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReversedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReversedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReversalJournalId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_JournalNumber",
                table: "JournalEntries",
                column: "JournalNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_PostingDate",
                table: "JournalEntries",
                column: "PostingDate");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_ValueDate",
                table: "JournalEntries",
                column: "ValueDate");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_Status",
                table: "JournalEntries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_SourceType_SourceId",
                table: "JournalEntries",
                columns: new[] { "SourceType", "SourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_PostingDate_Status",
                table: "JournalEntries",
                columns: new[] { "PostingDate", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "GLAccounts");
            migrationBuilder.DropTable(name: "JournalEntries");
        }
    }
}
