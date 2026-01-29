using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wekeza.Nexus.Infrastructure.Migrations;

/// <summary>
/// Initial migration for Wekeza Nexus fraud detection tables
/// Creates fraud_evaluations table with all required columns and indexes
/// </summary>
public partial class InitialNexusMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "fraud_evaluations",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                transaction_context_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                transaction_reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                fraud_score = table.Column<int>(type: "integer", nullable: false),
                decision = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                risk_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                primary_reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                contributing_reasons = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                explanation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                confidence = table.Column<double>(type: "double precision", nullable: false),
                evaluated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                processing_time_ms = table.Column<long>(type: "bigint", nullable: false),
                model_version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                was_allowed = table.Column<bool>(type: "boolean", nullable: false),
                requires_review = table.Column<bool>(type: "boolean", nullable: false),
                analyst_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                was_actual_fraud = table.Column<bool>(type: "boolean", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_fraud_evaluations", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_fraud_evaluations_evaluated_at",
            table: "fraud_evaluations",
            column: "evaluated_at");

        migrationBuilder.CreateIndex(
            name: "ix_fraud_evaluations_requires_review",
            table: "fraud_evaluations",
            column: "requires_review");

        migrationBuilder.CreateIndex(
            name: "ix_fraud_evaluations_transaction_reference",
            table: "fraud_evaluations",
            column: "transaction_reference");

        migrationBuilder.CreateIndex(
            name: "ix_fraud_evaluations_user_id",
            table: "fraud_evaluations",
            column: "user_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "fraud_evaluations");
    }
}
