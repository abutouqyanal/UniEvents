using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniEvents.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoritesFunctionality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendeeEvents_Events_EventId",
                table: "AttendeeEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendeeEvents_users_AttendeeId",
                table: "AttendeeEvents");

            migrationBuilder.CreateTable(
                name: "FavoriteEvents",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    DateFavorited = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteEvents", x => new { x.UserId, x.EventId });
                    table.ForeignKey(
                        name: "FK_FavoriteEvents_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FavoriteEvents_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteEvents_EventId",
                table: "FavoriteEvents",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendeeEvents_Events_EventId",
                table: "AttendeeEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendeeEvents_users_AttendeeId",
                table: "AttendeeEvents",
                column: "AttendeeId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendeeEvents_Events_EventId",
                table: "AttendeeEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendeeEvents_users_AttendeeId",
                table: "AttendeeEvents");

            migrationBuilder.DropTable(
                name: "FavoriteEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendeeEvents_Events_EventId",
                table: "AttendeeEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendeeEvents_users_AttendeeId",
                table: "AttendeeEvents",
                column: "AttendeeId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
