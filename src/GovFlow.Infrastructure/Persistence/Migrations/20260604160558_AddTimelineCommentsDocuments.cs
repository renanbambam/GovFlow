using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GovFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimelineCommentsDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "process_comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_process_comments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "process_documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    UploadedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_process_documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "process_timeline_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    EventType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    StepId = table.Column<Guid>(type: "uuid", nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_process_timeline_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_process_timeline_entries_process_instances_ProcessInstanceId",
                        column: x => x.ProcessInstanceId,
                        principalTable: "process_instances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_process_comments_ProcessInstanceId",
                table: "process_comments",
                column: "ProcessInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_process_documents_ProcessInstanceId",
                table: "process_documents",
                column: "ProcessInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_process_timeline_entries_ProcessInstanceId_Sequence",
                table: "process_timeline_entries",
                columns: new[] { "ProcessInstanceId", "Sequence" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "process_comments");

            migrationBuilder.DropTable(
                name: "process_documents");

            migrationBuilder.DropTable(
                name: "process_timeline_entries");
        }
    }
}
