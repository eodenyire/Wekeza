using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wekeza.Core.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPartyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartyNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PartyType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    MaritalStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RegistrationNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IncorporationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompanyType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Industry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PrimaryEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PrimaryPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SecondaryPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PreferredLanguage = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Addresses = table.Column<string>(type: "jsonb", nullable: false),
                    Identifications = table.Column<string>(type: "jsonb", nullable: false),
                    KYCStatus = table.Column<int>(type: "integer", nullable: false),
                    KYCCompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KYCExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RiskRating = table.Column<int>(type: "integer", nullable: false),
                    IsPEP = table.Column<bool>(type: "boolean", nullable: false),
                    IsSanctioned = table.Column<bool>(type: "boolean", nullable: false),
                    Relationships = table.Column<string>(type: "jsonb", nullable: false),
                    Segment = table.Column<int>(type: "integer", nullable: false),
                    SubSegment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OptInMarketing = table.Column<bool>(type: "boolean", nullable: false),
                    OptInSMS = table.Column<bool>(type: "boolean", nullable: false),
                    OptInEmail = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parties_PartyNumber",
                table: "Parties",
                column: "PartyNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parties_PrimaryEmail",
                table: "Parties",
                column: "PrimaryEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_PrimaryPhone",
                table: "Parties",
                column: "PrimaryPhone");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_RegistrationNumber",
                table: "Parties",
                column: "RegistrationNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_Status",
                table: "Parties",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_KYCStatus",
                table: "Parties",
                column: "KYCStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_RiskRating",
                table: "Parties",
                column: "RiskRating");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_Segment",
                table: "Parties",
                column: "Segment");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_PartyType",
                table: "Parties",
                column: "PartyType");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_IsPEP",
                table: "Parties",
                column: "IsPEP");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_IsSanctioned",
                table: "Parties",
                column: "IsSanctioned");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parties");
        }
    }
}
