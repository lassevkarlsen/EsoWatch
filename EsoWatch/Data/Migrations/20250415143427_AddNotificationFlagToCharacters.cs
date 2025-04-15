using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsoWatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationFlagToCharacters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotificationSent",
                table: "Characters",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationSent",
                table: "Characters");
        }
    }
}
