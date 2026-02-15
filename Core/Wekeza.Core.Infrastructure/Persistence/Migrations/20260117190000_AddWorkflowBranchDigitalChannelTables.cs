using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wekeza.Core.Infrastructure.Persistence.Migrations;

/// <summary>
/// Migration to add Workflow, Branch Operations, and Digital Channel tables for 200% completion
/// </summary>
public partial class AddWorkflowBranchDigitalChannelTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // ApprovalWorkflows table
        migrationBuilder.CreateTable(
            name: "ApprovalWorkflows",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkflowCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                WorkflowName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                WorkflowType = table.Column<int>(type: "integer", nullable: false),
                EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                CurrentLevel = table.Column<int>(type: "integer", nullable: false),
                MaxLevels = table.Column<int>(type: "integer", nullable: false),
                Priority = table.Column<int>(type: "integer", nullable: false),
                InitiatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                InitiatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                CompletedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                RejectionReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                BranchCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                RequiresMakerChecker = table.Column<bool>(type: "boolean", nullable: false),
                IsEscalated = table.Column<bool>(type: "boolean", nullable: false),
                EscalatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                EscalationReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApprovalWorkflows", x => x.Id);
            });

        // ApprovalSteps table
        migrationBuilder.CreateTable(
            name: "ApprovalSteps",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkflowId = table.Column<Guid>(type: "uuid", nullable: false),
                Level = table.Column<int>(type: "integer", nullable: false),
                ApproverRole = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                SpecificApprover = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                MinimumAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                MinimumAmountCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                MaximumAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                MaximumAmountCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ProcessedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Comments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApprovalSteps", x => x.Id);
                table.ForeignKey(
                    name: "FK_ApprovalSteps_ApprovalWorkflows_WorkflowId",
                    column: x => x.WorkflowId,
                    principalTable: "ApprovalWorkflows",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // WorkflowComments table
        migrationBuilder.CreateTable(
            name: "WorkflowComments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkflowId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                CommentType = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WorkflowComments", x => x.Id);
                table.ForeignKey(
                    name: "FK_WorkflowComments_ApprovalWorkflows_WorkflowId",
                    column: x => x.WorkflowId,
                    principalTable: "ApprovalWorkflows",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // WorkflowDocuments table
        migrationBuilder.CreateTable(
            name: "WorkflowDocuments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkflowId = table.Column<Guid>(type: "uuid", nullable: false),
                FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                UploadedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WorkflowDocuments", x => x.Id);
                table.ForeignKey(
                    name: "FK_WorkflowDocuments_ApprovalWorkflows_WorkflowId",
                    column: x => x.WorkflowId,
                    principalTable: "ApprovalWorkflows",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // Branches table
        migrationBuilder.CreateTable(
            name: "Branches",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                BranchCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                BranchName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                BranchType = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                OpeningDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ClosingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                TimeZone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                BusinessHoursStart = table.Column<TimeSpan>(type: "interval", nullable: false),
                BusinessHoursEnd = table.Column<TimeSpan>(type: "interval", nullable: false),
                ManagerId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                DeputyManagerId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                CashLimit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                CashLimitCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                TransactionLimit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                TransactionLimitCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                IsEODCompleted = table.Column<bool>(type: "boolean", nullable: false),
                LastEODDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                IsBODCompleted = table.Column<bool>(type: "boolean", nullable: false),
                LastBODDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Branches", x => x.Id);
            });

        // BranchVaults table
        migrationBuilder.CreateTable(
            name: "BranchVaults",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                VaultCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                VaultName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                VaultType = table.Column<int>(type: "integer", nullable: false),
                Capacity = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                CapacityCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                CurrentBalance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                CurrentBalanceCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                DailyLimit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                DailyLimitCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                DailyUsed = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                DailyUsedCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BranchVaults", x => x.Id);
                table.ForeignKey(
                    name: "FK_BranchVaults_Branches_BranchId",
                    column: x => x.BranchId,
                    principalTable: "Branches",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // BranchLimits table
        migrationBuilder.CreateTable(
            name: "BranchLimits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                LimitType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                LimitAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                LimitAmountCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BranchLimits", x => x.Id);
                table.ForeignKey(
                    name: "FK_BranchLimits_Branches_BranchId",
                    column: x => x.BranchId,
                    principalTable: "Branches",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // BranchPerformance table
        migrationBuilder.CreateTable(
            name: "BranchPerformance",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                PerformanceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                TransactionCount = table.Column<int>(type: "integer", nullable: false),
                TransactionVolume = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                TransactionVolumeCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                NewCustomers = table.Column<int>(type: "integer", nullable: false),
                NewAccounts = table.Column<int>(type: "integer", nullable: false),
                CalculatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                CalculatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BranchPerformance", x => x.Id);
                table.ForeignKey(
                    name: "FK_BranchPerformance_Branches_BranchId",
                    column: x => x.BranchId,
                    principalTable: "Branches",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // DigitalChannels table
        migrationBuilder.CreateTable(
            name: "DigitalChannels",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ChannelCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                ChannelName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                ChannelType = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                BaseUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                ApiVersion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                IsSecure = table.Column<bool>(type: "boolean", nullable: false),
                RequiresAuthentication = table.Column<bool>(type: "boolean", nullable: false),
                SupportsMFA = table.Column<bool>(type: "boolean", nullable: false),
                MaxConcurrentSessions = table.Column<int>(type: "integer", nullable: false),
                SessionTimeout = table.Column<TimeSpan>(type: "interval", nullable: false),
                DailyTransactionLimit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                DailyTransactionLimitCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                SingleTransactionLimit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                SingleTransactionLimitCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                MaxDailyTransactions = table.Column<int>(type: "integer", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DigitalChannels", x => x.Id);
            });

        // TaskAssignments table
        migrationBuilder.CreateTable(
            name: "TaskAssignments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TaskCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                TaskType = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                Priority = table.Column<int>(type: "integer", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                AssignedTo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                AssignedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CompletedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                CompletionNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                CancellationReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                BranchCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                RelatedEntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                RelatedEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                EstimatedHours = table.Column<int>(type: "integer", nullable: false),
                ActualHours = table.Column<int>(type: "integer", nullable: true),
                IsEscalated = table.Column<bool>(type: "boolean", nullable: false),
                EscalatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                EscalationReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TaskAssignments", x => x.Id);
            });

        // Create indexes
        migrationBuilder.CreateIndex(
            name: "IX_ApprovalWorkflows_WorkflowCode",
            table: "ApprovalWorkflows",
            column: "WorkflowCode",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ApprovalWorkflows_EntityType_EntityId",
            table: "ApprovalWorkflows",
            columns: new[] { "EntityType", "EntityId" });

        migrationBuilder.CreateIndex(
            name: "IX_ApprovalWorkflows_Status",
            table: "ApprovalWorkflows",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_ApprovalWorkflows_BranchCode",
            table: "ApprovalWorkflows",
            column: "BranchCode");

        migrationBuilder.CreateIndex(
            name: "IX_ApprovalWorkflows_InitiatedBy",
            table: "ApprovalWorkflows",
            column: "InitiatedBy");

        migrationBuilder.CreateIndex(
            name: "IX_ApprovalWorkflows_DueDate",
            table: "ApprovalWorkflows",
            column: "DueDate");

        migrationBuilder.CreateIndex(
            name: "IX_Branches_BranchCode",
            table: "Branches",
            column: "BranchCode",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Branches_Status",
            table: "Branches",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_Branches_BranchType",
            table: "Branches",
            column: "BranchType");

        migrationBuilder.CreateIndex(
            name: "IX_Branches_ManagerId",
            table: "Branches",
            column: "ManagerId");

        migrationBuilder.CreateIndex(
            name: "IX_DigitalChannels_ChannelCode",
            table: "DigitalChannels",
            column: "ChannelCode",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_DigitalChannels_ChannelType",
            table: "DigitalChannels",
            column: "ChannelType");

        migrationBuilder.CreateIndex(
            name: "IX_DigitalChannels_Status",
            table: "DigitalChannels",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_TaskAssignments_TaskCode",
            table: "TaskAssignments",
            column: "TaskCode",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_TaskAssignments_Status",
            table: "TaskAssignments",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_TaskAssignments_Priority",
            table: "TaskAssignments",
            column: "Priority");

        migrationBuilder.CreateIndex(
            name: "IX_TaskAssignments_AssignedTo",
            table: "TaskAssignments",
            column: "AssignedTo");

        migrationBuilder.CreateIndex(
            name: "IX_TaskAssignments_DueDate",
            table: "TaskAssignments",
            column: "DueDate");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ApprovalSteps");
        migrationBuilder.DropTable(name: "WorkflowComments");
        migrationBuilder.DropTable(name: "WorkflowDocuments");
        migrationBuilder.DropTable(name: "ApprovalWorkflows");
        migrationBuilder.DropTable(name: "BranchVaults");
        migrationBuilder.DropTable(name: "BranchLimits");
        migrationBuilder.DropTable(name: "BranchPerformance");
        migrationBuilder.DropTable(name: "Branches");
        migrationBuilder.DropTable(name: "DigitalChannels");
        migrationBuilder.DropTable(name: "TaskAssignments");
    }
}