using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wekeza.Core.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddTradeFinanceTables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create LetterOfCredits table
        migrationBuilder.CreateTable(
            name: "LetterOfCredits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                LCNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                ApplicantId = table.Column<Guid>(type: "uuid", nullable: false),
                BeneficiaryId = table.Column<Guid>(type: "uuid", nullable: false),
                IssuingBankId = table.Column<Guid>(type: "uuid", nullable: false),
                AdvisingBankId = table.Column<Guid>(type: "uuid", nullable: true),
                Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LastShipmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Terms = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                GoodsDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                IsTransferable = table.Column<bool>(type: "boolean", nullable: false),
                IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LetterOfCredits", x => x.Id);
            });

        // Create BankGuarantees table
        migrationBuilder.CreateTable(
            name: "BankGuarantees",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                BGNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                PrincipalId = table.Column<Guid>(type: "uuid", nullable: false),
                BeneficiaryId = table.Column<Guid>(type: "uuid", nullable: false),
                IssuingBankId = table.Column<Guid>(type: "uuid", nullable: false),
                CounterGuaranteeId = table.Column<Guid>(type: "uuid", nullable: true),
                Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                ClaimedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                ClaimedCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Terms = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                Purpose = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                IsRevocable = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BankGuarantees", x => x.Id);
            });

        // Create DocumentaryCollections table
        migrationBuilder.CreateTable(
            name: "DocumentaryCollections",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CollectionNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                DrawerId = table.Column<Guid>(type: "uuid", nullable: false),
                DraweeId = table.Column<Guid>(type: "uuid", nullable: false),
                RemittingBankId = table.Column<Guid>(type: "uuid", nullable: false),
                CollectingBankId = table.Column<Guid>(type: "uuid", nullable: false),
                PresentingBankId = table.Column<Guid>(type: "uuid", nullable: true),
                Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                CollectionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                PresentationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                AcceptanceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                MaturityDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Terms = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                Instructions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                ProtestRequired = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DocumentaryCollections", x => x.Id);
            });

        // Create LCAmendments table
        migrationBuilder.CreateTable(
            name: "LCAmendments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                LetterOfCreditId = table.Column<Guid>(type: "uuid", nullable: false),
                AmendmentNumber = table.Column<int>(type: "integer", nullable: false),
                AmendmentDetails = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                PreviousAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                PreviousCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                NewAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                NewCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                PreviousExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                NewExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                AmendmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LCAmendments", x => x.Id);
                table.ForeignKey(
                    name: "FK_LCAmendments_LetterOfCredits_LetterOfCreditId",
                    column: x => x.LetterOfCreditId,
                    principalTable: "LetterOfCredits",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // Create BGAmendments table
        migrationBuilder.CreateTable(
            name: "BGAmendments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                BankGuaranteeId = table.Column<Guid>(type: "uuid", nullable: false),
                AmendmentNumber = table.Column<int>(type: "integer", nullable: false),
                AmendmentDetails = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                PreviousAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                PreviousCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                NewAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                NewCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                PreviousExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                NewExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                AmendmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BGAmendments", x => x.Id);
                table.ForeignKey(
                    name: "FK_BGAmendments_BankGuarantees_BankGuaranteeId",
                    column: x => x.BankGuaranteeId,
                    principalTable: "BankGuarantees",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // Create BGClaims table
        migrationBuilder.CreateTable(
            name: "BGClaims",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                BankGuaranteeId = table.Column<Guid>(type: "uuid", nullable: false),
                ClaimAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                ClaimCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                ClaimReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                ClaimDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ProcessedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                ProcessingNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BGClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_BGClaims_BankGuarantees_BankGuaranteeId",
                    column: x => x.BankGuaranteeId,
                    principalTable: "BankGuarantees",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // Create TradeDocuments table (shared by LC and Collections)
        migrationBuilder.CreateTable(
            name: "TradeDocuments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                DocumentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                DocumentNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                TradeTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                TradeTransactionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "LC"),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                UploadedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Comments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TradeDocuments", x => x.Id);
            });

        // Create CollectionEvents table
        migrationBuilder.CreateTable(
            name: "CollectionEvents",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CollectionId = table.Column<Guid>(type: "uuid", nullable: false),
                EventType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                EventDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CollectionEvents", x => x.Id);
                table.ForeignKey(
                    name: "FK_CollectionEvents_DocumentaryCollections_CollectionId",
                    column: x => x.CollectionId,
                    principalTable: "DocumentaryCollections",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // Create BGClaimDocuments table
        migrationBuilder.CreateTable(
            name: "BGClaimDocuments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                DocumentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                DocumentNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                BGClaimId = table.Column<Guid>(type: "uuid", nullable: false),
                TradeTransactionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "BG_CLAIM"),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                UploadedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Comments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BGClaimDocuments", x => x.Id);
                table.ForeignKey(
                    name: "FK_BGClaimDocuments_BGClaims_BGClaimId",
                    column: x => x.BGClaimId,
                    principalTable: "BGClaims",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // Create indexes for performance
        migrationBuilder.CreateIndex(
            name: "IX_LetterOfCredits_LCNumber",
            table: "LetterOfCredits",
            column: "LCNumber",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_LetterOfCredits_ApplicantId",
            table: "LetterOfCredits",
            column: "ApplicantId");

        migrationBuilder.CreateIndex(
            name: "IX_LetterOfCredits_BeneficiaryId",
            table: "LetterOfCredits",
            column: "BeneficiaryId");

        migrationBuilder.CreateIndex(
            name: "IX_LetterOfCredits_IssuingBankId",
            table: "LetterOfCredits",
            column: "IssuingBankId");

        migrationBuilder.CreateIndex(
            name: "IX_LetterOfCredits_Status",
            table: "LetterOfCredits",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_LetterOfCredits_ExpiryDate",
            table: "LetterOfCredits",
            column: "ExpiryDate");

        migrationBuilder.CreateIndex(
            name: "IX_LetterOfCredits_IssueDate",
            table: "LetterOfCredits",
            column: "IssueDate");

        migrationBuilder.CreateIndex(
            name: "IX_BankGuarantees_BGNumber",
            table: "BankGuarantees",
            column: "BGNumber",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_BankGuarantees_PrincipalId",
            table: "BankGuarantees",
            column: "PrincipalId");

        migrationBuilder.CreateIndex(
            name: "IX_BankGuarantees_BeneficiaryId",
            table: "BankGuarantees",
            column: "BeneficiaryId");

        migrationBuilder.CreateIndex(
            name: "IX_BankGuarantees_IssuingBankId",
            table: "BankGuarantees",
            column: "IssuingBankId");

        migrationBuilder.CreateIndex(
            name: "IX_BankGuarantees_Status",
            table: "BankGuarantees",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_BankGuarantees_Type",
            table: "BankGuarantees",
            column: "Type");

        migrationBuilder.CreateIndex(
            name: "IX_BankGuarantees_ExpiryDate",
            table: "BankGuarantees",
            column: "ExpiryDate");

        migrationBuilder.CreateIndex(
            name: "IX_BankGuarantees_IssueDate",
            table: "BankGuarantees",
            column: "IssueDate");

        migrationBuilder.CreateIndex(
            name: "IX_DocumentaryCollections_CollectionNumber",
            table: "DocumentaryCollections",
            column: "CollectionNumber",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_DocumentaryCollections_DrawerId",
            table: "DocumentaryCollections",
            column: "DrawerId");

        migrationBuilder.CreateIndex(
            name: "IX_DocumentaryCollections_DraweeId",
            table: "DocumentaryCollections",
            column: "DraweeId");

        migrationBuilder.CreateIndex(
            name: "IX_DocumentaryCollections_Status",
            table: "DocumentaryCollections",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_TradeDocuments_TradeTransactionId",
            table: "TradeDocuments",
            column: "TradeTransactionId");

        migrationBuilder.CreateIndex(
            name: "IX_TradeDocuments_TradeTransactionType",
            table: "TradeDocuments",
            column: "TradeTransactionType");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "BGClaimDocuments");
        migrationBuilder.DropTable(name: "CollectionEvents");
        migrationBuilder.DropTable(name: "TradeDocuments");
        migrationBuilder.DropTable(name: "BGClaims");
        migrationBuilder.DropTable(name: "BGAmendments");
        migrationBuilder.DropTable(name: "LCAmendments");
        migrationBuilder.DropTable(name: "DocumentaryCollections");
        migrationBuilder.DropTable(name: "BankGuarantees");
        migrationBuilder.DropTable(name: "LetterOfCredits");
    }
}