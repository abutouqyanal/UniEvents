using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniEvents.Migrations
{
    /// <inheritdoc />
    public partial class secoundmig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Events_OrganizerId",
                table: "Events",
                column: "OrganizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_users_OrganizerId",
                table: "Events",
                column: "OrganizerId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_users_OrganizerId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_OrganizerId",
                table: "Events");
        }
    }
}
