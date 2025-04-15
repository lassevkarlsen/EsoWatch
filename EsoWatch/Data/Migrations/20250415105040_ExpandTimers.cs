using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsoWatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExpandTimers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Timers",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "NotificationSent",
                table: "Timers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Timers");

            migrationBuilder.DropColumn(
                name: "NotificationSent",
                table: "Timers");
        }
    }
}
