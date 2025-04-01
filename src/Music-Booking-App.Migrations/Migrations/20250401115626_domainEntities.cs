using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Booking_App.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class domainEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserCategory",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserCategory",
                table: "Users");
        }
    }
}
