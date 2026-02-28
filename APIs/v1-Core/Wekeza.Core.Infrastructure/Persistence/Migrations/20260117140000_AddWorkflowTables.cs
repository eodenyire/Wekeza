using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wekeza.Core.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // WorkflowInstances table
            migrationBuilder.CreateTable(
                name: "WorkflowInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WorkflowName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityReference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CurrentLevel = table.Column<int>(type: "integer", nullable: false),
                    RequiredLevels = table.Column<int>(type: "integer", nullable: false),
                    InitiatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InitiatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ApprovalSteps = table.Column<string>(type: "jsonb", nullable: false),
                    Comments = table.Column<string>(type: "jsonb", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsEscalated = table.Column<bool>(type: "boolean", nullable: false),
                    EscalatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EscalatedTo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RequestData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstances", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_WorkflowCode",
                table: "WorkflowInstances",
                column: "WorkflowCode");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_Status",
                table: "WorkflowInstances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_EntityType",
                table: "WorkflowInstances",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_EntityType_EntityId",
                table: "WorkflowInstances",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_InitiatedBy",
                table: "WorkflowInstances",
                column: "InitiatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_DueDate",
                table: "WorkflowInstances",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_IsEscalated",
                table: "WorkflowInstances",
                column: "IsEscalated");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_Status_DueDate",
                table: "WorkflowInstances",
                columns: new[] { "Status", "DueDate" });

            // ApprovalMatrices table
            migrationBuilder.CreateTable(
                name: "ApprovalMatrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MatrixCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MatrixName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Rules = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalMatrices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalMatrices_MatrixCode",
                table: "ApprovalMatrices",
                column: "MatrixCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalMatrices_EntityType",
                table: "ApprovalMatrices",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalMatrices_Status",
                table: "ApprovalMatrices",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "WorkflowInstances");
            migrationBuilder.DropTable(name: "ApprovalMatrices");
        }
    }
}
