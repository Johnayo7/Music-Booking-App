using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Booking_App.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class refactorEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ArtistId",
                table: "Bookings",
                newName: "ArtisteId");

            migrationBuilder.AddColumn<string>(
                name: "BuyerName",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventName",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventStatus",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizerName",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArtisteName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizerName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountStatus",
                table: "Artistes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyerName",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "EventName",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "EventStatus",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "OrganizerName",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ArtisteName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "EventName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "OrganizerName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "Artistes");

            migrationBuilder.RenameColumn(
                name: "ArtisteId",
                table: "Bookings",
                newName: "ArtistId");
        }
    }
}
