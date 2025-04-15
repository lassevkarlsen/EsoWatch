using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsoWatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPauseFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "Remaining",
                table: "Timers",
                type: "interval",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remaining",
                table: "Timers");
        }
    }
}
