using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniEvents.Migrations
{
    /// <inheritdoc />
    public partial class AddEventRatingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventRatings",
                columns: table => new
                {
                    EventRatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RatingValue = table.Column<int>(type: "int", nullable: false),
                    DateRated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    AttendeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRatings", x => x.EventRatingId);
                    table.ForeignKey(
                        name: "FK_EventRatings_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventRatings_users_AttendeeId",
                        column: x => x.AttendeeId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventRatings_AttendeeId",
                table: "EventRatings",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRatings_EventId",
                table: "EventRatings",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventRatings");
        }
    }
}
