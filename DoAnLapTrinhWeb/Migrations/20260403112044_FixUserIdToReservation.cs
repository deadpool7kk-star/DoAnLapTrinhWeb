using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnLapTrinhWeb.Migrations
{
    /// <inheritdoc />
    public partial class FixUserIdToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reservations");
        }
    }
}
