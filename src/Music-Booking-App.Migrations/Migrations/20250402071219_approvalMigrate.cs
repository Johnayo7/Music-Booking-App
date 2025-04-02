using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Booking_App.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class approvalMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewerId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewerName",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Artistes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewerId",
                table: "Artistes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewerName",
                table: "Artistes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ReviewerName",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Artistes");

            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "Artistes");

            migrationBuilder.DropColumn(
                name: "ReviewerName",
                table: "Artistes");
        }
    }
}
