using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniEvents.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMediaHandlingAndRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageFiles");

            migrationBuilder.CreateTable(
                name: "MediaFiles",
                columns: table => new
                {
                    MediaFileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    MediaType = table.Column<int>(type: "int", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AttendeeUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFiles", x => x.MediaFileId);
                    table.ForeignKey(
                        name: "FK_MediaFiles_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaFiles_users_AttendeeUserId",
                        column: x => x.AttendeeUserId,
                        principalTable: "users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_MediaFiles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_AttendeeUserId",
                table: "MediaFiles",
                column: "AttendeeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_EventId",
                table: "MediaFiles",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_UserId",
                table: "MediaFiles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaFiles");

            migrationBuilder.CreateTable(
                name: "ImageFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageType = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageFiles", x => x.Id);
                });
        }
    }
}
