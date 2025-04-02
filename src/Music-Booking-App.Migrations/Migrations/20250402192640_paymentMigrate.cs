using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Booking_App.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class paymentMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Tickets",
                newName: "ReferenceUrl");

            migrationBuilder.AddColumn<Guid>(
                name: "EventOrganizerId",
                table: "Tickets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FeeType",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizerName",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookingPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventName = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ReferenceId = table.Column<string>(type: "text", nullable: true),
                    PaymentStatus = table.Column<string>(type: "text", nullable: true),
                    FeeType = table.Column<string>(type: "text", nullable: true),
                    ReferenceUrl = table.Column<string>(type: "text", nullable: true),
                    OrganizerName = table.Column<string>(type: "text", nullable: true),
                    EventOrganizerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArtisteName = table.Column<string>(type: "text", nullable: true),
                    ArtisteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPayments", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingPayments");

            migrationBuilder.DropColumn(
                name: "EventOrganizerId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "FeeType",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "OrganizerName",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "ReferenceUrl",
                table: "Tickets",
                newName: "Status");
        }
    }
}
